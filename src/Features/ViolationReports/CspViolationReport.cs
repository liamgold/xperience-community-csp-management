using System.Text.Json.Serialization;

namespace XperienceCommunity.CSP.Features.ViolationReports;

public class CspViolationReport
{
    [JsonPropertyName("age")]
    public int Age { get; set; }

    [JsonPropertyName("body")]
    public ViolationBody? Body { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("user_agent")]
    public string UserAgent { get; set; } = string.Empty;
}

public class ViolationBody
{
    [JsonPropertyName("blockedURL")]
    public string BlockedURL { get; set; } = string.Empty;

    [JsonPropertyName("disposition")]
    public string Disposition { get; set; } = string.Empty;

    [JsonPropertyName("documentURL")]
    public string DocumentURL { get; set; } = string.Empty;

    [JsonPropertyName("effectiveDirective")]
    public string EffectiveDirective { get; set; } = string.Empty;

    [JsonPropertyName("lineNumber")]
    public int LineNumber { get; set; }

    [JsonPropertyName("originalPolicy")]
    public string OriginalPolicy { get; set; } = string.Empty;

    [JsonPropertyName("referrer")]
    public string Referrer { get; set; } = string.Empty;

    [JsonPropertyName("sample")]
    public string Sample { get; set; } = string.Empty;

    [JsonPropertyName("sourceFile")]
    public string SourceFile { get; set; } = string.Empty;

    [JsonPropertyName("statusCode")]
    public int StatusCode { get; set; }
}
