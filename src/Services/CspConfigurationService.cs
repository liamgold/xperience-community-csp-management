using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Websites;
using XperienceCommunity.CSPManagement;

namespace XperienceCommunity.CSP.Services
{
    public interface ICspConfigurationService
    {
        Task<IReadOnlyCollection<CSPConfigurationInfo>> GetChannelCspConfigurations(int channelId);
    }

    public class CspConfigurationService : ICspConfigurationService
    {
        private readonly IInfoProvider<CSPConfigurationInfo> _cspConfigurationInfoProvider;
        private readonly IProgressiveCache _cache;

        public CspConfigurationService(IInfoProvider<CSPConfigurationInfo> cspConfigurationInfoProvider, IProgressiveCache cache)
        {
            _cspConfigurationInfoProvider = cspConfigurationInfoProvider;
            _cache = cache;
        }

        public async Task<IReadOnlyCollection<CSPConfigurationInfo>> GetChannelCspConfigurations(int channelId)
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
                    .WhereEquals(nameof(CSPConfigurationInfo.CSPConfigurationChannelID), channelId)
                    .GetEnumerableTypedResultAsync()
                    .ConfigureAwait(false);

                return cspConfigurations.ToList();

            }, new CacheSettings(CacheHelper.CacheMinutes(), $"{nameof(CspConfigurationService)}.{nameof(GetChannelCspConfigurations)}|{channelId}"));
        }
    }
}
