namespace Nop.Plugin.Api.Models.Arch
{
    public enum ArchRefundStatus
    {
        RefundSuccessfullyCaptured = 1,
        RefundAlreadyCaptured = 2,
        UnableToCapture = 3,
        GivenAmountExceedsQuotation = 4,
        OrderDoesNotExist = 5,
        GivenAmountEqualsQuotationAmount = 6,
        RefundAmountLessThanZero = 7,
    }
}