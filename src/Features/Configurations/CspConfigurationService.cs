using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites;

namespace XperienceCommunity.CSP.Features.Configurations;

public interface ICspConfigurationService
{
    Task<IReadOnlyCollection<CSPConfigurationInfo>> GetCspConfigurationsByWebsiteChannelID(int websiteChannelId);
}

public class CspConfigurationService : ICspConfigurationService
{
    private readonly IInfoProvider<CSPConfigurationInfo> _cspConfigurationInfoProvider;
    private readonly IInfoProvider<WebsiteChannelInfo> _websiteChannelInfoProvider;
    private readonly IProgressiveCache _cache;

    public CspConfigurationService(IInfoProvider<CSPConfigurationInfo> cspConfigurationInfoProvider, IProgressiveCache cache, IInfoProvider<WebsiteChannelInfo> websiteChannelInfoProvider)
    {
        _cspConfigurationInfoProvider = cspConfigurationInfoProvider;
        _cache = cache;
        _websiteChannelInfoProvider = websiteChannelInfoProvider;
    }

    public async Task<IReadOnlyCollection<CSPConfigurationInfo>> GetCspConfigurationsByWebsiteChannelID(int websiteChannelId)
    {
        return await _cache.LoadAsync(async s =>
        {
            s.GetCacheDependency = () =>
                CacheHelper.GetCacheDependency(
                    [
                        $"{CSPConfigurationInfo.OBJECT_TYPE}|all",
                        $"{ChannelInfo.OBJECT_TYPE}|all",
                        $"{WebsiteChannelInfo.OBJECT_TYPE}|all",
                    ]);

            var cspConfigurations = await _cspConfigurationInfoProvider.Get()
                .Source(sourceItem => sourceItem.Join<WebsiteChannelInfo>(nameof(CSPConfigurationInfo.CSPConfigurationChannelID), nameof(WebsiteChannelInfo.WebsiteChannelChannelID)))
                .WhereEquals(nameof(WebsiteChannelInfo.WebsiteChannelID), websiteChannelId)
                .WhereTrue(nameof(CSPConfigurationInfo.CSPConfigurationEnabled))
                .GetEnumerableTypedResultAsync()
                .ConfigureAwait(false);

            return cspConfigurations.ToList();

        }, new CacheSettings(CacheHelper.CacheMinutes(), $"{nameof(CspConfigurationService)}.{nameof(GetCspConfigurationsByWebsiteChannelID)}|{websiteChannelId}"));
    }
}
