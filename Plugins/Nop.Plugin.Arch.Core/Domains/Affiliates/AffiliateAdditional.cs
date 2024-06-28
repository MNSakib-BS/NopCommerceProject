using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Affiliates;
public partial class AffiliateAdditional : BaseEntity, IStoreMappingSupported
{
    public int AffiliateId { get; set; }
    public bool LimitedToStores { get; set; }
}
