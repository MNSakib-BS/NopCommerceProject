using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Arch.Core.Domains.Api;
using Nop.Plugin.Arch.Core.Domains.Payments;
using Nop.Services.Configuration;
using Nop.Services.Payments;

namespace Nop.Plugin.Arch.Core.Services.Payments;
public class ArchPaymentService : IArchPaymentService
{
    #region Fields

    private readonly IPaymentPluginManager _paymentPluginManager;
    private readonly ISettingService _settingService;
    private readonly ICustomerWalletService _customerWalletService;
    private readonly IStoreContext _storeContext;

    #endregion

    #region Ctor

    public ArchPaymentService(IPaymentPluginManager paymentPluginManager,
        ISettingService settingService,
        ICustomerWalletService customerWalletService,
        IStoreContext storeContext)
    {
        _paymentPluginManager = paymentPluginManager;
        _settingService = settingService;
        _customerWalletService = customerWalletService;
        _storeContext = storeContext;
    }    

    #endregion

    #region Utilities

    private async Task<Dictionary<string, dynamic>> EftPaymentStatusAsync(string checkoutId)
    {
        var storeScope= await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var eFTPayPaymentSettings = _settingService.LoadSetting<EFTPayPaymentSettings>(storeScope);

        var paymentInfo = new PaymentInfoModel
        {
            EntityId = eFTPayPaymentSettings.EntityId,
            Url = eFTPayPaymentSettings.Url,
            BearerToken = eFTPayPaymentSettings.BearerToken
        };

        Dictionary<string, dynamic> responseData;
        string data = $"entityId={paymentInfo.EntityId}";
        string url = $"{paymentInfo.Url}/{checkoutId}/payment?{data}";
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        request.Method = "GET";
        request.Headers["Authorization"] = $"Bearer {paymentInfo.BearerToken}";
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
            reader.Close();
            dataStream.Close();
        }
        return responseData;
    }

    #endregion

    #region Methods

    public Task ArchRefundAsync(Order order, string description, int archRefundType, decimal refundAmount)
    {
        throw new NotImplementedException();
    }

    public virtual async Task<Dictionary<string, dynamic>> EftPayRequestAsync(PaymentInfoModel paymentInfo)
    {
        var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
        var eFTPayPaymentSettings = await _settingService.LoadSettingAsync<EFTPayPaymentSettings>(storeScope);

        paymentInfo.EntityId = eFTPayPaymentSettings.EntityId;
        paymentInfo.Currency = eFTPayPaymentSettings.Currency;
        paymentInfo.PaymentType = eFTPayPaymentSettings.PaymentType;
        paymentInfo.Url = eFTPayPaymentSettings.Url;
        paymentInfo.BearerToken = eFTPayPaymentSettings.BearerToken;

        Dictionary<string, dynamic> responseData;
        string data = $"entityId={paymentInfo.EntityId}&amount={paymentInfo.Amount}&currency={paymentInfo.Currency}&paymentType={paymentInfo.PaymentType}";
        string url = paymentInfo.Url;
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
        request.Method = "POST";
        request.Headers["Authorization"] = $"Bearer {paymentInfo.BearerToken}";
        request.ContentType = "application/x-www-form-urlencoded";
        Stream PostData = request.GetRequestStream();
        PostData.Write(buffer, 0, buffer.Length);
        PostData.Close();
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            responseData = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(reader.ReadToEnd());
            reader.Close();
            dataStream.Close();
        }
        return responseData;
    }

    public virtual async Task<PaymentStatus> GetOrderPaymentStatusAsync(string checkoutId)
    {
        var response = await EftPaymentStatusAsync(checkoutId);
        string resultCode = response["result"]["code"]?.ToString();

        Dictionary<Regex, Func<CustomPaymentStatus>> regexActions = new Dictionary<Regex, Func<CustomPaymentStatus>>
            {
                { new Regex("^(000\\.000\\.|000\\.100\\.1|000\\.[36]|000\\.400\\.[1][12]0)"), () => CustomPaymentStatus.Paid },
                { new Regex("^(000\\.400\\.0[^3]|000\\.400\\.100)"), () => CustomPaymentStatus.Paid },
                { new Regex("^(000\\.200)"), () => CustomPaymentStatus.Pending },
                { new Regex("^(800\\.400\\.5|100\\.400\\.500)"), () => CustomPaymentStatus.Pending },
                { new Regex("^(000\\.400\\.[1][0-9][1-9]|000\\.400\\.2)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(900\\.[1234]00|000\\.400\\.030)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.[56]|999\\.|600\\.1|800\\.800\\.[84])"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.39[765])"), () => CustomPaymentStatus.Failed },
                { new Regex("^(300\\.100\\.100)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.400\\.[0-3]|100\\.380\\.100|100\\.380\\.11|100\\.380\\.4|100\\.380\\.5)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.400\\.1)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.400\\.2|100\\.390)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.[32])"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.1[123456]0)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(600\\.[23]|500\\.[12]|800\\.121)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.[13]50)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.250|100\\.360)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(700\\.[1345][05]0)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(200\\.[123]|100\\.[53][07]|800\\.900|100\\.[69]00\\.500)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.800)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.700|100\\.900\\.[123467890][00-99])"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.100|100\\.2[01])"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.55)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(100\\.380\\.[23]|100\\.380\\.101)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(000\\.100\\.2)"), () => CustomPaymentStatus.Failed },
                { new Regex("^(800\\.[17]00|800\\.800\\.[123])"), () => CustomPaymentStatus.Failed }
            };

        foreach (var kvp in regexActions)
        {
            if (kvp.Key.IsMatch(resultCode))
            {
                return PaymentStatusAdapter.ToPaymentStatus(kvp.Value());
            }
        }

        return PaymentStatus.Pending;
    }


    public void GetById(Object id)
    {
        /////
    }

    public void Test()
    {       
        GetById(Convert.ToInt32(id));
    }

    #endregion
}
