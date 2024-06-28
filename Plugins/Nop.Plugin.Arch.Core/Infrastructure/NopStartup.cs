using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arch.Core.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Areas.Admin.Factories;
using Nop.Plugin.Arch.Core.Factories;
using Nop.Plugin.Arch.Core.Services.AffiliateAdditionals;
using Nop.Web.Areas.Admin.Factories;

namespace Nop.Plugin.Arch.Core.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 1001;

    public void Configure(IApplicationBuilder application)
    {
        
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //register custom services
        services.AddScoped<IAffiliateAdditionalService, AffiliateAdditionalService>();


        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ViewLocationExpander());
        });

        //admin factories
        services.AddScoped<IActivityLogModelFactory, ArchActivityLogModelFactory>();
        services.AddScoped<IAddressAttributeModelFactory, ArchAddressAttributeModelFactory>();
        services.AddScoped<IAffiliateModelFactory, ArchAffiliateModelFactory>();
        services.AddScoped<IBlogModelFactory, Areas.Admin.Factories.ArchBlogModelFactory>();
        services.AddScoped<ICampaignModelFactory, ArchCampaignModelFactory>();
        services.AddScoped<ICategoryModelFactory, ArchCategoryModelFactory>();
        services.AddScoped<ICheckoutAttributeModelFactory, ArchCheckoutAttributeModelFactory>();
        services.AddScoped<ICommonModelFactory, Areas.Admin.Factories.ArchCommonModelFactory>();
        services.AddScoped<ICountryModelFactory, Areas.Admin.Factories.ArchCountryModelFactory>();
        services.AddScoped<ICurrencyModelFactory, ArchCurrencyModelFactory>();
        services.AddScoped<ICustomerAttributeModelFactory, ArchCustomerAttributeModelFactory>();
        services.AddScoped<ICustomerModelFactory, Areas.Admin.Factories.ArchCustomerModelFactory>();
        services.AddScoped<ICustomerRoleModelFactory, ArchCustomerRoleModelFactory>();
        services.AddScoped<IDiscountModelFactory, ArchDiscountModelFactory>();
        services.AddScoped<Nop.Web.Areas.Admin.Factories.IDriverModelFactory, Areas.Admin.Factories.ArchDriverModelFactory>();
        services.AddScoped<IEmailAccountModelFactory, ArchEmailAccountModelFactory>();
        services.AddScoped<IForumModelFactory, Areas.Admin.Factories.ArchForumModelFactory>();
        services.AddScoped<IGiftCardModelFactory, Areas.Admin.Factories.ArchGiftCardModelFactory>();
        services.AddScoped<IHomeModelFactory, ArchHomeModelFactory>();
        services.AddScoped<IManufacturerModelFactory, ArchManufacturerModelFactory>();
        services.AddScoped<IMessageTemplateModelFactory, ArchMessageTemplateModelFactory>();
        services.AddScoped<INewsModelFactory, ArchNewsModelFactory>();
        services.AddScoped<IOrderModelFactory, Areas.Admin.Factories.ArchOrderModelFactory>();
        services.AddScoped<IPollModelFactory, ArchPollModelFactory>();
        services.AddScoped<IProductAttributeModelFactory, ArchProductAttributeModelFactory>();
        services.AddScoped<IProductModelFactory, Areas.Admin.Factories.ArchProductModelFactory>();
        services.AddScoped<IProductReviewModelFactory, ArchProductReviewModelFactory>();
        services.AddScoped<Nop.Web.Areas.Admin.Factories.IPromotionGroupModelFactory, Areas.Admin.Factories.ArchPromotionGroupModelFactory>();
        services.AddScoped<IQueuedEmailModelFactory, ArchQueuedEmailModelFactory>();
        services.AddScoped<Nop.Web.Areas.Admin.Factories.IRecommendedListModelFactory, Areas.Admin.Factories.ArchRecommendedListModelFactory>();
        services.AddScoped<IReportModelFactory, ArchReportModelFactory>();
        services.AddScoped<IReturnRequestModelFactory, Areas.Admin.Factories.ArchReturnRequestModelFactory>();
        services.AddScoped<IReviewTypeModelFactory, ArchReviewTypeModelFactory>();
        services.AddScoped<IScheduleTaskModelFactory, ArchScheduleTaskModelFactory>();
        services.AddScoped<ISettingModelFactory, ArchSettingModelFactory>();
        services.AddScoped<IShippingModelFactory, ArchShippingModelFactory>();
        services.AddScoped<ISpecificationAttributeModelFactory, ArchSpecificationAttributeModelFactory>();
        services.AddScoped<IStoreModelFactory, ArchStoreModelFactory>();
        services.AddScoped<ITemplateModelFactory, ArchTemplateModelFactory>();
        services.AddScoped<ITopicModelFactory, Areas.Admin.Factories.ArchTopicModelFactory>();
        services.AddScoped<IVendorAttributeModelFactory, ArchVendorAttributeModelFactory>();
        services.AddScoped<IVendorModelFactory, Areas.Admin.Factories.ArchVendorModelFactory>();

        //factories
        services.AddScoped<Nop.Web.Factories.IAddressModelFactory, ArchAddressModelFactory>();
        services.AddScoped<Nop.Web.Factories.IBlogModelFactory, Factories.ArchBlogModelFactory>();
        services.AddScoped<Nop.Web.Factories.ICheckoutModelFactory, ArchCheckoutModelFactory>();
        services.AddScoped<Nop.Web.Factories.ICommonModelFactory, Factories.ArchCommonModelFactory>();
        services.AddScoped<Nop.Web.Factories.ICountryModelFactory, Factories.ArchCountryModelFactory>();
        services.AddScoped<Nop.Web.Factories.ICustomerModelFactory, Factories.ArchCustomerModelFactory>();
        services.AddScoped<Nop.Web.Factories.IForumModelFactory, Factories.ArchForumModelFactory>();
        services.AddScoped<Nop.Web.Factories.IOrderModelFactory, Factories.ArchOrderModelFactory>();
        services.AddScoped<Nop.Web.Factories.IProductModelFactory, Factories.ArchProductModelFactory>();
        services.AddScoped<Nop.Web.Factories.IProfileModelFactory, ArchProfileModelFactory>();
        services.AddScoped<Nop.Web.Factories.IRecommendedListModelFactory, Factories.ArchRecommendedListModelFactory>();
        services.AddScoped<Nop.Web.Factories.IReturnRequestModelFactory, Factories.ArchReturnRequestModelFactory>();
        services.AddScoped<Nop.Web.Factories.IShoppingCartModelFactory, ArchShoppingCartModelFactory>();
        services.AddScoped<Nop.Web.Factories.ITopicModelFactory, Factories.ArchTopicModelFactory>();
        services.AddScoped<Nop.Web.Factories.IVendorModelFactory, Factories.ArchVendorModelFactory>();


    }
}
