using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Payments;
public class PaymentInfoModel
{
    public string? EntityId { get; set; }
    public string? Currency { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentType { get; set; }
    public string? Url { get; set; }
    public string? BearerToken { get; set; }
}
