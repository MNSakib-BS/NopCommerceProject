using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;

namespace Nop.Plugin.Arch.Core.Domains.StoreTypes;
public class StoreType : BaseEntity
{
    public string Name { get; set; }
    public int PictureId { get; set; }
}