using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.UIPages;
using XperienceCommunity.CSP.UIPages;

[assembly: UIApplication(
    identifier: CspApplicationPage.IDENTIFIER,
    type: typeof(CspApplicationPage),
    slug: "csp-management",
    name: "CSP management",
    category: BaseApplicationCategories.CONFIGURATION,
    icon: Icons.Badge,
    templateName: TemplateNames.SECTION_LAYOUT)]

namespace XperienceCommunity.CSP.UIPages;

internal class CspApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "csp-management";
}
