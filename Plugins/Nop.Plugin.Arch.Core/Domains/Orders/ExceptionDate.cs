using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
public class ExceptionDate:BaseEntity
{
    public string Title { get; set; }
    public DateTime DateTime { get; set; }
    public int StoreId { get; set; }
    public bool Deleted { get; set; }

}

