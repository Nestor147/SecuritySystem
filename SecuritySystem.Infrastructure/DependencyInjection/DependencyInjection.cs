using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Application.Services.Authentication;
using SecuritySystem.Application.Services.Authorization;
using SecuritySystem.Application.Services.Users;
using SecuritySystem.Core.Interfaces;
using SecuritySystem.Core.Interfaces.Core;
using SecuritySystem.Core.Interfaces.Core.SQLServer;
using SecuritySystem.Core.Interfaces.Core.SQLServer.ADO;
using SecuritySystem.Core.Interfaces.Validators;
using SecuritySystem.Core.Interfaces.Validators.Helpers;
using SecuritySystem.Infrastructure.Context;
using SecuritySystem.Infrastructure.Context.Core.SQLServer;
using SecuritySystem.Infrastructure.Context.Core.SQLServer.ADO;
using SecuritySystem.Infrastructure.Repositories;
using SecuritySystem.Infrastructure.Validators;
using SecuritySystem.Infrastructure.Validators.Helpers;

namespace SecuritySystem.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IAuthorizationRepositoryValidator, AuthorizationRepositoryValidator>();

            // helper
            services.AddScoped<IHelperProcessVal, HelpersRepositoryVal>();
            services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
            services.AddScoped<IApplicationsService, ApplicationsService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IResourceService, ResourceService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // 🔐 App key & token policy providers
            services.AddMemoryCache();
            services.AddScoped<IAppSigningKeyProvider, AppSigningKeyProvider>();
            services.AddScoped<IAppTokenPolicyProvider, AppTokenPolicyProvider>();

            // 🔐 NUEVO: protector de llave privada (dummy por ahora)
            services.AddScoped<IPrivateKeyProtector, AesGcmPrivateKeyProtector>();

            // Auth / Users
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            // ADO
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IAdo, Ado>();

            return services;
        }
    }
}
