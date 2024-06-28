using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Common;
public class SitemapAdditionalSettings:ISettings
{
    public bool SitemapGlobalStoreOnly { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether to include promotion groups to sitemap
    /// </summary>
    public bool SitemapIncludePromotionGroups { get; set; }
}
