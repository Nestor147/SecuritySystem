using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class User : AuditFields
    {
        public int Id { get; set; }
        public int? ExternalUserId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }

        public string PasswordHash { get; set; }
        public DateTime? LastPasswordChange { get; set; }

        public bool IsLocked { get; set; }
        public DateTime? LockDate { get; set; }

        public bool IsNewUser { get; set; }
        public bool KeepLoggedIn { get; set; }
    }
}
