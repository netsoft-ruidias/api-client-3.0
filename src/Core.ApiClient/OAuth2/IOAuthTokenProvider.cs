namespace Netsoft.Core.OAuth2
{
    using System.Threading.Tasks;
    using Netsoft.Core.Model;

    public interface IOAuthTokenProvider
    {
        string GetAccessToken();

        Task<ApiResponse<TokenResponse>> GetTokenAsync();
    }
}
