using Microsoft.Extensions.DependencyInjection;
using XperienceCommunity.CSP.Admin;
using XperienceCommunity.CSP.Features.Configurations;
using XperienceCommunity.CSP.Features.Nonce;
using XperienceCommunity.CSP.Features.ViolationReports;

namespace XperienceCommunity.CSP;

public static class CspServiceCollectionExtensions
{
    /// <summary>
    /// Adds all required services for CSP Management functionality
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddXperienceCommunityCspManagement(this IServiceCollection services)
    {
        services.AddScoped<ICspNonceService, CspNonceService>();

        services.AddSingleton<ICspModuleInstaller, CspModuleInstaller>();
        services.AddSingleton<ICspConfigurationService, CspConfigurationService>();
        services.AddSingleton<ICspReportService, CspReportService>();

        return services;
    }
}
