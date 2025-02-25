﻿# Xperience Community: Content Security Policy (CSP) Management

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


## Contributing

Feel free to submit issues or pull requests to the repository, this is a community package and everyone is welcome to support.

## License

Distributed under the MIT License. See [`LICENSE.md`](LICENSE.md) for more information.