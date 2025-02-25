using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using CMS;
using CMS.DataEngine;
using CMS.Helpers;
using XperienceCommunity.CSP;

[assembly: RegisterObjectType(typeof(CSPConfigurationInfo), CSPConfigurationInfo.OBJECT_TYPE)]

namespace XperienceCommunity.CSP
{
    /// <summary>
    /// Data container class for <see cref="CSPConfigurationInfo"/>.
    /// </summary>
    public partial class CSPConfigurationInfo : AbstractInfo<CSPConfigurationInfo, IInfoProvider<CSPConfigurationInfo>>, IInfoWithId
    {
        /// <summary>
        /// Object type.
        /// </summary>
        public const string OBJECT_TYPE = "xperiencecommunity.cspconfiguration";


        /// <summary>
        /// Type information.
        /// </summary>
        public static readonly ObjectTypeInfo TYPEINFO = new ObjectTypeInfo(typeof(IInfoProvider<CSPConfigurationInfo>), OBJECT_TYPE, "XperienceCommunity.CSPConfiguration", "CSPConfigurationID", null, null, null, null, null, null, null)
        {
            TouchCacheDependencies = true,
            DependsOn = new List<ObjectDependency>()
            {
                new ObjectDependency("CSPConfigurationChannelID", "cms.channel", ObjectDependencyEnum.Required),
            },
        };


        /// <summary>
        /// CSP configuration ID.
        /// </summary>
        [DatabaseField]
        public virtual int CSPConfigurationID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(CSPConfigurationID)), 0);
            set => SetValue(nameof(CSPConfigurationID), value);
        }


        /// <summary>
        /// CSP configuration channel ID.
        /// </summary>
        [DatabaseField]
        public virtual int CSPConfigurationChannelID
        {
            get => ValidationHelper.GetInteger(GetValue(nameof(CSPConfigurationChannelID)), 0);
            set => SetValue(nameof(CSPConfigurationChannelID), value);
        }


        /// <summary>
        /// CSP configuration source url.
        /// </summary>
        [DatabaseField]
        public virtual string CSPConfigurationSourceUrl
        {
            get => ValidationHelper.GetString(GetValue(nameof(CSPConfigurationSourceUrl)), String.Empty);
            set => SetValue(nameof(CSPConfigurationSourceUrl), value);
        }


        /// <summary>
        /// CSP configuration directives.
        /// </summary>
        [DatabaseField]
        public virtual string CSPConfigurationDirectives
        {
            get => ValidationHelper.GetString(GetValue(nameof(CSPConfigurationDirectives)), String.Empty);
            set => SetValue(nameof(CSPConfigurationDirectives), value);
        }


        /// <summary>
        /// CSP configuration enabled.
        /// </summary>
        [DatabaseField]
        public virtual bool CSPConfigurationEnabled
        {
            get => ValidationHelper.GetBoolean(GetValue(nameof(CSPConfigurationEnabled)), false);
            set => SetValue(nameof(CSPConfigurationEnabled), value);
        }


        /// <summary>
        /// CSP configuration use nonce.
        /// </summary>
        [DatabaseField]
        public virtual bool CSPConfigurationUseNonce
        {
            get => ValidationHelper.GetBoolean(GetValue(nameof(CSPConfigurationUseNonce)), false);
            set => SetValue(nameof(CSPConfigurationUseNonce), value);
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
        /// Creates an empty instance of the <see cref="CSPConfigurationInfo"/> class.
        /// </summary>
        public CSPConfigurationInfo()
            : base(TYPEINFO)
        {
        }


        /// <summary>
        /// Creates a new instances of the <see cref="CSPConfigurationInfo"/> class from the given <see cref="DataRow"/>.
        /// </summary>
        /// <param name="dr">DataRow with the object data.</param>
        public CSPConfigurationInfo(DataRow dr)
            : base(TYPEINFO, dr)
        {
        }
    }
}