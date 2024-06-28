using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.StoreTypes;
public class StoreTypeMapping : BaseEntity
{
    public int StoreTypeId { get; set; }
    public int StoreId { get; set; }
}