using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Catalog;
/// <summary>
/// Arch Weight type indicator
/// </summary>
public enum WeightType
{
    None = 0,

    /// <summary>
    /// Variable weight
    /// </summary>
    PerWeight = 1,

    /// <summary>
    /// Variable unit
    /// </summary>
    PerUnit = 2,
}
