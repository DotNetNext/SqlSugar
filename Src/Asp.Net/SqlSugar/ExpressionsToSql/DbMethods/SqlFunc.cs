using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqlFunc
    {
        public static bool HasNumber(object thisValue)
        {
            return thisValue.ObjToInt() > 0;
        }
        public static bool HasValue(object thisValue)
        {
            return thisValue.HasValue();
        }
        public static bool IsNullOrEmpty(object thisValue)
        {
            return thisValue.IsNullOrEmpty();
        }
        public static string ToLower(object thisValue)
        {
            return thisValue == null ? null : thisValue.ToString().ToLower();
        }
        public static string ToUpper(object thisValue)
        {
            return thisValue == null ? null : thisValue.ToString().ToUpper();
        }
        public static string Trim(object thisValue)
        {
            return thisValue == null ? null : thisValue.ToString().Trim();
        }
        public static bool Contains(string thisValue, string parameterValue)
        {
            return thisValue.Contains(parameterValue);
        }
        public static bool ContainsArray<T>(T[] thisValue, object InField)
        {
            return thisValue.Contains((T)InField);
        }
        public static bool ContainsArray<T>(List<T> thisValue, object InField)
        {
            return thisValue.Contains((T)InField);
        }
        public static bool ContainsArrayUseSqlParameters<T>(List<T> thisValue, object InField)
        {
            return thisValue.Contains((T)InField);
        }
        public static bool ContainsArrayUseSqlParameters<T>(T[] thisValue, object InField)
        {
            return thisValue.Contains((T)InField);
        }
        public static bool StartsWith(string thisValue, string parameterValue)
        {
            return thisValue.StartsWith(parameterValue);
        }
        public static bool EndsWith(string thisValue, string parameterValue)
        {
            return thisValue.EndsWith(parameterValue);
        }
        public new static bool Equals(object thisValue, object parameterValue)
        {
            return thisValue.Equals(parameterValue);
        }
        public  static bool EqualsNull(object thisValue, object parameterValue)
        {
            return thisValue.Equals(parameterValue);
        }
        public static bool DateIsSame(DateTime date1, DateTime date2)
        {
            return date1.ToString("yyyy-MM-dd") == date2.ToString("yyyy-MM-dd");
        }
        public static bool DateIsSame(DateTime? date1, DateTime? date2)
        {
            return ((DateTime)date1).ToString("yyyy-MM-dd") == ((DateTime)date2).ToString("yyyy-MM-dd");
        }
        public static bool DateIsSame(DateTime date1, DateTime date2, DateType dataType) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime DateAdd(DateTime date, int addValue, DateType dataType) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime DateAdd(DateTime date, int addValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int DateValue(DateTime date, DateType dataType) { throw new NotSupportedException("Can only be used in expressions"); }
        public static bool Between(object value, object start, object end) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult IIF<TResult>(bool Expression, TResult thenValue, TResult elseValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult IsNull<TResult>(TResult thisValue, TResult ifNullValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1,string value2) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2,string value3) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2,string value3,string value4) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4,string value5) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4, string value5,string value6) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4, string value5, string value6,string value7) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int ToInt32(object value) { return value.ObjToInt(); }
        public static long ToInt64(object value) { return Convert.ToInt64(value); }
        /// <summary>
        /// yyyy-MM-dd HH:mm:ss.fff
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime ToDate(object value) { return value.ObjToDate(); }
        public static DateTime ToDateShort(object value) { return value.ObjToDate(); }
        /// <summary>
        ///HH:mm:ss 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TimeSpan ToTime(object value) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string ToString(object value) { return value.ObjToString(); }
        public static string ToVarchar(object value) { return value.ObjToString(); }
        public static decimal ToDecimal(object value) { return value.ObjToDecimal(); }
        public static Guid ToGuid(object value) { return Guid.Parse(value.ObjToString()); }
        public static double ToDouble(object value) { return value.ObjToMoney(); }
        public static bool ToBool(object value) { return value.ObjToBool(); }
        public static string Substring(object value, int index, int length) { return value.ObjToString().Substring(index, length); }
        public static string Replace(object value, string oldChar, string newChar) { return value.ObjToString().Replace(oldChar, newChar); }
        public static int Length(object value) { return value.ObjToString().Length; }
        public static TResult AggregateSum<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateAvg<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateMin<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateMax<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int AggregateCount<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int AggregateDistinctCount<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult MappingColumn<TResult>(TResult oldColumnName,string newColumnName) { throw new NotSupportedException("Can only be used in expressions"); }
        /// <summary>
        ///Example: new NewT(){name=SqlFunc.GetSelfAndAutoFill(it)}  Generated SQL   it.*
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult GetSelfAndAutoFill<TResult>(TResult value) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime GetDate() { throw new NotSupportedException("Can only be used in expressions"); }
        public static string GetRandom() { throw new NotSupportedException("Can only be used in expressions"); }
        /// <summary>
        /// Subquery
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Subqueryable<T> Subqueryable<T>() where T:class,new(){ throw new NotSupportedException("Can only be used in expressions");}
        public static CaseThen  IF(bool condition) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int CharIndex(string findChar,string searchValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseAnd(int left, int right) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseInclusiveOR(int left, int right) { throw new NotSupportedException("Can only be used in expressions"); }
    }
}
