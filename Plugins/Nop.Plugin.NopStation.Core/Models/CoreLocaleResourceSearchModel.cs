using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Localization;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.NopStation.Core.Models
{
    public record CoreLocaleResourceSearchModel : BaseSearchModel
    {
        #region Ctor

        public CoreLocaleResourceSearchModel()
        {
            AvailableLanguages = new List<SelectListItem>();
            AvailablePlugins = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.NopStation.Core.Resources.List.SearchLanguageId")]
        public int SearchLanguageId { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Core.Resources.List.SearchResourceName")]
        public string SearchResourceName { get; set; }

        [NopResourceDisplayName("Admin.NopStation.Core.Resources.List.SearchPluginSystemName")]
        public string SearchPluginSystemName { get; set; }

        public IList<SelectListItem> AvailableLanguages { get; set; }
        public IList<SelectListItem> AvailablePlugins { get; set; }

        #endregion
    }
}
