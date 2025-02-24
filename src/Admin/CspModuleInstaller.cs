using CMS.ContentEngine;
using CMS.DataEngine;
using CMS.FormEngine;
using CMS.Modules;
using CMS.Scheduler;
using CMS.Scheduler.Internal;
using Kentico.Xperience.Admin.Base.Forms;
using XperienceCommunity.CSP.Tasks;
using static XperienceCommunity.CSP.Admin.CspConstants;

namespace XperienceCommunity.CSP.Admin;

internal interface ICspModuleInstaller
{
    void Install();
}

internal class CspModuleInstaller(IInfoProvider<ResourceInfo> resourceInfoProvider, IInfoProvider<ScheduledTaskConfigurationInfo> taskInfoProvider) : ICspModuleInstaller
{
    public void Install()
    {
        var resourceInfo = InstallModule();

        InstallModuleClasses(resourceInfo);
        InstallScheduledTasks();
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

    private static void InstallModuleClasses(ResourceInfo resourceInfo)
    {
        InstallCspConfigurationClass(resourceInfo);
        InstallCspViolationReportClass(resourceInfo);
    }

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

    private static void InstallCspViolationReportClass(ResourceInfo resourceInfo)
    {
        var info = DataClassInfoProvider.GetDataClassInfo(CSPViolationReportInfo.TYPEINFO.ObjectClassName) ??
                                      DataClassInfo.New(CSPViolationReportInfo.OBJECT_TYPE);

        info.ClassName = CSPViolationReportInfo.TYPEINFO.ObjectClassName;
        info.ClassTableName = CSPViolationReportInfo.TYPEINFO.ObjectClassName.Replace(".", "_");
        info.ClassDisplayName = "CSP Violation Report";
        info.ClassResourceID = resourceInfo.ResourceID;
        info.ClassType = ClassType.OTHER;

        var formInfo = FormHelper.GetBasicFormDefinition(nameof(CSPViolationReportInfo.CSPViolationReportID));

        var formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Age),
            Visible = true,
            DataType = FieldDataType.Integer,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(NumberInputProperties.Label), "Age" },
            }
        };
        formItem.SetComponentName(NumberInputComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Type),
            Visible = true,
            DataType = FieldDataType.Text,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Type" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Url),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Url" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.UserAgent),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "User Agent" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.BlockedURL),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Blocked URL" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Disposition),
            Visible = true,
            DataType = FieldDataType.Text,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Disposition" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.DocumentURL),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Document URL" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.EffectiveDirective),
            Visible = true,
            DataType = FieldDataType.Text,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Effective Directive" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.LineNumber),
            Visible = true,
            DataType = FieldDataType.Integer,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(NumberInputProperties.Label), "Line Number" },
            }
        };
        formItem.SetComponentName(NumberInputComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.OriginalPolicy),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Original Policy" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Referrer),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Referrer" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.Sample),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Sample" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.SourceFile),
            Visible = true,
            DataType = FieldDataType.LongText,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(TextInputProperties.Label), "Source File" },
            }
        };
        formItem.SetComponentName(TextAreaComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.StatusCode),
            Visible = true,
            DataType = FieldDataType.Integer,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(NumberInputProperties.Label), "Status Code" },
            }
        };
        formItem.SetComponentName(NumberInputComponent.IDENTIFIER);
        formInfo.AddFormItem(formItem);

        formItem = new FormFieldInfo
        {
            Name = nameof(CSPViolationReportInfo.ReportedAt),
            Visible = true,
            DataType = FieldDataType.DateTime,
            Enabled = true,
            AllowEmpty = false,
            Settings = new()
            {
                { nameof(DateTimeInputClientProperties.Label), "Reported At" },
            }
        };
        formItem.SetComponentName(DateTimeInputComponent.IDENTIFIER);
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

    private void InstallScheduledTasks()
    {
        var existingTask = taskInfoProvider.Get(ViolationReportCleanupTask.TASK_NAME);

        if (existingTask is not null)
        {
            return;
        }

        var interval = SchedulingHelper.EncodeInterval(new TaskInterval
        {
            Period = SchedulingHelper.PERIOD_MONTH,
        });

        var task = new ScheduledTaskConfigurationInfo
        {
            ScheduledTaskConfigurationScheduledTaskIdentifier = ViolationReportCleanupTask.IDENTIFIER,
            ScheduledTaskConfigurationDisplayName = $"Clean CSP violation reports",
            ScheduledTaskConfigurationData = "",
            ScheduledTaskConfigurationName = ViolationReportCleanupTask.TASK_NAME,
            ScheduledTaskConfigurationEnabled = false,
            ScheduledTaskConfigurationInterval = interval,
            ScheduledTaskConfigurationDeleteAfterLastRun = false,
            ScheduledTaskConfigurationCreatedDynamically = false,
        };
        task.Insert();
    }
}