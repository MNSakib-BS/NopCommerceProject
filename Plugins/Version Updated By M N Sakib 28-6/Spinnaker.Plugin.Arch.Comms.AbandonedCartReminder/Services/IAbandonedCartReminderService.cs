using System.Threading.Tasks;
using Nop.Core;
using Nop.Plugin.Arch.Core.Models.AbandonedCartReminder;

namespace Spinnaker.Plugin.Arch.AbandonedCartReminder.Services
{
    public interface IAbandonedCartReminderService
    {
        Task<AbandonedCartReminderModel> CreateAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> AddFormAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> EditFormAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> SubmitFormAsync(AbandonedCartReminderModel model);
        Task DeleteAbandonedCartReminderAsync(Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder abandonedCartReminder);
        Task<AbandonedCartReminderModel> GetAbandonedCartReminderFormAsync(int id);
        Task<AbandonedCartReminderSearchModel> PrepareSearchModelAsync(AbandonedCartReminderSearchModel searchModel);
        Task<AbandonedCartReminderListModel> PrepareFormListModelAsync(AbandonedCartReminderSearchModel searchModel);
        Task<IPagedList<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder>> SearchAsync(AbandonedCartReminderSearchModel model, int pageIndex = 0, int pageSize = 15);
        Task<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder> GetAbandonedCartReminderByIdAsync(int abandonedCartReminderId);
        Task<AbandonedCartReminderModel> PrepareReminderModelAsync(AbandonedCartReminderModel model);
        Task UpdateReminderAsync(Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder reminder);

        #region "Scheduled Reminder"
        Task<List<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminder>> GetActiveRemindersAsync();
        Task<List<Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminderQueue>> GetActiveReminderQueueAsync();
        Task SetReminderAsync(Nop.Plugin.Arch.Core.Domains.AbandonedCartReminders.AbandonedCartReminderQueue reminderQueue);
        #endregion
    }
}
