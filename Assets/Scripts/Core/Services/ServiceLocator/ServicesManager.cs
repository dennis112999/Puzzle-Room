using System;
using System.Collections.Generic;

namespace Tools.ServiceLocator
{
    public static class ServicesManager
    {
        private static readonly IDictionary<Type, object> Services = new Dictionary<Type, object>();

        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);

            if (Services.ContainsKey(type))
            {
                throw new ApplicationException("Service already registered");
            }

            Services[type] = service;
        }

        public static void UnregisterService<T>()
        {
            var type = typeof(T);

            if (!Services.TryGetValue(type, out var service))
            {
                throw new ApplicationException("Requested service not found");
            }

            if (service is IDisposable disposable)
            {
                disposable.Dispose();
            }

            Services.Remove(type);
        }

        public static T GetService<T>()
        {
            if (Services.TryGetValue(typeof(T), out var service))
            {
                return (T)service;
            }

            throw new ApplicationException("Requested service not found");
        }

        public static void DisposeAll()
        {
            foreach (var pair in Services)
            {
                if (pair.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            Services.Clear();
        }
    }
}