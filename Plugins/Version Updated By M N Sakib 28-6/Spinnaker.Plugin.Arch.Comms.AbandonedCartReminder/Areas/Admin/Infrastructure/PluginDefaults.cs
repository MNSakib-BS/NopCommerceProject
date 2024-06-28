using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Models.AbandonedCartReminder;
using Nop.Services.Localization;

namespace Spinnaker.Plugin.Arch.Comms.AbandonedCartReminder.Areas.Admin.Infrastructure;
public interface IPluginDefaults
{
    Task UpdatePluginResourcesAsync(string currentVersion, string targetVersion);
}

public class PluginDefaults : IPluginDefaults
{
    public PluginDefaults()
    {

    }

    private List<KeyValuePair<string, string>> PluginResources(string currentVersion, string targetVersion)
    {
        var list = new List<KeyValuePair<string, string>>();

        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_Description, nameof(AbandonedCartReminderModel.Description).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_DelayBeforeSend, nameof(AbandonedCartReminderModel.DelayBeforeSend).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_DelayPeriod, nameof(AbandonedCartReminderModel.DelayPeriod).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_DelayPeriodId, nameof(AbandonedCartReminderModel.DelayPeriodId).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_Active, nameof(AbandonedCartReminderModel.Active).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderModel.Field_LimitedToStores, nameof(AbandonedCartReminderModel.SelectedStoreIds).Humanize(LetterCasing.Title)));

        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderGridModel.Field_Description, nameof(AbandonedCartReminderGridModel.Description).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderGridModel.Field_DelayBeforeSend, nameof(AbandonedCartReminderGridModel.DelayBeforeSend).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderGridModel.Field_DelayPeriod, nameof(AbandonedCartReminderGridModel.DelayPeriod).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderGridModel.Field_DelayPeriodId, nameof(AbandonedCartReminderGridModel.DelayPeriodId).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(AbandonedCartReminderGridModel.Field_Active, nameof(AbandonedCartReminderGridModel.Active).Humanize(LetterCasing.Title)));

        return list;
    }

    public async Task UpdatePluginResourcesAsync(string currentVersion, string targetVersion)
    {
        var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
        var keyValuePairs = PluginResources(currentVersion, targetVersion);
        foreach (var keyValuePair in keyValuePairs)
        {
             await localizationService.AddOrUpdateLocaleResourceAsync(keyValuePair.Key, keyValuePair.Value);
        }
    }
}
