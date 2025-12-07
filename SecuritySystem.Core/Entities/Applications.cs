using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class Applications : AuditFields
    {
        public int Id { get; set; }
        public string Code { get; set; }          // e.g. "ATACADO"
        public string Name { get; set; }          // descriptive app name
        public string Url { get; set; }
        public string Icon { get; set; }
    }
}
