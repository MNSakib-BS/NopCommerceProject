using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Affiliates;
using Nop.Core;
using Nop.Plugin.Arch.Core.Domains.Affiliates;

namespace Nop.Plugin.Arch.Core.Services.AffiliateAdditionals;
public interface IAffiliateAdditionalService
{
    /// <summary>
    /// Gets all affiliates
    /// </summary>
    /// <param name="friendlyUrlName">Friendly URL name; null to load all records</param>
    /// <param name="firstName">First name; null to load all records</param>
    /// <param name="lastName">Last name; null to load all records</param>
    /// <param name="loadOnlyWithOrders">Value indicating whether to load affiliates only with orders placed (by affiliated customers)</param>
    /// <param name="ordersCreatedFromUtc">Orders created date from (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="ordersCreatedToUtc">Orders created date to (UTC); null to load all records. It's used only with "loadOnlyWithOrders" parameter st to "true".</param>
    /// <param name="pageIndex">Page index</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="showHidden">A value indicating whether to show hidden records</param>
    /// <param name="storeId">A value indicating whether to show specific store</param>
    /// <returns>
    /// A task that represents the asynchronous operation
    /// The task result contains the affiliates
    /// </returns>
    Task<IPagedList<Affiliate>> GetAllAffiliatesAsync(string friendlyUrlName = null,
       string firstName = null, string lastName = null,
       bool loadOnlyWithOrders = false,
       DateTime? ordersCreatedFromUtc = null, DateTime? ordersCreatedToUtc = null,
       int pageIndex = 0, int pageSize = int.MaxValue,
       bool showHidden = false,int storeId=0);

    /// <summary>
    /// Gets an affiliate by affiliate additional identifier
    /// </summary>
    /// <param name="affiliateId">Affiliate additional identifier</param>
    /// <returns>Affiliate Additional</returns>
    Task<AffiliateAdditional> GetAffiliateAddiitonalByAffiliateIdAsync(int affiliateId);

    /// <summary>
    /// Gets an affiliate by affiliate additional identifier
    /// </summary>
    /// <param name="Id">Affiliate additional identifier</param>
    /// <returns>Affiliate Additional</returns>
    Task<AffiliateAdditional> GetAffiliateAddiitonalByIdAsync(int id);

    /// <summary>
    /// Marks affiliate additional as deleted 
    /// </summary>
    /// <param name="affiliate additional">Affiliate additional</param>
    Task DeleteAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional);

    /// <summary>
    /// Inserts an affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task InsertAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional);

    /// <summary>
    /// Updates the affiliate
    /// </summary>
    /// <param name="affiliate">Affiliate</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAffiliateAdditionalAsync(AffiliateAdditional affiliateAdditional);
}
