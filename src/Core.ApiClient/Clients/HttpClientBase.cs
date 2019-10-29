namespace Netsoft.Core.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Netsoft.Core.Builders;
    using Netsoft.Core.Model;
    using Netsoft.Core.OAuth2;

    public abstract class HttpClientBase
    {
        protected Dictionary<string, string> headers = new Dictionary<string, string>();
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientBase"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> HttpClient (injected by DI).</param>
        public HttpClientBase(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public IOAuthTokenProvider OAuthTokenProvider { get; set; }

        protected async Task<ApiResponse<TResponse>> GetAsync<TResponse>(string relativeUri)
            where TResponse : class, new()
        {
            var result = await this.GetStreamAsync<TResponse, object>(HttpMethod.Get, relativeUri, null);

            return result;
        }

        protected async Task<ApiResponse<TResponse>> PostAsync<TResponse, TRequest>(string relativeUri, TRequest content)
            where TResponse : class, new()
        {
            var result = await this.GetStreamAsync<TResponse, TRequest>(HttpMethod.Post, relativeUri, content);

            return result;
        }

        protected async Task<ApiResponse<TResponse>> PutAsync<TResponse, TRequest>(string relativeUri, TRequest content)
            where TResponse : class, new()
        {
            var result = await this.GetStreamAsync<TResponse, TRequest>(HttpMethod.Put, relativeUri, content);

            return result;
        }

        protected async Task<ApiResponse<bool>> DeleteAsync(string relativeUri)
        {
            var result = await this.GetStreamAsync<object, object>(HttpMethod.Delete, relativeUri, null);

            return new ApiResponse<bool>()
            {
                Body = true,
                Headers = result.Headers,
                StatusCode = result.StatusCode,
            };
        }

        #region PatchAsync
        #endregion

        #region HeadAsync
        #endregion

        protected virtual Dictionary<string, string> HandleHeaders()
        {
            return null;
        }

        private async Task<ApiResponse<TResponse>> GetStreamAsync<TResponse, TRequest>(HttpMethod httpMethod, string relativeUri, TRequest content)
            where TResponse : class, new()
        {
            var messageBuilder = HttpMessageBuilder
                .Build(httpMethod, new Uri(this.httpClient.BaseAddress, relativeUri))
                .HandleHeaders(this.headers, this.HandleHeaders)
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
