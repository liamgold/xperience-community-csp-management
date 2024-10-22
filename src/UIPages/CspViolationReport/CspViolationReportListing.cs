using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using XperienceCommunity.CSP.UIPages;

[assembly: UIPage(
    parentType: typeof(CspApplicationPage),
    slug: "reports",
    uiPageType: typeof(CspViolationReportListing),
    name: "Violation Reports",
    templateName: TemplateNames.LISTING,
    order: 200)]

namespace XperienceCommunity.CSP.UIPages;

public class CspViolationReportListing : ListingPage
{
    protected override string ObjectType => CSPViolationReportInfo.OBJECT_TYPE;

    private readonly IInfoProvider<CSPViolationReportInfo> _cspViolationReportInfoProvider;

    public CspViolationReportListing(IInfoProvider<CSPViolationReportInfo> cspViolationReportInfoProvider)
    {
        _cspViolationReportInfoProvider = cspViolationReportInfoProvider;
    }

    public override async Task ConfigurePage()
    {
        PageConfiguration.MassActions.AddCommandWithConfirmation(
            label: "Delete selected reports",
            command: nameof(MassDelete),
            confirmation: "Delete selected reports?",
            confirmationButton: "Delete",
            confirmationDetail: "This can't be undone.",
            icon: Icons.Bin,
            destructive: true);

        PageConfiguration.TableActions.AddDeleteAction(nameof(Delete));

        PageConfiguration.ColumnConfigurations
            .AddColumn(nameof(CSPViolationReportInfo.DocumentURL), "Document URL", searchable: true)
            .AddColumn(nameof(CSPViolationReportInfo.EffectiveDirective), "Violated Directive", searchable: true)
            .AddColumn(nameof(CSPViolationReportInfo.BlockedURL), "Blocked URL", searchable: true)
            .AddColumn(nameof(CSPViolationReportInfo.ReportedAt), "Timestamp", searchable: true, defaultSortDirection: SortTypeEnum.Desc)
            .AddColumn(nameof(CSPViolationReportInfo.UserAgent), "User Agent", searchable: true);

        await base.ConfigurePage();
    }

    [PageCommand]
    public override Task<ICommandResponse<RowActionResult>> Delete(int id) => base.Delete(id);

    /// <summary>
    /// Deletes violation report items specified by the <paramref name="identifiers"/> parameter.
    /// </summary>
    [PageCommand]
    public async Task<ICommandResponse<MassActionResult>> MassDelete(IEnumerable<int> identifiers, CancellationToken cancellationToken)
    {
        using var transaction = new CMSTransactionScope();

        try
        {
            var whereCondition = new WhereCondition().WhereIn(nameof(CSPViolationReportInfo.CSPViolationReportID), identifiers.ToArray());
            _cspViolationReportInfoProvider.BulkDelete(whereCondition);

            transaction.Commit();

            var result = ResponseFrom(new MassActionResult(true));
            result.AddSuccessMessage($"Deleted {identifiers.Count()} items.");

            return result;
        }
        catch (Exception)
        {
            var result = ResponseFrom(new MassActionResult(false));
            result.AddErrorMessage("Delete failed!");
            return result;
        }
    }
}