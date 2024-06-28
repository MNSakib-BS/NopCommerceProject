using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.News;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Vendors;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Themes;
using Nop.Services.Topics;
using Nop.Web.Factories;
using Nop.Web.Framework.Themes;
using Nop.Web.Framework.UI;
using Nop.Web.Infrastructure.Cache;
using Nop.Web.Models.Common;

namespace Nop.Plugin.Arch.Core.Factories;

/// <summary>
/// Represents the common models factory
/// </summary>
public partial class ArchCommonModelFactory : ICommonModelFactory
{
    #region Fields

    protected readonly BlogSettings _blogSettings;
    protected readonly CaptchaSettings _captchaSettings;
    protected readonly CatalogSettings _catalogSettings;
    protected readonly CommonSettings _commonSettings;
    protected readonly CustomerSettings _customerSettings;
    protected readonly DisplayDefaultFooterItemSettings _displayDefaultFooterItemSettings;
    protected readonly ForumSettings _forumSettings;
    protected readonly ICurrencyService _currencyService;
    protected readonly ICustomerService _customerService;
    protected readonly IForumService _forumService;
    protected readonly IGenericAttributeService _genericAttributeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ILanguageService _languageService;
    protected readonly ILocalizationService _localizationService;
    protected readonly INopFileProvider _fileProvider;
    protected readonly INopHtmlHelper _nopHtmlHelper;
    protected readonly IPermissionService _permissionService;
    protected readonly IPictureService _pictureService;
    protected readonly IShoppingCartService _shoppingCartService;
    protected readonly IStaticCacheManager _staticCacheManager;
    protected readonly IStoreContext _storeContext;
    protected readonly IThemeContext _themeContext;
    protected readonly IThemeProvider _themeProvider;
    protected readonly ITopicService _topicService;
    protected readonly IUrlRecordService _urlRecordService;
    protected readonly IWebHelper _webHelper;
    protected readonly IWorkContext _workContext;
    protected readonly LocalizationSettings _localizationSettings;
    protected readonly MediaSettings _mediaSettings;
    protected readonly NewsSettings _newsSettings;
    protected readonly RobotsTxtSettings _robotsTxtSettings;
    protected readonly SitemapSettings _sitemapSettings;
    protected readonly SitemapXmlSettings _sitemapXmlSettings;
    protected readonly StoreInformationSettings _storeInformationSettings;
    protected readonly VendorSettings _vendorSettings;

    #endregion

    #region Ctor

    public ArchCommonModelFactory(BlogSettings blogSettings,
        CaptchaSettings captchaSettings,
        CatalogSettings catalogSettings,
        CommonSettings commonSettings,
        CustomerSettings customerSettings,
        DisplayDefaultFooterItemSettings displayDefaultFooterItemSettings,
        ForumSettings forumSettings,
        ICurrencyService currencyService,
        ICustomerService customerService,
        IForumService forumService,
        IGenericAttributeService genericAttributeService,
        IHttpContextAccessor httpContextAccessor,
        ILanguageService languageService,
        ILocalizationService localizationService,
        INopFileProvider fileProvider,
        INopHtmlHelper nopHtmlHelper,
        IPermissionService permissionService,
        IPictureService pictureService,
        IShoppingCartService shoppingCartService,
        IStaticCacheManager staticCacheManager,
        IStoreContext storeContext,
        IThemeContext themeContext,
        IThemeProvider themeProvider,
        ITopicService topicService,
        IUrlRecordService urlRecordService,
        IWebHelper webHelper,
        IWorkContext workContext,
        LocalizationSettings localizationSettings,
        MediaSettings mediaSettings,
        NewsSettings newsSettings,
        RobotsTxtSettings robotsTxtSettings,
        SitemapSettings sitemapSettings,
        SitemapXmlSettings sitemapXmlSettings,
        StoreInformationSettings storeInformationSettings,
        VendorSettings vendorSettings)
    {
        _blogSettings = blogSettings;
        _captchaSettings = captchaSettings;
        _catalogSettings = catalogSettings;
        _commonSettings = commonSettings;
        _customerSettings = customerSettings;
        _displayDefaultFooterItemSettings = displayDefaultFooterItemSettings;
        _forumSettings = forumSettings;
        _currencyService = currencyService;
        _customerService = customerService;
        _forumService = forumService;
        _genericAttributeService = genericAttributeService;
        _httpContextAccessor = httpContextAccessor;
        _languageService = languageService;
        _localizationService = localizationService;
        _fileProvider = fileProvider;
        _nopHtmlHelper = nopHtmlHelper;
        _permissionService = permissionService;
        _pictureService = pictureService;
        _shoppingCartService = shoppingCartService;
        _staticCacheManager = staticCacheManager;
        _storeContext = storeContext;
        _themeContext = themeContext;
        _themeProvider = themeProvider;
        _topicService = topicService;
        _urlRecordService = urlRecordService;
        _webHelper = webHelper;
        _workContext = workContext;
        _mediaSettings = mediaSettings;
        _localizationSettings = localizationSettings;
        _newsSettings = newsSettings;
        _robotsTxtSettings = robotsTxtSettings;
        _sitemapSettings = sitemapSettings;
        _sitemapXmlSettings = sitemapXmlSettings;
        _storeInformationSettings = storeInformationSettings;
        _vendorSettings = vendorSettings;
    }

