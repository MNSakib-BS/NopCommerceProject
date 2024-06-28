using Nop.Core.Domain.Shipping;
using Nop.Data;
using Nop.Services.Shipping;

namespace Nop.Plugin.Arch.Core.Services.Shipping;
public class ArchShippingService : IArchShippingService
{
    #region Fields

    private readonly IShippingService _shippingService;
    private readonly IRepository<ShippingMethod> _shippingMethodRepository;


    #endregion

    #region Ctor

    public ArchShippingService(IShippingService shippingService,
        IRepository<ShippingMethod> shippingMethodRepository)
    {
        _shippingService = shippingService;
        _shippingMethodRepository = shippingMethodRepository;
    }

    #endregion

    #region Methods
    public virtual async Task InsertUpdateShippingMethodAsync(ShippingMethod shippingMethod)
    {
        if (shippingMethod != null && !string.IsNullOrEmpty(shippingMethod.Name))
        {
            if (await GetShippingMethodByNameAsync(shippingMethod.Name) != null)
            {
                await _shippingService.UpdateShippingMethodAsync(shippingMethod);
            }
            else
            {
                await _shippingService.InsertShippingMethodAsync(shippingMethod);
            }
        }
    }

    public virtual async Task<ShippingMethod> GetShippingMethodByNameAsync(string shippingMethodName)
    {
        if (string.IsNullOrEmpty(shippingMethodName))
            return null;

        return await _shippingMethodRepository.Table.Where(x => x.Name.Equals(shippingMethodName)).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
    }

    #endregion
}
