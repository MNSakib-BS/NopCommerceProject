using System.ComponentModel;

namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Domain
{

    public enum DeliveryDayEnum
    {
        /// <summary>
        /// Monday (default)
        /// </summary>
        [Description("Monday")]
        Monday = 0,

        /// <summary>
        /// Tuesday
        /// </summary>
        [Description("Tuesday")]
        Tuesday = 1,

        /// <summary>
        /// Wednesday
        /// </summary>
        [Description("Wednesday")]
        Wednesday = 2,

        /// <summary>
        /// Thursday
        /// </summary>
        [Description("Thursday")]
        Thursday = 3,

        /// <summary>
        /// Friday
        /// </summary>
        [Description("Friday")]
        Friday = 4,

        /// <summary>
        /// Saturday
        /// </summary>
        [Description("Saturday")]
        Saturday = 5,

        /// <summary>
        /// Sunday
        /// </summary>
        [Description("Sunday")]
        Sunday = 6
    }
}