using Microsoft.EntityFrameworkCore;
using SecuritySystem.Core.Entities;
using SecuritySystem.Infrastructure.Mapping;

namespace SecuritySystem.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public virtual DbSet<Applications> Applications { get; set; }
        public virtual DbSet<CryptoKey> CryptoKeys { get; set; }
        public virtual DbSet<ApplicationTokenPolicy> ApplicationTokenPolicy { get; set; }
        public virtual DbSet<KnownDevice> KnownDevices { get; set; }
        public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }
        public virtual DbSet<LoginAudit> LoginAudits { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<ResourceEndpoint> ResourceEndpoints { get; set; }
        public virtual DbSet<ResourceMenu> ResourceMenus { get; set; }

        public virtual DbSet<RevokedToken> RevokedTokens { get; set; }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoleEndpoint> RoleEndpoints { get; set; }
        public virtual DbSet<RoleResourceMenu> RoleResourceMenus { get; set; }
        public virtual DbSet<RoleUser> RoleUsers { get; set; }

        public virtual DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceMenuConfiguration());
            modelBuilder.ApplyConfiguration(new ResourceEndpointConfiguration());
            modelBuilder.ApplyConfiguration(new RoleUserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleResourceMenuConfiguration());
            modelBuilder.ApplyConfiguration(new RoleEndpointConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            modelBuilder.ApplyConfiguration(new LoginAttemptConfiguration());
            modelBuilder.ApplyConfiguration(new RevokedTokenConfiguration());
            modelBuilder.ApplyConfiguration(new KnownDeviceConfiguration());
            modelBuilder.ApplyConfiguration(new LoginAuditConfiguration());
            modelBuilder.ApplyConfiguration(new CryptoKeyConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationTokenPolicyConfiguration());
            //modelBuilder.ApplyConfiguration(new AplicacionConfiguration());
            //modelBuilder.ApplyConfiguration(new AuditoriaLoginConfiguration());
            //modelBuilder.ApplyConfiguration(new DispositivoConocidoConfiguration());
            //modelBuilder.ApplyConfiguration(new IntentoFallidoLoginConfiguration());
            //modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());
            //modelBuilder.ApplyConfiguration(new RolConfiguration());
            //modelBuilder.ApplyConfiguration(new RolUsuarioConfiguration());
            //modelBuilder.ApplyConfiguration(new TokenRevocadoConfiguration());
            //modelBuilder.ApplyConfiguration(new UsuarioSistemaConfiguration());
            //modelBuilder.ApplyConfiguration(new ObjetosConfiguration());

            //modelBuilder.ApplyConfiguration(new ObjetoMenuConfiguration());
            //modelBuilder.ApplyConfiguration(new ObjetoPuntoConfiguration());
            //modelBuilder.ApplyConfiguration(new RoleObjetoMenuConfiguration());
            //modelBuilder.ApplyConfiguration(new RolePuntoConfiguration());
            base.OnModelCreating(modelBuilder);
            // Aquí mapeos personalizados
        }
    }
}
