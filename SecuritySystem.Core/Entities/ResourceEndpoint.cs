using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class ResourceEndpoint : AuditFields
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }

        public int ServiceType { get; set; }      // e.g. GET/POST code
        public string Endpoint { get; set; }      // route/url/action
        public string Description { get; set; }
    }
}
