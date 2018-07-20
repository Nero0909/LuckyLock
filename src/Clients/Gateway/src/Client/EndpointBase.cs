using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using Client.Extenstions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Client
{
    public abstract class EndpointBase : IDisposable
    {
        protected abstract string Version { get; }

        protected readonly List<MediaTypeFormatter> SupportedMediaTypeFormatters;
        protected readonly HttpClient Client;
        protected readonly GatewaySettings GatewaySettings;

        protected EndpointBase(GatewaySettings settings)
        {
            GatewaySettings = settings ?? throw new ArgumentNullException(nameof(settings));

            Client = new HttpClient();

            SupportedMediaTypeFormatters = new List<MediaTypeFormatter> {GetJsonMediaTypeFormatter()};
        }

        protected HttpRequestMessage CreateHttpRequest(HttpMethod method, string relativeUrl, string accessToken)
        {
            var uri = new Uri(relativeUrl, UriKind.Relative);
            var request = new HttpRequestMessage(method, uri);
            request.AddAuthenticationHeader(accessToken);
            return request;
        }

        public void Dispose()
        {
            Client?.Dispose();
        }

        private static MediaTypeFormatter GetJsonMediaTypeFormatter()
        {
            var formatter =
                new JsonMediaTypeFormatter
                {
                    SerializerSettings =
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        Converters = new List<JsonConverter> {new StringEnumConverter {CamelCaseText = true}}
                    }
                };

            return formatter;
        }
    }
}
