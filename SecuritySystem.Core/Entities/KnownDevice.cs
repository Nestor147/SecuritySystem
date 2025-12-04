using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi.Details;

namespace SecuritySystem.Core.Entities
{
    public class KnownDevice : AuditFields
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public string FingerprintHash { get; set; }
        public string DeviceName { get; set; }
        public string UserAgent { get; set; }
        public string IPAddress { get; set; }
    }
}
