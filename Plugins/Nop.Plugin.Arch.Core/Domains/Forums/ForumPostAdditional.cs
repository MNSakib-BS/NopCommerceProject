using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Stores;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Forums;
public class ForumPostAdditional : BaseEntity, IStoreMappingSupported
{
    public int ForumPostId { get; set; }
    public virtual bool LimitedToStores { get; set; }
}
