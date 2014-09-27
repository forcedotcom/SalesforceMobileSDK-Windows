/*
 * Copyright (c) 2013, salesforce.com, inc.
 * All rights reserved.
 * Redistribution and use of this software in source and binary forms, with or
 * without modification, are permitted provided that the following conditions
 * are met:
 * - Redistributions of source code must retain the above copyright notice, this
 * list of conditions and the following disclaimer.
 * - Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 * - Neither the name of salesforce.com, inc. nor the names of its contributors
 * may be used to endorse or promote products derived from this software without
 * specific prior written permission of salesforce.com, inc.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Salesforce.SDK.Adaptation
{
    internal enum Platform
    {
        Phone,
        Store
    }

    public class PlatformNotSupportedException : Exception
    {
    }

    public class PlatformAdapter
    {
        private static readonly object Lock = new object();
        private static readonly Dictionary<Type, object> ResolveCache = new Dictionary<Type, object>();

        private static Assembly _platformAssembly;

        private static Assembly PlatformAssembly
        {
            get
            {
                if (_platformAssembly == null)
                {
                    foreach (Platform platform in Enum.GetValues(typeof (Platform)))
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
            Type interfaceType = typeof (T);
            lock (Lock)
            {
                object instance;
                if (!ResolveCache.TryGetValue(interfaceType, out instance))
                {
                    Type concreteType = GetConcreteType(interfaceType);
                    if (concreteType != null)
                    {
                        instance = Activator.CreateInstance(concreteType);
                        ResolveCache.Add(interfaceType, instance);
                    }
                }

                if (instance == null)
                {
                    throw new PlatformNotSupportedException();
                }

                return (T) instance;
            }
        }

        private static Type GetConcreteType(Type interfaceType)
        {
            // For instance interface Foo.IAbc should have a concreate class Foo.Abc
            string concreteTypeName = interfaceType.Namespace + "." + interfaceType.Name.Substring(1);
            return PlatformAssembly.GetType(concreteTypeName);
        }

        private static Assembly TryLoadPlatformAssembly(Platform platform)
        {
            var assemblyName = new AssemblyName {Name = "Salesforce.SDK." + platform};
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