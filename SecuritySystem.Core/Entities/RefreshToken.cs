using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class RefreshToken : AuditFields
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public int ApplicationId { get; set; }

        public string TokenHash { get; set; }
        public DateTime ExpiresAt { get; set; }

        public bool Used { get; set; }
        public bool Revoked { get; set; }

        public DateTime TokenCreatedAt { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
    }
}
