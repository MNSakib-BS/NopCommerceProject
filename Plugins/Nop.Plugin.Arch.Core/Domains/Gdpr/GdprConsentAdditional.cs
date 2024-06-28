using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Gdpr;
public class GdprConsentAdditional:BaseEntity,IStoreMappingSupported
{
    public int GdprConsentId { get; set; }
    public virtual bool LimitedToStores { get; set; }
}
