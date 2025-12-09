namespace SecuritySystem.Core.Entities
{
    public class ApplicationTokenPolicy
    {
        public int ApplicationId { get; set; }
        public int AccessTokenMinutes { get; set; }
        public int RefreshTokenDays { get; set; }
        public bool RequireMfa { get; set; }
        public int RecordStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}
