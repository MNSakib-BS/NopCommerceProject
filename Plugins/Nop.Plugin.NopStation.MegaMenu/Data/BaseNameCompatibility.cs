using System;
using System.Collections.Generic;
using Nop.Data.Mapping;
using Nop.Plugin.NopStation.MegaMenu.Domains;

namespace Nop.Plugin.NopStation.MegaMenu.Data
{
    public class BaseNameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new Dictionary<Type, string>
        {
            { typeof(CategoryIcon), "NS_CategoryIcon" },
        };

        public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>
        {
        };
    }
}
