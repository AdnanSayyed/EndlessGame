using System;
using System.Collections.Generic;

namespace EndlessGame.Service
{
    public static class ServiceLocator
    {
        private static Dictionary<Type, object> services = new Dictionary<Type, object>();

        public static void RegisterService<T>(T service)
        {
            var type = typeof(T);
            if (!services.ContainsKey(type))
            {
                services[type] = service;
            }
        }

        public static T GetService<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                return (T)services[type];
            }
            throw new Exception($"Service of type {type} not registered.");
        }

        public static bool IsServiceRegistered<T>()
        {
            return services.ContainsKey(typeof(T));
        }

        public static void UnregisterService<T>()
        {
            var type = typeof(T);
            if (services.ContainsKey(type))
            {
                services.Remove(type);
            }
        }
    }
}