using CMS.ContentEngine;
using CMS.DataEngine;
using Kentico.Xperience.Admin.Base.FormAnnotations;
using XperienceCommunity.CSP.UIFormComponents;

namespace XperienceCommunity.CSP.UIPages;

internal class CspConfigurationEditModel
{
    [RequiredValidationRule]
    [ObjectIdSelectorComponent(
        objectType: ChannelInfo.OBJECT_TYPE,
        Label = "Channel",
        WhereConditionProviderType = typeof(WebsiteOnlyChannelInfoWhereConditionProvider),
        Order = 1)]
    public IEnumerable<int> ChannelIDs { get; set; } = [];

    [RequiredValidationRule]
    [TextInputComponent(Label = "Source Url", Order = 2)]
    public string? SourceUrl { get; set; }

    [RequiredValidationRule]
    [GeneralSelectorComponent(
                dataProviderType: typeof(CspDirectivesDataProvider),
                Label = "Directives",
                ExplanationTextAsHtml = true,
                ExplanationText = "Only <a href=\"https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy#fetch_directives\" target=\"_blank\">fetch directives</a> are supported",
                Order = 3,
                Placeholder = "Choose directives")]
    public IEnumerable<string> Directives { get; set; } = [];

    [RequiredValidationRule]
    [CheckBoxComponent(Label = "Enabled", Order = 3)]
    public bool Enabled { get; set; }

    public void MapToCSPConfigurationInfo(CSPConfigurationInfo info)
    {
        info.CSPConfigurationChannelID = ChannelIDs.FirstOrDefault();
        info.CSPConfigurationSourceUrl = SourceUrl;
        info.CSPConfigurationDirectives = string.Join(";", Directives);
        info.CSPConfigurationEnabled = Enabled;
    }

    public class WebsiteOnlyChannelInfoWhereConditionProvider : IObjectSelectorWhereConditionProvider
    {
        public WhereCondition Get() => new WhereCondition().WhereEquals(nameof(ChannelInfo.ChannelType), nameof(ChannelType.Website));
    }
}