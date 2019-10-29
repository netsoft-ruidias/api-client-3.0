namespace Netsoft.Core.OAuth2
{
    public interface IOAuthTokenStrategy
    {
        TokenResponse GetToken();

        void SetToken(TokenResponse token);
    }
}
