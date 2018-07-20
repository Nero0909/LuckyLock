using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Locks.API.Infrastructure.Services
{
    public class TokensClient : ITokensClient
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly TokenClient _tokenClient;

        public TokensClient(IConfiguration configuration, IHttpContextAccessor accessor)
        {
            var disco = DiscoveryClient.GetAsync(configuration["IdentityUrl"]).Result;
            _tokenClient = new TokenClient(disco.TokenEndpoint, "locks.client", configuration["TagsKey"]);
            _accessor = accessor;
        }

        public Task<TokenResponse> GetTokenForTags()
        {
            var userToken = GetUserToken();
            var payload = new
            {
                token = userToken
            };

            return _tokenClient.RequestCustomGrantAsync("delegation", "tags", payload);
        }

        private string GetUserToken()
        {
            var authHeader = _accessor.HttpContext.Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
               return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }
}
