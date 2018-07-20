using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Tags.Models;

namespace Client.Tags
{
    public class Tags : EndpointBase, ITags
    {
        protected override string Version => "v1";

        public Tags(GatewaySettings settings) : base(settings)
        {
            Client.BaseAddress = new Uri(settings.TagsUrl);
        }

        public async Task<Tag> GetByIdAsync(Guid id, string accessToken)
        {
            var url = $"{Version}/tags/{id}";

            using (var request = CreateHttpRequest(HttpMethod.Get, url, accessToken))
            using (var response = await Client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsAsync<Tag>(SupportedMediaTypeFormatters).ConfigureAwait(false);
            }
        }
    }
}
