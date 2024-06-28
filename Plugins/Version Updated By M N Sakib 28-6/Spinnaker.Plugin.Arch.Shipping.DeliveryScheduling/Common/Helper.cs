using Nop.Core.Infrastructure;
using Nop.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Common
{
    public static class Helper
    {
            public static string GetDescription(this Enum value)
            {
                var field = value.GetType().GetField(value.ToString());
                var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

                return attribute == null ? value.ToString() : attribute.Description;
            }
    }
}
