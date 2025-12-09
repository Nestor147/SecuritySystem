using SecuritySystem.Application.Interfaces.Authentication.Dtos;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IAppTokenPolicyProvider
    {
        Task<AppTokenPolicy> GetPolicyAsync(int applicationId, CancellationToken ct);
    }
}
