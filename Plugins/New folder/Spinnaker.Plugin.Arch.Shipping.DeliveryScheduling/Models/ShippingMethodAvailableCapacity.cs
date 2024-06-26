using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class ShippingMethodAvailableCapacity
    {
        public ShippingMethodAvailableCapacity()
        {
            TimeRange = new List<string>();

            Capacities = new Dictionary<int, (int, int)>();

            Days = new Dictionary<string, int>();
        }
        public Dictionary<string, int> Days { get; set; }

        public List<string> TimeRange { get; set; }

        public Dictionary<int, (int, int)> Capacities { get; set; }
       
    }
}
