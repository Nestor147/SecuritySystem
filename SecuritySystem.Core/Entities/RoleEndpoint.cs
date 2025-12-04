using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class RoleEndpoint : AuditFields
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ResourceEndpointId { get; set; }
    }
}
