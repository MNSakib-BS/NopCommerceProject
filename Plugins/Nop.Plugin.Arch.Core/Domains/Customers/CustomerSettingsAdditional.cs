using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public class CustomerSettingsAdditional:ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether to use 'Global Debtor Number' for all stores
    /// </summary>
    public bool UseGlobalDebtor { get; set; }

    /// <summary>
    /// Gets ot sets the loyalty card number values
    /// </summary>
    public bool LoyaltyCardNumberEnabled { get; set; }

    public int LoyaltyCardNumberMaxLength { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the customer is a debtor
    /// </summary>
    public bool IsDebtorEnabled { get; set; }
}
