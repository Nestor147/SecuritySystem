using Microsoft.IdentityModel.Tokens;
using SecuritySystem.Application.Interfaces.Authentication;
using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SecuritySystem.Application.Services.Authentication
{
    public sealed class RsaJwtIssuer : IJwtIssuer
    {
        public string CreateAccessToken(TokenDescriptor d, AppKeyMaterial key)
        {
            // Validaciones iniciales
            if (d is null) throw new ArgumentNullException(nameof(d));
            if (string.IsNullOrWhiteSpace(d.Subject)) throw new ArgumentException("Subject is required.", nameof(d.Subject));
            if (string.IsNullOrWhiteSpace(d.Issuer)) throw new ArgumentException("Issuer is required.", nameof(d.Issuer));
            if (string.IsNullOrWhiteSpace(d.Audience)) throw new ArgumentException("Audience is required.", nameof(d.Audience));
            if (string.IsNullOrWhiteSpace(d.Jti)) throw new ArgumentException("Jti is required.", nameof(d.Jti));
            if (d.Lifetime <= TimeSpan.Zero) throw new ArgumentException("Lifetime must be positive.", nameof(d.Lifetime));

            // Validación del key (ya debería tener RsaKey preprocesada)
            if (key is null) throw new InvalidOperationException("Signing key material is null.");
            if (key.RsaKey == null) throw new InvalidOperationException("RSA signing key is not available.");
            if (string.IsNullOrWhiteSpace(key.Kid)) throw new InvalidOperationException("Signing key KID is missing.");

            // Credenciales de firma (usando la clave RSA cargada)
            var creds = new SigningCredentials(key.RsaKey, SecurityAlgorithms.RsaSha256);

            var now = DateTime.UtcNow;
            var iat = ToUnix(now); // Fecha de creación (issued at)

            // Creación de claims para el token
            var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, d.Subject),        // Subject (usuario)
        new(JwtRegisteredClaimNames.Jti, d.Jti),            // JTI (JWT ID)
        new(JwtRegisteredClaimNames.Iat, iat.ToString(), ClaimValueTypes.Integer64), // Timestamp de emisión
        new("app_id", key.ApplicationId.ToString())         // ID de la aplicación
    };

            // Roles (si están definidos)
            if (d.Roles != null)
            {
                foreach (var r in d.Roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
                    claims.Add(new Claim(ClaimTypes.Role, r));
            }

            // Claims adicionales definidos por el usuario
            if (d.ExtraClaims != null)
                claims.AddRange(d.ExtraClaims);

            // Claims personalizados
            if (d.Claims != null)
            {
                foreach (var kv in d.Claims)
                {
                    if (kv.Value is string s)
                        claims.Add(new Claim(kv.Key, s));
                    else
                        claims.Add(new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty));
                }
            }

            // Creación del token con los claims y la firma
            var token = new JwtSecurityToken(
                issuer: d.Issuer,
                audience: d.Audience,
                claims: claims,
                notBefore: now,                             // No válido antes de la emisión
                expires: now.Add(d.Lifetime),              // Expiración del token
                signingCredentials: creds                  // Credenciales de firma RSA
            );

            // Agregar "kid" en el header
            token.Header["kid"] = key.Kid;

            // Retornar el token como string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static long ToUnix(DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();

    }
}
