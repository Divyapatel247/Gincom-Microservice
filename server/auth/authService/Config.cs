using IdentityServer4.Models;
using System.Collections.Generic;

public static class Config
{
   public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResources.Email(),
            new IdentityResource("roles", "User roles", new[] { "role" }) 
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api.read", "Read access to API", new[] { "email", "username", "role" }),
            new ApiScope("api.write", "Write access to API", new[] { "email", "username", "role" })
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            new Client
            {
                ClientId = "client",
                AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                ClientSecrets = { new Secret("secret".Sha256()) },
                
                AllowedScopes = { "api.read", "api.write", "openid", "profile", "email", "roles" },

                AllowOfflineAccess = true, 
                
                AlwaysIncludeUserClaimsInIdToken = true 
            }
        };
}