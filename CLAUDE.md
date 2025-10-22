# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a NuGet library that adds Content Security Policy (CSP) management functionality to Xperience by Kentico CMS. It provides an admin module for configuring CSP headers per web channel, middleware for applying CSP policies to HTTP responses, nonce support for inline scripts, and violation reporting.

**Key Features:**
- CSP configuration management via admin UI
- Response middleware for applying CSP headers
- Nonce generation for inline scripts
- CSP violation reporting and cleanup
- Scheduled task for automatic violation report deletion

## Build & Development Commands

### Building the Library
```powershell
# Build the main library (also generates NuGet package)
dotnet build src/XperienceCommunity.CSP.csproj

# Build entire solution
dotnet build XperienceCommunity.CSP.sln

# Build in Release mode
dotnet build src/XperienceCommunity.CSP.csproj -c Release
```

### Working with the DancingGoat Example
```powershell
# Build the example site
dotnet build examples/DancingGoat/DancingGoat.csproj

# Run the example site
dotnet run --project examples/DancingGoat/DancingGoat.csproj
```

### Package Management
This project uses Central Package Management (Directory.Packages.props):
- All package versions are centrally managed
- Lock files are enabled (`RestorePackagesWithLockFile`)
- When adding packages, add version to Directory.Packages.props

```powershell
# Restore packages
dotnet restore

# Update package lock files
dotnet restore --force-evaluate
```

## Architecture Overview

### Core Components

**1. Module System (Admin Integration)**
- `CspModule.cs` - Registers module with Xperience using `[assembly: CMS.RegisterModule]`
- `CspModuleInstaller.cs` - Creates database schema on app initialization via `OnInit` event hook
- Database tables: `XperienceCommunity_CSPConfiguration`, `XperienceCommunity_CSPViolationReport`

**2. Middleware Layer**
- `ContentSecurityPolicyMiddleware.cs` - Intercepts HTTP responses, builds CSP headers from database configurations, injects nonces into Kentico scripts
- Uses response body interception pattern (replaces `Response.Body` temporarily with MemoryStream)
- Regex-based script modification for Kentico updatable forms and KXT scripts

**3. Service Layer**
- `ICspConfigurationService` - Retrieves CSP configurations by channel (cached with `IProgressiveCache`)
- `ICspNonceService` - Generates unique nonce per request (scoped service)
- `ICspReportService` - Persists CSP violation reports to database

**4. Admin UI Layer**
- Pages use Xperience's declarative registration via `[assembly: UIApplication]` and `[assembly: UIPage]` attributes
- Model-based forms with `ModelEditPage<T>` base class
- Custom permissions: standard (VIEW, CREATE, DELETE, UPDATE) + violation report specific

**5. API Layer**
- `CspReportController.cs` - Endpoint at `/csp-report/report-to` for browser violation reports
- Accepts both single object and array JSON payloads

### Data Flow

**Request Handling:**
1. HTTP request arrives
2. Middleware retrieves configurations from database (cached)
3. Nonce generated per request
4. CSP header built from directives and URLs
5. HTML response modified to add nonce to scripts
6. Response headers set before body is sent

**Configuration Management:**
1. Admin creates/edits CSP config in UI
2. Form data bound to `CspConfigurationEditModel`
3. Model mapped to `CSPConfigurationInfo`
4. Persisted via `IInfoProvider<CSPConfigurationInfo>`
5. Cache invalidated via dependency tracking

**Violation Reporting:**
1. Browser detects CSP violation
2. POSTs JSON to `/csp-report/report-to`
3. Controller deserializes and validates
4. Service persists to database
5. Scheduled task auto-cleans reports older than configured days (default: 90)

## Key Patterns & Conventions

### Xperience by Kentico Integration

**Module Registration:**
- Use `[assembly: CMS.RegisterModule(typeof(CspModule))]` for auto-discovery
- Hook into `ApplicationEvents.Initialized` for setup logic
- Database schema installed automatically on first run

**Admin UI Pages:**
- Declarative registration via assembly attributes
- Hierarchical structure (application > listing > edit sections)
- Template-based routing with slugs

**Data Access:**
- All data models inherit from `AbstractInfo<T>`
- Use `IInfoProvider<T>` for CRUD operations
- Register with `[assembly: RegisterObjectType]`

### Service Lifetime Scoping
- **Scoped:** `CspNonceService` (request-specific nonce)
- **Singleton:** Configuration, Report services (application-wide)

### Caching Pattern
```csharp
_cache.LoadAsync(async s => {
    s.GetCacheDependency = () => CacheHelper.GetCacheDependency([...]);
    // fetch data
}, new CacheSettings(CacheHelper.CacheMinutes(), "cache-key"));
```

### String Conventions
- CSP directives stored as semicolon-separated values (e.g., "default-src;script-src;style-src")
- Parsed on-demand when building headers

### Configuration Options
Configuration via appsettings.json:
```json
"ContentSecurityPolicy": {
    "EnableReporting": true,
    "EnableReportOnlyMode": false,
    "ViolationReportsDeletionDays": 90
}
```

## Project Structure

```
src/
├── Admin/                      # Module registration & installation
├── Classes/                    # Generated ORM models (CSPConfigurationInfo, CSPViolationReportInfo)
├── Features/
│   ├── Configurations/         # Configuration service
│   ├── Nonce/                  # Nonce generation service
│   └── ViolationReports/       # Report service + API controller
├── Middleware/                 # CSP middleware
├── UIPages/                    # Admin UI pages (listing, create, edit)
├── UIFormComponents/           # Custom form components (data providers)
├── Tasks/                      # Scheduled tasks (violation cleanup)
└── CspServiceCollectionExtensions.cs  # DI registration

examples/DancingGoat/          # Example Xperience site with CSP module installed
```

## Important Implementation Notes

### When Modifying Middleware
- The middleware intercepts response bodies - ensure original stream is always restored
- Preview mode requests are excluded from CSP processing
- Only HTML responses are processed (checked via content type)
- Nonce injection uses regex patterns for Kentico-specific scripts

### When Adding New CSP Directives
- Update `CspDirectivesDataProvider.cs` to include new directives in dropdown
- Directives follow CSP Level 3 specification
- Support for fetch directives, document directives, and navigation directives

### When Modifying Database Schema
- Schema changes happen in `CspModuleInstaller.OnInit()`
- Use `DataClassInfo.New()` for creating classes
- Use `FormInfo` and `FormFieldInfo` for defining forms
- Always increment version number in csproj if schema changes

### Integration Requirements
Host application must call:
1. `builder.Services.AddXperienceCommunityCspManagement()` - registers services
2. `app.UseXperienceCommunityCspManagement()` - registers middleware

Place middleware after `app.UseKentico()` but before response-generating middleware.

## Debugging Tips

### Viewing CSP Headers
- Check browser DevTools > Network tab > Response Headers
- Look for `Content-Security-Policy` or `Content-Security-Policy-Report-Only`

### Testing Violation Reports
- Enable reporting in appsettings.json
- Trigger a CSP violation (e.g., inline script without nonce)
- Check `/csp-report/report-to` endpoint receives POST
- View reports in admin UI under CSP Management > Violation Reports

### Cache Issues
- CSP configurations are cached - changes may not appear immediately
- Cache invalidates automatically via object type dependencies
- For manual invalidation, restart application or touch the configuration

## Version & Compatibility

- Target Framework: .NET 8.0
- Xperience by Kentico: >= 30.11.0 (for v5.x)
- Nullable reference types: enabled
- Warnings as errors: enabled for nullable
