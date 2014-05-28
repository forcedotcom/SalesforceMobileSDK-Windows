using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Salesforce.SDK.Source.Security
{
    public interface IKeyGenerator
    {
        void GenerateKey(string salt, string password, out IBuffer keyMaterial, out IBuffer iv);
    }
}
