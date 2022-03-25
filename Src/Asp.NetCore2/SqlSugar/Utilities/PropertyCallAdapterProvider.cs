using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
namespace SqlSugar
{
    public interface IPropertyCallAdapter<TThis>
    {
        object InvokeGet(TThis @this);
        //add void InvokeSet(TThis @this, object value) if necessary
    }
    public class PropertyCallAdapter<TThis, TResult> : IPropertyCallAdapter<TThis>
    {
        private readonly Func<TThis, TResult> _getterInvocation;

        public PropertyCallAdapter(Func<TThis, TResult> getterInvocation)
        {
            _getterInvocation = getterInvocation;
        }

        public object InvokeGet(TThis @this)
        {
            return _getterInvocation.Invoke(@this);
        }
    }
    public class PropertyCallAdapterProvider<TThis>
    {
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, IPropertyCallAdapter<TThis>> _instances =
            new System.Collections.Concurrent.ConcurrentDictionary<string, IPropertyCallAdapter<TThis>>();

        public static IPropertyCallAdapter<TThis> GetInstance(string forPropertyName)
        {
            IPropertyCallAdapter<TThis> instance;
            if (!_instances.TryGetValue(forPropertyName, out instance))
            {
                var property = typeof(TThis).GetProperty(
                    forPropertyName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                MethodInfo getMethod;
                Delegate getterInvocation = null;
                if (property != null && (getMethod = property.GetGetMethod(true)) != null)
                {
                    var openGetterType = typeof(Func<,>);
                    var concreteGetterType = openGetterType
                        .MakeGenericType(typeof(TThis), property.PropertyType);

                    getterInvocation =
                        Delegate.CreateDelegate(concreteGetterType, null, getMethod);
                }
                else
                {
                    //throw exception or create a default getterInvocation returning null
                }

                var openAdapterType = typeof(PropertyCallAdapter<,>);
                var concreteAdapterType = openAdapterType
                    .MakeGenericType(typeof(TThis), property.PropertyType);
                instance = Activator
                    .CreateInstance(concreteAdapterType, getterInvocation)
                        as IPropertyCallAdapter<TThis>;

                _instances.GetOrAdd(forPropertyName, instance);
            }

            return instance;
        }
    }
}