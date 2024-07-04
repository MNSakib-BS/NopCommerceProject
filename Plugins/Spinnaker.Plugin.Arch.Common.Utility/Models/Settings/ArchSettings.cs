using Nop.Core.Configuration;
using Nop.Web.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.Utility.Models.Settings
{
    public class ArchSettings : ISettings
    {
        public string BaseDomain { get; set; }
        public string StoreLinkWidgetZone { get; set; }
        public ArchSettings()
        {
            BaseDomain = ".archestore.online";
            StoreLinkWidgetZone = PublicWidgetZones.HeaderMenuAfter;
        }
    }
}
