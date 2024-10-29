using CMS.DataEngine;

namespace XperienceCommunity.CSP.Features.ViolationReports;

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

    public Task<bool> ProcessViolationReport(CspViolationReport violationReport)
    {
        try
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
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<bool> ProcessViolationReports(IReadOnlyCollection<CspViolationReport> violationReports)
    {
        try
        {
            var insertReports = new List<CSPViolationReportInfo>(violationReports.Count);

            foreach (var violationReport in violationReports)
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

                insertReports.Add(violationReportInfo);
            }

            _cspViolationReportInfoProvider.BulkInsert(insertReports);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }
}