using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Models.StoreType;
using Nop.Services.Localization;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Areas.Admin.Infrastructure;
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

        list.Add(new KeyValuePair<string, string>(StoreTypeModel.Field_Name, nameof(StoreTypeModel.Name).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypeGridModel.Field_Name, nameof(StoreTypeGridModel.Name).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypeModel.Field_Image, nameof(StoreTypeModel.Image).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypePictureModel.Field_Image, nameof(StoreTypePictureModel.PictureId).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypePictureModel.Field_Image, nameof(StoreTypePictureModel.PictureUrl).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypeMappingModel.Field_Name, nameof(StoreTypeMappingModel.Name).Humanize(LetterCasing.Title)));
        list.Add(new KeyValuePair<string, string>(StoreTypeMappingGridModel.Field_Name, nameof(StoreTypeMappingGridModel.Name).Humanize(LetterCasing.Title)));

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
