using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Common;
public class SitemapXmlAdditionalSettings:ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to include promotion groups to sitemap.xml
    /// </summary>
    public bool SitemapXmlIncludePromotionGroups { get; set; }
}
