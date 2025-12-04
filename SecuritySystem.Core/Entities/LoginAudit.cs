using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class LoginAudit : AuditFields
    {
        public int Id { get; set; }
        public int? UserId { get; set; }

        public string Username { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }

        public bool? IsSuccessful { get; set; }
        public string Message { get; set; }

        public DateTime LoggedAt { get; set; }
    }
}
