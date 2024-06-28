using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Arch.Core.Models.AbandonedCartReminder
{
    public record AbandonedCartReminderModel : BaseNopEntityModel
    {
        #region "Ctor"

        public AbandonedCartReminderModel()
        {
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        #endregion

        #region "Fields"


        public const string Field_Description = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.Description";
        public const string Field_DelayBeforeSend = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.DelayBeforeSend";
        public const string Field_DelayPeriodId = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.DelayPeriodId";
        public const string Field_DelayPeriod = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.DelayPeriod";
        public const string Field_LimitedToStores = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.LimitedToStores";
        public const string Field_Active = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderModel.Active";


        #endregion

        #region "Properties"

        [NopResourceDisplayName(Field_Description)]
        public string Description { get; set; }

        [NopResourceDisplayName(Field_DelayBeforeSend)]
        public int? DelayBeforeSend { get; set; }

        [NopResourceDisplayName(Field_DelayPeriodId)]
        public int DelayPeriodId { get; set; }

        [NopResourceDisplayName(Field_DelayPeriod)]
        public MessageDelayPeriod DelayPeriod
        {
            get => (MessageDelayPeriod)DelayPeriodId;
            set => DelayPeriodId = (int)value;
        }

        [NopResourceDisplayName(Field_LimitedToStores)]
        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        [NopResourceDisplayName(Field_Active)]
        public bool Active { get; set; }

        #endregion
    }
}
