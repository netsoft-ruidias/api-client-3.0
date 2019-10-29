namespace Netsoft.Core.Extensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Netsoft.Core.Model;
    using Netsoft.Core.OAuth2;
    using Polly;

    public static class HttpClientBuilderExtensions
    {
        public static IHttpClientBuilder AddResilience(this IHttpClientBuilder clientBuilder, IPolicyConfig policyConfig)
        {
            if (policyConfig.HasResilicence)
            {
                clientBuilder.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(
                    new[] {
                        TimeSpan.FromSeconds(1),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(10),
                    }));

                clientBuilder.AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30)));

                if (policyConfig.HandlerLifetime.Ticks > 0)
                {
                    clientBuilder.SetHandlerLifetime(policyConfig.HandlerLifetime);
                }
            }

            return clientBuilder;
        }

        public static IHttpClientBuilder AddIdentity(this IHttpClientBuilder clientBuilder, IIdentityConfig identityConfig, IOAuthTokenStrategy tokenStrategy)
        {
            clientBuilder.ConfigureHttpClient(httpClient =>
            {
                var tokenProvider = new OAuthTokenProvider(httpClient, identityConfig, tokenStrategy);

                httpClient.DefaultRequestHeaders.Add(
                    Constants.RequestAuthorizationHeader,
                    tokenProvider.GetAccessToken());
            });

            return clientBuilder;
        }

    }
}
