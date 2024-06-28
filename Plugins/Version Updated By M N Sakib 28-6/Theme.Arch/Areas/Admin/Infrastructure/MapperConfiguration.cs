using AutoMapper;
using Nop.Core.Infrastructure.Mapper;
using Nop.Plugin.NopStation.Theme.Arch.Areas.Admin.Models;

namespace Nop.Plugin.NopStation.Theme.Arch.Areas.Admin.Infrastructure;
public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public int Order => 1;
    public MapperConfiguration()
    {
        CreateMap<ArchThemeSettings, ConfigurationModel>()
                .ForMember(model => model.CustomCss_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableImageLazyLoad_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FooterEmail_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.FooterLogoPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LazyLoadPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLogoAtPageFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowSupportedCardsPictureAtPageFooter_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.SupportedCardsPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ShowLoginPictureAtLoginPage_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.LoginPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.CustomProperties, options => options.Ignore())
                .ForMember(model => model.EnableDescriptionBoxOne_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxOneTitle_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxOneText_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxOnePictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxOneUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableDescriptionBoxTwo_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxTwoTitle_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxTwoText_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxTwoPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxTwoUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableDescriptionBoxThree_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxThreeTitle_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxThreeText_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxThreePictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxThreeUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.EnableDescriptionBoxFour_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxFourTitle_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxFourText_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxFourPictureId_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.DescriptionBoxFourUrl_OverrideForStore, options => options.Ignore())
                .ForMember(model => model.ActiveStoreScopeConfiguration, options => options.Ignore());
        CreateMap<ConfigurationModel, ArchThemeSettings>();
    }
}
