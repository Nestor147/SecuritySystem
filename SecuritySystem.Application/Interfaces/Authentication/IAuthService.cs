using SecuritySystem.Application.Interfaces.Authentication.Dtos;
using SecuritySystem.Core.Entities.SealedAuthentication;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IAuthService
    {
        Task<LoginResult> LoginAsync(LoginRequest req, CancellationToken ct);
        Task<LoginResult> RefreshAsync(RefreshRequest req, CancellationToken ct);
        Task LogoutAsync(LogoutRequest req, CancellationToken ct);
    }
}
