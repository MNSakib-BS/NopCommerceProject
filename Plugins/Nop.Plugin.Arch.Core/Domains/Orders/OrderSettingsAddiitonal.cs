using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class OrderSettingsAddiitonal: ISettings
{
    /// <summary>
    /// Gets or sets a value indicating whether customer can share a wallet across stores
    /// </summary>
    public bool IsWalletSharedAcrossStores { get; set; }
    /// <summary>
    /// Gets or sets the otp length
    /// </summary>
    public int OTPLength { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether order logging is disabled
    /// </summary>
    public bool OrderLoggingDisabled { get; set; }

    /// <summary>
    /// Reduces the number of number of steps in the checkout process
    /// </summary>
    public bool CheckoutStepReduction { get; set; }
}
