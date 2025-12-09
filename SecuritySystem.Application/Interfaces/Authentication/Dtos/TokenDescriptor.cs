using System.Security.Claims;

namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    /// <summary>
    /// DTO used to describe what should go inside an access token.
    /// </summary>
    public sealed class TokenDescriptor
    {
        public string Subject { get; }
        public string Issuer { get; }
        public string Audience { get; }
        public string Jti { get; }
        public TimeSpan Lifetime { get; }

        /// <summary>
        /// Optional role names to add as ClaimTypes.Role.
        /// </summary>
        public IEnumerable<string>? Roles { get; }

        /// <summary>
        /// Optional scalar claims (string, int, etc.).
        /// </summary>
        public IDictionary<string, object>? Claims { get; }

        /// <summary>
        /// Extra Claim objects to be added as-is.
        /// </summary>
        public IEnumerable<Claim>? ExtraClaims { get; }

        public TokenDescriptor(
            string subject,
            string issuer,
            string audience,
            string jti,
            TimeSpan lifetime,
            IEnumerable<string>? roles = null,
            IDictionary<string, object>? claims = null,
            IEnumerable<Claim>? extraClaims = null)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            Audience = audience ?? throw new ArgumentNullException(nameof(audience));
            Jti = jti ?? throw new ArgumentNullException(nameof(jti));
            Lifetime = lifetime <= TimeSpan.Zero
                ? throw new ArgumentException("Lifetime must be positive.", nameof(lifetime))
                : lifetime;

            Roles = roles;
            Claims = claims;
            ExtraClaims = extraClaims;
        }
    }
}
