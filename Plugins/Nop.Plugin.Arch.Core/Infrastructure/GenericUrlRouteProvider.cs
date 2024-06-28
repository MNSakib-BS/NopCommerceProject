using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nop.Core.Domain.Localization;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Mvc.Routing;

namespace Arch.Core.Infrastructure
{
    /// <summary>
    /// Represents provider that provided generic routes
    /// </summary>
    //public partial class GenericUrlRouteProvider : IRouteProvider
    //{
    //    #region Methods

    //    /// <summary>
    //    /// Register routes
    //    /// </summary>
    //    /// <param name="endpointRouteBuilder">Route builder</param>
    //    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    //    {
    //        var pattern = "{SeName}";
    //        if (DataSettingsManager.DatabaseIsInstalled)
    //        {
    //            var localizationSettings = endpointRouteBuilder.ServiceProvider.GetRequiredService<LocalizationSettings>();
    //            if (localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
    //            {
    //                var langservice = endpointRouteBuilder.ServiceProvider.GetRequiredService<ILanguageService>();
    //                var languages = langservice.GetAllLanguages().ToList();
    //                pattern = "{language:lang=" + languages.FirstOrDefault().UniqueSeoCode + "}/{SeName}";
    //            }
    //        }
    //        endpointRouteBuilder.MapDynamicControllerRoute<SlugRouteTransformer>(pattern);

    //        //and default one
    //        endpointRouteBuilder.MapControllerRoute(
    //            name: "Default",
    //            pattern: "{controller=ArchHome}/{action=Index}/{id?}");

    //        //generic URLs
    //        endpointRouteBuilder.MapControllerRoute(
    //            name: "GenericUrl",
    //            pattern: "{GenericSeName}",
    //            new { controller = "ArchCommon", action = "GenericUrl" });

    //        //define this routes to use in UI views (in case if you want to customize some of them later)
    //        endpointRouteBuilder.MapControllerRoute("Product", pattern, 
    //            new { controller = "ArchProduct", action = "ProductDetails" });

    //        endpointRouteBuilder.MapControllerRoute("Category", pattern, 
    //            new { controller = "ArchCatalog", action = "Category" });

    //        endpointRouteBuilder.MapControllerRoute("Manufacturer", pattern, 
    //            new { controller = "ArchCatalog", action = "Manufacturer" });

    //        endpointRouteBuilder.MapControllerRoute("PromotionGroup", pattern,
    //            new { controller = "ArchCatalog", action = "PromotionGroup" });

    //        endpointRouteBuilder.MapControllerRoute("Vendor", pattern, 
    //            new { controller = "ArchCatalog", action = "Vendor" });
            
    //        endpointRouteBuilder.MapControllerRoute("NewsItem", pattern, 
    //            new { controller = "ArchNews", action = "NewsItem" });

    //        endpointRouteBuilder.MapControllerRoute("BlogPost", pattern, 
    //            new { controller = "ArchBlog", action = "BlogPost" });

    //        endpointRouteBuilder.MapControllerRoute("Topic", pattern, 
    //            new { controller = "ArchTopic", action = "TopicDetails" });

    //        //product tags
    //        endpointRouteBuilder.MapControllerRoute("ProductsByTag", pattern,
    //            new { controller = "ArchCatalog", action = "ProductsByTag" });
    //    }

    //    #endregion

    //    #region Properties

    //    /// <summary>
    //    /// Gets a priority of route provider
    //    /// </summary>
    //    /// <remarks>
    //    /// it should be the last route. we do not set it to -int.MaxValue so it could be overridden (if required)
    //    /// </remarks>
    //    public int Priority => -int.MaxValue;

    //    #endregion
    //}
}
