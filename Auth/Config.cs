using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace Auth;

public static class Config
{    
    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            //new IdentityResource("color", new [] { "favorite_color" }),
            new IdentityResources.Email()
        };


    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("api1", "My API")
        };

    public static IEnumerable<Client> Clients =>
        new List<Client>
        {
            // machine to machine client
            new Client
            {
                ClientId = "flutter",
                ClientSecrets = { new Secret("fluttersecret".Sha256()) },
                
                AllowedGrantTypes = GrantTypes.Code,
                       // where to redirect to after login
                RedirectUris = { "https://localhost:7253/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:7253/signout-callback-oidc" },
                AllowOfflineAccess = true,
    
                // scopes that client has access to
                  AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1",
                    //"color"
                }
            },
                
            // interactive ASP.NET Core Web App
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                
                AllowedGrantTypes = GrantTypes.Code,
                    
                // where to redirect to after login
                RedirectUris = { "https://software-proj-blazor.chbk.run/signin-oidc", "http://software-proj-blazor.chbk.run/signin-oidc" },
                
                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://software-proj-blazor.chbk.run/signout-callback-oidc" },

                AllowOfflineAccess = true,

                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "api1",
                    //"color"
                }
            }
        };
}