using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SqlFunc
    {
        public static bool HasNumber(object thisValue)
        {
            return thisValue.ObjToInt() > 0;
        }
        public static bool HasValue(object thisValue)
        {
            return thisValue.IsValuable();
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
        public static bool ContainsArray<T>(T[] thisValue, object parameterValue)
        {
            return thisValue.Contains((T)parameterValue);
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
        public static bool DateIsSame(DateTime date1, DateTime date2)
        {
            return date1.ToString("yyyy-MM-dd") == date2.ToString("yyyy-MM-dd");
        }
        public static bool DateIsSame(DateTime? date1, DateTime? date2)
        {
            return ((DateTime)date1).ToString("yyyy-MM-dd") == ((DateTime)date2).ToString("yyyy-MM-dd");
        }
        public static bool DateIsSame(DateTime date1, DateTime date2, DateType dataType) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static DateTime DateAdd(DateTime date, int addValue, DateType dataType) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static DateTime DateAdd(DateTime date, int addValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static int DateValue(DateTime date, DateType dataType) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static bool Between(object value, object start, object end) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static int ToInt32(object value) { return value.ObjToInt(); }
        public static long ToInt64(object value) { return Convert.ToInt64(value); }
        public static DateTime ToDate(object value) { return value.ObjToDate(); }
        public static string ToString(object value) { return value.ObjToString(); }
        public static decimal ToDecimal(object value) { return value.ObjToDecimal(); }
        public static Guid ToGuid(object value) { return Guid.Parse(value.ObjToString()); }
        public static double ToDouble(object value) { return value.ObjToMoney(); }
        public static bool ToBool(object value) { return value.ObjToBool(); }
        public static string Substring(object value, int index, int length) { return value.ObjToString().Substring(index, length); }
        public static string Replace(object value, string oldChar, string newChar) { return value.ObjToString().Replace(oldChar, newChar); }
        public static int Length(object value) { return value.ObjToString().Length; }
        public static TResult AggregateSum<TResult>(TResult thisValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static TResult AggregateAvg<TResult>(TResult thisValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static TResult AggregateMin<TResult>(TResult thisValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static TResult AggregateMax<TResult>(TResult thisValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
        public static TResult AggregateCount<TResult>(TResult thisValue) { throw new NotSupportedException("This method is not supported by the current parameter"); }
    }
}
