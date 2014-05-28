using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Salesforce.SDK.Source.Security
{
    public sealed class EncryptionSettings
    {
        public string Password { get; set; }
        public string Salt { get; set; }
        public string SymmetricAlgorithm { get; private set; }
        public string KeyDerivationAlgorithm { get; private set; }
        private IKeyGenerator KeyGenerator;

        public EncryptionSettings(IKeyGenerator keyGenerator)
        {
            SymmetricAlgorithm = Encryptor.PreferredSymmetricAlgorithm;
            KeyDerivationAlgorithm = Encryptor.PreferredKeyDerivationAlgorithm;
            KeyGenerator = keyGenerator;
        }

        public void GenerateKey(out IBuffer keyMaterial, out IBuffer iv)
        {
            KeyGenerator.GenerateKey(Password, Salt, out keyMaterial, out iv);
        }
    }
}
