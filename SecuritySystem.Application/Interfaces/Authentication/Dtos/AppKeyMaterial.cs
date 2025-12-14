using Microsoft.IdentityModel.Tokens;

namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    public sealed class AppKeyMaterial
    {
        public int ApplicationId { get; }
        public string Kid { get; }
        public string PublicKeyPem { get; }
        public string PrivateKeyPem { get; }

        // ✅ opcional: key lista para firmar (evita ImportFromPem por request)
        public RsaSecurityKey? RsaKey { get; }

        public AppKeyMaterial(int applicationId, string kid, string publicKeyPem, string privateKeyPem, RsaSecurityKey? rsaKey = null)
        {
            ApplicationId = applicationId;
            Kid = kid;
            PublicKeyPem = publicKeyPem;
            PrivateKeyPem = privateKeyPem;
            RsaKey = rsaKey;
        }
    }

}
