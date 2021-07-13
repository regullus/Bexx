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

            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.PhoneNumber).HasColumnName("celular"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.UserName).HasColumnName("login"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.PhoneNumberConfirmed).HasColumnName("celularConfirmado"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.Email).HasColumnName("email"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.EmailConfirmed).HasColumnName("emailConfirmado"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.AccessFailedCount).HasColumnName("accessFailedCount"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.ConcurrencyStamp).HasColumnName("concurrencyStamp"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.LockoutEnabled).HasColumnName("lockoutEnabled"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.LockoutEnd).HasColumnName("lockoutEnd"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.NormalizedEmail).HasColumnName("normalizedEmail"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.NormalizedUserName).HasColumnName("normalizedLogin"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.PasswordHash).HasColumnName("passwordHash"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.SecurityStamp).HasColumnName("securityStamp"); });
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "usuario").Property(p => p.TwoFactorEnabled).HasColumnName("twoFactorEnabled"); });
            
            //modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "role"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "regra").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "regra").Property(p => p.Name).HasColumnName("nome"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "regra").Property(p => p.NormalizedName).HasColumnName("normalizedNome"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "regra").Property(p => p.ConcurrencyStamp).HasColumnName("concurrencyStamp"); });
            
            modelBuilder.Entity<IdentityUserRole<int>>(entity =>
            {
                entity.ToTable("user_role");
                //in case you chagned the TKey type
                entity.HasKey(key => new { key.UserId, key.RoleId });
            });
            modelBuilder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable(name: "usuarioRegra").Property(p => p.UserId).HasColumnName("idUsuario"); });
            modelBuilder.Entity<IdentityUserRole<int>>(entity => { entity.ToTable(name: "usuarioRegra").Property(p => p.RoleId).HasColumnName("idRegra"); });
          
            //modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable("usuarioClaims"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "usuarioClaims").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "usuarioClaims").Property(p => p.UserId).HasColumnName("idUsuario"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "usuarioClaims").Property(p => p.ClaimType).HasColumnName("claimTipo"); });
            modelBuilder.Entity<IdentityUserClaim<int>>(entity => { entity.ToTable(name: "usuarioClaims").Property(p => p.ClaimValue).HasColumnName("claimValor"); });

            modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
            {
                entity.ToTable("usuarioLogins");
                //in case you chagned the TKey type
                entity.HasKey(key => new { key.ProviderKey, key.LoginProvider });
            });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "usuarioLogin").Property(p => p.LoginProvider).HasColumnName("loginProvider"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "usuarioLogin").Property(p => p.ProviderKey).HasColumnName("providerKey"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "usuarioLogin").Property(p => p.ProviderDisplayName).HasColumnName("providerDisplayNome"); });
            modelBuilder.Entity<IdentityUserLogin<int>>(entity => { entity.ToTable(name: "usuarioLogin").Property(p => p.UserId).HasColumnName("idUsuario"); });

            //modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable("usuarioToken"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "usuarioToken").Property(p => p.UserId).HasColumnName("idUsuario"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "usuarioToken").Property(p => p.LoginProvider).HasColumnName("loginProvider"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "usuarioToken").Property(p => p.Name).HasColumnName("nome"); });
            modelBuilder.Entity<IdentityUserToken<int>>(entity => { entity.ToTable(name: "usuarioToken").Property(p => p.Value).HasColumnName("valor"); });

            //modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable("regraClaims"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "regraClaims").Property(p => p.Id).HasColumnName("id"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "regraClaims").Property(p => p.RoleId).HasColumnName("regraId"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "regraClaims").Property(p => p.ClaimType).HasColumnName("claimTipo"); });
            modelBuilder.Entity<IdentityRoleClaim<int>>(entity => { entity.ToTable(name: "regraClaims").Property(p => p.ClaimValue).HasColumnName("claimValor"); });

            //fim
        }
    }
}
