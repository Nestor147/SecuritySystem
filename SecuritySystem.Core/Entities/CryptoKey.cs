using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class CryptoKey : AuditFields
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public byte KeyType { get; set; }  // 1 = RSA signing, etc.
        public int Version { get; set; }

        public int? ApplicationId { get; set; }

        public string PublicKeyPem { get; set; } = default!;
        public byte[] EncryptedPrivateKey { get; set; } = default!;

        public bool IsActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Thumbprint { get; set; }

        public int RecordStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}
