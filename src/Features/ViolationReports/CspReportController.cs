using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace XperienceCommunity.CSP.Features.ViolationReports;

[ApiController]
[Route("csp-report")]
public class CspReportController : ControllerBase
{
    private readonly ICspReportService _cspReportService;

    public CspReportController(ICspReportService cspReportService)
    {
        _cspReportService = cspReportService;
    }

    [HttpPost("report-to")]
    public async Task<IActionResult> ReportTo([FromBody] JsonElement reports)
    {
        if (reports.ValueKind == JsonValueKind.Array)
        {
            var violationReports = new List<CspViolationReport>(reports.GetArrayLength());

            foreach (var item in reports.EnumerateArray())
            {
                var violationReport = JsonSerializer.Deserialize<CspViolationReport>(item.GetRawText());

                if (violationReport is null)
                {
                    continue;
                }

                violationReports.Add(violationReport);
            }

            await _cspReportService.ProcessViolationReports(violationReports);
        }
        else if (reports.ValueKind == JsonValueKind.Object)
        {
            var violationReport = JsonSerializer.Deserialize<CspViolationReport>(reports.GetRawText());

            if (violationReport is not null)
            {
                await _cspReportService.ProcessViolationReport(violationReport);
            }
        }
        else
        {
            return BadRequest("Invalid report format.");
        }

        return Ok();
    }
}