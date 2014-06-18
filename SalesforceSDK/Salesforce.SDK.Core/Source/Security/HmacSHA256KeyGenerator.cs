using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.System.Profile;

namespace Salesforce.SDK.Source.Security
{
    /// <summary>
    /// This class is a sample encryption key generator. It is highly recommended that you roll your own and provide it's configuration in your Application class
    /// extending SalesforceApplication or SaleforcePhoneApplication. 
    /// </summary>
    public sealed class HmacSHA256KeyGenerator : IKeyGenerator
    {
        public void GenerateKey(string password, string salt, out Windows.Storage.Streams.IBuffer keyMaterial, out Windows.Storage.Streams.IBuffer iv)
        {
            IBuffer saltBuffer = CryptographicBuffer.ConvertStringToBinary(salt, BinaryStringEncoding.Utf8);
            KeyDerivationParameters keyParams = KeyDerivationParameters.BuildForSP800108(saltBuffer, GetNonce());
            KeyDerivationAlgorithmProvider kdf = KeyDerivationAlgorithmProvider.OpenAlgorithm(Encryptor.Settings.KeyDerivationAlgorithm);
            IBuffer passwordBuffer = CryptographicBuffer.ConvertStringToBinary(password, BinaryStringEncoding.Utf8);
            CryptographicKey keyOriginal = kdf.CreateKey(passwordBuffer);

            int keySize = 256;
            int ivSize = 128 / 8;
            uint totalData = (uint)(keySize + ivSize);
            IBuffer keyDerived = CryptographicEngine.DeriveKeyMaterial(keyOriginal, keyParams, totalData);
            
            byte[] keyMaterialBytes = keyDerived.ToArray();
            keyMaterial = WindowsRuntimeBuffer.Create(keyMaterialBytes, 0, keySize, keySize);
            iv = WindowsRuntimeBuffer.Create(keyMaterialBytes, keySize, ivSize, ivSize);
        }

        /// <summary>
        /// It is recommended you generate a way that is unique for the app/device. In this example we used the network adapter ID and FullName of the type of this class.
        /// </summary>
        /// <returns></returns>
        private static string GetDeviceUniqueId()
        {
            var networkProfiles = Windows.Networking.Connectivity.NetworkInformation.GetConnectionProfiles();
            var adapter = networkProfiles[0].NetworkAdapter;
            HashAlgorithmProvider alg = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(adapter.NetworkAdapterId.ToString() + typeof(HmacSHA256KeyGenerator).FullName, BinaryStringEncoding.Utf8);
            IBuffer hashed = alg.HashData(buff);
            return CryptographicBuffer.EncodeToHexString(hashed);
        }

        /// <summary>
        /// This utility function returns a nonce value for authenticated encryption modes.
        /// </summary>
        /// <returns></returns>
        private static IBuffer GetNonce()
        {
            return CryptographicBuffer.ConvertStringToBinary(GetDeviceUniqueId(), BinaryStringEncoding.Utf8);
        }
    }
}
