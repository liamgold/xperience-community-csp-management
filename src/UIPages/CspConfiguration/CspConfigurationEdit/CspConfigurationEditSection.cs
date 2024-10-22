using Kentico.Xperience.Admin.Base;
using XperienceCommunity.CSP.UIPages.CspConfiguration;

[assembly: UIPage(
    parentType: typeof(CspConfigurationListing),
    slug: PageParameterConstants.PARAMETERIZED_SLUG,
    uiPageType: typeof(CspConfigurationEditSection),
    name: "Edit section for CSP configurations",
    templateName: TemplateNames.SECTION_LAYOUT,
    order: 300)]

namespace XperienceCommunity.CSP.UIPages.CspConfiguration;

public class CspConfigurationEditSection : EditSectionPage<CSPConfigurationInfo>
{
}