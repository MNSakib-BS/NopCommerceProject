using Nop.Core.Domain.Messages;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Arch.Core.Models.AbandonedCartReminder
{

    #region AbandonedCartReminderListModel

    public record AbandonedCartReminderListModel : BasePagedListModel<AbandonedCartReminderGridModel>
    {
    }

    #endregion

    #region  AbandonedCartReminderSearchModel

    public record AbandonedCartReminderSearchModel : BaseSearchModel
    {
        #region "Properties"
        public string? Description { get; set; }
        public int? DelayBeforeSend { get; set; }
        public int DelayPeriodId { get; set; }
        public MessageDelayPeriod DelayPeriod
        {
            get => (MessageDelayPeriod)DelayPeriodId;
            set => DelayPeriodId = (int)value;
        }
        public bool LimitedToStores { get; set; }
        public bool Active { get; set; }

        #endregion
    }

    #endregion

    #region AbandonedCartReminderGridModel

    public record AbandonedCartReminderGridModel : BaseNopEntityModel
    {
        #region "Fields"

        public const string Field_Description = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.Description";
        public const string Field_DelayBeforeSend = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.DelayBeforeSend";
        public const string Field_DelayPeriodId = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.DelayPeriodId";
        public const string Field_DelayPeriod = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.DelayPeriod";
        public const string Field_LimitedToStores = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.LimitedToStores";
        public const string Field_Active = "Arch.Core.Models.AbandonedCartReminder.AbandonedCartReminderGridModel.Active";

        #endregion

        #region "Propeties"

        [NopResourceDisplayName(Field_Description)]
        public string? Description { get; set; }

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

        [NopResourceDisplayName(Field_Active)]
        public bool Active { get; set; }

        #endregion
    }

    #endregion

}
