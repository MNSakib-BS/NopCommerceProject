using System;
using Nop.Core.Domain.Common;

namespace Nop.Plugin.Api.Factories
{
    public class AddressFactory : IFactory<Address>
    {
        public async Task<Address> InitializeAsync()
        {
            var address = new Address
                          {
                              CreatedOnUtc = DateTime.UtcNow
                          };

            return address;
        }
    }
}
