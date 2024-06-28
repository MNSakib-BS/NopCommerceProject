using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Stores;

namespace Nop.Plugin.Arch.Core.Domains.Forums;
public class ForumAdditional:BaseEntity,IStoreMappingSupported
{
    public int ForumId { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the entity is limited/restricted to certain stores
    /// </summary>
    public virtual bool LimitedToStores { get; set; }
}
