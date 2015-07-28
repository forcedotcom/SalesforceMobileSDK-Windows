namespace Core.Security
{
    public interface IEncryptionService
    {
        string Decrypt(string encryptedString);

        string Encrypt(string decryptedString);
    }
}
