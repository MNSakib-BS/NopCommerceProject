using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Messages;
public class EmailAccountAdditional: BaseEntity
{
    public int EmailAccountId { get; set; }
    public int StoreId { get; set; }
}
