using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Salesforce.WinSDK.Adaptation
{
    enum Platform
    {
        Phone,
        Store
    }

    public class PlatformNotSupportedException : Exception { }

    public class PlatformAdapter
    {
        private static readonly Object _lock = new Object();
        private static Dictionary<Type, Object> _resolveCache = new Dictionary<Type, Object>();

        private static Assembly _platformAssembly;
        static Assembly PlatformAssembly
        {
            get
            {
                if (_platformAssembly == null)
                {
                    foreach (Platform platform in Enum.GetValues(typeof(Platform)))
                    {
                        _platformAssembly = TryLoadPlatformAssembly(platform);
                        if (_platformAssembly != null)
                        {
                            break;
                        }
                    }
                }
                return _platformAssembly;
            }
        }


        public static T Resolve<T>()
        {
            Type interfaceType = typeof(T);
            lock (_lock)
            {
                Object instance;
                if (!_resolveCache.TryGetValue(interfaceType, out instance))
                {
                    Type concreteType = GetConcreteType(interfaceType);
                    if (concreteType != null)
                    {
                        instance = Activator.CreateInstance(concreteType);
                        _resolveCache.Add(interfaceType, instance);
                    }
                }

                if (instance == null)
                {
                    throw new PlatformNotSupportedException();
                }

                return (T)instance;
            }
        }

        private static Type GetConcreteType(Type interfaceType)
        {
            // For instance interface Foo.IAbc should have a concreate class Foo.Abc
            String concreteTypeName = interfaceType.Namespace + "." + interfaceType.Name.Substring(1);
            return PlatformAssembly.GetType(concreteTypeName);
        }

        private static Assembly TryLoadPlatformAssembly(Platform platform)
        {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "SalesforceSDK." + platform.ToString();
            try
            {
                return Assembly.Load(assemblyName);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}