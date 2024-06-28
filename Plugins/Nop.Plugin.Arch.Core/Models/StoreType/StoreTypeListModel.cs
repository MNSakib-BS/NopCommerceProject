using Nop.Web.Framework.Models;

namespace Nop.Plugin.Arch.Core.Models.StoreType
{
    #region "StoreTypeListModel"
    public record StoreTypeListModel : BasePagedListModel<StoreTypeGridModel>
    {
    }

    #endregion

    #region "StoreTypeSearchModel"

    public record StoreTypeSearchModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public int Image { get; set; }
    }

    #endregion

    #region "StoreTypeGridModel"

    public record StoreTypeGridModel : BaseNopEntityModel
    {
        #region "Fields"

        public const string Field_Name = "Nop.Arch.StoreType.Models.StoreTypeGridModel.Name";
        public const string Field_Image = "Nop.Arch.StoreType.Models.StoreTypeGridModel.Image";

        #endregion

        public string? Name { get; set; }
        public int Image { get; set; }
    }

    #endregion

}
