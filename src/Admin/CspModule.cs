using CMS.Base;
using CMS.Core;
using CMS.DataEngine;
using Microsoft.Extensions.DependencyInjection;
using XperienceCommunity.CSP.Admin;

[assembly: CMS.RegisterModule(typeof(CspModule))]
namespace XperienceCommunity.CSP.Admin;

internal class CspModule : Module
{
    private ICspModuleInstaller? _installer;

    public CspModule() : base(nameof(CspModule))
    {
    }

    protected override void OnInit(ModuleInitParameters parameters)
    {
        base.OnInit(parameters);

        var services = parameters.Services;

        _installer = services.GetRequiredService<ICspModuleInstaller>();

        ApplicationEvents.Initialized.Execute += InitializeModule;
    }

    private async void InitializeModule(object? sender, EventArgs e)
    {
        if (_installer is null)
        {
            return;
        }

        await _installer.Install();
    }
}
