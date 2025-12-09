using SecuritySystem.Application.Interfaces.Authentication.Dtos;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken ct);
    }
}
