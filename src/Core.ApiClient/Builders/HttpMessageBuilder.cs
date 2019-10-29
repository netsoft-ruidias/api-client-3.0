namespace Netsoft.Core.Builders
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.Json;

    internal class HttpMessageBuilder
    {
        private HttpMessageBuilder(HttpRequestMessage httpRequestMessage)
        {
            this.RequestMessage = httpRequestMessage;
        }

        public HttpRequestMessage RequestMessage { get; }

        public static HttpMessageBuilder Build(HttpMethod method, Uri uri)
        {
            return new HttpMessageBuilder(
                new HttpRequestMessage(method, uri));
        }

        internal HttpMessageBuilder HandleAutentication(Func<string> handlerCallback)
        {
            if (handlerCallback != null)
            {
                var accessToken = handlerCallback.Invoke();

                if (!string.IsNullOrWhiteSpace(accessToken))
                {
                    this.RequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }

            return this;
        }

        internal HttpMessageBuilder HandleContent(HttpContent httpContent)
        {
            this.RequestMessage.Content = httpContent;

            return this;
        }

        internal HttpMessageBuilder HandleContent<T>(T content)
        {
            if (content is HttpContent httpContent)
            {
                return this.HandleContent(httpContent);
            }

            if (content is StringContent stringContent)
            {
                return this.HandleContent(stringContent);
            }

            if (content is FormUrlEncodedContent formUrlEncodedContent)
            {
                return this.HandleContent(formUrlEncodedContent);
            }

            if (content != null)
            {
                return this.HandleContent(
                    this.CreateHttpContent(content));
            }

            return this;
        }

        internal HttpMessageBuilder HandleHeaders(Dictionary<string, string> headers, Func<Dictionary<string, string>> handlerCallback)
        {
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    this.RequestMessage.Headers.Add(header.Key, header.Value);
                }
            }

            if (handlerCallback != null)
            {
                var handlerCallbackResult = handlerCallback.Invoke();

                if (handlerCallbackResult != null)
                {
                    foreach (var header in handlerCallbackResult)
                    {
                        if (this.RequestMessage.Headers.Contains(header.Key))
                        {
                            this.RequestMessage.Headers.Remove(header.Key);
                        }

                        this.RequestMessage.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return this;
        }

        private HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var memoryStream = new MemoryStream();
                using (var utf8JsonWriter = new Utf8JsonWriter(memoryStream))
                {
                    JsonSerializer.Serialize(utf8JsonWriter, content);
                }
                memoryStream.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(memoryStream);

                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
    }
}
