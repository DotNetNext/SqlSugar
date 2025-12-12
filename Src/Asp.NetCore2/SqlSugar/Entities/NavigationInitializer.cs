using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;

namespace SqlSugar
{
    internal static class OneToOneGlobalInstanceRegistry
    {
        private static readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private static readonly object _lock = new object();

        public static Dictionary<Type, object> Instances => _instances;

        public static bool IsNavWithDefaultValue(PropertyInfo navObjectNamePropety)
        {
            return OneToOneGlobalInstanceRegistry.Instances?.ContainsKey(navObjectNamePropety.PropertyType) == true;
        }
        public static bool IsAny() 
        {
            return _instances?.Count>0;
        }
        public static bool IsNavigationInitializerCreated(object instance)
        {
            if (instance == null)
                return false;

            Type type = instance.GetType();

            lock (_lock)
            {
                return _instances.ContainsKey(type) && _instances[type] == instance;
            }
        }

        public static void RegisterInstance(Type type, object instance)
        {
            lock (_lock)
            {
                if (!_instances.ContainsKey(type))
                {
                    _instances[type] = instance;
                }
            }
        }
    }

    public class OneToOneInitializer<T> where T : new()
    {
        public static implicit operator T(OneToOneInitializer<T> initializer)
        {
            Type type = typeof(T);

            if (!OneToOneGlobalInstanceRegistry.Instances.ContainsKey(type))
            {
                T instance = new T();
                OneToOneGlobalInstanceRegistry.RegisterInstance(type, instance);
            }

            return (T)OneToOneGlobalInstanceRegistry.Instances[type];
        }
    }
}
