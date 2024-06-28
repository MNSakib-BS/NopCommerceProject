using Nop.Core.Configuration;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.DeliveryScheduling
{
    public class DeliverySchedulingSettings : ISettings
    {
        public string WidgetZones { get; set; }

        public int HourOffset { get; set; }

        public int DayOffset { get; set; }

    }
}