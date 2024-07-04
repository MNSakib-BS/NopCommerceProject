using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Plugin.Api.Delta;
using Nop.Plugin.Api.DTO.Drivers;
using Nop.Plugin.Api.DTO.Errors;
using Nop.Plugin.Api.Factories;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.JSON.ActionResults;
using Nop.Plugin.Api.JSON.Serializers;
using Nop.Plugin.Api.MappingExtensions;
using Nop.Plugin.Api.ModelBinders;
using Nop.Plugin.Api.Services;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using NopCustomerDefaults = Nop.Plugin.Arch.Core.Domains.Customers.NopCustomerDefaults;

namespace Nop.Plugin.Api.Controllers
{
    public class DriversController : BaseApiController
    {
        private readonly IDriverApiService _driverApiService;
        private readonly ICustomerApiService _customerApiService;
        private readonly IEncryptionService _encryptionService;
        private readonly IFactory<Customer> _factory;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ILanguageService _languageService;

        // We resolve the customer settings this way because of the tests.
        // The auto mocking does not support concreate types as dependencies. It supports only interfaces.
        private CustomerSettings _customerSettings;
        private CustomerSettings CustomerSettings
        {
            get
            {
                if (_customerSettings == null)
                {
                    _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
                }

                return _customerSettings;
            }
        }

        public DriversController(
            IDriverApiService driverApiService,
            ICustomerApiService customerApiService,
            IJsonFieldsSerializer jsonFieldsSerializer,
            IAclService aclService,
            ICustomerService customerService,
            IStoreMappingService storeMappingService,
            IStoreService storeService,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IGenericAttributeService genericAttributeService,
            IEncryptionService encryptionService,
            IFactory<Customer> factory,
            IPictureService pictureService,
            IWorkflowMessageService workflowMessageService,
            ILanguageService languageService) :
            base(jsonFieldsSerializer, aclService, customerService, storeMappingService, storeService, discountService, customerActivityService,
                 localizationService, pictureService)
        {
            _driverApiService = driverApiService;
            _customerApiService = customerApiService;
            _factory = factory;
            _encryptionService = encryptionService;
            _genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
            _languageService = languageService;
        }

        [HttpPost]
        [Route("/api/drivers")]
        [ProducesResponseType(typeof(DriversRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> CreateDriver(
            [ModelBinder(typeof(JsonModelBinder<DriverDto>))]
            Delta<DriverDto> driverDelta)
        {
            ValidateEmail(driverDelta.Dto);

            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
                return Error();

            // Inserting the new customer
            var newCustomer = await _factory.InitializeAsync();
            driverDelta.Merge(newCustomer);
            newCustomer.Active = false;

            await CustomerService.InsertCustomerAsync(newCustomer);
            driverDelta.Dto.Id = newCustomer.Id;

            // Generic attributes
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.FirstNameAttribute, driverDelta.Dto.FirstName); 
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.LastNameAttribute, driverDelta.Dto.LastName);
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.PhoneAttribute, driverDelta.Dto.Phone);
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.VehicleRegistrationAttribute, driverDelta.Dto.VehicleRegistrationNumber);
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.DriversLicenceAttribute, driverDelta.Dto.DriversLicenceNumber);

            // Picture
            InsertPicture(driverDelta.Dto);

            // Password
            if (!string.IsNullOrWhiteSpace(driverDelta.Dto.Password))
            {
                AddPassword(driverDelta.Dto.Password, newCustomer);
            }

            // Roles
            var allCustomerRoles =await CustomerService.GetAllCustomerRolesAsync(true);

            var driverRole = allCustomerRoles.FirstOrDefault(i => i.SystemName == NopCustomerDefaults.DriverRoleName);
            var apiUserRole = allCustomerRoles.FirstOrDefault(i => i.SystemName == Constants.Roles.ApiRoleSystemName);
            var registeredRole = allCustomerRoles.FirstOrDefault(i => i.SystemName == NopCustomerDefaults.RegisteredRoleName);

            driverDelta.Dto.RoleIds.Add(driverRole.Id);
            driverDelta.Dto.RoleIds.Add(apiUserRole.Id);
            driverDelta.Dto.RoleIds.Add(registeredRole.Id);

