using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.Configuration;
using Nop.Plugin.Api.Domain;
using Nop.Plugin.Api.Infrastructure;
using Nop.Plugin.Api.Models.Authentication;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Plugin.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly ApiConfiguration _apiConfiguration;
        private readonly ApiSettings _apiSettings;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerApiService _customerApiService;

        public TokenService(
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerActivityService customerActivityService,
            CustomerSettings customerSettings,
            ApiSettings apiSettings,
            ApiConfiguration apiConfiguration,
            ICustomerApiService customerApiService)
        {
            _customerService = customerService;
            _customerRegistrationService = customerRegistrationService;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            _apiSettings = apiSettings;
            _apiConfiguration = apiConfiguration;
            _customerApiService = customerApiService;
        }

        public async Task<TokenResponse> GenerateAsync(TokenRequest model)
        {
            if (string.IsNullOrEmpty(model.Username))
                return new TokenResponse("Missing username");

            if (string.IsNullOrEmpty(model.Password))
                return new TokenResponse("Missing password");

            var customer = await ValidateUserAsync(model);
            if (customer != null)
            {
                return await GenerateTokenAsync(customer);
            }

            return new TokenResponse("Access Denied");
        }

        private async Task<Customer> ValidateUserAsync(TokenRequest model)
        {
            var result =await LoginCustomerAsync(model);

            if (result == CustomerLoginResults.Successful)
            {
                var customer =_customerSettings.UsernamesEnabled
                    ?await _customerService.GetCustomerByUsernameAsync(model.Username)
                    :await _customerService.GetCustomerByEmailAsync(model.Username);

                //activity log
                await _customerActivityService.InsertActivityAsync( customer, "Api.TokenRequest", "User API token request", customer);

                return  customer;
            }

            return null;
        }

        private async Task<TokenResponse> GenerateTokenAsync(Customer customer)
        {
            var customerDto =await _customerApiService.GetCustomerByIdAsync(customer.Id);

            var expiresInSeconds = new DateTimeOffset(DateTime.Now.AddMinutes(GetTokenExpiryInMinutes())).ToUnixTimeSeconds();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, expiresInSeconds.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.NameIdentifier, customer.CustomerGuid.ToString()),
                new Claim(CustomClaimTypes.CustomerId, customer.Id.ToString()),
                new Claim(CustomClaimTypes.FirstName, customerDto.FirstName),
                new Claim(CustomClaimTypes.LastName, customerDto.LastName),
                _customerSettings.UsernamesEnabled ? new Claim(ClaimTypes.Name, customer.Username) :
                    new Claim(ClaimTypes.Name, customer.Email)
            };

            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_apiConfiguration.SecurityKey)),
                SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(new JwtHeader(signingCredentials), new JwtPayload(claims));
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);


            return new TokenResponse(accessToken, expiresInSeconds);
        }

        private async Task<CustomerLoginResults> LoginCustomerAsync(TokenRequest model)
        {
            var loginResult =await _customerRegistrationService
                .ValidateCustomerAsync(model.Username, model.Password);

            return loginResult;
        }

        private int GetTokenExpiryInMinutes()
        {
            return _apiSettings.TokenExpiryMinutes <= 0
                ? Constants.Configurations.DefaultAccessTokenExpirationInMinutes
                : _apiSettings.TokenExpiryMinutes;
        }
    }
}