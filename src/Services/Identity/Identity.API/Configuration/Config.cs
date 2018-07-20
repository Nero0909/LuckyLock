using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace Identity.API.Configuration
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("tags", "Tags service"),
                new ApiResource("locks", "Locks service"),
                new ApiResource("event_aggregator", "Events aggregator service")
            };
        }

        public static IEnumerable<Client> GetClients(IConfiguration configuration)
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

                    ClientSecrets =
                    {
                        new Secret(configuration["MvcClientKey"].Sha256())
                    },

                    RedirectUris = { $"{configuration["MvcClientUrl"]}/signin-oidc" },
                    PostLogoutRedirectUris = { $"{configuration["MvcClientUrl"]}/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "tags",
                        "locks",
                        "event_aggregator",
                    },
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "tags.client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(configuration["TagsKey"].Sha256())
                    },

                    AllowedGrantTypes = { "delegation" },

                    AllowedScopes = new List<string>
                    {
                        "locks"
                    }
                },
                new Client
                {
                    ClientId = "locks.client",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret(configuration["LocksKey"].Sha256())
                    },

                    AllowedGrantTypes = { "delegation" },

                    AllowedScopes = new List<string>
                    {
                        "tags"
                    }
                }
            };
        }

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "turk",
                    Password = "qwerty",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Christopher"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "john",
                    Password = "qwerty",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "John"),
                        new Claim("website", "https://alice.com")
                    }
                }
            };
        }
    }
}