            // Map driver roles
            foreach (var roleId in driverDelta.Dto.RoleIds)
            {
                await _customerApiService.AddCustomerRoleMappingAsync(new CustomerCustomerRoleMapping { CustomerId = newCustomer.Id, CustomerRoleId = roleId });
            }

            // Preparing the result dto of the new customer
            // We do not prepare the shopping cart items because we have a separate endpoint for them.
            var newCustomerDto = newCustomer.ToDto();

            // Set the fist and last name separately because they are not part of the customer entity, but are saved in the generic attributes.
            newCustomerDto.FirstName = driverDelta.Dto.FirstName;
            newCustomerDto.LastName = driverDelta.Dto.LastName;

            var newDriverDto = newCustomerDto.ToDriverDto();

            newDriverDto.Phone = driverDelta.Dto.Phone;
            newDriverDto.VehicleRegistrationNumber = driverDelta.Dto.VehicleRegistrationNumber;
            newDriverDto.DriversLicenceNumber = driverDelta.Dto.DriversLicenceNumber;

            // Activity log
           await CustomerActivityService.InsertActivityAsync("AddNewDriver", await LocalizationService.GetResourceAsync("ActivityLog.AddNewCustomer"), newCustomer);

            // Send Validation Email
            await _genericAttributeService.SaveAttributeAsync(newCustomer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
            await _workflowMessageService.SendCustomerEmailValidationMessageAsync(newCustomer, 0);

            var driversRootObject = new DriversRootObject();
            driversRootObject.Drivers.Add(newDriverDto);

            var json = JsonFieldsSerializer.Serialize(driversRootObject, string.Empty);
            return new RawJsonActionResult(json);
        }

        [HttpPut]
        [Route("/api/drivers")]
        [ProducesResponseType(typeof(DriversRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        [Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateDriver(
            [ModelBinder(typeof(JsonModelBinder<DriverDto>))]
            Delta<DriverDto> driverDelta)
        {
            // Here we display the errors if the validation has failed at some point.
            if (!ModelState.IsValid)
                return Error();

            var currentCustomer = await _customerApiService.GetCustomerEntityByIdAsync(driverDelta.Dto.Id);

            if (currentCustomer == null)
            {
                return Error(HttpStatusCode.NotFound, "driver", "not found");
            }

            driverDelta.Merge(currentCustomer);

            await CustomerService.UpdateCustomerAsync(currentCustomer);

            await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.FirstNameAttribute, driverDelta.Dto.FirstName);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.LastNameAttribute, driverDelta.Dto.LastName);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.PhoneAttribute, driverDelta.Dto.Phone);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.VehicleRegistrationAttribute, driverDelta.Dto.VehicleRegistrationNumber);
            await _genericAttributeService.SaveAttributeAsync(currentCustomer, NopCustomerDefaults.DriversLicenceAttribute, driverDelta.Dto.DriversLicenceNumber);

            AddUpdatePicture(driverDelta.Dto);

            // Password
            if (!string.IsNullOrWhiteSpace(driverDelta.Dto.Password))
            {
                AddPassword(driverDelta.Dto.Password, currentCustomer);
            }

            // Activity log
            await CustomerActivityService.InsertActivityAsync("EditDriver", await LocalizationService.GetResourceAsync("ActivityLog.EditCustomer"), currentCustomer);

            var driversRootObject = new DriversRootObject();
            driversRootObject.Drivers.Add(driverDelta.Dto);

            var json = JsonFieldsSerializer.Serialize(driversRootObject, string.Empty);
            return new RawJsonActionResult(json);
        }

