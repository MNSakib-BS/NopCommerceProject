using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data.Mapping;

namespace Spinnaker.Plugin.Arch.Common.HealthMonitor.Mapping;
public class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>();

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
}
