using CMS.DataEngine;
using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using XperienceCommunity.CSP.UIPages.CspConfiguration;
using IFormItemCollectionProvider = Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider;

[assembly: UIPage(
    parentType: typeof(CspConfigurationListing),
    slug: "create",
    uiPageType: typeof(CspConfigurationCreate),
    name: "Create a CSP configuration",
    templateName: TemplateNames.EDIT,
    order: 200)]

namespace XperienceCommunity.CSP.UIPages.CspConfiguration;

internal class CspConfigurationCreate : ModelEditPage<CspConfigurationEditModel>
{
    private CspConfigurationEditModel? _model;
    private readonly IInfoProvider<CSPConfigurationInfo> _cspConfigurationInfoProvider;
    private readonly IPageLinkGenerator _pageLinkGenerator;

    protected override CspConfigurationEditModel Model => _model ??= new CspConfigurationEditModel();

    public CspConfigurationCreate(
        IFormItemCollectionProvider formItemCollectionProvider,
        IFormDataBinder formDataBinder,
        IPageLinkGenerator pageLinkGenerator,
        IInfoProvider<CSPConfigurationInfo> cspConfigurationInfoProvider)
        : base(formItemCollectionProvider, formDataBinder)
    {
        _pageLinkGenerator = pageLinkGenerator;
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

        var listingUrl = _pageLinkGenerator.GetPath<CspConfigurationListing>();

        var navigateResponse = NavigateTo(listingUrl);

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