        /// <summary>
        ///     Retrieve driver by specified id
        /// </summary>
        /// <param name="id">Id of the driver</param>
        /// <param name="fields">Fields from the driver you want your json to contain</param>
        /// <response code="200">OK</response>
        /// <response code="404">Not Found</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        [Route("/api/drivers/{id}")]
        [ProducesResponseType(typeof(DriversRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [GetRequestsErrorInterceptorActionFilter]
        [Authorize(Policy = JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDriverById(int id, string fields = "")
        {
            if (id <= 0)
            {
                return Error(HttpStatusCode.BadRequest, "id", "invalid id");
            }

            var driver = await _driverApiService.GetDriverByIdAsync(id);

            if (driver == null)
            {
                return Error(HttpStatusCode.NotFound, "driver", "not found");
            }

            var driversRootObject = new DriversRootObject();
            driversRootObject.Drivers.Add(driver);

            var json = JsonFieldsSerializer.Serialize(driversRootObject, fields);

            return new RawJsonActionResult(json);
        }

        [HttpGet]
        [Route("/api/drivers/passwordrecoverysend")]
        [ProducesResponseType(typeof(DriversRootObject), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorsRootObject), 422)]
        public async Task<IActionResult> PasswordRecoverySend(string email)
        {
            var customer = await _customerApiService.GetCustomerByEmailAsync(email);
            if (customer != null && customer.Active && !customer.Deleted)
            {
                //save token and current date
                var passwordRecoveryToken = Guid.NewGuid();
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, passwordRecoveryToken.ToString());

                var generatedDateTime = DateTime.UtcNow;
                await _genericAttributeService.SaveAttributeAsync(customer, NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                //send email
                await _workflowMessageService.SendCustomerPasswordRecoveryMessageAsync(customer, 0);

                var driversRootObject = new DriversRootObject();
                driversRootObject.Drivers.Add(new DriverDto() { Email = email });
                var json = JsonFieldsSerializer.Serialize(driversRootObject, string.Empty);

                return new RawJsonActionResult(json);
            }
            else
            {
                return Error(HttpStatusCode.UnprocessableEntity, "email", "invalid email");
            }
        }

        private async void ValidateEmail(DriverDto driver)
        {
            if (!string.IsNullOrWhiteSpace(driver.Email) &&await _customerApiService.GetCustomerByEmailAsync(driver.Email) != null)
                ModelState.AddModelError("email", "email is already registered");
        }

        private async void InsertPicture(DriverDto driver)
        {
            if (driver.Photo == null)
                return;

            var photoSeoName =await PictureService.GetPictureSeNameAsync("driver-profile-photo");
            var photoBytes = Convert.FromBase64String(driver.Photo);

            var picture =await PictureService.InsertPictureAsync(photoBytes, driver.PhotoMimeType, photoSeoName);

            var customerPicture = new CustomerPicture()
            {
                CustomerId = driver.Id,
                PictureId = picture.Id
            };

            CustomerService.InsertCustomerPicture(customerPicture);
        }

        private async Task AddUpdatePicture(DriverDto driver)
        {
            var pictureMapping = await _customerApiService.GetCustomerPictureMappingAsync(driver.Id);

            if (pictureMapping == null)
            {
                InsertPicture(driver);
            }
            else
            {
                var photoSeoName =await PictureService.GetPictureSeNameAsync("driver-profile-photo");
                var photoBytes = string.IsNullOrEmpty(driver.Photo) ? null : Convert.FromBase64String(driver.Photo);

                await PictureService.UpdatePictureAsync(pictureMapping.PictureId, photoBytes, driver.PhotoMimeType, photoSeoName, validateBinary: false);
            }
        }

        private async Task AddPassword(string newPassword, Customer customer)
        {
            var customerPassword = new CustomerPassword
            {
                CustomerId = customer.Id,
                PasswordFormat = CustomerSettings.DefaultPasswordFormat,
                CreatedOnUtc = DateTime.UtcNow
            };

            switch (CustomerSettings.DefaultPasswordFormat)
            {
                case PasswordFormat.Clear:
                    {
                        customerPassword.Password = newPassword;
                    }
                    break;
                case PasswordFormat.Encrypted:
                    {
                        customerPassword.Password = _encryptionService.EncryptText(newPassword);
                    }
                    break;
                case PasswordFormat.Hashed:
                    {
                        var saltKey = _encryptionService.CreateSaltKey(5);
                        customerPassword.PasswordSalt = saltKey;
                        customerPassword.Password = _encryptionService.CreatePasswordHash(newPassword, saltKey, CustomerSettings.HashedPasswordFormat);
                    }
                    break;
            }

            await CustomerService.InsertCustomerPasswordAsync(customerPassword);

            await CustomerService.UpdateCustomerAsync(customer);
        }
        
    }
}

