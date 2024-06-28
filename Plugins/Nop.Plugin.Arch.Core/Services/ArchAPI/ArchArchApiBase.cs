using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Stores;
using Nop.Core.Infrastructure;
using Nop.Plugin.Arch.Core.Domains.Orders;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Plugin.Arch.Core.Services.Orders;
using Nop.Plugin.Arch.Core.Services.Helpers;
using Nop.Plugin.Arch.Core.Domains.Api;
using ArchServiceReference;
using Nop.Plugin.Arch.Core.Helpers;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Arch.Core.Services.Customers;
using Nop.Plugin.Arch.Core.Domains.Customers;
using Nop.Plugin.Arch.Core.Services.Stores;

namespace Nop.Plugin.Arch.Core.Services.ArchAPI;
public class ArchArchApiBase
{
    #region Fields

    private const string DATE_FORMAT = "u";
    private const string DEBTOR_UPDATE_SETTING = "DebtorFullListHostedService_LastUpdate";

    protected readonly IArchInvoiceService _archInvoiceService;
    protected readonly ILogger _logger;
    protected readonly IObjectConverter _objectConverter;
    private readonly ICustomerAdditionalService _customerAdditionalService;
    private readonly CustomerSettingsAdditional _customerSettingsAdditional;
    private readonly IStoreAdditionalService _storeAdditionalService;
    protected int _storeId;
    protected readonly CustomerSettings _customerSettings;
    protected ISettingService _settingService { get; }
    public ArchApiSettings ArchSettings { get; set; }
    public ECommerceBOClient Client { get; private set; }
    private bool? _archRequestDebuggingEnabled;
    private bool? _archResponseDebuggingEnabled;

    #endregion

    #region Ctor

    public ArchArchApiBase(ISettingService settingService,
        ECommerceBOClient client,
        CustomerSettings customerSettings,
        ILogger logger,
        IObjectConverter objectConverter,
        ICustomerAdditionalService customerAdditionalService,
        CustomerSettingsAdditional customerSettingsAdditional,
        IStoreAdditionalService storeAdditionalService)
    {
        _settingService = settingService;
        _customerSettings = customerSettings;
        _logger = logger;
        _objectConverter = objectConverter;
        _customerAdditionalService = customerAdditionalService;
        _customerSettingsAdditional = customerSettingsAdditional;
        _storeAdditionalService = storeAdditionalService;
        Client = client;
        _archInvoiceService = EngineContext.Current.Resolve<IArchInvoiceService>();
        EnsureSettings();

        DisableCertValidationAuth();
    }

    #endregion

    #region Utitlies

    private void EnsureSettings()
    {
        if (Client != null)
        {
            Client = null;
            Client = EngineContext.Current.Resolve<ECommerceBOClient>();
        }

        ArchSettings = _settingService.LoadSetting<ArchApiSettings>(_storeId);

        if (string.IsNullOrWhiteSpace(ArchSettings.ApiEndpointAddress))
            return;
        if (ArchSettings.ApiEndpointAddress.StartsWith("https"))// ensure security mode is set when url is SSL
        {
            var binding = Client.Endpoint.Binding as BasicHttpBinding;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
        }

        if (ArchSettings.ApiEndpointAddress.StartsWith("https"))// ensure security mode is set when url is SSL
        {
            var binding = Client.Endpoint.Binding as BasicHttpBinding;
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
        }

        Client.Endpoint.Address = new EndpointAddress(ArchSettings.ApiEndpointAddress);

        var sendTimeSpanParts = TimeSpanParts.New(ArchSettings.SendTimeout);
        Client.Endpoint.Binding.SendTimeout = new TimeSpan(sendTimeSpanParts.Hours, sendTimeSpanParts.Minutes, sendTimeSpanParts.Seconds);

        var receiveTimeSpanParts = TimeSpanParts.New(ArchSettings.ReceiveTimeout);
        Client.Endpoint.Binding.ReceiveTimeout = new TimeSpan(receiveTimeSpanParts.Hours, receiveTimeSpanParts.Minutes, receiveTimeSpanParts.Seconds);
    }

    protected virtual T GetSetting<T>(string settingName, T defaultValue = default)
    {
        var setting = _settingService.GetSetting(settingName, _storeId)?.Value;
        if (setting == null)
            return defaultValue;

        return _objectConverter.ToType<T>(setting);
    }

    private void DisableCertValidationAuth()
    {
        if (ArchSettings.DisableCertValidationAuth)
        {
            var noCertValidationAuth = new X509ServiceCertificateAuthentication()
            {
                CertificateValidationMode = X509CertificateValidationMode.None
            };

            Client.ClientCredentials.ServiceCertificate.SslCertificateAuthentication = noCertValidationAuth;
        }
    }

