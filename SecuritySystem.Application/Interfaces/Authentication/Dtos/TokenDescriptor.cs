using System.Security.Claims;

namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    /// <summary>
    /// DTO used to describe what should go inside an access token.
    /// </summary>
    /// 
    public sealed class TokenDescriptor
    {
        public string Subject { get; }
        public string Issuer { get; }
        public string Audience { get; }
        public string Jti { get; }
        public TimeSpan Lifetime { get; }
        public IEnumerable<string>? Roles { get; }
        public IDictionary<string, object>? Claims { get; }
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
            Subject = subject;
            Issuer = issuer;
            Audience = audience;
            Jti = jti;
            Lifetime = lifetime;
            Roles = roles;
            Claims = claims;
            ExtraClaims = extraClaims;
        }
    }

}