    #endregion

    #region Utilities

    /// <summary>
    /// Get the number of unread private messages
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the number of private messages
    /// </returns>
    protected virtual async Task<int> GetUnreadPrivateMessagesAsync()
    {
        var result = 0;
        var customer = await _workContext.GetCurrentCustomerAsync();
        if (_forumSettings.AllowPrivateMessages && !await _customerService.IsGuestAsync(customer))
        {
            var store = await _storeContext.GetCurrentStoreAsync();
            var privateMessages = await _forumService.GetAllPrivateMessagesAsync(store.Id,
                0, customer.Id, false, null, false, string.Empty, 0, 1);

            if (privateMessages.TotalCount > 0)
            {
                result = privateMessages.TotalCount;
            }
        }

        return result;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Prepare the logo model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the logo model
    /// </returns>
    public virtual async Task<LogoModel> PrepareLogoModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var model = new LogoModel
        {
            StoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name)
        };

        var cacheKey = _staticCacheManager.PrepareKeyForDefaultCache(NopModelCacheDefaults.StoreLogoPath
            , store, await _themeContext.GetWorkingThemeNameAsync(), _webHelper.IsCurrentConnectionSecured());
        model.LogoPath = await _staticCacheManager.GetAsync(cacheKey, async () =>
        {
            var logo = string.Empty;
            var logoPictureId = _storeInformationSettings.LogoPictureId;

            if (logoPictureId > 0)
                logo = await _pictureService.GetPictureUrlAsync(logoPictureId, showDefaultPicture: false);

            if (string.IsNullOrEmpty(logo))
            {
                //use default logo
                var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;
                var storeLocation = _mediaSettings.UseAbsoluteImagePath ? _webHelper.GetStoreLocation() : $"{pathBase}/";
                logo = $"{storeLocation}Themes/{await _themeContext.GetWorkingThemeNameAsync()}/Content/images/logo.png";
            }

            return logo;
        });

