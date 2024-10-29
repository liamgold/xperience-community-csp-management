using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.RegularExpressions;
using XperienceCommunity.CSP.Features.Configurations;
using XperienceCommunity.CSP.Features.Nonce;

namespace XperienceCommunity.CSP;

public class ContentSecurityPolicyMiddleware
{
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
                // Replace the response body stream with a MemoryStream to intercept the response, while keeping the original stream open, so that the response can be copied back to it later
                // This allows other middlewares to write to the response body without breaking the CSP middleware
                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Call the next middleware in the pipeline
                await _next(context);

                // Check if the response is an HTML page
                if (IsHtmlResponse(context))
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

                    if (groupedConfigurations.Any())
                    {
                        var cspNonceService = context.RequestServices.GetRequiredService<ICspNonceService>();
                        var nonce = cspNonceService?.Nonce ?? string.Empty;

                        var cspHeader = BuildCspHeader(groupedConfigurations, nonce);

                        if (_options.EnableReporting == true)
                        {
                            cspHeader += "; report-to xbyk-csp-report";
                        }

                        if (!string.IsNullOrWhiteSpace(cspHeader))
                        {
                            // Using OnStarting to set the CSP header before the response is sent
                            context.Response.OnStarting(() =>
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

                                return Task.CompletedTask;
                            });
                        }

                        // Read the response body from the MemoryStream
                        responseBodyStream.Seek(0, SeekOrigin.Begin);
                        var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();
                        var modifiedResponseBody = AddNonceToKenticoScripts(responseBody, nonce);

                        responseBodyStream.SetLength(0);
                        await using var writer = new StreamWriter(responseBodyStream, leaveOpen: true);
                        await writer.WriteAsync(modifiedResponseBody);
                        await writer.FlushAsync();
                    }

                    // Copy the (potentially modified) response body back to the original stream
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    await responseBodyStream.CopyToAsync(originalBodyStream);
                }
                else
                {
                    // If not HTML, simply copy the response back to the original stream
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
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
                context.Response.Body = originalBodyStream;
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
        var scriptPattern = @"(<script\b[^>]*>)(.*?)(<\/script>)";
        var regex = new Regex(scriptPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

        var updatedContent = regex.Replace(content, match =>
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