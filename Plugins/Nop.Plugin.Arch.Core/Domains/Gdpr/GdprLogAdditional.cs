using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Gdpr;
public class GdprLogAdditional : BaseEntity, IStoreMappingSupported
{
    public int GdprLogId { get; set; }
    public virtual bool LimitedToStores { get; set; }

}
