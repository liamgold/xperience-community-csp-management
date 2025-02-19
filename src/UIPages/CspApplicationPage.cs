using CMS.Membership;
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

[UIPermission(SystemPermissions.VIEW)]
[UIPermission(SystemPermissions.CREATE)]
[UIPermission(SystemPermissions.DELETE)]
[UIPermission(SystemPermissions.UPDATE)]
[UIPermission(CspPermissions.VIOLATION_REPORT_VIEW, "View Violation Reports")]
[UIPermission(CspPermissions.VIOLATION_REPORT_DELETE, "Delete Violation Reports")]
internal class CspApplicationPage : ApplicationPage
{
    public const string IDENTIFIER = "csp-management";
}

internal static class CspPermissions
{
    public const string VIOLATION_REPORT_VIEW = "Csp.ViolationReport.View";

    public const string VIOLATION_REPORT_DELETE = "Csp.ViolationReport.Delete";
}