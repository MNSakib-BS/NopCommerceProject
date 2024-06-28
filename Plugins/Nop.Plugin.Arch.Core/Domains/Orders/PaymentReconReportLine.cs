using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Arch.Core.Domains.Orders;
/// <summary>
/// Represents a payment recon report line
/// </summary>
public  class PaymentReconReportLine
{
    public int OrderNumber { get; set; }
    public decimal OrderTotal { get; set; }
    public decimal? CustomerWalletDeduction { get; set; }
    public decimal? PaidOnAccount { get; set; }
    public decimal PaymentGatewayAmount { get; set; }
    public string? PaymentGateway { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public decimal? WalletAmount { get; set; }
    //public WalletTransactionType? WalletTransactionType { get; set; }
    //public CashbackStatus? CashbackStatus { get; set; }
    public bool IsDebtor { get; set; }
}