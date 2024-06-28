using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Arch.Core.Domains.Affiliates;
using Nop.Plugin.Arch.Core.Services.Stores;
using Nop.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.AffiliateAdditionals;
public class AffiliateAdditionalService : IAffiliateAdditionalService
{
    #region Fields

    private readonly IRepository<AffiliateAdditional> _affiliateAdditionalRepository;
    private readonly IRepository<Affiliate> _affiliateRepository;
    private readonly IRepository<Address> _addressRepository;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreMappingAdditionalService _storeMappingAdditionalService;
    private readonly IRepository<Order> _orderRepository;

    #endregion

    #region Ctor

    public AffiliateAdditionalService(IRepository<AffiliateAdditional> affiliateAdditionalRepository,
        IRepository<Affiliate> affiliateRepository,
        IRepository<Address> addressRepository,
        IStoreMappingService storeMappingService,
        IStoreMappingAdditionalService storeMappingAdditionalService,
        IRepository<Order> orderRepository)
    {
        _affiliateAdditionalRepository = affiliateAdditionalRepository;
        _affiliateRepository = affiliateRepository;
        _addressRepository = addressRepository;
        _storeMappingService = storeMappingService;
        _storeMappingAdditionalService = storeMappingAdditionalService;
        _orderRepository = orderRepository;
    }

    #endregion

    #region Methods

    public virtual async Task DeleteAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional)
    {
        await _affiliateAdditionalRepository.DeleteAsync(affiliateAdditional);
    }

    public virtual async Task<AffiliateAdditional> GetAffiliateAddiitonalByAffiliateIdAsync(int affiliateId)
    {
        return await _affiliateAdditionalRepository.Table.Where(e => e.AffiliateId == affiliateId).FirstOrDefaultAsync();
    }

    public virtual async Task<AffiliateAdditional> GetAffiliateAddiitonalByIdAsync(int id)
    {
        return await _affiliateAdditionalRepository.GetByIdAsync(id, cache => default, useShortTermCache: true);

    }

    public virtual async Task<IPagedList<Affiliate>> GetAllAffiliatesAsync(string friendlyUrlName = null, string firstName = null, string lastName = null, bool loadOnlyWithOrders = false, DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null, int pageIndex = 0, int pageSize = int.MaxValue, bool showHidden = false, int storeId = 0)
    {
        var query = _affiliateRepository.Table;
        var query_AffiliateAdditional = _affiliateAdditionalRepository.Table;

        if (!string.IsNullOrWhiteSpace(friendlyUrlName))
            query = query.Where(a => a.FriendlyUrlName.Contains(friendlyUrlName));

        if (!string.IsNullOrWhiteSpace(firstName))
            query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.FirstName.Contains(firstName)
                    select aff;

        if (!string.IsNullOrWhiteSpace(lastName))
            query = from aff in query
                    join addr in _addressRepository.Table on aff.AddressId equals addr.Id
                    where addr.LastName.Contains(lastName)
                    select aff;

        if (!showHidden)
            query = query.Where(a => a.Active);
        query = query.Where(a => !a.Deleted);

        query_AffiliateAdditional =await _storeMappingAdditionalService.FilterStoresAsync(query_AffiliateAdditional, storeId);

        query = from aff in query
                join ad_aff in query_AffiliateAdditional on aff.Id equals ad_aff.AffiliateId
                select aff;

        if (loadOnlyWithOrders)
        {
            var ordersQuery = _orderRepository.Table;
            if (ordersCreatedFromUtc.HasValue)
                ordersQuery = ordersQuery.Where(o => ordersCreatedFromUtc.Value <= o.CreatedOnUtc);
            if (ordersCreatedToUtc.HasValue)
                ordersQuery = ordersQuery.Where(o => ordersCreatedToUtc.Value >= o.CreatedOnUtc);
            ordersQuery = ordersQuery.Where(o => !o.Deleted);

            query = from a in query
                    join o in ordersQuery on a.Id equals o.AffiliateId
                    select a;
        }

        query = query.Distinct().OrderByDescending(a => a.Id);
       
        return await query.ToPagedListAsync(pageIndex, pageSize);
       
    }

    public virtual async Task InsertAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional)
    {
        await _affiliateAdditionalRepository.InsertAsync(affiliateAdditional);
    }

    public virtual async Task UpdateAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional)
    {
        await _affiliateAdditionalRepository.UpdateAsync(affiliateAdditional);
    }

    #endregion

}
