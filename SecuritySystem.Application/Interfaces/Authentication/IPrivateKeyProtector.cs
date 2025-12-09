namespace SecuritySystem.Application.Interfaces.Authentication
{
    public interface IPrivateKeyProtector
    {
        byte[] EncryptPrivateKey(string privateKeyPem);
        string DecryptPrivateKey(byte[] encryptedPrivateKey);
    }
}
