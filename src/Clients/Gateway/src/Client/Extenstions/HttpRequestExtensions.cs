using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Client.Extenstions
{
    public static class HttpRequestExtensions
    {
        public static void AddAuthenticationHeader(this HttpRequestMessage request, string accessToken)
        {
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);
            }
        }
    }
}
