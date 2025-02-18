using Kentico.Xperience.Admin.Base;
using Kentico.Xperience.Admin.Base.Forms;
using XperienceCommunity.CSP.UIPages.CspConfiguration;

[assembly: UIPage(
    parentType: typeof(CspConfigurationEditSection),
    slug: "edit",
    uiPageType: typeof(CspConfigurationEdit),
    name: "Edit a CSP configuration",
    templateName: TemplateNames.EDIT,
    order: UIPageOrder.First)]

namespace XperienceCommunity.CSP.UIPages.CspConfiguration;

internal class CspConfigurationEdit : InfoEditPage<CSPConfigurationInfo>
{
    public CspConfigurationEdit(IFormComponentMapper formComponentMapper, IFormDataBinder formDataBinder)
        : base(formComponentMapper, formDataBinder)
    {
    }

    [PageParameter(typeof(IntPageModelBinder))]
    public override int ObjectId { get; set; }
}








//using CMS.DataEngine;
//using Kentico.Xperience.Admin.Base;
//using Kentico.Xperience.Admin.Base.Forms;
//using XperienceCommunity.CSP.UIPages.CspConfiguration;
//using IFormItemCollectionProvider = Kentico.Xperience.Admin.Base.Forms.Internal.IFormItemCollectionProvider;

//[assembly: UIPage(
//    parentType: typeof(CspConfigurationEditSection),
//    slug: "edit",
//    uiPageType: typeof(CspConfigurationEdit),
//    name: "Edit a CSP configuration",
//    templateName: TemplateNames.EDIT,
//    order: UIPageOrder.First)]

//namespace XperienceCommunity.CSP.UIPages.CspConfiguration;

//internal class CspConfigurationEdit : ModelEditPage<CspConfigurationEditModel>
//{
//    private CspConfigurationEditModel? _model;
//    private readonly IInfoProvider<CSPConfigurationInfo> _cspConfigurationInfoProvider;

//    public CspConfigurationEdit(
//        IFormItemCollectionProvider formItemCollectionProvider,
//        IFormDataBinder formDataBinder,
//        IInfoProvider<CSPConfigurationInfo> cspConfigurationInfoProvider)
//        : base(formItemCollectionProvider, formDataBinder)
//    {
//        _cspConfigurationInfoProvider = cspConfigurationInfoProvider;
//    }

//    protected override CspConfigurationEditModel Model
//    {
//        get
//        {
//            if (_model != null)
//            {
//                return _model;
//            }

//            var info = _cspConfigurationInfoProvider.Get(ObjectID);
//            if (info == null)
//            {
//                return new CspConfigurationEditModel();
//            }

//            _model = new CspConfigurationEditModel()
//            {
//                ChannelIDs = [info.CSPConfigurationChannelID],
//                Directives = info.CSPConfigurationDirectives.Split(';', StringSplitOptions.RemoveEmptyEntries),
//                SourceUrl = info.CSPConfigurationSourceUrl,
//                Enabled = info.CSPConfigurationEnabled,
//                UseNonce = info.CSPConfigurationUseNonce,
//            };

//            return _model;
//        }
//    }

//    [PageParameter(typeof(IntPageModelBinder))]
//    public int ObjectID { get; set; }

//    protected override async Task<ICommandResponse> ProcessFormData(CspConfigurationEditModel model, ICollection<IFormItem> formItems)
//    {
//        var info = _cspConfigurationInfoProvider.Get(ObjectID);

//        model.MapToCSPConfigurationInfo(info);

//        _cspConfigurationInfoProvider.Set(info);

//        return await base.ProcessFormData(model, formItems);
//    }
//}
