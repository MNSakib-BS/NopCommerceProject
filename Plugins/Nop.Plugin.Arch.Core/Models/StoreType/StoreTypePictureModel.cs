using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Arch.Core.Models.StoreType
{
    #region "StoreTypePictureListModel"

    public partial record StoreTypePictureListModel : BasePagedListModel<StoreTypePictureModel>
    {
    }

    #endregion

    #region "StoreTypePictureModel"

    public record StoreTypePictureModel : BaseNopEntityModel
    {
        #region "Fields"

        public const string Field_Image = "Nop.Arch.Models.StoreType.StoreTypePictureModel.Picture";

        #endregion

        #region "Properties"

        public int StoreTypeId { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName(Field_Image)]
        public int PictureId { get; set; }

        [NopResourceDisplayName(Field_Image)]
        public string? PictureUrl { get; set; }

        public string? OverrideAltAttribute { get; set; }

        public string? OverrideTitleAttribute { get; set; }
        #endregion
    }

    #endregion

    #region "StoreTypePictureSearchModel"

    public partial record StoreTypePictureSearchModel : BaseSearchModel
    {
        #region Properties
        public int StoreTypeId { get; set; }

        #endregion
    }

    #endregion

}
