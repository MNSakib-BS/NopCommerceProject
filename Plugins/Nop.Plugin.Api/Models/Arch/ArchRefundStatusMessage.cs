using System.Collections.Generic;

namespace Nop.Plugin.Api.Models.Arch
{
    public static class ArchRefundStatusMessage
    {
        public static KeyValuePair<int, string> RefundSuccessfullyCaptured =
            new KeyValuePair<int, string>((int)ArchRefundStatus.RefundSuccessfullyCaptured, "Refund was successfully captured.");

        public static KeyValuePair<int, string> RefundAlreadyCaptured =
            new KeyValuePair<int, string>((int)ArchRefundStatus.RefundAlreadyCaptured, "Refund has already been captured.");
        
        public static KeyValuePair<int, string> UnableToCapture =
            new KeyValuePair<int, string>((int)ArchRefundStatus.UnableToCapture, "Unable to capture refund. Authentication hash is incorrect");

        public static KeyValuePair<int, string> RefundAmountLessThanZero =
           new KeyValuePair<int, string>((int)ArchRefundStatus.RefundAmountLessThanZero, "Cannot process refund, the given amount is less than 0 ");

        public static KeyValuePair<int, string> GivenAmountExceedsQuotation =
            new KeyValuePair<int, string>((int)ArchRefundStatus.GivenAmountExceedsQuotation, "The given amount is greater than the quotation amount.");

        public static KeyValuePair<int, string> OrderDoesNotExist =
            new KeyValuePair<int, string>((int)ArchRefundStatus.OrderDoesNotExist, "The order does not exist.");

        public static KeyValuePair<int, string> GivenAmountEqualsQuotationAmount =
            new KeyValuePair<int, string>((int)ArchRefundStatus.GivenAmountEqualsQuotationAmount, "The given amount is the same as the quotation amount. No refund will be captured.");
    }
}