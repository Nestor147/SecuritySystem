using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class ResourceMenu : AuditFields
    {
        public int Id { get; set; }
        public int ResourceId { get; set; }

        public int Level { get; set; }
        public int IndentLevel { get; set; }
    }
}
