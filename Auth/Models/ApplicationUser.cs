// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using Microsoft.AspNetCore.Identity;

namespace Auth.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
public class ApplicationRole : IdentityRole<Guid>
{

}
public class ApplicationUserRole : IdentityUserRole<Guid>
{

}
public class ApplicationUserClaim : IdentityUserClaim<Guid>
{

}
public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
{

}
public class ApplicationUserToken : IdentityUserToken<Guid>
{
    
}
public class ApplicationUserLogin : IdentityUserLogin<Guid>
{

}