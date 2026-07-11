# Xperience Community: Content Security Policy (CSP) Management

[![NuGet](https://img.shields.io/nuget/v/XperienceCommunity.CSP.svg)](https://www.nuget.org/packages/XperienceCommunity.CSP)
[![Downloads](https://img.shields.io/nuget/dt/XperienceCommunity.CSP?color=cc9900)](https://www.nuget.org/packages/XperienceCommunity.CSP)
[![License](https://img.shields.io/badge/License-MIT-brightgreen?style=flat)](https://github.com/liamgold/xperience-community-csp-management/blob/main/LICENSE.md)
[![CI](https://github.com/liamgold/xperience-community-csp-management/actions/workflows/ci.yml/badge.svg)](https://github.com/liamgold/xperience-community-csp-management/actions/workflows/ci.yml)
[![GitHub stars](https://img.shields.io/github/stars/liamgold/xperience-community-csp-management?style=flat&label=stars&logo=github)](https://github.com/liamgold/xperience-community-csp-management/stargazers)

## Description

Adds a module to the admin site for easy configuration of Content Security Policy (CSP) headers for web channels.

## Screenshots

Once installed, a new module appears in the navigation and the dashboard:
<a href="/src/images/navigation-tile.jpg">
  <img src="/src/images/navigation-tile.jpg" width="800" alt="CSP Management module in navigation">
</a>

Screen for creating a new CSP configuration (on a per source url basis):
<a href="/src/images/create-new.jpg">
  <img src="/src/images/create-new.jpg" width="800" alt="Create a new CSP configuration screen">
</a>

Listing shows all configurations, including which web channel they are assigned to.
<a href="/src/images/csp-listing.jpg">
  <img src="/src/images/csp-listing.jpg" width="800" alt="CSP configuration listing screen">
</a>

## Additional Features

| Feature                            | Version Added   | Documentation                                                     |
| -----------------                  | --------------- | ---------------                                                   |
| CSP Nonce Support                  | 2.2.0           | [Nonce-Support.md](./docs/Nonce-Support.md)                       |
| CSP Violation Report Support       | 3.0.0           | [Violation-Report-Support.md](./docs/Violation-Report-Support.md) |
| User Permissions                   | 4.0.0           | [User-Permissions.md](./docs/User-Permissions.md)                 |
| Automatic Violation Report Cleanup | 4.0.0           | [Violation-Report-Cleanup.md](./docs/Violation-Report-Cleanup.md) |

## Library Version Matrix

| Xperience Version | Library Version |
| ----------------- | --------------- |
| >= 30.11.0        | 5.0.0           |
| >= 30.1.3         | 4.0.0           |
| >= 29.1.4         | 2.0.0           |
| >= 28.3.0         | 1.0.0           |

## Dependencies

- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.xperience.io/xp/changelog)

## Package Installation

Add the package to your application using the .NET CLI

```powershell
dotnet add package XperienceCommunity.CSP
```

## Quick Start

1. Install NuGet package above.

1. Register the CSP management services using `builder.Services.AddXperienceCommunityCspManagement()`:

   ```csharp
   // Program.cs

   var builder = WebApplication.CreateBuilder(args);

   builder.Services.AddKentico();

   // ...

   builder.Services.AddXperienceCommunityCspManagement();
   ```


1. Register the CSP management middleware using `app.UseXperienceCommunityCspManagement()`:

   ```csharp
    var app = builder.Build();

    app.UseKentico();

    // ...

    app.UseXperienceCommunityCspManagement();
   ```

1. That's it, launch your website and the module should be installed ready to go! Once you've configured your CSP headers, load a page on the website and check the headers in your browser console.


## Troubleshooting

### Namespace clash after running code generation

This module installs its own database classes (`XperienceCommunity.CSPConfiguration` and `XperienceCommunity.CSPViolationReport`) when your application starts. If you run the Xperience [code generator](https://docs.kentico.com/documentation/developers-and-admins/api/generate-code-files-for-system-objects) for classes (for example after an upgrade), it will generate a **duplicate** copy of these classes into your own project, because the module's classes are not excluded by default.

This leaves two classes registered for the same object type, which causes errors such as the following when deleting a **single** violation report (or CSP configuration) from the admin UI:

```
Unable to cast object of type 'XperienceCommunity.Classes.CSPViolationReport.CSPViolationReportInfo' to type 'XperienceCommunity.CSP.CSPViolationReportInfo'.
```

**Fix:** delete the duplicate generated classes from your project. They will have a namespace beginning with `XperienceCommunity.Classes.` (typically under a `Classes/` folder), rather than the `XperienceCommunity.CSP` namespace shipped by the package.

**Prevention:** exclude this module's classes when you run code generation. You can target just this module, or exclude all community-package classes under the `XperienceCommunity` prefix:

```powershell
# Exclude only this module's classes (recommended)
dotnet run --no-build -- --kxp-codegen --type "Classes" --exclude "XperienceCommunity.CSP*"

# Or exclude all XperienceCommunity.* community-package classes
dotnet run --no-build -- --kxp-codegen --type "Classes" --exclude "XperienceCommunity.*"
```

> The `--exclude` pattern matches class names. This module registers `XperienceCommunity.CSPConfiguration` and `XperienceCommunity.CSPViolationReport`, so `XperienceCommunity.CSP*` targets exactly these two. `XperienceCommunity.*` is broader — it also excludes any other classes named with that prefix (including other community packages, or your own), so only use it if that is what you intend. This same clash can affect any community package whose classes are not excluded from generation.


## Contributing

Feel free to submit issues or pull requests to the repository, this is a community package and everyone is welcome to support.

## License

Distributed under the MIT License. See [`LICENSE.md`](LICENSE.md) for more information.
