using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Locks.API.Infrastructure.Services
{
    public interface ITokensClient
    {
        Task<TokenResponse> GetTokenForLocks();
    }
}
