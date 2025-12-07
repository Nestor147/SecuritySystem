using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class RoleUser : AuditFields
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserId { get; set; }
        public int IsInspector { get; set; }
        
    }
}
