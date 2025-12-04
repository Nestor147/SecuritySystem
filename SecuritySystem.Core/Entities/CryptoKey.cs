using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class CryptoKey : AuditFields
    {
        public int Id { get; set; }
        public string Name { get; set; }             // e.g. "Auth.JwtMain"
        public byte KeyType { get; set; }           // 1=RSA signing, 2=RSA encryption, 3=AES, etc.
        public int Version { get; set; }

        public int? ApplicationId { get; set; }

        public string PublicKeyPem { get; set; }
        public byte[] EncryptedPrivateKey { get; set; }

        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Thumbprint { get; set; }
    }
}
