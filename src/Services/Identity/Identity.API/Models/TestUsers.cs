// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Test;

namespace Identity.API.Models
{
    public class TestUsers
    {
        public static List<TestUser> Users = new List<TestUser>
        {
            new TestUser{SubjectId = "818727", Username = "christopher", Password = "qwerty", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "Christopher Turk"),
                    new Claim(JwtClaimTypes.GivenName, "Christopher"),
                    new Claim(JwtClaimTypes.FamilyName, "Turk"),
                    new Claim(JwtClaimTypes.Email, "Duncan@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://scrubs.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'Sacred Heart', 'locality': 'California', 'postal_code': 90002, 'country': 'US' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                }
            },
            new TestUser{SubjectId = "88421113", Username = "bob", Password = "bob", 
                Claims = 
                {
                    new Claim(JwtClaimTypes.Name, "John Dorian"),
                    new Claim(JwtClaimTypes.GivenName, "John"),
                    new Claim(JwtClaimTypes.FamilyName, "Dorian"),
                    new Claim(JwtClaimTypes.Email, "Michael@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                    new Claim(JwtClaimTypes.WebSite, "http://scrubs.com"),
                    new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'Sacred Heart', 'locality': 'California', 'postal_code': 90002, 'country': 'US' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json),
                    new Claim("location", "somewhere")
                }
            }
        };
    }
}