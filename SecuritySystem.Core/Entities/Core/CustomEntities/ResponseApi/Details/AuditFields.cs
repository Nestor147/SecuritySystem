namespace SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details
{
    public class AuditFields
    {

        public string CreatedBy { get; set; } = "SECURITY_SYSTEM";
        public int RecordStatus { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
