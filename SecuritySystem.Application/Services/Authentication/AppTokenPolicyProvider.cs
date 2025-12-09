using Microsoft.Extensions.Caching.Memory;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Interfaces.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class AppTokenPolicyProvider : IAppTokenPolicyProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        public AppTokenPolicyProvider(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<AppTokenPolicy> GetPolicyAsync(int applicationId, CancellationToken ct)
        {
            var cacheKey = $"app-token-policy-{applicationId}";
            if (_cache.TryGetValue(cacheKey, out AppTokenPolicy cached))
                return cached;

            // ✅ Usamos el método del repositorio genérico
            var row = await _unitOfWork.ApplicationTokenPolicyRepository
                .FirstOrDefaultAsync(p => p.ApplicationId == applicationId && p.RecordStatus == 1, ct);

            var policy = row is null
                ? new AppTokenPolicy          // política por defecto
                {
                    AccessTokenMinutes = 10,
                    RefreshTokenDays = 15,
                    RequireMfa = false
                }
                : new AppTokenPolicy
                {
                    AccessTokenMinutes = row.AccessTokenMinutes,
                    RefreshTokenDays = row.RefreshTokenDays,
                    RequireMfa = row.RequireMfa
                };

            _cache.Set(cacheKey, policy, TimeSpan.FromMinutes(10));
            return policy;
        }
    }
}
