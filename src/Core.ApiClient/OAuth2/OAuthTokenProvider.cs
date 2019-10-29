namespace Netsoft.Core.OAuth2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Netsoft.Core.Builders;
    using Netsoft.Core.Model;

    public sealed class OAuthTokenProvider : IOAuthTokenProvider
    {
        private readonly HttpClient httpClient;
        private readonly IIdentityConfig identityConfig;
        private readonly IOAuthTokenStrategy tokenStrategy;

        internal OAuthTokenProvider(HttpClient httpClient, IIdentityConfig identityConfig, IOAuthTokenStrategy tokenStrategy)
        {
            this.httpClient = httpClient;
            this.identityConfig = identityConfig;
            this.tokenStrategy = tokenStrategy;
        }

        public string GetAccessToken()
        {
            // ToDo: needs refactoring!

            var token = this.GetTokenAsync().Result;

            if (token == null || token.Body == null)
            {
                return string.Empty;
            }

            return $"{token.Body.Token_Type} {token.Body.Access_Token}";
        }

        public async Task<ApiResponse<TokenResponse>> GetTokenAsync()
        {
            if (this.identityConfig == null)
            {
                return new ApiResponse<TokenResponse>
                {
                    StatusCode = System.Net.HttpStatusCode.NoContent,
                    Headers = null,
                    Body = null,
                };
            }

            // ToDo: needs refactoring!
            if (this.tokenStrategy != null)
            {
                var token = this.tokenStrategy.GetToken();

                if (token != null && token.Access_Token.Length > 0)
                {
                    return new ApiResponse<TokenResponse>
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Body = this.tokenStrategy.GetToken(),
                    };
                }
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { Constants.GrantTypeKey, Constants.GrantTypeClientCredentials },
                { Constants.ScopeKey, this.identityConfig.AuthorizationScopes },
                { Constants.ClientIdKey, this.identityConfig.ClientId },
                { Constants.ClientSecretKey, this.identityConfig.ClientSecret },
            });

            var result = await this.GetStreamAsync<TokenResponse>(content);

            this.tokenStrategy.SetToken(
                result.StatusCode == System.Net.HttpStatusCode.OK
                    ? result.Body
                    : null);

            return result;
        }

        private async Task<ApiResponse<TResponse>> GetStreamAsync<TResponse>(FormUrlEncodedContent content)
            where TResponse : class, new()
        {
            var uriBuilder = new UriBuilder(this.identityConfig.AuthorityUrl);
            uriBuilder.Path = Constants.Identity4RelativeUrl;

            var messageBuilder = HttpMessageBuilder
                .Build(HttpMethod.Post, uriBuilder.Uri)
                .HandleContent(content);

            using (var request = messageBuilder.RequestMessage)
            {
                var response = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    var body = await JsonSerializer.DeserializeAsync<TResponse>(
                        stream,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var responseHeaders = response.Headers.Select(x => new ApiResponseHeader { Name = x.Key, Values = x.Value });

                    return new ApiResponse<TResponse>
                    {
                        StatusCode = response.StatusCode,
                        Headers = responseHeaders,
                        Body = body,
                    };
                }
            }

        }
    }
}
