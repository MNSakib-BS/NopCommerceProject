using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Mobile;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Arch.Core.Areas.Admin.Factories
{
    /// <summary>
    /// Represents the customer model factory implementation
    /// </summary>
    public partial class ArchDriverModelFactory : DriverModelFactory
    {
        #region Fields

        private readonly CustomerSettings _customerSettings;
        private readonly MobileSettings _mobileSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IPictureService _pictureService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly MediaSettings _mediaSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IStoreMappingSupportedModelFactory _storeMappingSupportedModelFactory;
        private readonly IWorkContext _workContext;
        private readonly IMessageTemplateService _messageTemplateService;

        #endregion

        #region Ctor

        public ArchDriverModelFactory(CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            GdprSettings gdprSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAffiliateService affiliateService,
            IAuthenticationPluginManager authenticationPluginManager,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IPictureService pictureService,
            IStoreContext storeContext,
            IStoreService storeService,
            MediaSettings mediaSettings,
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings,
            IStoreMappingSupportedModelFactory storeMappingSupportedModelFactory,
            IWorkContext workContext,
            MobileSettings mobileSettings,
            IMessageTemplateService messageTemplateService)
            : base( customerSettings,
             dateTimeSettings,
             gdprSettings,
             forumSettings,
             aclSupportedModelFactory,
             affiliateService,
             authenticationPluginManager,
             baseAdminModelFactory,
             customerAttributeParser,
             customerAttributeService,
             customerService,
             dateTimeHelper,
             externalAuthenticationService,
             genericAttributeService,
             localizationService,
             newsLetterSubscriptionService,
             pictureService,
             storeContext,
             storeService,
             mediaSettings,
             rewardPointsSettings,
             taxSettings,
             storeMappingSupportedModelFactory,
             workContext,
             mobileSettings,
             messageTemplateService)
        {
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _gdprSettings = gdprSettings;
            _forumSettings = forumSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _affiliateService = affiliateService;
            _authenticationPluginManager = authenticationPluginManager;
            _baseAdminModelFactory = baseAdminModelFactory;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _externalAuthenticationService = externalAuthenticationService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _pictureService = pictureService;
            _storeContext = storeContext;
            _storeService = storeService;
            _mediaSettings = mediaSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _taxSettings = taxSettings;
            _storeMappingSupportedModelFactory = storeMappingSupportedModelFactory;
            _workContext = workContext;
            _mobileSettings = mobileSettings;
            _messageTemplateService = messageTemplateService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare the reward points model to add to the customer
        /// </summary>
        /// <param name="model">Reward points model to add to the customer</param>
        protected override void PrepareAddRewardPointsToCustomerModel(AddRewardPointsToCustomerModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Message = string.Empty;
            model.ActivatePointsImmediately = true;
            model.StoreId = _storeContext.CurrentStore.Id;

            //prepare available stores
            _baseAdminModelFactory.PrepareStores(model.AvailableStores, false);
        }

        /// <summary>
        /// Prepare customer associated external authorization models
        /// </summary>
        /// <param name="models">List of customer associated external authorization models</param>
        /// <param name="customer">Customer</param>
        protected override void PrepareAssociatedExternalAuthModels(IList<CustomerAssociatedExternalAuthModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            foreach (var record in _externalAuthenticationService.GetCustomerExternalAuthenticationRecords(customer))
            {
                var method = _authenticationPluginManager.LoadPluginBySystemName(record.ProviderSystemName);
                if (method == null)
                    continue;

                models.Add(new CustomerAssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = method.PluginDescriptor.FriendlyName
                });
            }
        }

        /// <summary>
        /// Prepare customer attribute models
        /// </summary>
        /// <param name="models">List of customer attribute models</param>
        /// <param name="customer">Customer</param>
        protected override void PrepareCustomerAttributeModels(IList<CustomerModel.CustomerAttributeModel> models, Customer customer)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            //get available customer attributes
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes(storeId: _storeContext.ActiveStoreScopeConfiguration);
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerModel.CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.Name,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new CustomerModel.CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.Name,
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(attributeValueModel);
                    }
                }

                //set already selected attributes
                if (customer != null)
                {
                    var selectedCustomerAttributes = _genericAttributeService
                        .GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedCustomerAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                        case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedCustomerAttributes))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedCustomerAttributes, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                        case AttributeControlType.Datepicker:
                        case AttributeControlType.ColorSquares:
                        case AttributeControlType.ImageSquares:
                        case AttributeControlType.FileUpload:
                        default:
                            //not supported attribute control types
                            break;
                    }
                }

                models.Add(attributeModel);
            }
        }

        /// <summary>
        /// Prepare reward points search model
        /// </summary>
        /// <param name="searchModel">Reward points search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Reward points search model</returns>
        protected override CustomerRewardPointsSearchModel PrepareRewardPointsSearchModel(CustomerRewardPointsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer address search model
        /// </summary>
        /// <param name="searchModel">Customer address search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer address search model</returns>
        protected override CustomerAddressSearchModel PrepareCustomerAddressSearchModel(CustomerAddressSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer order search model
        /// </summary>
        /// <param name="searchModel">Customer order search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer order search model</returns>
        protected override CustomerOrderSearchModel PrepareCustomerOrderSearchModel(CustomerOrderSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer shopping cart search model
        /// </summary>
        /// <param name="searchModel">Customer shopping cart search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer shopping cart search model</returns>
        protected override CustomerShoppingCartSearchModel PrepareCustomerShoppingCartSearchModel(CustomerShoppingCartSearchModel searchModel,
            Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare available shopping cart types (search shopping cart by default)
            searchModel.ShoppingCartTypeId = (int)ShoppingCartType.ShoppingCart;
            _baseAdminModelFactory.PrepareShoppingCartTypes(searchModel.AvailableShoppingCartTypes, false);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer activity log search model
        /// </summary>
        /// <param name="searchModel">Customer activity log search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer activity log search model</returns>
        protected override CustomerActivityLogSearchModel PrepareCustomerActivityLogSearchModel(CustomerActivityLogSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions search model</returns>
        protected override CustomerBackInStockSubscriptionSearchModel PrepareCustomerBackInStockSubscriptionSearchModel(
            CustomerBackInStockSubscriptionSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer back in stock subscriptions search model
        /// </summary>
        /// <param name="searchModel">Customer back in stock subscriptions search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer back in stock subscriptions search model</returns>
        protected override CustomerAssociatedExternalAuthRecordsSearchModel PrepareCustomerAssociatedExternalAuthRecordsSearchModel(
            CustomerAssociatedExternalAuthRecordsSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();
            //prepare external authentication records
            PrepareAssociatedExternalAuthModels(searchModel.AssociatedExternalAuthRecords, customer);

            return searchModel;
        }
        
        #endregion

        #region Methods

        /// <summary>
        /// Prepare customer search model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>Customer search model</returns>
        public override CustomerSearchModel PrepareCustomerSearchModel(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            searchModel.AvatarEnabled = _customerSettings.AllowCustomersToUploadAvatars;
            searchModel.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            searchModel.LastNameEnabled = _customerSettings.LastNameEnabled;
            searchModel.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            searchModel.CompanyEnabled = _customerSettings.CompanyEnabled;
            searchModel.PhoneEnabled = _customerSettings.PhoneEnabled;
            searchModel.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;

            //search driver customers by default
            var driverRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.DriverRoleName);
            if (driverRole != null)
                searchModel.SelectedCustomerRoleIds.Add(driverRole.Id);

            //prepare available customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(searchModel);

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare paged customer list model
        /// </summary>
        /// <param name="searchModel">Customer search model</param>
        /// <returns>Customer list model</returns>
        public override CustomerListModel PrepareCustomerListModel(CustomerSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get parameters to filter customers
            int.TryParse(searchModel.SearchDayOfBirth, out var dayOfBirth);
            int.TryParse(searchModel.SearchMonthOfBirth, out var monthOfBirth);

            //get customers
            var customers = _customerService.GetAllCustomers(customerRoleIds: searchModel.SelectedCustomerRoleIds.ToArray(),
                email: searchModel.SearchEmail,
                username: searchModel.SearchUsername,
                firstName: searchModel.SearchFirstName,
                lastName: searchModel.SearchLastName,
                dayOfBirth: dayOfBirth,
                monthOfBirth: monthOfBirth,
                company: searchModel.SearchCompany,
                phone: searchModel.SearchPhone,
                zipPostalCode: searchModel.SearchZipPostalCode,
                ipAddress: searchModel.SearchIpAddress,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize, storeId: _storeContext.ActiveStoreScopeConfiguration);

            //prepare list model
            var model = new CustomerListModel().PrepareToGrid(searchModel, customers, () =>
            {
                return customers.Select(customer =>
                {
                    //fill in model values from the entity
                    var customerModel = customer.ToModel<CustomerModel>();

                    //convert dates to the user time
                    customerModel.Email = _customerService.IsRegistered(customer) ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest");
                    customerModel.FullName = _customerService.GetCustomerFullName(customer);
                    customerModel.Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute);
                    customerModel.Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
                    customerModel.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);

                    customerModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    customerModel.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);

                    //fill in additional values (not existing in the entity)
                    customerModel.CustomerRoleNames = string.Join(", ", _customerService.GetCustomerRoles(customer).Select(role => role.Name));
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        var avatarPictureId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute);
                        customerModel.AvatarUrl = _pictureService.GetPictureUrl(avatarPictureId, _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled, defaultPictureType: PictureType.Avatar);
                    }

                    return customerModel;
                });
            });

            return model;
        }

        /// <summary>
        /// Prepare customer picture search model
        /// </summary>
        /// <param name="searchModel">Customer picture search model</param>
        /// <param name="customer">Customer</param>
        /// <returns>Customer picture search model</returns>
        protected override CustomerPictureSearchModel PrepareCustomerPictureSearchModel(CustomerPictureSearchModel searchModel, Customer customer)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            searchModel.CustomerId = customer.Id;

            //prepare page parameters
            searchModel.SetGridPageSize();

            return searchModel;
        }

        /// <summary>
        /// Prepare customer model
        /// </summary>
        /// <param name="model">Customer model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of some properties of model</param>
        /// <returns>Customer model</returns>
        public override CustomerModel PrepareCustomerModel(CustomerModel model, Customer customer, bool excludeProperties = false)
        {
            if (customer != null)
            {
                //fill in model values from the entity
                model ??= new CustomerModel();

                model.Id = customer.Id;
                model.DisplayVatNumber = _taxSettings.EuVatEnabled;
                model.AllowSendingOfPrivateMessage = _customerService.IsRegistered(customer) &&
                    _forumSettings.AllowPrivateMessages;
                model.AllowSendingOfWelcomeMessage = _customerService.IsRegistered(customer) &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                model.AllowReSendingOfActivationMessage = _customerService.IsRegistered(customer) && !customer.Active &&
                    _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                model.GdprEnabled = _gdprSettings.GdprEnabled;

                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Username = customer.Username;
                    model.VendorId = customer.VendorId;
                    model.AdminComment = customer.AdminComment;
                    model.IsTaxExempt = customer.IsTaxExempt;
                    model.Active = customer.Active;
                    model.DriversLicenceNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.DriversLicenceAttribute);
                    model.VehicleRegistrationNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VehicleRegistrationAttribute);
                    model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                    model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
                    model.Gender = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.GenderAttribute);
                    model.DateOfBirth = _genericAttributeService.GetAttribute<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
                    model.Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute);
                    model.StreetAddress = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                    model.StreetAddress2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                    model.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                    model.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
                    model.County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute);
                    model.CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute);
                    model.StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
                    model.Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
                    model.Fax = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute);
                    model.TimeZoneId = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.TimeZoneIdAttribute);
                    model.VatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);
                    model.VatNumberStatusNote = _localizationService.GetLocalizedEnum((VatNumberStatus)_genericAttributeService
                        .GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute));
                    model.CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc);
                    model.LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc);
                    model.LastIpAddress = customer.LastIpAddress;
                    model.LastVisitedPage = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastVisitedPageAttribute);
                    model.SelectedCustomerRoleIds = _customerService.GetCustomerRoleIds(customer).ToList();
                    model.RegisteredInStore = _storeService.GetAllStores()
                        .FirstOrDefault(store => store.Id == customer.RegisteredInStoreId)?.Name ?? string.Empty;
                    model.DefaultStore = _storeService.GetAllStores()
                        .FirstOrDefault(store => store.Id == customer.DefaultStoreId)?.Name ?? string.Empty;
                    model.DisplayRegisteredInStore = model.Id > 0 && !string.IsNullOrEmpty(model.RegisteredInStore) &&
                                                     _storeService.GetAllStores().Select(x => x.Id).Count() > 1;
                    model.DisplayDefaultStore = model.Id > 0 && !string.IsNullOrEmpty(model.DefaultStore) &&
                                                     _storeService.GetAllStores().Select(x => x.Id).Count() > 1;

                    //prepare model affiliate
                    var affiliate = _affiliateService.GetAffiliateById(customer.AffiliateId);
                    if (affiliate != null)
                    {
                        model.AffiliateId = affiliate.Id;
                        model.AffiliateName = _affiliateService.GetAffiliateFullName(affiliate);
                    }

                    //prepare model newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        model.SelectedNewsletterSubscriptionStoreIds = _storeService.GetAllStores(_workContext.CurrentCustomer?.Id)
                            .Where(store => _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id) != null)
                            .Select(store => store.Id).ToList();
                    }
                }
                //prepare reward points model
                model.DisplayRewardPointsHistory = _rewardPointsSettings.Enabled;
                if (model.DisplayRewardPointsHistory)
                    PrepareAddRewardPointsToCustomerModel(model.AddRewardPoints);

                //prepare nested search models
                PrepareRewardPointsSearchModel(model.CustomerRewardPointsSearchModel, customer);
                PrepareCustomerAddressSearchModel(model.CustomerAddressSearchModel, customer);
                PrepareCustomerOrderSearchModel(model.CustomerOrderSearchModel, customer);
                PrepareCustomerShoppingCartSearchModel(model.CustomerShoppingCartSearchModel, customer);
                PrepareCustomerPictureSearchModel(model.CustomerPictureSearchModel, customer);
                PrepareCustomerActivityLogSearchModel(model.CustomerActivityLogSearchModel, customer);
                PrepareCustomerBackInStockSubscriptionSearchModel(model.CustomerBackInStockSubscriptionSearchModel, customer);
                PrepareCustomerAssociatedExternalAuthRecordsSearchModel(model.CustomerAssociatedExternalAuthRecordsSearchModel, customer);
            }
            else
            {
                //whether to fill in some of properties
                if (!excludeProperties)
                {
                    //precheck Driver Role as a default role while creating a new customer through admin
                    var driverRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.DriverRoleName);
                    if (driverRole != null)
                        model.SelectedCustomerRoleIds.Add(driverRole.Id);
                }
            }

            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.FaxEnabled = _customerSettings.FaxEnabled;

            //set default values for the new model
            if (customer == null)
            {
                model.Active = true;
                model.DisplayVatNumber = false;
            }

            //prepare available vendors
            _baseAdminModelFactory.PrepareVendors(model.AvailableVendors,
                defaultItemText: _localizationService.GetResource("Admin.Customers.Customers.Fields.Vendor.None"));

            //prepare model customer attributes
            PrepareCustomerAttributeModels(model.CustomerAttributes, customer);

            //prepare available stores
            _storeMappingSupportedModelFactory.PrepareModelStores(model, customer, excludeProperties);

            //prepare model stores for newsletter subscriptions
            model.AvailableNewsletterSubscriptionStores = _storeService.GetAllStores(_workContext.CurrentCustomer?.Id).Select(store => new SelectListItem
            {
                Value = store.Id.ToString(),
                Text = store.Name,
                Selected = model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id)
            }).ToList();

            //prepare model customer roles
            _aclSupportedModelFactory.PrepareModelCustomerRoles(model);

            //prepare available time zones
            _baseAdminModelFactory.PrepareTimeZones(model.AvailableTimeZones, false);

            //prepare available countries and states
            if (_customerSettings.CountryEnabled)
            {
                _baseAdminModelFactory.PrepareCountries(model.AvailableCountries);
                if (_customerSettings.StateProvinceEnabled)
                    _baseAdminModelFactory.PrepareStatesAndProvinces(model.AvailableStates, model.CountryId == 0 ? null : (int?)model.CountryId);
            }

            //prepare app download sms model
            model.SendAppDownloadSMS = new CustomerModel.SendAppDownloadSMSModel();
            model.SendAppDownloadSMS.Link = _mobileSettings.AppDownloadLink;

            var templates = _messageTemplateService.GetMessageTemplatesByName(MessageTemplateSystemNames.AppDownloadMessage, _storeContext.ActiveStoreScopeConfiguration);
            if (templates != null && templates.Count > 0)
            {
                var template = templates.Where(i => i.IsActive).FirstOrDefault();
                model.SendAppDownloadSMS.Message = template?.Body;
            }            

            return model;
        }

        #endregion
    }
}