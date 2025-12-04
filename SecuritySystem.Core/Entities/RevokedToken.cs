using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class RevokedToken : AuditFields
    {
        public Guid Jti { get; set; }            // JWT Id
        public int UserId { get; set; }

        public string Reason { get; set; }
        public DateTime RevokedAt { get; set; }
    }
}
