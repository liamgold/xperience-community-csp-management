using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;
using XperienceCommunity.CSP.Services;

namespace XperienceCommunity.CSP;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebsiteChannelContext _websiteChannelContext;
    private readonly ICspConfigurationService _cspConfigurationService;
    private readonly ILogger<ContentSecurityPolicyMiddleware> _logger;

    public ContentSecurityPolicyMiddleware(
        RequestDelegate next,
        IWebsiteChannelContext websiteChannelContext,
        ICspConfigurationService cspConfigurationService,
        ILogger<ContentSecurityPolicyMiddleware> logger)
    {
        _next = next;
        _websiteChannelContext = websiteChannelContext;
        _cspConfigurationService = cspConfigurationService;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Kentico().Preview().Enabled)
        {
            try
            {
                // Intercept the response
                var originalBodyStream = context.Response.Body;

                using var responseBodyStream = new MemoryStream();
                context.Response.Body = responseBodyStream;

                // Proceed through the pipeline
                await _next(context);

                // Check if the response is an HTML page
                if (IsHtmlResponse(context))
                {
                    var websiteChannelId = _websiteChannelContext.WebsiteChannelID;
                    var configurations = await _cspConfigurationService.GetCspConfigurationsByWebsiteChannelID(websiteChannelId);

                    var cspNonceService = context.RequestServices.GetRequiredService<ICspNonceService>();
                    var nonce = cspNonceService?.Nonce ?? string.Empty;

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

                    var cspHeader = sb.ToString().TrimEnd(' ', ';');

                    if (!string.IsNullOrWhiteSpace(cspHeader))
                    {
                        context.Response.Headers.ContentSecurityPolicy = cspHeader;
                    }

                    // Rewind the response stream and read its content
                    responseBodyStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

                    // Modify the response body content
                    var modifiedResponseBody = AddNonceToKenticoScripts(responseBody, nonce);

                    // Write the modified content back to the response stream
                    await using (var writer = new StreamWriter(originalBodyStream, leaveOpen: true))
                    {
                        await writer.WriteAsync(modifiedResponseBody);
                        await writer.FlushAsync();
                    }
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
        }
        else
        {
            await _next(context);
        }
    }

    private static bool IsHtmlResponse(HttpContext context)
    {
        // Check if the response content type indicates HTML
        var contentType = context.Response.ContentType;
        return contentType?.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static string AddNonceToKenticoScripts(string content, string nonce)
    {
        var scriptPattern = @"(<script\b[^>]*>)(.*?)(<\/script>)";
        var regex = new Regex(scriptPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

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