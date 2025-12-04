using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class Role : AuditFields
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
    }
}
