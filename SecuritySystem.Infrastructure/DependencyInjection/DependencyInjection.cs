using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecuritySystem.Application.Interfaces.Authorization;
using SecuritySystem.Application.Services.Authorization;
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
            //services.AddScoped<IAuthService, AuthService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            ////validadores
            services.AddTransient<IAuthorizationRepositoryValidator, AuthorizationRepositoryValidator>();
            ////Repositorios

            ////helper
            services.AddScoped<IHelperProcessVal, HelpersRepositoryVal>();
            services.AddScoped<IAuthorizationRepository, AuthorizationRepository>();
            services.AddScoped<IApplicationsService, ApplicationsService>();
            services.AddScoped<IMenuService, MenuService>();
            //services.AddScoped<IObjetoService, ObjetoService>();
            //services.AddScoped<IRolService, RolService>();

            //ADO
            services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddScoped<IAdo, Ado>();

            return services;
        }
    }
}
