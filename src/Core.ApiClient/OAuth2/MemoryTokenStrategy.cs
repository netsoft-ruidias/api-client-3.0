namespace Netsoft.Core.OAuth2
{
    using System;

    public class MemoryTokenStrategy : IOAuthTokenStrategy
    {
        private TokenResponse cachedToken;

        public TokenResponse GetToken()
        {
            if (this.cachedToken != null && this.cachedToken.ExpirationDate > DateTime.UtcNow)
            {
                return this.cachedToken;
            }

            return null;
        }

        public void SetToken(TokenResponse token)
        {
            this.cachedToken = token;
        }
    }
}
