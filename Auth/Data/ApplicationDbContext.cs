using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Auth.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.Xml;
using Microsoft.Extensions.Options;

namespace Auth.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin, ApplicationRoleClaim, ApplicationUserToken>
{
    private readonly BusinessOwnerInfoConfig _businessOwnerInfoConfig;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<BusinessOwnerInfoConfig> businessOwnerInfoConfig)
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        _businessOwnerInfoConfig = businessOwnerInfoConfig.Value;
    }

    protected override async void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        var hasher = new PasswordHasher<ApplicationUser>();
        var businessOwnerUserId = Guid.NewGuid();
        var customerRoleId = Guid.NewGuid();
        var businessOwnerRoleId = Guid.NewGuid();
        var workerRoleId = Guid.NewGuid();

        builder.Entity<ApplicationUser>().HasData(new List<ApplicationUser>
        {
            new ApplicationUser
            {
                AccessFailedCount = 0,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                Email = _businessOwnerInfoConfig.Email,
                EmailConfirmed = false,
                FirstName = _businessOwnerInfoConfig.FirstName
                ,LastName=_businessOwnerInfoConfig.LastName
                ,Id =businessOwnerUserId,
                LockoutEnabled = true,
                NormalizedEmail=_businessOwnerInfoConfig.Email.Normalize()
                ,NormalizedUserName=_businessOwnerInfoConfig.Username.Normalize()
                ,PasswordHash=hasher.HashPassword(null, _businessOwnerInfoConfig.Password)
                ,PhoneNumber=_businessOwnerInfoConfig.PhoneNumber.Normalize()
                ,PhoneNumberConfirmed=false,
                SecurityStamp= Guid.NewGuid().ToString(),
                TwoFactorEnabled=false,
                UserName =_businessOwnerInfoConfig.Username
            }
        });

        builder.Entity<ApplicationRole>().HasData(new List<ApplicationRole>
        {
            new ApplicationRole
            {
                Id=customerRoleId,
                Name = "Customer",
                ConcurrencyStamp=Guid.NewGuid().ToString(),
                NormalizedName="Customer".Normalize()
            },
            new ApplicationRole
            {
                Id=businessOwnerRoleId,
                Name = "BusinessOwner",
                ConcurrencyStamp=Guid.NewGuid().ToString(),
                NormalizedName="BusinessOwner".Normalize()
            },
            new ApplicationRole
            {
                Id=workerRoleId,
                Name = "Worker ",
                ConcurrencyStamp=Guid.NewGuid().ToString(),
                NormalizedName="Worker".Normalize()
            }
        });

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.Property(a => a.UserId).HasColumnType("uuid");
            entity.Property(a => a.RoleId).HasColumnType("uuid");
        });
        builder.Entity<ApplicationUserRole>().HasData(new List<ApplicationUserRole>
        {
            new ApplicationUserRole
            {
                RoleId=businessOwnerRoleId,
                UserId=businessOwnerUserId
            }
        });
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
