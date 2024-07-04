using Microsoft.AspNetCore.Http;
using Nop.Plugin.Api.DTO.Drivers;
using Nop.Plugin.Api.Helpers;
using System.Collections.Generic;

namespace Nop.Plugin.Api.Validators
{
    public class DriverDtoValidator : BaseDtoValidator<DriverDto>
    {
        public DriverDtoValidator(IHttpContextAccessor httpContextAccessor, IJsonHelper jsonHelper, Dictionary<string, object> requestJsonDictionary)
            : base(httpContextAccessor, jsonHelper, requestJsonDictionary)
        {
            SetEmailRule();
            SetPasswordRule();
            SetFirstNameRule();
            SetLastNameRule();
            SetPhoneRule();
            SetVehicleRegistrationNumberRule();
            SetDriversLicenceNumberRule();
        }

        private void SetEmailRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.Email, "invalid email", "email");
        }

        private void SetPasswordRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.Password, "invalid password", "password");
        }

        private void SetFirstNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.FirstName, "invalid first name", "first name");
        }

        private void SetLastNameRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.LastName, "invalid last name", "last name");
        }

        private void SetPhoneRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.Phone, "invalid phone", "phone");
        }

        private void SetVehicleRegistrationNumberRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.VehicleRegistrationNumber, "invalid vehicle registration", "vehicle registration");
        }

        private void SetDriversLicenceNumberRule()
        {
            SetNotNullOrEmptyCreateOrUpdateRule(i => i.DriversLicenceNumber, "invalid drivers licence", "drivers licence");
        }
    }
}
