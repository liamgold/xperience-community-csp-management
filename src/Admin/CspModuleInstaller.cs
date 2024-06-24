using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using Kentico.Xperience.Admin.Base.Forms;
using static XperienceCommunity.CSP.Admin.CspConstants;

namespace XperienceCommunity.CSP.Admin;

internal interface ICspModuleInstaller
{
    void Install();
}

internal class CspModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider) : ICspModuleInstaller
{
    public void Install()
    {
        var resourceInfo = InstallModule();
        InstallModuleClasses(resourceInfo);
    }

    private ResourceInfo InstallModule()
    {
        var resourceInfo = resourceInfoProvider.Get(ResourceConstants.ResourceName)
            ?? new ResourceInfo();

        resourceInfo.ResourceDisplayName = ResourceConstants.ResourceDisplayName;
        resourceInfo.ResourceName = ResourceConstants.ResourceName;
        resourceInfo.ResourceDescription = ResourceConstants.ResourceDescription;
        resourceInfo.ResourceIsInDevelopment = ResourceConstants.ResourceIsInDevelopment;

        if (resourceInfo.HasChanged)
        {
            resourceInfoProvider.Set(resourceInfo);
        }

        return resourceInfo;
    }

    private static void InstallModuleClasses(ResourceInfo resourceInfo) => InstallCspConfigurationClass(resourceInfo);

    private static void InstallCspConfigurationClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(CSPConfigurationInfo.TYPEINFO.ObjectClassName) ??
                                      DataClassInfo.New(CSPConfigurationInfo.OBJECT_TYPE);

        info.ClassName = CSPConfigurationInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = CSPConfigurationInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "CSP Configuration";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(CSPConfigurationInfo.CSPConfigurationID));

        var formItem = new FormFieldInfo
        {
            Name = nameof(CSPConfigurationInfo.CSPConfigurationChannelID),
            Visible = false,
            DataType = FieldDataType.Integer,
            Enabled = true,
            ReferenceToObjectType = ChannelInfo.OBJECT_TYPE,
            ReferenceType = ObjectDependencyEnum.Required
        };
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPConfigurationInfo.CSPConfigurationSourceUrl),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Source url" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPConfigurationInfo.CSPConfigurationDirectives),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Directives" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPConfigurationInfo.CSPConfigurationEnabled),
            Visible = true,
            DataType = FieldDataType.Boolean,
            Enabled = true,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Enabled" },
            }
        };
        formItem.SetComponentName(CheckBoxComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPConfigurationInfo.CSPConfigurationUseNonce),
            Visible = true,
            DataType = FieldDataType.Boolean,
            Enabled = true,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Use Nonce?" },
            }
        };
        formItem.SetComponentName(CheckBoxComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        SetFormDefinition(info, formInfo);

        if (info.HasChanged)
        {
            DataClassInfoProvider.SetDataClassInfo(info);
        }
    }

    /// <summary>
    /// Ensure that the form is not upserted with any existing form
    /// </summary>
    /// <param name="info"></param>
    /// <param name="form"></param>
    private static void SetFormDefinition(DataClassInfo info, FormInfo form)
    {
        if (info.ClassID > 0)
        {
            var existingForm = new FormInfo(info.ClassFormDefinition);
            existingForm.CombineWithForm(form, new());
            info.ClassFormDefinition = existingForm.GetXmlDefinition();
        }
        else
        {
            info.ClassFormDefinition = form.GetXmlDefinition();
        }
    }
}