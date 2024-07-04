using Nop.Core.Domain.Orders;

namespace Nop.Plugin.Api.Factories
{
    public class ReturnRequestFactory : IFactory<ReturnRequest>
    {
        public async Task<ReturnRequest> InitializeAsync()
        {
            return  new ReturnRequest()
            {

            };
        }

      
    }
}
