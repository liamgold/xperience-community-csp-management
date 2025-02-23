using CMS.DataEngine;
using CMS.Scheduler;
using Microsoft.Extensions.Options;
using XperienceCommunity.CSP.Tasks;

[assembly: RegisterScheduledTask(ViolationReportCleanupTask.IDENTIFIER, typeof(ViolationReportCleanupTask))]
namespace XperienceCommunity.CSP.Tasks;

public class ViolationReportCleanupTask : IScheduledTask
{
    public const string IDENTIFIER = "XperienceCommunity.CSP.ViolationReportCleanupTask";

    internal const string TASK_NAME = "CSP.ViolationReportCleanup";

    private readonly IInfoProvider<CSPViolationReportInfo> _cspViolationReportInfoProvider;
    private readonly ContentSecurityPolicyOptions _options;

    public ViolationReportCleanupTask(
        IInfoProvider<CSPViolationReportInfo> cspViolationReportInfoProvider,
        IOptions<ContentSecurityPolicyOptions>? options)
    {
        _cspViolationReportInfoProvider = cspViolationReportInfoProvider;
        _options = options?.Value ?? new ContentSecurityPolicyOptions();
    }

    public Task<ScheduledTaskExecutionResult> Execute(ScheduledTaskConfigurationInfo task, CancellationToken cancellationToken)
    {
        using var transaction = new CMSTransactionScope();

        try
        {
            var dateCutOff = DateTime.UtcNow.AddDays(-_options.ViolationReportsDeletionDays);

            var whereCondition = new WhereCondition().WhereLessOrEquals(nameof(CSPViolationReportInfo.ReportedAt), dateCutOff);

            _cspViolationReportInfoProvider.BulkDelete(whereCondition);

            transaction.Commit();

            return Task.FromResult(ScheduledTaskExecutionResult.Success);
        }
        catch (Exception)
        {
            return Task.FromResult(new ScheduledTaskExecutionResult("An error occurred while executing the scheduled task."));
        }
    }
}