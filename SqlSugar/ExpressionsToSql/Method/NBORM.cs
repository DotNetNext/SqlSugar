using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class NBORM
    {
        public static bool IsNullOrEmpty(object thisValue) { throw new NotImplementedException(); }
        public static string ToLower(object thisValue) { throw new NotImplementedException(); }
        public static string ToUpper(object thisValue) { throw new NotImplementedException(); }
        public static string Trim(object thisValue) { throw new NotImplementedException(); }
        public static bool Contains(string thisValue, string parameterValue) {  throw new NotImplementedException();}
        public static bool StartsWith(string thisValue, string parameterValue) { throw new NotImplementedException(); }
        public static bool EndsWith(string thisValue, string parameterValue) { throw new NotImplementedException(); }
        public new static bool Equals(object thisValue, object parameterValue) { throw new NotImplementedException(); }
        public static bool DateIsSame(DateTime date1, DateTime date2) {  throw new NotImplementedException();}
        public static bool DateIsSame(DateTime date1, DateTime date2, DateType dataType)  { throw new NotImplementedException(); }
        public static bool DateAdd(DateTime date, int addValue, DateType millisecond){ throw new NotImplementedException(); }
        public static bool DateAdd(DateTime date, int addValue) { throw new NotImplementedException(); }
        public static bool DateValue(DateTime date, DateType dataType) { throw new NotImplementedException(); }
        public static bool Between(object value, object start, object end){ throw new NotImplementedException();}
    }
}
