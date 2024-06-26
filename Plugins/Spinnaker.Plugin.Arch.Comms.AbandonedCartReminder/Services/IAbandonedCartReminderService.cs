using System.Threading.Tasks;
using Nop.Core;

namespace Spinnaker.Plugin.Arch.AbandonedCartReminder.Services
{
    public interface IAbandonedCartReminderService
    {
        Task<AbandonedCartReminderModel> CreateAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> AddFormAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> EditFormAsync(AbandonedCartReminderModel model);
        Task<AbandonedCartReminderModel> SubmitFormAsync(AbandonedCartReminderModel model);
        Task DeleteAbandonedCartReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder abandonedCartReminder);
        Task<AbandonedCartReminderModel> GetAbandonedCartReminderFormAsync(int id);
        Task<AbandonedCartReminderSearchModel> PrepareSearchModelAsync(AbandonedCartReminderSearchModel searchModel);
        Task<AbandonedCartReminderListModel> PrepareFormListModelAsync(AbandonedCartReminderSearchModel searchModel);
        Task<IPagedList<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>> SearchAsync(AbandonedCartReminderSearchModel model, int pageIndex = 0, int pageSize = 15);
        Task<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder> GetAbandonedCartReminderByIdAsync(int abandonedCartReminderId);
        Task<AbandonedCartReminderModel> PrepareReminderModelAsync(AbandonedCartReminderModel model);
        Task UpdateReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder reminder);

        #region "Scheduled Reminder"
        Task<List<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminder>> GetActiveRemindersAsync();
        Task<List<Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue>> GetActiveReminderQueueAsync();
        Task SetReminderAsync(Nop.Arch.Domain.AbandonedCartReminder.AbandonedCartReminderQueue reminderQueue);
        #endregion
    }
}
