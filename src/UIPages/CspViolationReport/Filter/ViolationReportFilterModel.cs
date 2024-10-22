using Kentico.Xperience.Admin.Base.Filters;
using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace XperienceCommunity.CSP.UIPages.CspViolationReport;

internal class ViolationReportFilterModel
{
    [DateTimeInputComponent(Label = "Date From", Order = 1)]
    [FilterCondition(BuilderType = typeof(DateFromWhereConditionBuilder), ColumnName = "ReportedAt")]
    public DateTime? DateFrom { get; set; }

    [DateTimeInputComponent(Label = "Date To", Order = 2)]
    [FilterCondition(BuilderType = typeof(DateToWhereConditionBuilder), ColumnName = "ReportedAt")]
    public DateTime? DateTo { get; set; }
}