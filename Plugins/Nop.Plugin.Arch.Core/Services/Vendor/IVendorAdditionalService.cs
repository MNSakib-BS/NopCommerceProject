using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Arch.Core.Domains.Catalog;
using Nop.Plugin.Arch.Core.Domains.Vendors;

namespace Nop.Plugin.Arch.Core.Services.Vendor;
public interface IVendorAdditionalService
{   
    Task<VendorAdditional> GetVendorAdditionalByVendorIdAsync(int vendorId);   
    Task<VendorAdditional> GetVendorAdditionalByIdAsync(int id);  
    Task DeleteVendorAdditionalAsync(VendorAdditional vendorAdditional);   
    Task InsertVendorAdditionalAsync(VendorAdditional vendorAdditional);   
    Task UpdateVendorAdditionalAsync(VendorAdditional vendorAdditional);
}
