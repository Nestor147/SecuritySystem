using SecuritySystem.Application.Interfaces.Authentication.Dtos;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IAppSigningKeyProvider
    {
        Task<AppKeyMaterial> GetActiveSigningKeyAsync(int applicationId, CancellationToken ct);
    }
}
