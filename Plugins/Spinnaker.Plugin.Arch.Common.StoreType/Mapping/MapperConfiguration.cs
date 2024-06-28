using AutoMapper;
using LinqToDB.Tools;
using Nop.Plugin.Arch.Core.Domains.StoreTypes;
using Nop.Plugin.Arch.Core.Models.StoreType;
using Nop.Core.Infrastructure.Mapper;
using Nop.Web.Extensions;
using NUglify.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spinnaker.Plugin.Arch.Common.StoreType.Mapping
{
    public class MapperConfiguration : Profile, IOrderedMapperProfile
    {
        public int Order => 100;

        public MapperConfiguration()
        {
            CreateMap<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType, StoreTypeModel>()
                .ForMember(model => model.AddPictureModel, options => options.Ignore())
                .ForMember(model => model.StoreTypePictureSearchModel, options => options.Ignore())
                .ForMember(model => model.StoreTypeMappingModels, options => options.Ignore())
                .ForMember(model => model.StoreTypeMappingSearchModel, options => options.Ignore());
            CreateMap<StoreTypeModel, Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();

            CreateMap<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType, StoreTypePictureModel>()
                .ForMember(model => model.OverrideAltAttribute, options => options.Ignore())
                .ForMember(model => model.OverrideTitleAttribute, options => options.Ignore())
                .ForMember(model => model.PictureUrl, options => options.Ignore());


            CreateMap<Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType, StoreTypeMappingModel>();
            CreateMap<StoreTypeMappingModel, Nop.Plugin.Arch.Core.Domains.StoreTypes.StoreType>();

            CreateMap<StoreTypeMapping, StoreTypeMappingGridModel>();
            CreateMap<StoreTypeMappingGridModel, StoreTypeMapping>();
        }
    }
}
