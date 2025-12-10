using SecuritySystem.Core.Entities.core.CustomEntities.ResponseApi;
using SecuritySystem.Core.Entities.SealedAuthentication;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IUserService
    {
        Task<ResponsePost> CreateUserAsync(CreateUserRequest request, CancellationToken ct);
    }
}
