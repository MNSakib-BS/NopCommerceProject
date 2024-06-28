using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Customers;
public static partial class NopCustomerDefaults
{
    /// <summary>
    /// Gets a system name of 'driver' customer role
    /// </summary>
    public static string DriverRoleName => "Driver";
    public static Regex ValidPhoneNumberRegex => new Regex("^([0-9][0-9]+)$");
    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorNumber'
    /// </summary>
    public static string DebtorNumberAttribute => "DebtorNumber";
    /// <summary>
    /// Gets a name of generic attribute to store the value of 'IsDebtor'
    /// </summary>
    public static string IsDebtorAttribute => "IsDebtor";
    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorAccountName'
    /// </summary>
    public static string DebtorAccountNameAttribute => "DebtorAccountName";
    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorType'
    /// </summary>
    public static string DebtorTypeAttribute => "DebtorType";
    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorStatus'
    /// </summary>
    public static string DebtorAccountStatusAttribute => "DebtorAccountStatus";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorErrorCode'
    /// </summary>
    public static string DebtorErrorCodeAttribute => "DebtorErrorCode";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorDeliveryMethod'
    /// </summary>
    public static string DebtorDeliveryMethod => "DebtorDeliveryMethod";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorDeliveryChargeType'
    /// </summary>
    public static string DebtorDeliveryChargeValueType => "DebtorDeliveryChargeValueType";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DebtorDeliveryChargeValue'
    /// </summary>
    public static string DebtorDeliveryChargeValue => "DebtorDeliveryChargeValue";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'PriceTier'
    /// </summary>
    public static string PriceTierAttribute => "PriceTier";
    public static string PriceNumberAttribute => "PriceNumber";

    /// <summary>
    /// Gets the default price tier
    /// </summary>
    public static int PriceTierDefault => 1;

    #region Driver attributes

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'VehicleRegistration'
    /// </summary>
    public static string VehicleRegistrationAttribute => "VehicleRegistration";

    /// <summary>
    /// Gets a name of generic attribute to store the value of 'DriversLicence'
    /// </summary>
    public static string DriversLicenceAttribute => "DriversLicence";

    #endregion

    public static string CompareProductAttribute => "CompareProductId";

    public static string SearchHistoryAttribute => "SearchHistory";
}
