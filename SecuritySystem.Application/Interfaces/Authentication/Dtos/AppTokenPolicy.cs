namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    public sealed class AppTokenPolicy
    {
        public int AccessTokenMinutes { get; init; }
        public int RefreshTokenDays { get; init; }
        public bool RequireMfa { get; init; }
    }
}
