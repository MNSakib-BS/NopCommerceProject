using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Vendors;

namespace Nop.Plugin.Arch.Core.Services.Vendor;
public class VendorAdditionalService : IVendorAdditionalService
{
    #region Fields

    private readonly IRepository<VendorAdditional> _vendorAdditionalRepository;

    #endregion

    #region Ctor

    public VendorAdditionalService(IRepository<VendorAdditional> vendorAdditionalRepository)
    {
        _vendorAdditionalRepository = vendorAdditionalRepository;
    }

    #endregion

    #region Utilities

    #endregion

    #region Methods

    public virtual async Task DeleteVendorAdditionalAsync(VendorAdditional vendorAdditional)
    {
        await _vendorAdditionalRepository.DeleteAsync(vendorAdditional);
    }

    public virtual async Task<VendorAdditional> GetVendorAdditionalByIdAsync(int id)
    {
        return await _vendorAdditionalRepository.GetByIdAsync(id, cache => default, false);
    }

    public virtual async Task<VendorAdditional> GetVendorAdditionalByVendorIdAsync(int vendorId)
    {
        return await _vendorAdditionalRepository.Table.Where(e => e.VendorId == vendorId).FirstOrDefaultAsync();
    }

    public virtual async Task InsertVendorAdditionalAsync(VendorAdditional vendorAdditional)
    {
        await _vendorAdditionalRepository.InsertAsync(vendorAdditional);
    }

    public virtual async Task UpdateVendorAdditionalAsync(VendorAdditional vendorAdditional)
    {
        await _vendorAdditionalRepository.UpdateAsync(vendorAdditional);
    }

    #endregion

}
