using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Interfaces.Core;
using System.Security.Cryptography;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class AppSigningKeyProvider : IAppSigningKeyProvider
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPrivateKeyProtector _protector;
        private readonly IMemoryCache _cache;

        public AppSigningKeyProvider(IUnitOfWork unitOfWork, IPrivateKeyProtector protector, IMemoryCache cache)
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

            try
            {
                var now = DateTime.UtcNow;

                var keys = await _unitOfWork.CryptoKeyRepository.WhereAsync(k =>
                       k.ApplicationId == applicationId
                    && k.KeyType == 1
                    && k.IsActive == true
                    && k.RecordStatus == 1
                    && k.StartDate <= now
                    && (k.EndDate == null || k.EndDate > now),
                    ct);

                var keyEntity = keys
                    .OrderByDescending(k => k.Version)
                    .FirstOrDefault()
                    ?? throw new InvalidOperationException($"No active signing key found for ApplicationId={applicationId}");

                var privatePem = _protector.DecryptPrivateKey(keyEntity.EncryptedPrivateKey);
                var kid = keyEntity.Thumbprint ?? $"app-{applicationId}-v{keyEntity.Version}";

                if (string.IsNullOrWhiteSpace(privatePem))
                    throw new InvalidOperationException($"Decrypted private key PEM is empty for ApplicationId={applicationId} (Kid={kid}).");

                if (string.IsNullOrWhiteSpace(kid))
                    throw new InvalidOperationException($"Signing key KID is empty for ApplicationId={applicationId}.");

                // ✅ Parse RSA ONCE and cache it
                var rsa = RSA.Create();
                rsa.ImportFromPem(privatePem.AsSpan());

                var rsaKey = new RsaSecurityKey(rsa) { KeyId = kid };

                var material = new AppKeyMaterial(
                    applicationId,
                    kid,
                    keyEntity.PublicKeyPem,
                    privatePem,
                    rsaKey
                );

                // Cache + dispose RSA when entry is evicted
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
                    .RegisterPostEvictionCallback((k, v, reason, state) =>
                    {
                        if (v is AppKeyMaterial m && m.RsaKey?.Rsa is IDisposable d)
                        {
                            try { d.Dispose(); } catch { }
                        }
                    });

                _cache.Set(cacheKey, material, cacheOptions);

                return material;
            }
            catch
            {
                // Important: bubble up with the real exception
                throw;
            }
        }
    }
}
