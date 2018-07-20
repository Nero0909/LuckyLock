using System;
using System.Threading.Tasks;
using IdentityModel.Client;
using Locks.API.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Tags.API.Infrastructure.Services
{
    public class TokensClient : ITokensClient
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly TokenClient _tokenClient;

        public TokensClient(IConfiguration configuration, IHttpContextAccessor accessor)
        {
            var disco = DiscoveryClient.GetAsync(configuration["IdentityUrl"]).Result;
            _tokenClient = new TokenClient(disco.TokenEndpoint, "tags.client", configuration["LocksKey"]);
            _accessor = accessor;
        }

        public Task<TokenResponse> GetTokenForLocks()
        {
            var userToken = GetUserToken();
            var payload = new
            {
                token = userToken
            };

            return _tokenClient.RequestCustomGrantAsync("delegation", "locks", payload);
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
