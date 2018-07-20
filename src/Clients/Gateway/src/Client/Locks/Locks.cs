using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Client.Tags.Models;

namespace Client.Locks
{
    public class Locks : EndpointBase, ILocks
    {
        protected override string Version => "v1";

        public Locks(GatewaySettings settings) : base(settings)
        {
            Client.BaseAddress = new Uri(settings.LocksUrl);
        }


        public async Task<bool> CheckLinkedLocksExistence(Guid tagId, string accessToken)
        {
            var url = $"{Version}/tags/{tagId}/lockslinks";

            using (var request = CreateHttpRequest(HttpMethod.Head, url, accessToken))
            using (var response = await Client.SendAsync(request).ConfigureAwait(false))
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return false;
                }

                response.EnsureSuccessStatusCode();

                return true;
            }
        }
    }
}
