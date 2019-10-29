namespace Netsoft.Core.Model
{
    public interface IIdentityConfig
    {
        string AuthorityUrl { get; }

        string AuthorizationScopes { get; }

        string ClientId { get; }

        string ClientSecret { get; }
    }
}