        return model;
    }

    /// <summary>
    /// Prepare the language selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the language selector model
    /// </returns>
    public virtual async Task<LanguageSelectorModel> PrepareLanguageSelectorModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var availableLanguages = (await _languageService
                .GetAllLanguagesAsync(storeId: store.Id))
            .Select(x => new LanguageModel
            {
                Id = x.Id,
                Name = x.Name,
                FlagImageFileName = x.FlagImageFileName,
            }).ToList();

        var model = new LanguageSelectorModel
        {
            CurrentLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
            AvailableLanguages = availableLanguages,
            UseImages = _localizationSettings.UseImagesForLanguageSelection
        };

        return model;
    }

    /// <summary>
    /// Prepare the currency selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the currency selector model
    /// </returns>
    public virtual async Task<CurrencySelectorModel> PrepareCurrencySelectorModelAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        var availableCurrencies = await (await _currencyService
                .GetAllCurrenciesAsync(storeId: store.Id))
            .SelectAwait(async x =>
            {
                //currency char
                var currencySymbol = !string.IsNullOrEmpty(x.DisplayLocale)
                    ? new RegionInfo(x.DisplayLocale).CurrencySymbol
                    : x.CurrencyCode;

                //model
                var currencyModel = new CurrencyModel
                {
                    Id = x.Id,
                    Name = await _localizationService.GetLocalizedAsync(x, y => y.Name),
                    CurrencySymbol = currencySymbol
                };

                return currencyModel;
            }).ToListAsync();

        var model = new CurrencySelectorModel
        {
            CurrentCurrencyId = (await _workContext.GetWorkingCurrencyAsync()).Id,
            AvailableCurrencies = availableCurrencies
        };

        return model;
    }

    /// <summary>
    /// Prepare the tax type selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the ax type selector model
    /// </returns>
    public virtual async Task<TaxTypeSelectorModel> PrepareTaxTypeSelectorModelAsync()
    {
        var model = new TaxTypeSelectorModel
        {
            CurrentTaxType = await _workContext.GetTaxDisplayTypeAsync()
        };

        return model;
    }

    /// <summary>
    /// Prepare the header links model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the header links model
    /// </returns>
    public virtual async Task<HeaderLinksModel> PrepareHeaderLinksModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var store = await _storeContext.GetCurrentStoreAsync();

        var unreadMessageCount = await GetUnreadPrivateMessagesAsync();
        var unreadMessage = string.Empty;
        var alertMessage = string.Empty;
        if (unreadMessageCount > 0)
        {
            unreadMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.TotalUnread"), unreadMessageCount);

            //notifications here
            if (_forumSettings.ShowAlertForPM &&
                !await _genericAttributeService.GetAttributeAsync<bool>(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, store.Id))
            {
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.NotifiedAboutNewPrivateMessagesAttribute, true, store.Id);
                alertMessage = string.Format(await _localizationService.GetResourceAsync("PrivateMessages.YouHaveUnreadPM"), unreadMessageCount);
            }
        }

        var model = new HeaderLinksModel
        {
            RegistrationType = _customerSettings.UserRegistrationType,
            IsAuthenticated = await _customerService.IsRegisteredAsync(customer),
            CustomerName = await _customerService.IsRegisteredAsync(customer) ? await _customerService.FormatUsernameAsync(customer) : string.Empty,
            ShoppingCartEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
            WishlistEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
            AllowPrivateMessages = await _customerService.IsRegisteredAsync(customer) && _forumSettings.AllowPrivateMessages,
            UnreadPrivateMessages = unreadMessage,
            AlertMessage = alertMessage,
        };
        //performance optimization (use "HasShoppingCartItems" property)
        if (customer.HasShoppingCartItems)
        {
            model.ShoppingCartItems = (await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id))
                .Sum(item => item.Quantity);

            model.WishlistItems = (await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.Wishlist, store.Id))
                .Sum(item => item.Quantity);
        }

        return model;
    }

    /// <summary>
    /// Prepare the admin header links model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the admin header links model
    /// </returns>
    public virtual async Task<AdminHeaderLinksModel> PrepareAdminHeaderLinksModelAsync()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();

        var model = new AdminHeaderLinksModel
        {
            ImpersonatedCustomerName = await _customerService.IsRegisteredAsync(customer) ? await _customerService.FormatUsernameAsync(customer) : string.Empty,
            IsCustomerImpersonated = _workContext.OriginalCustomerIfImpersonated != null,
            DisplayAdminLink = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel),
            EditPageUrl = _nopHtmlHelper.GetEditPageUrl()
        };

        return model;
    }

    /// <summary>
    /// Prepare the social model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the social model
    /// </returns>
    public virtual async Task<SocialModel> PrepareSocialModelAsync()
    {
        var model = new SocialModel
        {
            FacebookLink = _storeInformationSettings.FacebookLink,
            TwitterLink = _storeInformationSettings.TwitterLink,
            YoutubeLink = _storeInformationSettings.YoutubeLink,
            InstagramLink = _storeInformationSettings.InstagramLink,
            WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
            NewsEnabled = _newsSettings.Enabled,
        };

        return model;
    }

    /// <summary>
    /// Prepare the footer model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the footer model
    /// </returns>
    public virtual async Task<FooterModel> PrepareFooterModelAsync()
    {
        //footer topics
        var store = await _storeContext.GetCurrentStoreAsync();
        var topicModels = await (await _topicService.GetAllTopicsAsync(store.Id))
            .Where(t => t.IncludeInFooterColumn1 || t.IncludeInFooterColumn2 || t.IncludeInFooterColumn3)
            .SelectAwait(async t => new FooterModel.FooterTopicModel
            {
                Id = t.Id,
                Name = await _localizationService.GetLocalizedAsync(t, x => x.Title),
                SeName = await _urlRecordService.GetSeNameAsync(t),
                IncludeInFooterColumn1 = t.IncludeInFooterColumn1,
                IncludeInFooterColumn2 = t.IncludeInFooterColumn2,
                IncludeInFooterColumn3 = t.IncludeInFooterColumn3
            }).ToListAsync();

        //model
        var model = new FooterModel
        {
            StoreName = await _localizationService.GetLocalizedAsync(store, x => x.Name),
            WishlistEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableWishlist),
            ShoppingCartEnabled = await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableShoppingCart),
            SitemapEnabled = _sitemapSettings.SitemapEnabled,
            SearchEnabled = _catalogSettings.ProductSearchEnabled,
            WorkingLanguageId = (await _workContext.GetWorkingLanguageAsync()).Id,
            BlogEnabled = _blogSettings.Enabled,
            CompareProductsEnabled = _catalogSettings.CompareProductsEnabled,
            ForumEnabled = _forumSettings.ForumsEnabled,
            NewsEnabled = _newsSettings.Enabled,
            RecentlyViewedProductsEnabled = _catalogSettings.RecentlyViewedProductsEnabled,
            NewProductsEnabled = _catalogSettings.NewProductsEnabled,
            DisplayTaxShippingInfoFooter = _catalogSettings.DisplayTaxShippingInfoFooter,
            HidePoweredByNopCommerce = _storeInformationSettings.HidePoweredByNopCommerce,
            IsHomePage = _webHelper.GetStoreLocation().Equals(_webHelper.GetThisPageUrl(false), StringComparison.InvariantCultureIgnoreCase),
            AllowCustomersToApplyForVendorAccount = _vendorSettings.AllowCustomersToApplyForVendorAccount,
            AllowCustomersToCheckGiftCardBalance = _customerSettings.AllowCustomersToCheckGiftCardBalance && _captchaSettings.Enabled,
            Topics = topicModels,
            DisplaySitemapFooterItem = _displayDefaultFooterItemSettings.DisplaySitemapFooterItem,
            DisplayContactUsFooterItem = _displayDefaultFooterItemSettings.DisplayContactUsFooterItem,
            DisplayProductSearchFooterItem = _displayDefaultFooterItemSettings.DisplayProductSearchFooterItem,
            DisplayNewsFooterItem = _displayDefaultFooterItemSettings.DisplayNewsFooterItem,
            DisplayBlogFooterItem = _displayDefaultFooterItemSettings.DisplayBlogFooterItem,
            DisplayForumsFooterItem = _displayDefaultFooterItemSettings.DisplayForumsFooterItem,
            DisplayRecentlyViewedProductsFooterItem = _displayDefaultFooterItemSettings.DisplayRecentlyViewedProductsFooterItem,
            DisplayCompareProductsFooterItem = _displayDefaultFooterItemSettings.DisplayCompareProductsFooterItem,
            DisplayNewProductsFooterItem = _displayDefaultFooterItemSettings.DisplayNewProductsFooterItem,
            DisplayCustomerInfoFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerInfoFooterItem,
            DisplayCustomerOrdersFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerOrdersFooterItem,
            DisplayCustomerAddressesFooterItem = _displayDefaultFooterItemSettings.DisplayCustomerAddressesFooterItem,
            DisplayShoppingCartFooterItem = _displayDefaultFooterItemSettings.DisplayShoppingCartFooterItem,
            DisplayWishlistFooterItem = _displayDefaultFooterItemSettings.DisplayWishlistFooterItem,
            DisplayApplyVendorAccountFooterItem = _displayDefaultFooterItemSettings.DisplayApplyVendorAccountFooterItem
        };

        return model;
    }

    /// <summary>
    /// Prepare the contact us model
    /// </summary>
    /// <param name="model">Contact us model</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact us model
    /// </returns>
    public virtual async Task<ContactUsModel> PrepareContactUsModelAsync(ContactUsModel model, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            model.Email = customer.Email;
            model.FullName = await _customerService.GetCustomerFullNameAsync(customer);
        }

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;

        return model;
    }

    public override UltraContactUsModel PrepareUltraContactUsModel(UltraContactUsModel model, bool excludeProperties, AreaOfConcern area = AreaOfConcern.General)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.ContactUsModel == null)
            model.ContactUsModel = new ContactUsModel();

        if (!excludeProperties)
        {
            var customer = _workContext.CurrentCustomer;

            model.ContactUsModel = PrepareContactUsModel(model.ContactUsModel, false);

            model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
            model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
            model.ContactNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
        }

        model = GetContactUltraDropdownOptions(model, area);
        model.ContactUsModel.FullName = string.Empty;
        if (!string.IsNullOrWhiteSpace(model.FirstName) && !string.IsNullOrWhiteSpace(model.LastName))
            model.ContactUsModel.FullName = $"{model.FirstName} {model.LastName}";
        else
        {
            if (!string.IsNullOrWhiteSpace(model.FirstName))
                model.ContactUsModel.FullName = model.FirstName;

            if (!string.IsNullOrWhiteSpace(model.LastName))
                model.ContactUsModel.FullName = model.LastName;
        }

        return model;
    }

    public override UltraContactUsModel GetContactUltraDropdownOptions(UltraContactUsModel model, AreaOfConcern area = AreaOfConcern.General)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        model.AreaOfConcernOptions = Enum.GetValues(typeof(AreaOfConcern)).Cast<AreaOfConcern>().Select(v => new SelectListItem
        {
            Text = _localizationService.GetLocalizedEnum(v),
            Value = ((int)v).ToString(),
            Selected = v == area

        }).ToList();



        //Getting Provinces
        var country = _countryService.GetCountryByTwoLetterIsoCode("ZA");
        if (country != null)
        {
            var provinces = _stateProvinceService.GetStateProvincesByCountryId(country.Id);
            if (provinces.Any())
            {
                model.AvailableProvince = provinces.Select(v => new SelectListItem
                {
                    Text = v.Name,
                    Value = v.Id.ToString(),
                }).ToList();
            }
        }

        return model;

    }

    public override AboutUsNavigationModel PrepareAboutUsNavigationModel(int selectedTabId = 0)
    {
        var model = new AboutUsNavigationModel();
        var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);
        var aboutUsTopic = _topicService.GetTopicBySystemName("AboutUs");
        model.AboutUsNavigationItems.Add(new AboutUsNavigationItemModel
        {
            UrlRoute = urlHelper.RouteUrl("Topic", new { SeName = _urlRecordService.GetSeName(aboutUsTopic) }),
            Title = _localizationService.GetResource("AboutUs.AboutUltra"),
            Tab = AboutUsNavigationEnum.AboutUltra,
            ItemClass = "about-us-about-ultra"
        });

        model.AboutUsNavigationItems.Add(new AboutUsNavigationItemModel
        {
            UrlRoute = urlHelper.RouteUrl("UltraCareers"),
            Title = _localizationService.GetResource("AboutUs.Careers"),
            Tab = AboutUsNavigationEnum.Careers,
            ItemClass = "about-us-careers"
        });

        model.SelectedTab = (AboutUsNavigationEnum)selectedTabId;

        return model;
    }

    /// <summary>
    /// Prepare the contact vendor model
    /// </summary>
    /// <param name="model">Contact vendor model</param>
    /// <param name="vendor">Vendor</param>
    /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the contact vendor model
    /// </returns>
    public virtual async Task<ContactVendorModel> PrepareContactVendorModelAsync(ContactVendorModel model, Vendor vendor, bool excludeProperties)
    {
        ArgumentNullException.ThrowIfNull(model);

        ArgumentNullException.ThrowIfNull(vendor);

        if (!excludeProperties)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            model.Email = customer.Email;
            model.FullName = await _customerService.GetCustomerFullNameAsync(customer);
        }

        model.SubjectEnabled = _commonSettings.SubjectFieldOnContactUsForm;
        model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnContactUsPage;
        model.VendorId = vendor.Id;
        model.VendorName = await _localizationService.GetLocalizedAsync(vendor, x => x.Name);

        return model;
    }

    /// <summary>
    /// Prepare the sitemap model
    /// </summary>
    /// <param name="pageModel">Sitemap page model</param>
    /// <returns>Sitemap model</returns>
    public override SitemapModel PrepareSitemapModel(SitemapPageModel pageModel)
    {
        var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapPageModelKey,
            _workContext.WorkingLanguage,
            _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer),
            _storeContext.CurrentStore);

        var cachedModel = _staticCacheManager.Get(cacheKey, () =>
        {
            //get URL helper
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            var model = new SitemapModel();

            //prepare common items
            var commonGroupTitle = _localizationService.GetResource("Sitemap.General");

            //home page
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = _localizationService.GetResource("Homepage"),
                Url = urlHelper.RouteUrl("Homepage")
            });

            //search
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = _localizationService.GetResource("Search"),
                Url = urlHelper.RouteUrl("ProductSearch")
            });

            //news
            if (_newsSettings.Enabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("News"),
                    Url = urlHelper.RouteUrl("NewsArchive")
                });
            }

            //blog
            if (_blogSettings.Enabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Blog"),
                    Url = urlHelper.RouteUrl("Blog")
                });
            }

            //forums
            if (_forumSettings.ForumsEnabled)
            {
                model.Items.Add(new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _localizationService.GetResource("Forum.Forums"),
                    Url = urlHelper.RouteUrl("Boards")
                });
            }

            //contact us
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = _localizationService.GetResource("ContactUs"),
                Url = urlHelper.RouteUrl("ContactUs")
            });

            //customer info
            model.Items.Add(new SitemapModel.SitemapItemModel
            {
                GroupTitle = commonGroupTitle,
                Name = _localizationService.GetResource("Account.MyAccount"),
                Url = urlHelper.RouteUrl("CustomerInfo")
            });

            //at the moment topics are in general category too
            if (_sitemapSettings.SitemapIncludeTopics)
            {
                var topics = _topicService.GetAllTopics(storeId: _storeContext.CurrentStore.Id)
                    .Where(topic => topic.IncludeInSitemap);

                model.Items.AddRange(topics.Select(topic => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = commonGroupTitle,
                    Name = _hasMultipleLanguages ? _localizationService.GetLocalized(topic, x => x.Title) : topic.Title,
                    Url = urlHelper.RouteUrl("Topic", new { SeName = _urlRecordService.GetSeName(topic) })
                }));
            }

            //blog posts
            if (_sitemapSettings.SitemapIncludeBlogPosts && _blogSettings.Enabled)
            {
                var blogPostsGroupTitle = _localizationService.GetResource("Sitemap.BlogPosts");
                var blogPosts = _blogService.GetAllBlogPosts(storeId: _storeContext.CurrentStore.Id)
                    .Where(p => p.IncludeInSitemap);

                model.Items.AddRange(blogPosts.Select(post => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = blogPostsGroupTitle,
                    Name = post.Title,
                    Url = urlHelper.RouteUrl("BlogPost", new { SeName = _urlRecordService.GetSeName(post) })
                }));
            }

            //news
            if (_sitemapSettings.SitemapIncludeNews && _newsSettings.Enabled)
            {
                var newsGroupTitle = _localizationService.GetResource("Sitemap.News");
                var news = _newsService.GetAllNews(storeId: _storeContext.CurrentStore.Id);
                model.Items.AddRange(news.Select(newsItem => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = newsGroupTitle,
                    Name = newsItem.Title,
                    Url = urlHelper.RouteUrl("NewsItem", new { SeName = _urlRecordService.GetSeName(newsItem) })
                }));
            }

            //categories
            if (_sitemapSettings.SitemapIncludeCategories)
            {
                var categoriesGroupTitle = _localizationService.GetResource("Sitemap.Categories");
                var categories = _categoryService.GetAllCategories(storeId: _storeContext.CurrentStore.Id);
                model.Items.AddRange(categories.Select(category => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = categoriesGroupTitle,
                    Name = _hasMultipleLanguages ? _localizationService.GetLocalized(category, x => x.Name) : category.Name,
                    Url = urlHelper.RouteUrl("Category", new { SeName = _urlRecordService.GetSeName(category) })
                }));
            }

            //manufacturers
            if (_sitemapSettings.SitemapIncludeManufacturers)
            {
                var manufacturersGroupTitle = _localizationService.GetResource("Sitemap.Manufacturers");
                var manufacturers = _manufacturerService.GetAllManufacturers(storeId: _storeContext.CurrentStore.Id);
                model.Items.AddRange(manufacturers.Select(manufacturer => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = manufacturersGroupTitle,
                    Name = _hasMultipleLanguages ? _localizationService.GetLocalized(manufacturer, x => x.Name) : manufacturer.Name,
                    Url = urlHelper.RouteUrl("Manufacturer", new { SeName = _urlRecordService.GetSeName(manufacturer) })
                }));
            }

            //products
            if (_sitemapSettings.SitemapIncludeProducts)
            {
                var productsGroupTitle = _localizationService.GetResource("Sitemap.Products");
                var products = _productService.SearchProducts(storeId: _storeContext.CurrentStore.Id, visibleIndividuallyOnly: true);
                model.Items.AddRange(products.Select(product => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = productsGroupTitle,
                    Name = _hasMultipleLanguages ? _localizationService.GetLocalized(product, x => x.Name) : product.Name,
                    Url = urlHelper.RouteUrl("Product", new { SeName = _urlRecordService.GetSeName(product) })
                }));
            }

            //product tags
            if (_sitemapSettings.SitemapIncludeProductTags)
            {
                var productTagsGroupTitle = _localizationService.GetResource("Sitemap.ProductTags");
                var productTags = _productTagService.GetAllProductTags(storeId: _storeContext.ActiveStoreScopeConfiguration);
                model.Items.AddRange(productTags.Select(productTag => new SitemapModel.SitemapItemModel
                {
                    GroupTitle = productTagsGroupTitle,
                    Name = _hasMultipleLanguages ? _localizationService.GetLocalized(productTag, x => x.Name) : productTag.Name,
                    Url = urlHelper.RouteUrl("ProductsByTag", new { SeName = _urlRecordService.GetSeName(productTag) })
                }));
            }

            return model;
        });

        //prepare model with pagination
        _sitemapSettings.SitemapPageSize = (_sitemapSettings.SitemapPageSize == 0) ? 10 : _sitemapSettings.SitemapPageSize;
        pageModel.PageSize = Math.Max(pageModel.PageSize, _sitemapSettings.SitemapPageSize);
        pageModel.PageNumber = Math.Max(pageModel.PageNumber, 1);

        var pagedItems = new PagedList<SitemapModel.SitemapItemModel>(cachedModel.Items, pageModel.PageNumber - 1, pageModel.PageSize);
        var sitemapModel = new SitemapModel { Items = pagedItems };
        sitemapModel.PageModel.LoadPagedList(pagedItems);

        return sitemapModel;
    }

    /// <summary>
    /// Get the sitemap in XML format
    /// </summary>
    /// <param name="id">Sitemap identifier; pass null to load the first sitemap or sitemap index file</param>
    /// <returns>Sitemap as string in XML format</returns>
    public override string PrepareSitemapXml(int? id)
    {
        var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.SitemapSeoModelKey, id,
            _workContext.WorkingLanguage,
            _customerService.GetCustomerRoleIds(_workContext.CurrentCustomer), _storeContext.GlobalStore);

        var siteMap = _staticCacheManager.Get(cacheKey, () => _sitemapGenerator.Generate(id));

        return siteMap;
    }

    /// <summary>
    /// Prepare the store theme selector model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the store theme selector model
    /// </returns>
    public virtual async Task<StoreThemeSelectorModel> PrepareStoreThemeSelectorModelAsync()
    {
        var model = new StoreThemeSelectorModel();

        var currentTheme = await _themeProvider.GetThemeBySystemNameAsync(await _themeContext.GetWorkingThemeNameAsync());
        model.CurrentStoreTheme = new StoreThemeModel
        {
            Name = currentTheme?.SystemName,
            Title = currentTheme?.FriendlyName
        };

        model.AvailableStoreThemes = (await _themeProvider.GetThemesAsync()).Select(x => new StoreThemeModel
        {
            Name = x.SystemName,
            Title = x.FriendlyName
        }).ToList();

        return model;
    }

    /// <summary>
    /// Prepare the favicon model
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the favicon model
    /// </returns>
    public virtual Task<FaviconAndAppIconsModel> PrepareFaviconAndAppIconsModelAsync()
    {
        var model = new FaviconAndAppIconsModel
        {
            HeadCode = _commonSettings.FaviconAndAppIconsHeadCode
        };

        return Task.FromResult(model);
    }

    /// <summary>
    /// Get robots.txt file
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the robots.txt file as string
    /// </returns>
    public virtual async Task<string> PrepareRobotsTextFileAsync()
    {
        var sb = new StringBuilder();

        //if robots.custom.txt exists, let's use it instead of hard-coded data below
        var robotsFilePath = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsCustomFileName);
        if (_fileProvider.FileExists(robotsFilePath))
        {
            //the robots.txt file exists
            var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsFilePath, Encoding.UTF8);
            sb.Append(robotsFileContent);
        }
        else
        {
            sb.AppendLine("User-agent: *");

            //sitemap
            if (_sitemapXmlSettings.SitemapXmlEnabled && _robotsTxtSettings.AllowSitemapXml)
                sb.AppendLine($"Sitemap: {_webHelper.GetStoreLocation()}sitemap.xml");
            else
                sb.AppendLine("Disallow: /sitemap.xml");

            //host
            sb.AppendLine($"Host: {_webHelper.GetStoreLocation()}");

            //usual paths
            foreach (var path in _robotsTxtSettings.DisallowPaths)
                sb.AppendLine($"Disallow: {path}");

            //localizable paths (without SEO code)
            foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                sb.AppendLine($"Disallow: {path}");

            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var store = await _storeContext.GetCurrentStoreAsync();
                //URLs are localizable. Append SEO code
                foreach (var language in await _languageService.GetAllLanguagesAsync(storeId: store.Id))
                    if (_robotsTxtSettings.DisallowLanguages.Contains(language.Id))
                        sb.AppendLine($"Disallow: /{language.UniqueSeoCode}*");
                    else
                        foreach (var path in _robotsTxtSettings.LocalizableDisallowPaths)
                            sb.AppendLine($"Disallow: /{language.UniqueSeoCode}{path}");
            }

            foreach (var additionsRule in _robotsTxtSettings.AdditionsRules)
                sb.AppendLine(additionsRule);

            //load and add robots.txt additions to the end of file.
            var robotsAdditionsFile = _fileProvider.Combine(_fileProvider.MapPath("~/wwwroot"), RobotsTxtDefaults.RobotsAdditionsFileName);
            if (_fileProvider.FileExists(robotsAdditionsFile))
            {
                sb.AppendLine();
                var robotsFileContent = await _fileProvider.ReadAllTextAsync(robotsAdditionsFile, Encoding.UTF8);
                sb.Append(robotsFileContent);
            }
        }

        return sb.ToString();
    }

    #endregion
}