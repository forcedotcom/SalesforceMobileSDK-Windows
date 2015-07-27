using System;
using Core.Logging;
using Core.Security;
using Core.Settings;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Salesforce.SDK.Auth;

namespace Salesforce.SDK.Core
{
    public class SDKServiceLocator
    {
        /// <summary>
        /// Our default service locator provider
        /// </summary>
        public static IServiceLocator Default
        {
            get
            {
                return ServiceLocator.Current;
            }
        }

        /// <summary>
        /// Registers the given type with the default service locator provider.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered</typeparam>
        public static void RegisterService<TClass>()
            where TClass : class
        {
            SimpleIoc.Default.Register<TClass>();
        }

        /// <summary>
        /// Registers the given type with the default service locator provider.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered</typeparam>
        /// <param name="factory">The factory method that creates the instance when the given type is resolved.</param>
        public static void RegisterService<TClass>(Func<TClass> factory)
            where TClass : class
        {
            SimpleIoc.Default.Register<TClass>(factory);
        }

        /// <summary>
        /// Registers the given type with the default service locator provider with the given key.
        /// </summary>
        /// <typeparam name="TClass">The type that is being registered</typeparam>
        /// <param name="factory">The factory method that creates the instance when the given type is resolved.</param>
        /// <param name="key">The key for which the given instance is registered.</param>
        public static void RegisterService<TClass>(Func<TClass> factory, string key)
            where TClass : class
        {
            SimpleIoc.Default.Register<TClass>(factory, key);
        }

        /// <summary>
        /// Registers the given type with the default service locator provider for the given interface.
        /// </summary>
        /// <typeparam name="TInterface">The interface for which instances will be resolved.</typeparam>
        /// <typeparam name="TClass">The type that is being registered</typeparam>
        public static void RegisterService<TInterface, TClass>()
            where TInterface : class
            where TClass : class
        {
            SimpleIoc.Default.Register<TInterface, TClass>();
        }

        /// <summary>
        /// Shorthand way to get the registered instance of a given service.
        /// </summary>
        /// <returns>The registered implementation of the service.</returns>
        /// <typeparam name="TInterface">The Type of the service to get.</typeparam>
        public static TService Get<TService>()
            where TService : class
        {
            return Default.GetInstance<TService>();
        }

        /// <summary>
        /// Unregisters a class from the service locator provider and removes all the previously created instances.
        /// </summary>
        /// <typeparam name="TClass">The type that is being unregistered</typeparam>
        public static void UnRegisterService<TClass>()
            where TClass : class
        {
            SimpleIoc.Default.Unregister<TClass>();
        }

        /// <summary>
        /// Unregisters the instance with the given key
        /// </summary>
        /// <typeparam name="TClass">The type that is being unregistered</typeparam>
        /// <param name="key">The key of the instance</param>
        public static void UnRegister<TClass>(string key)
            where TClass : class
        {
            SimpleIoc.Default.Unregister<TClass>(key);
        }

        /// <summary>
        /// Returns a value indicating if the given type is registered in the IOC.
        /// </summary>
        /// <returns><c>true</c> if the given type is registered; otherwise, <c>false</c>.</returns>
        /// <param name="type">The type to check</param>
        public static bool IsServiceRegistered(Type type)
        {
            // There seems to be a bug with SimpleIOC where it will
            // return false if a service is registered with a factory
            // so beware
            return SimpleIoc.Default.IsRegistered<Type>();
        }

        /// <summary>
        /// Register our default locator provider and any global
        /// instances we need here
        /// </summary>
        static SDKServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
        }

        public static void RegisterSDKServices(
            IAuthHelper authHelper, 
            ILoggingService loggingService, 
            IApplicationInformationService appInfoService,
            IEncryptionService encryptionService)
        {
            
        }
    }
}
