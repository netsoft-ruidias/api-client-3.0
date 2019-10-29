namespace Demo.ServiceConsumer.Gateway.Model
{
    using Netsoft.Core.Model;

    public class IdentityConfig : IIdentityConfig
	{
		public string AuthorityUrl { get; set; }

		public string AuthorizationScopes { get; set; }

		public string ClientId { get; set; }

		public string ClientSecret { get; set; }
	}
}
