using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data.Mapping;

namespace Spinnaker.Plugin.Arch.Shipping.AddressValidator.Mapping;
public partial class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>();

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
}
