using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public class EFTPayPaymentSettings : ISettings
{
    public string? EntityId { get; set; }
    public string? Currency { get; set; }
    public string? PaymentType { get; set; }
    public string? Url { get; set; }
    public string? BearerToken { get; set; }

}
