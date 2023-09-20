using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class CommonExtensions
    {
        public static Dictionary<string, object> ToDictionary<T>(this List<T> list, string keyPropertyName, string valuePropertyName)
        {
            var keyProperty = typeof(T).GetProperty(keyPropertyName);
            var valueProperty = typeof(T).GetProperty(valuePropertyName);

            return list.ToDictionary(
                item => keyProperty.GetValue(item)+"",
                item => valueProperty.GetValue(item)
            );
        }
        public static MethodInfo GetMyMethod(this Type type,string name, int argCount) 
        {
            return type.GetMethods().FirstOrDefault(it => it.Name == name && it.GetParameters().Length == argCount);
        }
        public static MethodInfo GetMyMethod(this Type type, string name, int argCount,Type parameterType)
        {
            return type.GetMethods().FirstOrDefault(it => 
            it.Name == name && 
            it.GetParameters().Length == argCount&&
            it.GetParameters().First().ParameterType==parameterType);
        }
        public static MethodInfo GetMyMethod(this Type type, string name, int argCount, bool isList)
        {
            var methods= type.GetMethods().Where(it => it.Name == name).Where(it =>
            it.GetParameters().Length == argCount&&
            it.GetParameters()[0].ToString().Contains("List`") == isList).ToList(); 
            return methods.First();
        }

        public static MethodInfo GetMyMethod(this Type type, string name, int argCount, Type parameterType,Type parameterType2)
        {
            return type.GetMethods().Where(it=>it.Name == name).FirstOrDefault(it =>
            it.GetParameters().Length == argCount &&
            it.GetParameters().First().ParameterType == parameterType&&
            it.GetParameters()[1].ParameterType == parameterType2) ;
        }

        public static MethodInfo GetMyMethodNoGen(this Type type, string name, int argCount, Type parameterType, Type parameterType2)
        {
            return type.GetMethods().Where(it => it.Name == name&&it?.GetGenericArguments()?.Count()==0).FirstOrDefault(it =>
            it.GetParameters().Length == argCount &&
            it.GetParameters().First().ParameterType == parameterType &&
            it.GetParameters()[1].ParameterType == parameterType2);
        }
        public static MethodInfo GetMyMethod(this Type type, string name, int argCount, Type parameterType, Type parameterType2, Type parameterType3)
        {
            return type.GetMethods().Where(it => it.Name == name).FirstOrDefault(it =>
            it.GetParameters().Length == argCount &&
            it.GetParameters().First().ParameterType == parameterType &&
            it.GetParameters()[1].ParameterType == parameterType2&&
            it.GetParameters()[2].ParameterType == parameterType3);
        }
        public static MethodInfo GetMyMethod(this Type type, string name, int argCount, Type parameterType, Type parameterType2, Type parameterType3,Type parameterType4)
        {
            return type.GetMethods().Where(it => it.Name == name).FirstOrDefault(it =>
            it.GetParameters().Length == argCount &&
            it.GetParameters().First().ParameterType == parameterType &&
            it.GetParameters()[1].ParameterType == parameterType2 &&
            it.GetParameters()[2].ParameterType == parameterType3&&
            it.GetParameters()[3].ParameterType == parameterType4);
        }
        public static MethodInfo GetMyMethod(this Type type, string name, int argCount, Type parameterType, Type parameterType2, Type parameterType3, Type parameterType4, Type parameterType5)
        {
            return type.GetMethods().Where(it => it.Name == name).FirstOrDefault(it =>
            it.GetParameters().Length == argCount &&
            it.GetParameters().First().ParameterType == parameterType &&
            it.GetParameters()[1].ParameterType == parameterType2 &&
            it.GetParameters()[2].ParameterType == parameterType3 &&
            it.GetParameters()[3].ParameterType == parameterType4&&
            it.GetParameters()[4].ParameterType == parameterType5);
        }
        public static List<T> ToList<T>(this  T thisValue,Func<T,T> action) where T:class,new()
        {
            return new List<T> { thisValue };
        }
        public static List<T> ToList<T>(this IEnumerable<T> thisValue, Func<T, T> action) where T : class, new()
        {
            return  thisValue.ToList();
        }
        public static IEnumerable<T> WhereIF<T>(this IEnumerable<T> thisValue, bool isOk, Func<T, bool> predicate) 
        {
            if (isOk)
            {
                return thisValue.Where(predicate);
            }
            else 
            {
                return thisValue;
            }
        }
        public static IEnumerable<T> MappingField<T>(this IEnumerable<T> thisValue,Func<T, object> leftField, Func<object> rightField)
        {
            return thisValue;
        }
        public static List<T> MappingField<T>(this T thisValue, Func<T, object> leftField, Func<object> rightField) where T:class
        {
            return new List<T>() { thisValue };
        }
        public static bool Any<T>(this IEnumerable<T> thisValue,  List<IConditionalModel> conditionalModels) 
        {
            throw new Exception("Can only be used in expressions");
        }
        public static IEnumerable<T> Where<T>(this IEnumerable<T> thisValue, List<IConditionalModel> conditionalModels)
        {
            throw new Exception("Can only be used in expressions");
        }
    }
}
namespace System.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static bool Contains<T>(this IEnumerable<T> thisValue, T likeKey, bool isNvarchar)
        {
            return thisValue.Contains(likeKey);
        }
    }
}