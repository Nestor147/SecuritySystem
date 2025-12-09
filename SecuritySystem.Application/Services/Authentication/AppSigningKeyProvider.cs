using Microsoft.Extensions.Caching.Memory;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Interfaces.Core;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class AppSigningKeyProvider : IAppSigningKeyProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrivateKeyProtector _protector;
        private readonly IMemoryCache _cache;

        public AppSigningKeyProvider(
            IUnitOfWork unitOfWork,
            IPrivateKeyProtector protector,
            IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _protector = protector;
            _cache = cache;
        }

        public async Task<AppKeyMaterial> GetActiveSigningKeyAsync(int applicationId, CancellationToken ct)
        {
            var cacheKey = $"app-signing-key-{applicationId}";
            if (_cache.TryGetValue(cacheKey, out AppKeyMaterial cached))
                return cached;

            var now = DateTime.UtcNow;

            // ✅ Usamos WhereAsync del repositorio genérico
            var keys = await _unitOfWork.CryptoKeyRepository.WhereAsync(k =>
                   k.ApplicationId == applicationId
                && k.KeyType == 1               // 1 = RSA signing
                && k.IsActive == true
                && k.RecordStatus == 1
                && k.StartDate <= now
                && (k.EndDate == null || k.EndDate > now),
                ct);

            var keyEntity = keys
                .OrderByDescending(k => k.Version)
                .FirstOrDefault()
                ?? throw new InvalidOperationException(
                    $"No active signing key found for ApplicationId={applicationId}");

            var privatePem = _protector.DecryptPrivateKey(keyEntity.EncryptedPrivateKey);
            var kid = keyEntity.Thumbprint ?? $"app-{applicationId}-v{keyEntity.Version}";

            var material = new AppKeyMaterial(
                applicationId,
                kid,
                keyEntity.PublicKeyPem,
                privatePem
            );

            _cache.Set(cacheKey, material, TimeSpan.FromMinutes(5));
            return material;
        }
    }
}
