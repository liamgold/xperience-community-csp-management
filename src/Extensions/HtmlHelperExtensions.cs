using CMS.Core;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using XperienceCommunity.CSP.Services;

namespace XperienceCommunity.CSP.Extensions
{
    /// <summary>
    /// Provides system extension methods for <see cref="HtmlHelperExtensionPoint"/>.
    /// </summary>
    public static class HtmlHelperExtensions
    {
        public static IHtmlContent ActivityLoggingScriptV2WithNonce(this HtmlHelperExtensionPoint htmlHelper, bool logPageVisit = true, bool logCustomActivity = true, string loggingFunctionName = "kxt")
        {
            IActionContextAccessor actionContextAccessor = Service.Resolve<IActionContextAccessor>();

            // Get the HttpContext from the ActionContext
            var httpContext = actionContextAccessor.ActionContext?.HttpContext;

            if (httpContext is null)
            {
                return new HtmlString(string.Empty);
            }

            // Get the nonce service from the HttpContext
            var cspNonceService = httpContext.RequestServices.GetRequiredService<ICspNonceService>();
            var nonce = cspNonceService.Nonce;

            // Get the original script HTML content
            var originalContent = Kentico.Activities.Web.Mvc.HtmlHelperExtensions.ActivityLoggingScriptV2(htmlHelper, logPageVisit, logCustomActivity, loggingFunctionName);
            var contentString = originalContent?.ToString() ?? string.Empty;

            // Add the nonce to the inline script
            var updatedContent = AddNonceToScript(contentString, nonce);

            return new HtmlString(updatedContent);
        }

        private static string AddNonceToScript(string content, string nonce)
        {
            // Regular expression to match inline scripts
            var scriptPattern = @"(<script\b[^>]*>)(.*?)(<\/script>)";
            var regex = new Regex(scriptPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            // Add nonce to the inline script
            var updatedContent = regex.Replace(content, match =>
            {
                var openingTag = match.Groups[1].Value;
                var scriptContent = match.Groups[2].Value;
                var closingTag = match.Groups[3].Value;

                if (scriptContent.Contains("window['kxt']"))
                {
                    openingTag = openingTag.Insert(openingTag.Length - 1, $" nonce=\"{nonce}\"");
                }

                return $"{openingTag}{scriptContent}{closingTag}";
            });

            return updatedContent;
        }
    }
}
