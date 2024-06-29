using Microsoft.Extensions.DependencyInjection;
using XperienceCommunity.CSP.Admin;
using XperienceCommunity.CSP.Services;

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

        return services;
    }
}
