using Duende.IdentityServer.AspNetIdentity;
using Duende.IdentityServer.Models;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Duende.IdentityServer;
using IdentityModel;

namespace Auth;

public class CustomProfileService : ProfileService<ApplicationUser>
{
    public CustomProfileService(UserManager<ApplicationUser> userManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory) : base(userManager, claimsFactory)
    {
    }

    protected override async Task GetProfileDataAsync(ProfileDataRequestContext context, ApplicationUser user)
    {

        var principal = await GetUserClaimsAsync(user);
        var id = (ClaimsIdentity)principal.Identity;
        id.AddClaim(new Claim(JwtClaimTypes.GivenName, user.FirstName));
        id.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.LastName));

        context.AddRequestedClaims(principal.Claims);
    }
}