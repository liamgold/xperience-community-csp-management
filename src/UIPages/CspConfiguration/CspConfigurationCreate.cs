using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using XperienceCommunity.CSP.UIPages;
using IFormItemCollectionProvider = Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider;

[assembly: UIPage(
    parentType: typeof(CspConfigurationListing),
    slug: "create",
    uiPageType: typeof(CspConfigurationCreate),
    name: "Create a CSP configuration",
    templateName: TemplateNames.EDIT,
    order: 200)]

namespace XperienceCommunity.CSP.UIPages;

internal class CspConfigurationCreate : ModelEditPage<CspConfigurationEditModel>
{
    private CspConfigurationEditModel? _model;
    private readonly IInfoProvider<CSPConfigurationInfo> _cspConfigurationInfoProvider;
    private readonly IPageUrlGenerator _pageUrlGenerator;

    protected override CspConfigurationEditModel Model => _model ??= new CspConfigurationEditModel();

    public CspConfigurationCreate(
        IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IPageUrlGenerator pageUrlGenerator,
        IInfoProvider<CSPConfigurationInfo> cspConfigurationInfoProvider)
        : base(formItemCollectionProvider, formDataBinder)
    {
        _pageUrlGenerator = pageUrlGenerator;
        _cspConfigurationInfoProvider = cspConfigurationInfoProvider;
    }

    public override Task<ICommandResponse<FormChangeResult>> Change(FormChangeCommandArguments args) => base.Change(args);

    protected override async Task<ICommandResponse> ProcessFormData(CspConfigurationEditModel model, ICollection<IFormItem> formItems)
    {
        CreateCodeSnippetInfo(model);

        var navigateResponse = await NavigateToEditPage(model, formItems);

        return navigateResponse;
    }

    private async Task<INavigateResponse> NavigateToEditPage(CspConfigurationEditModel model, ICollection<IFormItem> formItems)
    {
        var baseResult = await base.ProcessFormData(model, formItems);

        var navigateResponse = NavigateTo(
            _pageUrlGenerator.GenerateUrl<CspConfigurationListing>());

        foreach (var message in baseResult.Messages)
        {
            navigateResponse.Messages.Add(message);
        }

        return navigateResponse;
    }

    private void CreateCodeSnippetInfo(CspConfigurationEditModel model)
    {
        var infoObject = new CSPConfigurationInfo();

        model.MapToCSPConfigurationInfo(infoObject);

        _cspConfigurationInfoProvider.Set(infoObject);
    }
}
