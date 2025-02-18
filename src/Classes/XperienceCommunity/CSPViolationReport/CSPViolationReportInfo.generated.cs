using System;
using System.Data;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using XperienceCommunity.CSP;

[assembly: RegisterObjectType(typeof(CSPViolationReportInfo), CSPViolationReportInfo.OBJECT_TYPE)]

namespace XperienceCommunity.CSP
{
    /// <summary>
    /// Data container class for <see cref="CSPViolationReportInfo"/>.
    /// </summary>
    public partial class CSPViolationReportInfo : AbstractInfo<CSPViolationReportInfo, IInfoProvider<CSPViolationReportInfo>>, IInfoWithId
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "xperiencecommunity.cspviolationreport";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<CSPViolationReportInfo>), OBJECT_TYPE, "XperienceCommunity.CSPViolationReport", "CSPViolationReportID", null, null, null, null, null, null, null)
        {
            TouchCacheDependencies = true,
        };


        /// <summary>
        /// CSP violation report ID.
        /// </summary>
        [DatabaseField]
        public virtual int CSPViolationReportID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(CSPViolationReportID)), 0);
            set => SetValue(nameof(CSPViolationReportID), value);
        }


        /// <summary>
        /// Age.
        /// </summary>
        [DatabaseField]
        public virtual int Age
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(Age)), 0);
            set => SetValue(nameof(Age), value);
        }


        /// <summary>
        /// Type.
        /// </summary>
        [DatabaseField]
        public virtual string Type
        {
            get => ValidationHelper.GetString(GetValue(nameof(Type)), String.Empty);
            set => SetValue(nameof(Type), value);
        }


        /// <summary>
        /// Url.
        /// </summary>
        [DatabaseField]
        public virtual string Url
        {
            get => ValidationHelper.GetString(GetValue(nameof(Url)), String.Empty);
            set => SetValue(nameof(Url), value);
        }


        /// <summary>
        /// User agent.
        /// </summary>
        [DatabaseField]
        public virtual string UserAgent
        {
            get => ValidationHelper.GetString(GetValue(nameof(UserAgent)), String.Empty);
            set => SetValue(nameof(UserAgent), value);
        }


        /// <summary>
        /// Blocked URL.
        /// </summary>
        [DatabaseField]
        public virtual string BlockedURL
        {
            get => ValidationHelper.GetString(GetValue(nameof(BlockedURL)), String.Empty);
            set => SetValue(nameof(BlockedURL), value);
        }


        /// <summary>
        /// Disposition.
        /// </summary>
        [DatabaseField]
        public virtual string Disposition
        {
            get => ValidationHelper.GetString(GetValue(nameof(Disposition)), String.Empty);
            set => SetValue(nameof(Disposition), value);
        }


        /// <summary>
        /// Document URL.
        /// </summary>
        [DatabaseField]
        public virtual string DocumentURL
        {
            get => ValidationHelper.GetString(GetValue(nameof(DocumentURL)), String.Empty);
            set => SetValue(nameof(DocumentURL), value);
        }


        /// <summary>
        /// Effective directive.
        /// </summary>
        [DatabaseField]
        public virtual string EffectiveDirective
        {
            get => ValidationHelper.GetString(GetValue(nameof(EffectiveDirective)), String.Empty);
            set => SetValue(nameof(EffectiveDirective), value);
        }


        /// <summary>
        /// Line number.
        /// </summary>
        [DatabaseField]
        public virtual int LineNumber
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(LineNumber)), 0);
            set => SetValue(nameof(LineNumber), value);
        }


        /// <summary>
        /// Original policy.
        /// </summary>
        [DatabaseField]
        public virtual string OriginalPolicy
        {
            get => ValidationHelper.GetString(GetValue(nameof(OriginalPolicy)), String.Empty);
            set => SetValue(nameof(OriginalPolicy), value);
        }


        /// <summary>
        /// Referrer.
        /// </summary>
        [DatabaseField]
        public virtual string Referrer
        {
            get => ValidationHelper.GetString(GetValue(nameof(Referrer)), String.Empty);
            set => SetValue(nameof(Referrer), value);
        }


        /// <summary>
        /// Sample.
        /// </summary>
        [DatabaseField]
        public virtual string Sample
        {
            get => ValidationHelper.GetString(GetValue(nameof(Sample)), String.Empty);
            set => SetValue(nameof(Sample), value);
        }


        /// <summary>
        /// Source file.
        /// </summary>
        [DatabaseField]
        public virtual string SourceFile
        {
            get => ValidationHelper.GetString(GetValue(nameof(SourceFile)), String.Empty);
            set => SetValue(nameof(SourceFile), value);
        }


        /// <summary>
        /// Status code.
        /// </summary>
        [DatabaseField]
        public virtual int StatusCode
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(StatusCode)), 0);
            set => SetValue(nameof(StatusCode), value);
        }


        /// <summary>
        /// Reported at.
        /// </summary>
        [DatabaseField]
        public virtual DateTime ReportedAt
        {
            get => ValidationHelper.GetDateTime(GetValue(nameof(ReportedAt)), DateTimeHelper.ZERO_TIME);
            set => SetValue(nameof(ReportedAt), value);
        }


        /// <summary>
        /// Deletes the object using appropriate provider.
        /// </summary>
        protected override void DeleteObject()
        {
            Provider.Delete(this);
        }


        /// <summary>
        /// Updates the object using appropriate provider.
        /// </summary>
        protected override void SetObject()
        {
            Provider.Set(this);
        }


        /// <summary>
        /// Creates an empty instance of the <see cref="CSPViolationReportInfo"/> class.
        /// </summary>
        public CSPViolationReportInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="CSPViolationReportInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public CSPViolationReportInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}