    protected virtual DateTime GetLastUpdate(string settingName)
    {
        var lastUpdate = ArchSettings.Epoch;

        var lastUpdateSetting = _settingService.GetSetting(settingName, _storeId);
        if (lastUpdateSetting != null)
        {
            lastUpdate = DateTime.ParseExact(lastUpdateSetting.Value, DATE_FORMAT, null);
        }

        return lastUpdate;
    }

    #endregion

    #region Methods

    public virtual void SetStoreID(int storeId)
    {
        // Reset the WCF client here to ensure we load data for the correct store when it comes from the refund api
        _storeId = storeId;
        EnsureSettings();
    }

    public virtual bool ArchRequestDebuggingEnabled
    {
        get
        {
            _archRequestDebuggingEnabled ??= GetSetting<bool>("EnableArchRequestDebugging");
            return _archRequestDebuggingEnabled.Value;
        }
    }

    public virtual bool ArchResponseDebuggingEnabled
    {
        get
        {
            _archResponseDebuggingEnabled ??= GetSetting<bool>("EnableArchResponseDebugging");
            return _archResponseDebuggingEnabled.Value;
        }
    }

    public virtual bool DiscardOrder(decimal transactionTrackingNumber, out string responseMessage)
    {
        if (transactionTrackingNumber == 0M)
        {
            responseMessage = string.Empty;
            return true; // Order not in Arch, cancel
        }
        var request = new DiscardOrderRequest
        {
            SystemAuthenticationCode = ArchSettings.SystemAuthenticationCode,
            TransactionTrackingNumber = transactionTrackingNumber,
            Usernumber = 0,
            WorkstationNumber = 0
        };

        if (ArchRequestDebuggingEnabled)
            _logger.InsertLog(LogLevel.Debug, "ArchRequestDebugging.DiscardOrderRequest", JsonConvert.SerializeObject(request));

        var discardResponse = Client.DiscardOrderAsync(request).GetAwaiter().GetResult();

        if (ArchResponseDebuggingEnabled)
            _logger.InsertLog(LogLevel.Debug, "ArchResponseDebuggingEnabled.DiscardOrderRequest", JsonConvert.SerializeObject(discardResponse));

        if (discardResponse.Success && !string.IsNullOrWhiteSpace(discardResponse.ResponseMessage))
        {
            _logger.Information(discardResponse.ResponseMessage);
        }
        else if (!discardResponse.Success)
        {
            var errorMessage = string.IsNullOrWhiteSpace(discardResponse.ExceptionMessage)
                ? string.IsNullOrWhiteSpace(discardResponse.ExceptionMessage) ? $"Discard order failed for transaction {transactionTrackingNumber}." : discardResponse.ResponseMessage
                : discardResponse.ExceptionMessage;

            _logger.Error(errorMessage);
        }

        responseMessage = $"{discardResponse.ResponseMessage}{(discardResponse.Success ? "" : $". {discardResponse.ExceptionMessage}")}";
        return discardResponse.Success;
    }

    public virtual async Task<IList<ArchInvoiceItem>> EnsureInvoice(decimal transactionTrackingNumber, int storeId)
    {
        try
        {
            var archInvoiceItems = await _archInvoiceService.GetInvoiceItemsAsync(transactionTrackingNumber, storeId);
            if (archInvoiceItems.Count > 0)
                return archInvoiceItems;

            var request = new GetInvoiceDetailsRequest
            {
                SystemAuthenticationCode = ArchSettings.SystemAuthenticationCode,
                TransactionTrackingNumber = transactionTrackingNumber
            };

            if (ArchRequestDebuggingEnabled)
                await _logger.InsertLogAsync(LogLevel.Debug, "ArchRequestDebugging.GetInvoiceDetailsRequest", JsonConvert.SerializeObject(request));

            if (_storeId != storeId)
                SetStoreID(storeId);

            var invoiceResponse = Client.GetInvoiceDetailsAsync(request).GetAwaiter().GetResult();

            if (ArchResponseDebuggingEnabled)
                await _logger.InsertLogAsync(LogLevel.Debug, "ArchResponseDebuggingEnabled.GetInvoiceDetailsRequest", JsonConvert.SerializeObject(invoiceResponse));

            if (invoiceResponse.List == null)
                return archInvoiceItems;

            if (transactionTrackingNumber > 0)
            {
                foreach (var invoiceDetailElement in invoiceResponse.List)
                {
                    var archInvoiceItem = new ArchInvoiceItem
                    {
                        TransactionTrackingNumber = transactionTrackingNumber,
                        DebtorNumber = invoiceDetailElement.DebtorNumber,
                        LoyaltyCardNumber = invoiceDetailElement.LoyaltyCardNumber,
                        OrderNumber = invoiceDetailElement.OrderNumber,
                        ProductCode = invoiceDetailElement.ProductCode,
                        PackQuantity = Convert.ToDecimal(invoiceDetailElement.PackQuantity),
                        PackSellingPriceIncl = Convert.ToDecimal(invoiceDetailElement.PackSellingPriceIncl),
                        PackSize = Convert.ToDecimal(invoiceDetailElement.PackSize),
                        Quantity = Convert.ToDecimal(invoiceDetailElement.Quantity),
                        TaxRate = Convert.ToDecimal(invoiceDetailElement.TaxRate),
                        TotalLineDiscount = Convert.ToDecimal(invoiceDetailElement.TotalLineDiscount),
                        TotalLineIncl = Convert.ToDecimal(invoiceDetailElement.TotalLineIncl),
                        StoreId = storeId
                    };
                    await _archInvoiceService.InsertInvoiceItemAsync(archInvoiceItem);
                }
            }

            archInvoiceItems = await _archInvoiceService.GetInvoiceItemsAsync(transactionTrackingNumber, storeId);
            return archInvoiceItems;
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to ensure invoice for Order with TTN {transactionTrackingNumber}.", ex);
            return new List<ArchInvoiceItem>();
        }
    }

