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
        //PageConfiguration.HeaderActions.AddCommandWithConfirmation(
        //    label: "Delete All",
        //    command: nameof(DeleteAll),
        //    confirmation: "Delete all reports?",
        //    confirmationButton: "Delete",
        //    confirmationDetail: "This can't be undone.",
        //    icon: Icons.Bin,
        //    destructive: true);

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

    [PageCommand]
    public Task<ICommandResponse<MassActionResult>> DeleteAll()
    {
        using var transaction = new CMSTransactionScope();

        _cspViolationReportInfoProvider.BulkDelete(new WhereCondition());

        transaction.Commit();

        // TODO: Can't get this to trigger a full reload of the table after all rows deleted.
        return Task.FromResult<ICommandResponse<MassActionResult>>(new CommandResponse<MassActionResult>(new MassActionResult(reload: true, refetchAll: true)));
    }
}