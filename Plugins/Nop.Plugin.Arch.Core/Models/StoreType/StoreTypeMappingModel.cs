using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Arch.Core.Models.StoreType
{
    #region "StoreTypeMappingModel"

    public record StoreTypeMappingModel : BaseNopEntityModel
    {
        #region "Fields"

        public const string Field_Name = "Nop.Arch.StoreType.Models.StoreTypeMappingModel.Name";

        #endregion

        [NopResourceDisplayName(Field_Name)]
        public string? Name { get; set; }
        public int StoreTypeId { get; set; }
        public int StoreId { get; set; }
    }

    #endregion

    #region "StoreTypeMappingListModel"

    public record StoreTypeMappingListModel : BasePagedListModel<StoreTypeMappingGridModel>
    {
    }

    #endregion

    #region "StoreTypeMappingSearchModel"

    public record StoreTypeMappingSearchModel : BaseSearchModel
    {
        public string? Name { get; set; }
        public int StoreTypeId { get; set; }
        public int StoreId { get; set; }
    }

    #endregion

    #region "StoreTypeMappingGridModel"

    public record StoreTypeMappingGridModel : BaseNopEntityModel
    {
        #region "Fields"

        public const string Field_Name = "Nop.Arch.StoreType.Models.StoreTypeMappingGridModel.Name";

        #endregion

        [NopResourceDisplayName(Field_Name)]
        public string? Name { get; set; }
        public int StoreTypeId { get; set; }
        public int StoreId { get; set; }
    }

    #endregion

}
