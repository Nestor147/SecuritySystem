using Microsoft.EntityFrameworkCore.Storage;
using SecuritySystem.Core.Entities;

namespace SecuritySystem.Core.Interfaces.Core
{
    public interface IUnitOfWork : IDisposable
    {
        public IGenericRepository<Applications> ApplicationRepository { get; }

        // Users
        public IGenericRepository<User> UserRepository { get; }

        // Roles
        public IGenericRepository<Role> RoleRepository { get; }

        // Resources (páginas / nodos / componentes)
        public IGenericRepository<Resource> ResourceRepository { get; }

        // Menú de recursos
        public IGenericRepository<ResourceMenu> ResourceMenuRepository { get; }

        // Endpoints de recursos (acciones / APIs)
        public IGenericRepository<ResourceEndpoint> ResourceEndpointRepository { get; }

        // Asignación de roles a usuarios
        public IGenericRepository<RoleUser> RoleUserRepository { get; }

        // Permisos de menú por rol
        public IGenericRepository<RoleResourceMenu> RoleResourceMenuRepository { get; }

        // Permisos de endpoints por rol
        public IGenericRepository<RoleEndpoint> RoleEndpointRepository { get; }

        // Refresh tokens
        public IGenericRepository<RefreshToken> RefreshTokenRepository { get; }

        // Intentos de login
        public IGenericRepository<LoginAttempt> LoginAttemptRepository { get; }

        // JWT revocados (blacklist por JTI)
        public IGenericRepository<RevokedToken> RevokedTokenRepository { get; }

        // Dispositivos conocidos
        public IGenericRepository<KnownDevice> KnownDeviceRepository { get; }

        // Auditoría de login
        public IGenericRepository<LoginAudit> LoginAuditRepository { get; }

        // Claves criptográficas (RSA/AES)
        public IGenericRepository<CryptoKey> CryptoKeyRepository { get; }

        IGenericRepository<T> Repository<T>() where T : class;

        int SaveChanges();
        Task<int> SaveChangesAsync();

        // Transacciones sincronas (puedes dejarlas si las necesitas)
        IDbContextTransaction BeginTransaction();
        void Commit();
        void Rollback();

        // ✅ Transacciones asíncronas (necesarias)
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
