using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Common;
public class DisplayDefaultMenuItemAdditionalSettings : ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to display "promotions" menu item
    /// </summary>
    public bool DisplayPromotionsMenuItem { get; set; }
}
