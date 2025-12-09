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
            if (d is null) throw new ArgumentNullException(nameof(d));
            if (string.IsNullOrWhiteSpace(d.Subject)) throw new ArgumentException("Subject is required.", nameof(d.Subject));
            if (string.IsNullOrWhiteSpace(d.Issuer)) throw new ArgumentException("Issuer is required.", nameof(d.Issuer));
            if (string.IsNullOrWhiteSpace(d.Audience)) throw new ArgumentException("Audience is required.", nameof(d.Audience));
            if (string.IsNullOrWhiteSpace(d.Jti)) throw new ArgumentException("Jti is required.", nameof(d.Jti));
            if (d.Lifetime <= TimeSpan.Zero) throw new ArgumentException("Lifetime must be positive.", nameof(d.Lifetime));

            using var rsa = RSA.Create();
            rsa.ImportFromPem(key.PrivateKeyPem.AsSpan());

            var rsaKey = new RsaSecurityKey(rsa) { KeyId = key.Kid };
            var creds = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256);

            var now = DateTime.UtcNow;
            var iat = ToUnix(now);

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, d.Subject),
            new(JwtRegisteredClaimNames.Jti, d.Jti),
            new(JwtRegisteredClaimNames.Iat, iat.ToString(), ClaimValueTypes.Integer64),
            new("app_id", key.ApplicationId.ToString())
        };

            if (d.Roles is not null)
            {
                foreach (var r in d.Roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
                    claims.Add(new Claim(ClaimTypes.Role, r));
            }

            if (d.ExtraClaims is not null)
                claims.AddRange(d.ExtraClaims);

            if (d.Claims is not null)
            {
                foreach (var kv in d.Claims)
                {
                    if (kv.Value is string s)
                        claims.Add(new Claim(kv.Key, s));
                    else
                        claims.Add(new Claim(kv.Key, kv.Value?.ToString() ?? string.Empty));
                }
            }

            var token = new JwtSecurityToken(
                issuer: d.Issuer,
                audience: d.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(d.Lifetime),
                signingCredentials: creds
            );

            token.Header["kid"] = key.Kid;

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static long ToUnix(DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();
    }
}
