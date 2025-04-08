using IdentityServer4.Models;
using System.Collections.Generic;

public static class Config
{
    public static IEnumerable<ApiResource> ApiResources =>
    new List<ApiResource>
    {
        new ApiResource("api", "Main API")
        {
            Scopes = { "api.read", "api.write" },
            UserClaims = { "role" } // include role if needed
        }
    };
    public static IEnumerable<IdentityResource> IdentityResources =>
         new List<IdentityResource>
         {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new("custom", "Custom User Data", ["username", "role"]),
         };

    public static IEnumerable<ApiScope> ApiScopes =>
    new List<ApiScope>
    {
        new ApiScope("api.read", "Read Access"),
        new ApiScope("api.write", "Write Access")
    };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedScopes = { "api.read", "api.write", "openid", "profile", "email", "role" },

                AllowOfflineAccess = true,

                AlwaysIncludeUserClaimsInIdToken = true
            }
        };
}