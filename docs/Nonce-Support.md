# CSP Nonce Support

This module supports the use of nonces in your CSP headers. This was added in version [v2.2.0](https://github.com/liamgold/xperience-community-csp-management/releases/tag/v2.2.0) of the library.

When creating a new CSP configuration, you can enable or disable a nonce being added for the selected directives. This will add a nonce to the header.

You are responsible for adding the nonce to your inline scripts and styles. You can use the `CspNonceService` to get the current nonce value. This service can be injected into your services, controllers, or views.

```csharp
public class MyService
{
    private readonly ICspNonceService _cspNonceService;

    public MyService(ICspNonceService cspNonceService)
    {
        _cspNonceService = cspNonceService;
    }

    public string GetNonce()
    {
        return _cspNonceService.Nonce;
    }
}
```