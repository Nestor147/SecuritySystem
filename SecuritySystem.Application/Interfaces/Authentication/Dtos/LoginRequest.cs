namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    public sealed class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int ApplicationId { get; set; }
    }
}
