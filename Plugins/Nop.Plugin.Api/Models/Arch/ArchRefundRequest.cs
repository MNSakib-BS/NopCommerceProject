namespace Nop.Plugin.Api.Models.Arch
{
    public class ArchRefundRequest
    {
        public decimal transaction_code { get; set; }
        public int refund_type { get; set; }
        public decimal total { get; set; }
        public string authentication_code { get; set; }
        public int storeid { get; set; }

        public override string ToString()
        {
            return $"transaction_code:{transaction_code} | refund_type:{refund_type} | total:{total} | authentication_code:{authentication_code}";
        }
    }
}