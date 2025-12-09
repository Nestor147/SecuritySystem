namespace SecuritySystem.Application.Interfaces.Authentication.Dtos
{
    public sealed record AppKeyMaterial(
        int ApplicationId,
        string Kid,
        string PublicKeyPem,
        string PrivateKeyPem // decrypted, used only in memory
    );
}
