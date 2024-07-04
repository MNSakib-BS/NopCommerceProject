using Nop.Plugin.Api.Models.Authentication;

namespace Nop.Plugin.Api.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateAsync(TokenRequest customer);
    }
}