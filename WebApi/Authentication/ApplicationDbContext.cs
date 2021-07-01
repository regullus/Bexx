using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.Identity;

namespace WebApi.Authentication
{
    public class ApplicationDbContext : KeyApiAuthorizationDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(
            DbContextOptions options,
            IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

           // modelBuilder.Entity<ApplicationUser>().Ignore(x => x.PhoneNumber);
           // modelBuilder.Entity<ApplicationUser>().Ignore(x => x.PhoneNumberConfirmed);

            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.PhoneNumber).HasColumnName("celular"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.UserName).HasColumnName("login"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.PhoneNumberConfirmed).HasColumnName("celular_confirmado"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.Email).HasColumnName("email"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.EmailConfirmed).HasColumnName("email_confirmed"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.AccessFailedCount).HasColumnName("access_failed_count"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.ConcurrencyStamp).HasColumnName("concurrency_stamp"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.LockoutEnabled).HasColumnName("lockout_enabled"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.LockoutEnd).HasColumnName("lockout_end"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.NormalizedEmail).HasColumnName("normalized_email"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.NormalizedUserName).HasColumnName("normalized_login"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.PasswordHash).HasColumnName("password_hash"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.SecurityStamp).HasColumnName("security_stamp"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.TwoFactorEnabled).HasColumnName("two_factor_enabled"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "users").Property(p => p.EmailConfirmed).HasColumnName("email_confirmed"); });

            //modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role").Property(p => p.Name).HasColumnName("name"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role").Property(p => p.NormalizedName).HasColumnName("normalized_name"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role").Property(p => p.ConcurrencyStamp).HasColumnName("concurrency_stamp"); });
            
            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("user_role");
                //in case you chagned the TKey type
                entity.HasKey(key => new { key.UserId, key.RoleId });
            });
            modelBuilder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable(name: "user_role").Property(p => p.UserId).HasColumnName("user_id"); });
            modelBuilder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable(name: "user_role").Property(p => p.RoleId).HasColumnName("role_id"); });
          
            //modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable("user_claims"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "user_claims").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "user_claims").Property(p => p.UserId).HasColumnName("user_id"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "user_claims").Property(p => p.ClaimType).HasColumnName("claim_type"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "user_claims").Property(p => p.ClaimValue).HasColumnName("claim_value"); });

            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("user_logins");
                //in case you chagned the TKey type
                entity.HasKey(key => new { key.ProviderKey, key.LoginProvider });
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "user_logins").Property(p => p.LoginProvider).HasColumnName("login_provider"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "user_logins").Property(p => p.ProviderKey).HasColumnName("provider_key"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "user_logins").Property(p => p.ProviderDisplayName).HasColumnName("provider_display_name"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "user_logins").Property(p => p.UserId).HasColumnName("user_id"); });

            //modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable("user_token"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "user_token").Property(p => p.UserId).HasColumnName("user_id"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "user_token").Property(p => p.LoginProvider).HasColumnName("login_provider"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "user_token").Property(p => p.Name).HasColumnName("name"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "user_token").Property(p => p.Value).HasColumnName("value"); });

            //modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable("role_claims"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "role_claims").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "role_claims").Property(p => p.RoleId).HasColumnName("role_id"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "role_claims").Property(p => p.ClaimType).HasColumnName("claim_type"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "role_claims").Property(p => p.ClaimValue).HasColumnName("claim_value"); });

            //fim
        }
    }
}
