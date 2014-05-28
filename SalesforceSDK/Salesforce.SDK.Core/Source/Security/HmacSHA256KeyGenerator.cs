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

        private static string GetHardwareId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;
            var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(hardwareId);

            byte[] bytes = new byte[hardwareId.Length];
            dataReader.ReadBytes(bytes);

            return BitConverter.ToString(bytes);
        }

        /// <summary>
        /// This utility function returns a nonce value for authenticated encryption modes.
        /// </summary>
        /// <returns></returns>
        private static IBuffer GetNonce()
        {
            return CryptographicBuffer.ConvertStringToBinary(GetHardwareId(), BinaryStringEncoding.Utf8);
        }
    }
}
