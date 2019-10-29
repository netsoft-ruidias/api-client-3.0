namespace Netsoft.Core.OAuth2
{
    using System;

    public class TokenResponse
    {
        public string Access_Token { get; set; }

        public int Expires_In { get; set; }

        public string Token_Type { get; set; }

        public string Error { get; set; }

        public DateTime ExpirationDate
        {
            get
            {
                return DateTime.UtcNow.AddSeconds(this.Expires_In);
            }
        }

    }
}
