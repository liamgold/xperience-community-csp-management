using CMS.Websites.Routing;
using Kentico.Content.Web.Mvc;
using Kentico.Web.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Text;
using XperienceCommunity.CSP.Services;

namespace XperienceCommunity.CSP;

public class ContentSecurityPolicyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebsiteChannelContext _websiteChannelContext;
    private readonly ICspConfigurationService _cspConfigurationService;

    public ContentSecurityPolicyMiddleware(RequestDelegate next, IWebsiteChannelContext websiteChannelContext, ICspConfigurationService cspConfigurationService)
    {
        _next = next;
        _websiteChannelContext = websiteChannelContext;
        _cspConfigurationService = cspConfigurationService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Kentico().Preview().Enabled)
        {
            var channelId = _websiteChannelContext.WebsiteChannelID;

            var configurations = await _cspConfigurationService.GetChannelCspConfigurations(channelId);

            var groupedConfigurations = configurations
                .SelectMany(c => c.CSPConfigurationDirectives
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .Select(directive => new DirectiveUrl { Directive = directive.Trim(), Url = c.CSPConfigurationSourceUrl }))
                .GroupBy(x => x.Directive);

            var sb = new StringBuilder();
            foreach (var group in groupedConfigurations)
            {
                sb.Append($"{group.Key} {string.Join(" ", group.Select(config => config.Url))}; ");
            }

            var cspHeader = sb.ToString().TrimEnd(' ', ';');

            context.Response.Headers.ContentSecurityPolicy = new(cspHeader);
        }

        await _next(context);
    }

    private struct DirectiveUrl
    {
        public string Directive { get; set; }
        public string Url { get; set; }
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
