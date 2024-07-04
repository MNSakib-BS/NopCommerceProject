using System;
using System.Threading.Tasks;
using Nop.Plugin.Api.Models.Arch;

namespace Nop.Plugin.Api.Services
{
    public interface IArchService
    {
        Task<string> ComputePayloadChecksumAsync(ArchRefundRequest request);
        Task<ArchRefundResponse> RefundAsync(ArchRefundRequest request, Func<ArchRefundRequest, bool> payloadChecksumValidation);
        Task ProcessInvoiceFromArchAsync(int orderId, decimal refundAmount, bool isFullRefund);
    }
}
