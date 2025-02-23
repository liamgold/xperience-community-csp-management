# Automatic Violation Report Cleanup

As of version [v4.0.0](https://github.com/liamgold/xperience-community-csp-management/releases/tag/v4.0.0), the module now automatically installs a [scheduled task](https://docs.kentico.com/x/scheduled_tasks_xp) to clean up violation reports older than a configured number of days.

This task is **disabled** by default, you need to enable the scheduled task within the admin site.

The default number of days to keep violation reports for is 90 days, this can be configured using the `ContentSecurityPolicyOptions` configuration.

## Setup

You can configure the number of days by adding the following configuration to your `appsettings.json`:

```json
{
  "ContentSecurityPolicy": {
	"ViolationReportsDeletionDays": 30
  }
}
```

This configuration will keep violation reports for 30 days, any older reports will be deleted.

You will also need to register the `ContentSecurityPolicyOptions` configuration in your application:

```csharp

var builder = WebApplication.CreateBuilder(args);

// ...

builder.Services.Configure<ContentSecurityPolicyOptions>(builder.Configuration.GetSection("ContentSecurityPolicy"));

var app = builder.Build();

// ...
```

## Configuration

`ViolationReportsDeletionDays` is the number of days to keep violation reports for.