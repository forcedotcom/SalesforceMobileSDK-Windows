using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Salesforce.SDK.Source.Security
{
    public class Encryptor
    {
        public static readonly string PreferredSymmetricAlgorithm = SymmetricAlgorithmNames.AesCbcPkcs7;
        public static readonly string PreferredKeyDerivationAlgorithm = KeyDerivationAlgorithmNames.Sp800108CtrHmacSha256;
        public static EncryptionSettings Settings { get; private set; }

        public static void init(EncryptionSettings settings)
        {
            Settings = settings;
        }

        public static string Encrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            IBuffer keyMaterial;
            IBuffer iv;
            Settings.GenerateKey(out keyMaterial, out iv);

            IBuffer clearTextBuffer = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);

            // Setup an AES key, using AES in CBC mode and applying PKCS#7 padding on the input
            SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(Settings.SymmetricAlgorithm);
            CryptographicKey key = provider.CreateSymmetricKey(keyMaterial);

            // Encrypt the data and convert it to a Base64 string
            IBuffer encrypted = CryptographicEngine.Encrypt(key, clearTextBuffer, iv);
            string ciphertextString = CryptographicBuffer.EncodeToBase64String(encrypted);

            return ciphertextString;
        }

        public static string Decrypt(string text)
        {
            if (String.IsNullOrWhiteSpace(text))
            {
                return null;
            }
            IBuffer keyMaterial;
            IBuffer iv;
            Settings.GenerateKey(out keyMaterial, out iv);

            SymmetricKeyAlgorithmProvider provider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(Settings.SymmetricAlgorithm);
            CryptographicKey key = provider.CreateSymmetricKey(keyMaterial);

            IBuffer ciphertextBuffer = CryptographicBuffer.DecodeFromBase64String(text);
            IBuffer decryptedBuffer = CryptographicEngine.Decrypt(key, ciphertextBuffer, iv);
            byte[] decryptedArray = decryptedBuffer.ToArray();
            return Encoding.UTF8.GetString(decryptedArray, 0, decryptedArray.Length);
        }
    }
}
