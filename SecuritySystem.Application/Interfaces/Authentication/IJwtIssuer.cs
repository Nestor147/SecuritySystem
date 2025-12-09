using SecuritySystem.Application.Interfaces.Authentication.Dtos;

namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IJwtIssuer
    {
        string CreateAccessToken(TokenDescriptor descriptor, AppKeyMaterial key);
    }
}
