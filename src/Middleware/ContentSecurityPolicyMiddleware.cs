using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Buffers;
using System.Text;
using System.Text.RegularExpressions;
using XperienceCommunity.CSP.Features.Configurations;
using XperienceCommunity.CSP.Features.Nonce;

namespace XperienceCommunity.CSP;

public partial class ContentSecurityPolicyMiddleware
{
    private static readonly object CspHeadersRegisteredKey = new();

    private readonly RequestDelegate _next;
    private readonly IWebsiteChannelContext _websiteChannelContext;
    private readonly ICspConfigurationService _cspConfigurationService;
    private readonly ILogger<ContentSecurityPolicyMiddleware> _logger;
    private readonly ContentSecurityPolicyOptions _options;

    public ContentSecurityPolicyMiddleware(
        RequestDelegate next,
        IWebsiteChannelContext websiteChannelContext,
        ICspConfigurationService cspConfigurationService,
        IOptions<ContentSecurityPolicyOptions>? options,
        ILogger<ContentSecurityPolicyMiddleware> logger)
    {
        _next = next;
        _websiteChannelContext = websiteChannelContext;
        _cspConfigurationService = cspConfigurationService;
        _logger = logger;
        _options = options?.Value ?? new ContentSecurityPolicyOptions();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Kentico().Preview().Enabled)
        {
            // Store the original response body stream
            var originalBodyStream = context.Response.Body;

            try
            {
                var websiteChannelId = _websiteChannelContext.WebsiteChannelID;
                var configurations = await _cspConfigurationService.GetCspConfigurationsByWebsiteChannelID(websiteChannelId);

                var groupedConfigurations = configurations
                    .SelectMany(c => c.CSPConfigurationDirectives
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Select(directive => new DirectiveUrl
                        {
                            Directive = directive.Trim(),
                            Url = c.CSPConfigurationSourceUrl,
                            UseNonce = c.CSPConfigurationUseNonce
                        }))
                    .GroupBy(x => x.Directive);

                // Without any configuration there is nothing to add to the response
                if (!groupedConfigurations.Any())
                {
                    await _next(context);
                    return;
                }

                var cspNonceService = context.RequestServices.GetRequiredService<ICspNonceService>();
                var nonce = cspNonceService?.Nonce ?? string.Empty;

                var cspHeader = BuildCspHeader(groupedConfigurations, nonce);

                if (_options.EnableReporting == true)
                {
                    cspHeader += "; report-to xbyk-csp-report";
                }

                // Re-executed requests (exception handler, status code pages) run this middleware again for the same response, so the headers are only registered once
                if (!string.IsNullOrWhiteSpace(cspHeader) && context.Items.TryAdd(CspHeadersRegisteredKey, true))
                {
                    // Using OnStarting to set the CSP header before the response is sent, when the content type is known
                    context.Response.OnStarting(() =>
                    {
                        if (IsHtmlResponse(context))
                        {
                            if (_options.EnableReporting == true)
                            {
                                context.Response.Headers.Append("Reporting-Endpoints", "xbyk-csp-report=\"/csp-report/report-to\"");
                            }

                            if (_options.EnableReportOnlyMode == true)
                            {
                                context.Response.Headers.ContentSecurityPolicyReportOnly = cspHeader;
                            }
                            else
                            {
                                context.Response.Headers.ContentSecurityPolicy = cspHeader;
                            }
                        }

                        return Task.CompletedTask;
                    });
                }

                // Without a nonce in the header, Kentico scripts don't need one either, so the response doesn't need to be intercepted
                if (!groupedConfigurations.Any(group => group.Any(x => x.UseNonce)))
                {
                    await _next(context);
                    return;
                }

                // Replace the response body stream with a MemoryStream to intercept the response, while keeping the original stream open, so that the response can be copied back to it later
                // This allows other middlewares to write to the response body without breaking the CSP middleware
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Call the next middleware in the pipeline
                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);

                // Only HTML pages containing Kentico scripts need rewriting, anything else is copied back to the original stream unchanged
                if (IsHtmlResponse(context) && ContainsKenticoScripts(responseBodyStream))
                {
                    var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                    var modifiedResponseBody = AddNonceToKenticoScripts(responseBody, nonce);

                    // Write the modified response body straight to the original stream, as UTF-8 without a BOM like StreamWriter
                    var buffer = ArrayPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(modifiedResponseBody));
                    try
                    {
                        var length = Encoding.UTF8.GetBytes(modifiedResponseBody, 0, modifiedResponseBody.Length, buffer, 0);
                        await originalBodyStream.WriteAsync(buffer.AsMemory(0, length));
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                    }
                }
                else
                {
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the Content Security Policy.");
                throw;
            }
            finally
            {
                // Always restore the original response body stream
                // Only when it was replaced, as setting it to the same stream would wrap the server's response body feature
                if (context.Response.Body != originalBodyStream)
                {
                    context.Response.Body = originalBodyStream;
                }
            }
        }
        else
        {
            await _next(context);
        }
    }

    private static bool IsHtmlResponse(HttpContext context)
    {
        var contentType = context.Response?.ContentType;
        return contentType?.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool ContainsKenticoScripts(MemoryStream responseBodyStream)
    {
        // Checks the raw UTF-8 bytes for the scripts AddNonceToKenticoScripts looks for, so pages without them are never decoded
        var responseBody = responseBodyStream.GetBuffer().AsSpan(0, (int)responseBodyStream.Length);

        return responseBody.IndexOf("window.kentico.updatableFormHelper.registerEventListeners"u8) >= 0 ||
            responseBody.IndexOf("window['kxt']"u8) >= 0;
    }

    private static string BuildCspHeader(IEnumerable<IGrouping<string, DirectiveUrl>> groupedConfigurations, string nonce)
    {
        var sb = new StringBuilder();

        foreach (var group in groupedConfigurations)
        {
            var useNonce = group.Any(x => x.UseNonce);
            if (useNonce)
            {
                sb.Append($"{group.Key} 'nonce-{nonce}' {string.Join(" ", group.Select(config => config.Url))}; ");
            }
            else
            {
                sb.Append($"{group.Key} {string.Join(" ", group.Select(config => config.Url))}; ");
            }
        }

        return sb.ToString().TrimEnd(' ', ';');
    }

    private static string AddNonceToKenticoScripts(string content, string nonce)
    {
        var updatedContent = ScriptElementRegex().Replace(content, match =>
        {
            var openingTag = match.Groups[1].Value;
            var scriptContent = match.Groups[2].Value;
            var closingTag = match.Groups[3].Value;

            if (scriptContent.Contains("window.kentico.updatableFormHelper.registerEventListeners") ||
                scriptContent.Contains("window['kxt']"))
            {
                openingTag = openingTag.Insert(openingTag.Length - 1, $" nonce=\"{nonce}\"");
            }

            return $"{openingTag}{scriptContent}{closingTag}";
        });

        return updatedContent;
    }

    [GeneratedRegex(@"(<script\b[^>]*>)(.*?)(<\/script>)", RegexOptions.Singleline | RegexOptions.IgnoreCase)]
    private static partial Regex ScriptElementRegex();

    private struct DirectiveUrl
    {
        public string Directive { get; set; }
        public string Url { get; set; }
        public bool UseNonce { get; set; }
    }
}

public class ContentSecurityPolicyOptions
{
    public bool? EnableReporting { get; set; } = false;

    public bool? EnableReportOnlyMode { get; set; } = false;

    public int ViolationReportsDeletionDays { get; set; } = 90;
}

public static class ContentSecurityPolicyMiddlewareExtensions
{
    /// <summary>
    ///     Add the CSP middleware.
    /// </summary>
    /// <param name="builder">The Microsoft.AspNetCore.Builder.IApplicationBuilder to add the middleware to.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseXperienceCommunityCspManagement(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ContentSecurityPolicyMiddleware>();
    }
}