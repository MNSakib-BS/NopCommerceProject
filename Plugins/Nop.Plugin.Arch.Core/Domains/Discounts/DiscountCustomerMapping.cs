using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Discounts;

namespace Nop.Plugin.Arch.Core.Domains.Discounts;
/// <summary>
/// Represents a discount-customer mapping class
/// </summary>
public class DiscountCustomerMapping : DiscountMapping
{
    /// <summary>
    /// Gets or sets the customer identifier
    /// </summary>
    public override int EntityId { get; set; }
}
