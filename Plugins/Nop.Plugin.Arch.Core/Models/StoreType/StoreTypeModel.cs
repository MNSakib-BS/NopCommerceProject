using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Arch.Core.Models.StoreType
{
    public record StoreTypeModel : BaseNopEntityModel
    {
        #region "Ctor"
        public StoreTypeModel()
        {
            StoreTypeMappingModels = new List<StoreTypeMappingModel>();
            AddStoreTypeMappingModel = new StoreTypeMappingModel();
            StoreTypeMappingSearchModel = new StoreTypeMappingSearchModel();
            AddPictureModel = new StoreTypePictureModel();
            StoreTypePictureSearchModel = new StoreTypePictureSearchModel();
        }

        #endregion

        #region "Fields"

        public const string Field_Name = "Nop.Arch.Models.StoreTypeModel.Name";
        public const string Field_Image = "Nop.Arch.Models.StoreTypeModel.Image";

        #endregion

        #region "Properties"

        [NopResourceDisplayName(Field_Name)]
        public string Name { get; set; }
        [NopResourceDisplayName(Field_Image)]
        public int Image { get; set; }
        public string Store { get; set; }

        //Pictures
        public StoreTypePictureModel AddPictureModel { get; set; }
        public StoreTypePictureSearchModel StoreTypePictureSearchModel { get; set; }

        //StoreType - Store Mapping
        public StoreTypeMappingModel AddStoreTypeMappingModel { get; set; }
        public StoreTypeMappingSearchModel StoreTypeMappingSearchModel { get; set; }
        public IList<StoreTypeMappingModel> StoreTypeMappingModels { get; set; }

        #endregion
    }
}