    public virtual async Task UpdateCustomerDebtorInformation(Customer customer, string debtorNumber, ICustomerService customerService)
    {
        var request = new GetDebtorsRequest()
        {
            SystemAuthenticationCode = ArchSettings.SystemAuthenticationCode,
            DebtorNumber = debtorNumber,
            LastUpdate = GetLastUpdate(DEBTOR_UPDATE_SETTING)
        };

        if (ArchRequestDebuggingEnabled)
            await _logger.InsertLogAsync(LogLevel.Debug, "ArchRequestDebugging.GetDebtorsRequest", JsonConvert.SerializeObject(request));

        GetDebtorsResponse debtorsResponse = Client.GetDebtorListAsync(request).GetAwaiter().GetResult();

        if (ArchResponseDebuggingEnabled)
            await _logger.InsertLogAsync(LogLevel.Debug, "ArchResponseDebuggingEnabled.GetDebtorsRequest", JsonConvert.SerializeObject(debtorsResponse));

        if (!debtorsResponse.Success)
            await _logger.InsertLogAsync(LogLevel.Error, debtorsResponse.ResponseMessage, debtorsResponse.ExceptionMessage, customer);

        GetDebtorsResponse.DebtorElement element = debtorsResponse.List.FirstOrDefault();
        if (element == null)
            await _logger.InsertLogAsync(LogLevel.Information, $"No update to Debtor Status for customer {customer.Id} with debtor number {debtorNumber}.", customer: customer);

        var debtor = debtorsResponse.List?.FirstOrDefault();

        var success = debtorsResponse.Success && debtor != null;
        if (success)
        {
            await _customerAdditionalService.UpdateDebtorInformationAsync(customer,
                 debtor?.DebtorNumber,
                 Convert.ToInt32(debtor?.PriceNumber),
                 debtor?.AccountName,
                 debtor.DebtorType,
                 debtor?.Status,
                 debtor.ErrorCode,
                 debtor.DeliveryMethod,
                 debtor.DeliveryChargeValueType,
                 debtor.DeliveryChargeValue,
                 _customerSettingsAdditional.UseGlobalDebtor ? 0 : _storeId,
                 archApiDebtorCallSuccess: success);
        }
        else
        {
            await _customerAdditionalService.ClearDebtorInformationAsync(customer, _storeId, success);
        }
    }

    public async Task<(bool Success, int PriceNumber)> VerifyDebtorNumber(string debtorNumber, Store store)
    {
        var request = new GetDebtorsRequest
        {
            DebtorNumber = debtorNumber,
            SystemAuthenticationCode = ArchSettings.SystemAuthenticationCode,
            LastUpdate = ArchSettings.Epoch
        };

        if (ArchRequestDebuggingEnabled)
            _logger.InsertLog(LogLevel.Debug, "ArchRequestDebugging.GetDebtorsRequest", JsonConvert.SerializeObject(request));

        var response = await Client.GetDebtorListAsync(request);

        if (ArchResponseDebuggingEnabled)
            _logger.InsertLog(LogLevel.Debug, "ArchResponseDebuggingEnabled.GetDebtorsRequest", JsonConvert.SerializeObject(response));

        var debtor = response.List?.FirstOrDefault();
        var success = response.Success && debtor != null;

        int priceNumber = 0; // Default value

        if (store != null)
        {
            var storeAdditional = await _storeAdditionalService.GetStoreAddiitonalByStoreIdAsync(store.Id);

            priceNumber = success
                ? Convert.ToInt32(debtor.PriceNumber)
                : storeAdditional?.DefaultPriceTierId ?? NopCustomerDefaultsAdditional.PriceTierDefault;
        }
        else
        {
            priceNumber = NopCustomerDefaultsAdditional.PriceTierDefault; // or some default value in case store is null
        }

        return (success, priceNumber);
    }

    #endregion

}
