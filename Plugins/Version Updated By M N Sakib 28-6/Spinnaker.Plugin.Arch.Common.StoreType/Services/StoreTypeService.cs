using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Models.StoreType;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Media;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Stores;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Services
{
    public class StoreTypeService : IStoreTypeService
    {
        private readonly IRepository<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType> StoreTypeRepository;
        private readonly IPictureService PictureService;
        private readonly IStoreService StoreService;
        private readonly IRepository<StoreTypeMapping> StoreTypeMappingRepository;


        public StoreTypeService(IRepository<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType> storeTypeRepository,
                                   IPictureService pictureService, IStoreService storeService, IRepository<StoreTypeMapping> storeTypeMappingRepository
                                 )
        {
            StoreTypeRepository = storeTypeRepository;
            PictureService = pictureService;
            StoreTypeMappingRepository = storeTypeMappingRepository;
            StoreService = storeService;

        }

        public async Task<StoreTypeModel> CreateAsync(StoreTypeModel model)
        {
            var entity = model.ToEntity<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();
            await StoreTypeRepository.InsertAsync(entity);
            return model;
        }

        public async Task<StoreTypeModel> AddFormAsync(StoreTypeModel model)
        {
            var form = model.ToEntity<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();
            if (model.AddPictureModel != null)
            {
                form.PictureId = model.AddPictureModel.PictureId;
            }
            await StoreTypeRepository.InsertAsync(form);
            return form.ToModel<StoreTypeModel>();
        }

        public async Task<StoreTypeModel> EditFormAsync(StoreTypeModel model)
        {
            var form = model.ToEntity<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();
            if (model.AddPictureModel != null)
            {
                form.PictureId = model.AddPictureModel.PictureId;
            }
            await StoreTypeRepository.UpdateAsync(form);
            return form.ToModel<StoreTypeModel>();
        }

        public async Task<StoreTypeModel> SubmitFormAsync(StoreTypeModel model)
        {
            var item = model.ToEntity<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();
            await StoreTypeRepository.InsertAsync(item);
            return item.ToModel<StoreTypeModel>();
        }

        public async Task DeleteStoreTypeAsync(Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType storeType)
        {
            if (storeType == null)
                throw new ArgumentNullException(nameof(storeType));

            List<StoreTypeMapping> storeTypeMappings = (await GetStoreMappingsByStoreTypeIdAsync(storeType.Id)).ToList();
            if (storeTypeMappings != null && storeTypeMappings.Any())
            {
                foreach (StoreTypeMapping storeTypeMapping in storeTypeMappings)
                {
                    await StoreTypeMappingRepository.DeleteAsync(storeTypeMapping);
                }
            }

            await StoreTypeRepository.DeleteAsync(storeType);
        }

        public async Task<StoreTypeModel> GetStoreTypeFormAsync(int id)
        {
            var entity = await StoreTypeRepository.GetByIdAsync(id);
            var entityModel = entity?.ToModel<StoreTypeModel>();

            if (entityModel != null)
            {
                var customerPictureModel = entity.ToModel<StoreTypePictureModel>();
                if (customerPictureModel.PictureId != 0)
                {
                    var picture = await PictureService.GetPictureByIdAsync(entity.PictureId) ?? throw new Exception("Picture cannot be loaded");
                    customerPictureModel.PictureUrl =(await PictureService.GetPictureUrlAsync( picture)).ToString();
                    customerPictureModel.OverrideAltAttribute = picture.AltAttribute;
                    customerPictureModel.OverrideTitleAttribute = picture.TitleAttribute;

                    entityModel.AddPictureModel = customerPictureModel;
                }
            }

            return entityModel;
        }

        public async Task<StoreTypeSearchModel> PrepareSearchModelAsync(StoreTypeSearchModel searchModel)
        {
            searchModel.SetGridPageSize();
            return searchModel;
        }

        public async Task<StoreTypeListModel> PrepareFormListModelAsync(StoreTypeSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));
            var forms = await SearchAsync(searchModel);

            var model = new StoreTypeListModel().PrepareToGrid(searchModel, forms, () =>
            {
                return forms.Select(item =>
                {
                    var formModel = new StoreTypeGridModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                    };

                    return formModel;
                });
            });

            return model;
        }

        public async Task<IPagedList<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>> SearchAsync(StoreTypeSearchModel model, int pageIndex = 0, int pageSize = 15)
        {
            var query = StoreTypeRepository.Table;

            if (!string.IsNullOrWhiteSpace(model.Name))
                query = query.Where(p => p.Name.Contains(model.Name));

            return await Task.FromResult(new PagedList<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>(query.ToList(), pageIndex, pageSize, false));
        }

        public async Task<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType> GetStoreTypeByIdAsync(int storeTypeId)
        {
            if (storeTypeId == 0)
                return null;
            return await StoreTypeRepository.GetByIdAsync(storeTypeId, cache => default);
        }

        #region "StoreTypeMapping"
        public async Task<StoreTypeMapping> GetMappingByStoreTypeIdAsync(int storeTypeId, int storeId)
        {
            return await StoreTypeMappingRepository.Table
                .Where(x => x.StoreTypeId == storeTypeId && x.StoreId == storeId)
                .FirstOrDefaultAsync();
        }

        public async Task<StoreTypeMapping> GetMappingByStoreIdAsync(int storeId)
        {
            return await StoreTypeMappingRepository.Table
                .Where(x => x.StoreId == storeId)
                .FirstOrDefaultAsync();
        }

        public async Task InsertStoreTypeMappingAsync(StoreTypeMapping storeTypeMapping)
        {
            if (storeTypeMapping == null)
                throw new ArgumentNullException(nameof(storeTypeMapping));

            await StoreTypeMappingRepository.InsertAsync(storeTypeMapping);
        }

        public async Task<List<SelectListItem>> GetStoresAsync()
        {
            var storeList = await StoreService.GetAllStoresAsync();
            var storeItemList = new List<SelectListItem>();

            foreach (var store in storeList)
            {
                storeItemList.Add(new SelectListItem() { Value = store.Id.ToString(), Text = store.Name });
            }

            return storeItemList;
        }

        public async Task<IList<StoreTypeMapping>> GetStoreMappingsByStoreTypeIdAsync(int storeTypeId)
        {
            var query = StoreTypeMappingRepository.Table
                .Where(x => x.StoreTypeId == storeTypeId)
                .OrderBy(x => x.Id);
            return await query.ToListAsync();
        }
        public async Task<StoreTypeMappingListModel> PrepareStoreTypeMappingListModelAsync(StoreTypeMappingSearchModel searchModel, Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType storeType)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (storeType == null)
                throw new ArgumentNullException(nameof(storeType));

            var storeTypeMappings = await GetStoreMappingsByStoreTypeIdAsync(storeType.Id);
            var pagedStoreTypeMappings = storeTypeMappings.ToPagedList(searchModel);

            var model = await new StoreTypeMappingListModel().PrepareToGridAsync(searchModel, pagedStoreTypeMappings, () =>
            {
                return pagedStoreTypeMappings.SelectAwait(async storeTypeMapping =>
                {
                    var storeTypeMappingModel = storeTypeMapping.ToModel<StoreTypeMappingGridModel>();

                    if (storeTypeMapping.StoreId != 0)
                    {
                        var store =await StoreService.GetStoreByIdAsync(storeTypeMapping.StoreId) ?? throw new Exception("Store cannot be loaded");
                        storeTypeMappingModel.StoreTypeId = storeType.Id;
                        storeTypeMappingModel.Name = store.Name;
                        storeTypeMappingModel.StoreId = storeTypeMapping.StoreId;
                    }

                    return storeTypeMappingModel;
                });
            });

            return model;
        }

        public async Task<StoreTypeMapping> GetStoreTypeMappingByIdAsync(int storeTypeMappingId)
        {
            if (storeTypeMappingId == 0)
                return null;

            return await StoreTypeMappingRepository.GetByIdAsync(storeTypeMappingId, cache => default);
        }

        public async Task DeleteStoreTypeMappingAsync(StoreTypeMapping storeTypeMapping)
        {
            if (storeTypeMapping == null)
                throw new ArgumentNullException(nameof(storeTypeMapping));

            await StoreTypeMappingRepository.DeleteAsync(storeTypeMapping);
        }        
        #endregion
    }
}
