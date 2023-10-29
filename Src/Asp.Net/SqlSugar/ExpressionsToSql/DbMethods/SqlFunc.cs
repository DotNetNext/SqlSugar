using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqlFunc
    {
        public static bool FullTextContains(string [] columnNames, string keyword)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool FullTextContains(string columnName,string keyword)
        {
            throw new NotSupportedException("Can only be used in expressions");
        } 
        public static int Floor(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int Ceil(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int WeekOfYear(DateTime fieldName) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string Left(string value,int number) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string Right(string value,int number)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string PadLeft(string value,int number, char padChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool Like(string fieldName, string likeValue) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int Modulo(decimal numA, decimal numB)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int CompareTo(decimal numA, decimal numB)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int CompareTo(int numA, int numB)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int CompareTo(string strA, string strB) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int RowNumber(object orderByField, object partitionBy) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int RowNumber(object orderByField)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int RowCount(object countFiledName,object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int RowCount()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowSum<TRestult>(TRestult filedName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowSum<TRestult>(TRestult filedName, object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowMax<TRestult>(TRestult filedName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowMax<TRestult>(TRestult filedNameobject,object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowMin<TRestult>(TRestult filedName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowMin<TRestult>(TRestult filedName, object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowAvg<TRestult>(TRestult filedName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static TRestult RowAvg<TRestult>(TRestult filedName, object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string JsonField(object json,string fieldName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string JsonIndex(object json, int jsonIndex)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string JsonField(object json, string fieldName,string includeFieldName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static string JsonField(object json, string fieldName, string includeFieldName, string ThenIncludeFieldName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string JsonField(object json, string fieldName, string includeFieldName, string ThenIncludeFieldName, string ThenIncludeFieldName2)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string JsonField(object json, string fieldName, string includeFieldName, string ThenIncludeFieldName, string ThenIncludeFieldName2, string ThenIncludeFieldName3)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool JsonContainsFieldName(object json, string fieldName)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static int JsonArrayLength(object json)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static string JsonParse(object json)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool JsonLike(object json,string likeStr)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static T Desc<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static T Asc<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int DateDiff(DateType dateType,DateTime littleTime, DateTime bigTime) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool GreaterThan(object thisValue,object gtValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static string Stuff(string sourceString, int start, int length, string AddString)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool GreaterThanOrEqual(object thisValue, object gtValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool LessThan(object thisValue, object ltValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool LessThanOrEqual(object thisValue, object ltValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
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
        public static string TrimEnd(object thisValue,string trimChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static string TrimStart(object thisValue, string trimChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
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

        public static bool Exists(string subQueryableName_Or_OneToOnePropertyName)  
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool Exists<valueType>(valueType subQueryableName_Or_OneToOnePropertyName) where valueType : struct
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool Exists(string subQueryableName_Or_OneToOnePropertyName, List<IConditionalModel> conditionalModels)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool Exists<valueType>(valueType subQueryableName_Or_OneToOnePropertyName, List<IConditionalModel> conditionalModels) where valueType : struct
        {
            throw new NotSupportedException("Can only be used in expressions");
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
        public static TResult IIF<TResult>(bool isTrue, TResult thenValue, TResult elseValue) { return isTrue ? thenValue : elseValue; }
        public static TResult IsNull<TResult>(TResult thisValue, TResult ifNullValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1,string value2) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2,string value3) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2,string value3,string value4) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4,string value5) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4, string value5,string value6) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string MergeString(string value1, string value2, string value3, string value4, string value5, string value6,string value7) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int ToInt32(object value) { return value.ObjToInt(); }
        public static float ToSingle(object value) { return Convert.ToSingle(value); }
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
        public static Guid NewUid() { throw new NotSupportedException("Can only be used in expressions"); }
        public static double ToDouble(object value) { return value.ObjToMoney(); }
        public static bool ToBool(object value) { return value.ObjToBool(); }
        public static string Substring(object value, int index, int length) { return value.ObjToString().Substring(index, length); }
        public static string Replace(object value, string oldChar, string newChar) { return value.ObjToString().Replace(oldChar, newChar); }
        public static int Length(object value) { return value.ObjToString().Length; }
        public static TResult AggregateSum<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateSumNoNull<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string Collate(string thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateAvg<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateAvgNoNull<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateMin<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult AggregateMax<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int AggregateCount<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int AggregateDistinctCount<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult MappingColumn<TResult>(TResult type,string newColumnName) { throw new NotSupportedException("Can only be used in expressions"); }
        public static TResult MappingColumn<TResult>(string newColumnName) { throw new NotSupportedException("Can only be used in expressions"); }
        /// <summary>
        ///Example: new NewT(){name=SqlFunc.GetSelfAndAutoFill(it)}  Generated SQL   it.*
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult GetSelfAndAutoFill<TResult>(TResult value) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime GetDate() { throw new NotSupportedException("Can only be used in expressions"); }
        public static string GetRandom() { throw new NotSupportedException("Can only be used in expressions"); }


        public static T Abs<T>( T value) { throw new NotSupportedException("Can only be used in expressions"); }
        public static T Round<T>(T value,int precision) { throw new NotSupportedException("Can only be used in expressions"); }

        /// <summary>
        /// Subquery
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Subqueryable<T> Subqueryable<T>() where T:class,new(){ throw new NotSupportedException("Can only be used in expressions");}
        public static CaseThen  IF(bool condition) { throw new NotSupportedException("Can only be used in expressions"); }
        [Obsolete("多库下参数顺序不一至，为了保证多库下更好体验请使用 SqlFunc.CharIndexNew")]
        public static int CharIndex(string findChar,string searchValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int CharIndexNew(string stringValue, string charValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseAnd(int left, int right) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseInclusiveOR(int left, int right) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseAnd(long left, long right) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int BitwiseInclusiveOR(long left, long right) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime Oracle_ToDate(string date,string format) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string Oracle_ToChar(DateTime date, string format) { throw new NotSupportedException("Can only be used in expressions"); }
        public static string Oracle_ToChar(object objValue, string format) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int SqlServer_DateDiff(string dateType,DateTime date1,DateTime date2) { throw new NotSupportedException("Can only be used in expressions"); }

        public static bool JsonListObjectAny(object jsonListObject, string fieldName, object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool JsonArrayAny(object jsonArray,object arrayValue) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool SplitIn(string CommaSegmentationString, string inValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool SplitIn(string CommaSegmentationString, string inValue,char splitChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool ListAny<T>(List<T> listConstant, Expression<Func<T,bool>> expression)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool ListAll<T>(List<T> listConstant, Expression<Func<T, bool>> expression)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
         
        public static string OnlyInSelectConvertToString(string stringValue, MethodInfo methodInfo)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
    }
}
