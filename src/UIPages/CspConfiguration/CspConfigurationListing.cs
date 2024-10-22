using CMS.Base;
using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using XperienceCommunity.CSP.UIPages;
using XperienceCommunity.CSP.UIPages.CspConfiguration;

[assembly: UIPage(
    parentType: typeof(CspApplicationPage),
    slug: "list",
    uiPageType: typeof(CspConfigurationListing),
    name: "Configurations",
    templateName: TemplateNames.LISTING,
    order: UIPageOrder.First)]

namespace XperienceCommunity.CSP.UIPages.CspConfiguration;

public class CspConfigurationListing : ListingPage
{
    protected override string ObjectType => CSPConfigurationInfo.OBJECT_TYPE;

    private readonly IInfoProvider<ChannelInfo> _channelProvider;

    public CspConfigurationListing(IInfoProvider<ChannelInfo> channelProvider)
    {
        _channelProvider = channelProvider;
    }

    public override async Task ConfigurePage()
    {
        var allChannels = await _channelProvider.Get().GetEnumerableTypedResultAsync();

        PageConfiguration.HeaderActions.AddLink<CspConfigurationCreate>("New CSP configuration");
        PageConfiguration.TableActions.AddDeleteAction(nameof(Delete));
        PageConfiguration.AddEditRowAction<CspConfigurationEdit>();

        PageConfiguration.ColumnConfigurations
            .AddColumn(
                nameof(CSPConfigurationInfo.CSPConfigurationChannelID),
                "Channel",
                formatter: (value, _) => allChannels.FirstOrDefault(c => c.ChannelID == (int)value)?.ChannelDisplayName ?? ""
            )
            .AddColumn(nameof(CSPConfigurationInfo.CSPConfigurationSourceUrl), "Source Url", searchable: true)
            .AddColumn(nameof(CSPConfigurationInfo.CSPConfigurationDirectives), "Directives", formatter: FormatDirectives)
            .AddColumn(nameof(CSPConfigurationInfo.CSPConfigurationEnabled), "Enabled")
            .AddColumn(nameof(CSPConfigurationInfo.CSPConfigurationUseNonce), "Use Nonce?");

        await base.ConfigurePage();
    }

    [PageCommand]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);

    private string FormatDirectives(object objectValue, IDataContainer dataContainer)
    {
        var directivesValue = objectValue?.ToString();

        if (string.IsNullOrWhiteSpace(directivesValue))
        {
            return "No directives selected.";
        }

        var directives = directivesValue.Split(';', StringSplitOptions.RemoveEmptyEntries);

        if (directives.Length == 0)
        {
            return "No directives selected.";
        }

        return string.Join(", ", directives);
    }
}