# CSP Violation Report Support

As of version [v3.0.0](https://github.com/liamgold/xperience-community-csp-management/releases/tag/v3.0.0), this module supports the use of [CSP violation reports](https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Security-Policy/report-to).

## Setup

You can enable reporting mode by adding the following configuration to your `appsettings.json`:

```json
{
  "ContentSecurityPolicy": {
	"EnableReporting": true,
	"EnableReportOnlyMode": true
  }
}
```

You will also need to register the `ContentSecurityPolicyOptions` configuration in your application:

```csharp

var builder = WebApplication.CreateBuilder(args);

// ...

builder.Services.Configure<ContentSecurityPolicyOptions>(builder.Configuration.GetSection("ContentSecurityPolicy"));

var app = builder.Build();

// ...
```

## Configuration

`EnableReporting` is set to `false` by default. When set to `true`, the system captures requests that violate existing CSP headers and shows this information within the reporting area of the CSP module.

`EnableReportOnlyMode` is set to `false` by default. When set to `true`, the system will only report violations and not enforce the CSP headers.