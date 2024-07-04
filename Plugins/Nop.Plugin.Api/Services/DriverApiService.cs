using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Nop.Data;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Api.DTO.Customers;
using Nop.Plugin.Api.DTO.Drivers;
using Nop.Plugin.Api.MappingExtensions;

namespace Nop.Plugin.Api.Services
{
    public class DriverApiService : IDriverApiService
    {
        private readonly ICustomerApiService _customerApiService;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<GenericAttribute> _genericAttributeRepository;

        public DriverApiService(
            ICustomerApiService customerApiService,
            IRepository<Customer> customerRepository,
            IRepository<GenericAttribute> genericAttributeRepository)
        {
            _customerApiService = customerApiService;
            _customerRepository = customerRepository;
            _genericAttributeRepository = genericAttributeRepository;
        }

        public async Task<DriverDto> GetDriverByIdAsync(int id, bool showDeleted = false)
        {
            var customerDto =await _customerApiService.GetCustomerByIdAsync(id, showDeleted);
            var driverDto = customerDto.ToDriverDto();

            var customerAttributeMappings = (from customer in _customerRepository.Table //NoTracking
                                             join attribute in _genericAttributeRepository.Table //NoTracking
                                                 on customer.Id equals attribute.EntityId
                                             where customer.Id == id &&
                                                   attribute.KeyGroup == "Customer"
                                             select new CustomerAttributeMappingDto
                                             {
                                                 Attribute = attribute,
                                                 Customer = customer
                                             }).ToList();

            if (customerAttributeMappings.Count > 0)
            {
                foreach (var mapping in customerAttributeMappings)
                {
                    if (!showDeleted && mapping.Customer.Deleted)
                    {
                        continue;
                    }

                    if (mapping.Attribute != null)
                    {
                        if (mapping.Attribute.Key.Equals(NopCustomerDefaults.PhoneAttribute, StringComparison.InvariantCultureIgnoreCase))
                        {
                            driverDto.Phone = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(NopCustomerDefaults.VehicleRegistrationAttribute, StringComparison.InvariantCultureIgnoreCase))
                        {
                            driverDto.VehicleRegistrationNumber = mapping.Attribute.Value;
                        }
                        else if (mapping.Attribute.Key.Equals(NopCustomerDefaults.DriversLicenceAttribute, StringComparison.InvariantCultureIgnoreCase))
                        {
                            driverDto.DriversLicenceNumber = mapping.Attribute.Value;
                        }
                    }
                }
            }

            return driverDto;
        }
    }
}
