using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SecuritySystem.Core.Entities;
using SecuritySystem.Core.Interfaces.Core;
using System.Security.Cryptography;

namespace SecuritySystem.Web.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class JwksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public JwksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Public JWKS endpoint exposing all active RSA signing keys.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("/.well-known/jwks.json")]
        public async Task<IActionResult> GetJwks(CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            // Tomamos solo llaves RSA de firma activas
            var keys = await _unitOfWork.CryptoKeyRepository
                .WhereAsync(k =>
                    k.KeyType == 1 &&          // 1 = RSA signing
                    k.IsActive == true &&
                    k.RecordStatus == 1 &&
                    k.StartDate <= now &&
                    (k.EndDate == null || k.EndDate > now),
                    ct);

            var jwks = new
            {
                keys = keys.Select(ToJwk).ToList()
            };

            return Ok(jwks);
        }

        private static object ToJwk(CryptoKey key)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(key.PublicKeyPem.AsSpan());
            var parameters = rsa.ExportParameters(false);

            return new
            {
                kty = "RSA",
                use = "sig",
                alg = "RS256",
                kid = key.Thumbprint ?? $"app-{key.ApplicationId}-v{key.Version}",

                // n y e en Base64URL
                n = Base64UrlEncode(parameters.Modulus!),
                e = Base64UrlEncode(parameters.Exponent!),

                // Campo extra para que tus APIs sepan de qué aplicación es esta key
                app_id = key.ApplicationId
            };
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
