namespace XperienceCommunity.CSP.Features.ViolationReports
{
    public interface ICspReportService
    {
        Task<bool> ProcessViolationReport(CspViolationReport violationReport);

        Task<bool> ProcessViolationReports(IReadOnlyCollection<CspViolationReport> violationReports);
    }

    public class CspReportService : ICspReportService
    {
        public async Task<bool> ProcessViolationReport(CspViolationReport violationReport)
        {
            // TODO: add logic to process a single violation report

            return true;
        }

        public async Task<bool> ProcessViolationReports(IReadOnlyCollection<CspViolationReport> violationReports)
        {
            // TODO: add logic to process multiple violation reports

            return true;
        }
    }
}
