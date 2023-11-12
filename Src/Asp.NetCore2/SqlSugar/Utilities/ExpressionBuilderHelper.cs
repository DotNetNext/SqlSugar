using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class ExpressionBuilderHelper
    {
        public static object CallFunc(Type type, object[] param, object methodData, string methodName)
        {
            MethodInfo mi = methodData.GetType().GetMethod(methodName).MakeGenericMethod(new Type[] { type });
            var ret = mi.Invoke(methodData, param);
            return ret;
        }
        public static T CallFunc<T>(object param, object methodData, string methodName)
        {
            Type type = param.GetType();
            MethodInfo mi = methodData.GetType().GetMethod(methodName).MakeGenericMethod(new Type[] { type });
            var ret = mi.Invoke(methodData, new object[] { param });
            return (T)ret;
        }

        public static T CallStaticFunc<T>(object param, Type methodType, string methodName)
        {
            Type type = param.GetType();
            MethodInfo mi = methodType.GetMethod(methodName).MakeGenericMethod(new Type[] { type });
            var ret = mi.Invoke(null, new object[] { param });
            return (T)ret;
        }
        /// <summary>
        /// Create Expression
        /// </summary>
        public static Expression CreateExpression(Expression left, Expression value, ExpressionType type)
        {

            if (type == ExpressionType.Equal)
            {
                return Expression.Equal(left, Expression.Convert(value, left.Type));
            }
            else if (type == ExpressionType.NotEqual)
            {
                return Expression.NotEqual(left, Expression.Convert(value, left.Type));
            }
            else 
            {
                //Not implemented, later used in writing
                return Expression.Equal(left, Expression.Convert(value, left.Type));
            }
        }
        public static Expression CreateExpressionLike<ColumnType>(Type entityType,string propertyName,List<ColumnType>  list) 
        {
            var parameter = Expression.Parameter(entityType, "p");
            MemberExpression memberProperty = Expression.PropertyOrField(parameter, propertyName);
            MethodInfo method = typeof(List<>).MakeGenericType(typeof(ColumnType)).GetMethod("Contains");
            ConstantExpression constantCollection = Expression.Constant(list);

            MethodCallExpression methodCall = Expression.Call(constantCollection, method, memberProperty);

            var expression = Expression.Lambda(methodCall, parameter);
            return expression;
        }
        public static Expression<Func<T, object>> CreateNewFields<T>(EntityInfo entity,List<string> propertyNames)
        {
            Type sourceType = typeof(T);
            Dictionary<string, PropertyInfo> sourceProperties = entity.Columns.Where(it=> propertyNames.Contains(it.PropertyName)).ToDictionary(it=>it.PropertyName,it=>it.PropertyInfo);

            Type dynamicType = LinqRuntimeTypeBuilder.GetDynamicType(sourceProperties.Values);

            ParameterExpression sourceItem = Expression.Parameter(sourceType, "t");
            IEnumerable<MemberBinding> bindings = dynamicType.GetRuntimeProperties().Select(p => Expression.Bind(p, Expression.Property(sourceItem, sourceProperties[p.Name]))).OfType<MemberBinding>();

            return Expression.Lambda<Func<T, object>>(Expression.MemberInit(
                Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), bindings), sourceItem); 
        }
        public static Expression CreateExpressionSelectField(Type classType, string propertyName, Type propertyType)
        {
            ParameterExpression parameter = Expression.Parameter(classType, "it");

            // 创建属性表达式
            PropertyInfo propertyInfo = classType.GetProperty(propertyName);
            MemberExpression property = Expression.Property(parameter, propertyInfo);

            // 创建Lambda表达式
            Type funcType = typeof(Func<,>).MakeGenericType(classType, propertyType);
            LambdaExpression lambda = Expression.Lambda(funcType, property, parameter);
            return lambda;
        }
        public static Expression CreateExpressionSelectFieldObject(Type classType, string propertyName)
        {
            ParameterExpression parameter = Expression.Parameter(classType, "it");

          
            PropertyInfo propertyInfo = classType.GetProperty(propertyName);
            MemberExpression property = Expression.Property(parameter, propertyInfo);
             
            UnaryExpression convert = Expression.Convert(property, typeof(object));
            var funcType = typeof(Func<,>).MakeGenericType(classType, typeof(object));
            LambdaExpression lambda = Expression.Lambda(funcType, convert, parameter);
            return lambda;
        }
    }
    internal static class LinqRuntimeTypeBuilder
    {
        private static readonly AssemblyName AssemblyName = new AssemblyName() { Name = "LinqRuntimeTypes4iTheoChan" };
        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, Type> BuiltTypes = new Dictionary<string, Type>();

        static LinqRuntimeTypeBuilder()
        {
            ModuleBuilder = AssemblyBuilder.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run).DefineDynamicModule(AssemblyName.Name);
        }

        private static string GetTypeKey(Dictionary<string, Type> properties)
        {
            //TODO: optimize the type caching -- if fields are simply reordered, that doesn't mean that they're actually different types, so this needs to be smarter
            string key = string.Empty;
            foreach (var prop in properties)
                key += prop.Key + ";" + prop.Value.Name + ";";

            return key;
        }

        private const MethodAttributes RuntimeGetSetAttrs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

        public static Type BuildDynamicType(Dictionary<string, Type> properties)
        {
            if (null == properties)
                throw new ArgumentNullException(nameof(properties));
            if (0 == properties.Count)
                throw new ArgumentOutOfRangeException(nameof(properties), "fields must have at least 1 field definition");

            try
            {
                // Acquires an exclusive lock on the specified object.
                Monitor.Enter(BuiltTypes);
                string className = GetTypeKey(properties);

                if (BuiltTypes.ContainsKey(className))
                    return BuiltTypes[className];

                TypeBuilder typeBdr = ModuleBuilder.DefineType(className, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);

                foreach (var prop in properties)
                {
                    var propertyBdr = typeBdr.DefineProperty(name: prop.Key, attributes: PropertyAttributes.None, returnType: prop.Value, parameterTypes: null);
                    var fieldBdr = typeBdr.DefineField("itheofield_" + prop.Key, prop.Value, FieldAttributes.Private);

                    MethodBuilder getMethodBdr = typeBdr.DefineMethod("get_" + prop.Key, RuntimeGetSetAttrs, prop.Value, Type.EmptyTypes);
                    ILGenerator getIL = getMethodBdr.GetILGenerator();
                    getIL.Emit(OpCodes.Ldarg_0);
                    getIL.Emit(OpCodes.Ldfld, fieldBdr);
                    getIL.Emit(OpCodes.Ret);

                    MethodBuilder setMethodBdr = typeBdr.DefineMethod("set_" + prop.Key, RuntimeGetSetAttrs, null, new Type[] { prop.Value });
                    ILGenerator setIL = setMethodBdr.GetILGenerator();
                    setIL.Emit(OpCodes.Ldarg_0);
                    setIL.Emit(OpCodes.Ldarg_1);
                    setIL.Emit(OpCodes.Stfld, fieldBdr);
                    setIL.Emit(OpCodes.Ret);

                    propertyBdr.SetGetMethod(getMethodBdr);
                    propertyBdr.SetSetMethod(setMethodBdr);
                }

                BuiltTypes[className] = typeBdr.CreateType();

                return BuiltTypes[className];
            }
            catch
            {
                throw;
            }
            finally
            {
                Monitor.Exit(BuiltTypes);
            }
        }

        private static string GetTypeKey(IEnumerable<PropertyInfo> properties)
        {
            return GetTypeKey(properties.ToDictionary(f => f.Name, f => f.PropertyType));
        }

        public static Type GetDynamicType(IEnumerable<PropertyInfo> properties)
        {
            return BuildDynamicType(properties.ToDictionary(f => f.Name, f => f.PropertyType));
        }
    } 
}