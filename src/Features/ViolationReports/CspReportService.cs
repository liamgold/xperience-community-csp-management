using CMS.DataEngine;

namespace XperienceCommunity.CSP.Features.ViolationReports
{
    public interface ICspReportService
    {
        Task<bool> ProcessViolationReport(CspViolationReport violationReport);

        Task<bool> ProcessViolationReports(IReadOnlyCollection<CspViolationReport> violationReports);
    }

    public class CspReportService : ICspReportService
    {
        private readonly IInfoProvider<CSPViolationReportInfo> _cspViolationReportInfoProvider;

        public CspReportService(IInfoProvider<CSPViolationReportInfo> cspViolationReportInfoProvider)
        {
            _cspViolationReportInfoProvider = cspViolationReportInfoProvider;
        }

        public async Task<bool> ProcessViolationReport(CspViolationReport violationReport)
        {
            var violationReportInfo = new CSPViolationReportInfo
            {
                Age = violationReport.Age,
                Type = violationReport.Type,
                Url = violationReport.Url,
                UserAgent = violationReport.UserAgent,
                BlockedURL = violationReport.Body?.BlockedURL,
                Disposition = violationReport.Body?.Disposition,
                DocumentURL = violationReport.Body?.DocumentURL,
                EffectiveDirective = violationReport.Body?.EffectiveDirective,
                LineNumber = violationReport.Body?.LineNumber ?? 0,
                OriginalPolicy = violationReport.Body?.OriginalPolicy,
                Referrer = violationReport.Body?.Referrer,
                Sample = violationReport.Body?.Sample,
                SourceFile = violationReport.Body?.SourceFile,
                StatusCode = violationReport.Body?.StatusCode ?? 0,
                ReportedAt = DateTime.UtcNow,
            };

            _cspViolationReportInfoProvider.Set(violationReportInfo);

            return true;
        }

        public async Task<bool> ProcessViolationReports(IReadOnlyCollection<CspViolationReport> violationReports)
        {
            var tasks = violationReports.Select(ProcessViolationReport);

            await Task.WhenAll(tasks);

            return true;
        }
    }
}
