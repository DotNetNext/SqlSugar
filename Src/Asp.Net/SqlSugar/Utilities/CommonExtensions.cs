using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class CommonExtensions
    {
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