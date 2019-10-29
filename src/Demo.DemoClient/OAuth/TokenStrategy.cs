namespace Demo.DemoClient.OAuth
{
    using System;
    using Netsoft.Core.OAuth2;

    // ToDo: Create here your own strategy

    public class TokenStrategy : IOAuthTokenStrategy
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
