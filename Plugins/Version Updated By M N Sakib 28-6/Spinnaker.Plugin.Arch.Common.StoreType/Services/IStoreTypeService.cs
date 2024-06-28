using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Models.StoreType;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Services
{


    public interface IStoreTypeService
    {
        Task<StoreTypeModel> CreateAsync(StoreTypeModel model);
        Task<StoreTypeModel> AddFormAsync(StoreTypeModel model);
        Task<StoreTypeModel> EditFormAsync(StoreTypeModel model);
        Task<StoreTypeModel> SubmitFormAsync(StoreTypeModel model);
        Task DeleteStoreTypeAsync(Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType storeType);
        Task<StoreTypeModel> GetStoreTypeFormAsync(int id);
        Task<StoreTypeSearchModel> PrepareSearchModelAsync(StoreTypeSearchModel searchModel);
        Task<StoreTypeListModel> PrepareFormListModelAsync(StoreTypeSearchModel searchModel);
        Task<IPagedList<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>> SearchAsync(StoreTypeSearchModel model, int pageIndex = 0, int pageSize = 15);
        Task<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType> GetStoreTypeByIdAsync(int storeTypeId);

        #region "Store Type Mapping"
        Task<StoreTypeMapping> GetMappingByStoreTypeIdAsync(int storeTypeId, int storeId);
        Task<StoreTypeMapping> GetMappingByStoreIdAsync(int storeId);
        Task InsertStoreTypeMappingAsync(StoreTypeMapping storeTypeMapping);
        Task<StoreTypeMappingListModel> PrepareStoreTypeMappingListModelAsync(StoreTypeMappingSearchModel searchModel, Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType storeType);
        Task<StoreTypeMapping> GetStoreTypeMappingByIdAsync(int storeTypeMappingId);
        Task<List<SelectListItem>> GetStoresAsync();
        Task DeleteStoreTypeMappingAsync(StoreTypeMapping storeTypeMapping);
        #endregion
    }
}