using Microsoft.EntityFrameworkCore.Storage;

namespace SecuritySystem.Core.Interfaces.Core
{
    public interface IUnitOfWork : IDisposable
    {
        // IUnitOfWork.cs
        //public IGenericRepository<Aplicacion> AplicacionRepository { get; }
        //public IGenericRepository<AuditoriaLogin> AuditoriaLoginRepository { get; }
        //public IGenericRepository<DispositivoConocido> DispositivoConocidoRepository { get; }
        //public IGenericRepository<IntentoFallidoLogin> IntentoFallidoLoginRepository { get; }
        //public IGenericRepository<RefreshToken> RefreshTokenRepository { get; }
        //public IGenericRepository<Rol> RolRepository { get; }
        //public IGenericRepository<RolUsuario> RolUsuarioRepository { get; }
        //public IGenericRepository<TokenRevocado> TokenRevocadoRepository { get; }
        //public IGenericRepository<UsuarioSistema> UsuarioSistemaRepository { get; }
        //public IGenericRepository<Objeto> ObjetoRepository { get; }
        //public IGenericRepository<ObjetoMenu> ObjetoMenuRepository { get; }
        //public IGenericRepository<ObjetoPunto> ObjetoPuntoRepository { get; }
        //public IGenericRepository<RolObjetoMenu> RoleObjetoMenuRepository { get; }
        //public IGenericRepository<RolePunto> RolePuntoRepository { get; }

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
