using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace Spinnaker.Plugin.Arch.Shipping.DeliveryScheduling.Models
{
    public class AvailableDeliveryDateTimesTableModel
    {
        public AvailableDeliveryDateTimesTableModel() => this.Times = (IList<AvailableDeliveryTimesTableModel>)new List<AvailableDeliveryTimesTableModel>();

        public DateTime DeliveryDateOnUtc { get; set; }

        public string DeliveryDateString { get; set; }

        [NotMapped]
        public IList<AvailableDeliveryTimesTableModel> Times { get; set; }
    }
}
