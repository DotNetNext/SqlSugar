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
        public static long UNIX_TIMESTAMP(DateTime dateTime) 
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static T Coalesce<T>(T value1, T value2)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
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

        public static int Rank(object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int Rank(object orderByField)
        {
            throw new NotSupportedException("Can only be used in expressions");
        } 
        public static int DenseRank(object orderByField, object partitionBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static int DenseRank(object orderByField)
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
        public static bool GreaterThan_LinqDynamicCore(object thisValue, object ltValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static bool LessThan(object thisValue, object ltValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        public static bool LessThan_LinqDynamicCore(object thisValue, object ltValue)
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
        public static bool ContainsArray<T>(T[] thisValue, object InField, bool isNvarchar)
        {
            return thisValue.Contains((T)InField, true);
        }
        public static bool ContainsArray<T>(List<T> thisValue, object InField, bool isNvarchar)
        {
            return thisValue.Contains((T)InField, true);
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
        public static DateTimeOffset DateAdd(DateTimeOffset date, int addValue, DateType dataType) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTime DateAdd(DateTime date, int addValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static DateTimeOffset DateAdd(DateTimeOffset date, int addValue) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int DateValue(DateTime date, DateType dataType) { throw new NotSupportedException("Can only be used in expressions"); }
        public static int DateValue(DateTimeOffset date, DateTimeOffset dataType) { throw new NotSupportedException("Can only be used in expressions"); }
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
        public static int AggregateDistinctSum<TResult>(TResult thisValue) { throw new NotSupportedException("Can only be used in expressions"); }
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
        public static bool PgsqlArrayContains(object jsonArray, object arrayValue)
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
        public static void SelectFields(string fieldName1)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static void SelectFields(string fieldName1,string fieldName2)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static void SelectFields(string fieldName1, string fieldName2, string fieldName3)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static void SelectFields(string fieldName1, string fieldName2, string fieldName3, string fieldName4)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }
        public static void SelectFields(string fieldName1, string fieldName2, string fieldName3, string fieldName4, string fieldName5)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #region String Functions - Extended

        /// <summary>
        /// Reverses a string value
        /// </summary>
        public static string Reverse(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Repeats a string value a specified number of times
        /// </summary>
        public static string Replicate(string value, int count)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the starting position of a pattern in a string
        /// </summary>
        public static int PatIndex(string pattern, string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a four-character (SOUNDEX) code to evaluate the similarity of two strings
        /// </summary>
        public static string Soundex(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the difference between the SOUNDEX values of two strings
        /// </summary>
        public static int Difference(string value1, string value2)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Formats a value with the specified format
        /// </summary>
        public static string Format(object value, string format)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a Unicode string with delimiters added to make the input string a valid SQL Server delimited identifier
        /// </summary>
        public static string QuoteName(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a Unicode string with delimiters added using specified quote character
        /// </summary>
        public static string QuoteName(string value, string quoteChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Concatenates strings with a separator
        /// </summary>
        public static string ConcatWs(string separator, params object[] values)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Pads a string on the right with a specified character to a certain length
        /// </summary>
        public static string PadRight(string value, int length, char padChar)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the ASCII code value of the leftmost character
        /// </summary>
        public static int Ascii(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the character for the specified ASCII code
        /// </summary>
        public static string Char(int asciiCode)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the Unicode value for the first character of the input expression
        /// </summary>
        public static int Unicode(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the Unicode character for the specified integer code
        /// </summary>
        public static string NChar(int unicodeValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns part of a character expression (alternative to Substring)
        /// </summary>
        public static string SubstringIndex(string value, string delimiter, int count)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Math Functions - Extended

        /// <summary>
        /// Returns the value of a number raised to the power of another number
        /// </summary>
        public static double Power(double value, double power)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the square root of a number
        /// </summary>
        public static double Sqrt(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns e raised to the power of a specified number
        /// </summary>
        public static double Exp(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the natural logarithm of a number
        /// </summary>
        public static double Log(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the natural logarithm of a number with specified base
        /// </summary>
        public static double Log(double value, double logBase)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the base-10 logarithm of a number
        /// </summary>
        public static double Log10(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Converts radians to degrees
        /// </summary>
        public static double Degrees(double radians)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Converts degrees to radians
        /// </summary>
        public static double Radians(double degrees)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the sign of a number (-1, 0, or 1)
        /// </summary>
        public static int Sign(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the constant value of PI
        /// </summary>
        public static double PI()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the sine of an angle specified in radians
        /// </summary>
        public static double Sin(double radians)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the cosine of an angle specified in radians
        /// </summary>
        public static double Cos(double radians)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the tangent of an angle specified in radians
        /// </summary>
        public static double Tan(double radians)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the arc sine (inverse sine) of a number
        /// </summary>
        public static double Asin(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the arc cosine (inverse cosine) of a number
        /// </summary>
        public static double Acos(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the arc tangent (inverse tangent) of a number
        /// </summary>
        public static double Atan(double value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the arc tangent of two numbers
        /// </summary>
        public static double Atan2(double y, double x)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Truncates a number to the specified number of decimal places
        /// </summary>
        public static decimal Truncate(decimal value, int decimals)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Date Functions - Extended

        /// <summary>
        /// Returns the last day of the month for a specified date
        /// </summary>
        public static DateTime EndOfMonth(DateTime date)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a date from the specified year, month, and day
        /// </summary>
        public static DateTime DateFromParts(int year, int month, int day)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a datetime from the specified date and time parts
        /// </summary>
        public static DateTime DateTimeFromParts(int year, int month, int day, int hour, int minute, int second)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a time from the specified hour, minute, and second
        /// </summary>
        public static TimeSpan TimeFromParts(int hour, int minute, int second)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the name of the specified date part (e.g., "January" for month 1)
        /// </summary>
        public static string DateName(DateType dateType, DateTime date)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the integer value of the specified date part
        /// </summary>
        public static int DatePart(DateType dateType, DateTime date)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the quarter of the year for a specified date (1-4)
        /// </summary>
        public static int Quarter(DateTime date)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the day of the year for a specified date (1-366)
        /// </summary>
        public static int DayOfYear(DateTime date)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns a datetime value adjusted to a specified time zone offset
        /// </summary>
        public static DateTimeOffset SwitchOffset(DateTimeOffset dateTimeOffset, string timeZone)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current UTC date and time
        /// </summary>
        public static DateTime GetUtcDate()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current system date and time with higher precision
        /// </summary>
        public static DateTime SysDateTime()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current UTC date and time with higher precision
        /// </summary>
        public static DateTime SysUtcDateTime()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current system date and time offset
        /// </summary>
        public static DateTimeOffset SysDateTimeOffset()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Conversion Functions - Extended

        /// <summary>
        /// Tries to cast a value to the specified type, returns NULL if conversion fails
        /// </summary>
        public static T TryCast<T>(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Tries to convert a value to the specified type, returns NULL if conversion fails
        /// </summary>
        public static T TryConvert<T>(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Tries to parse a string to the specified type, returns NULL if parsing fails
        /// </summary>
        public static T TryParse<T>(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Casts a value to the specified SQL type
        /// </summary>
        public static T Cast<T>(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Converts a value to the specified SQL type (SQL CONVERT function)
        /// </summary>
        public static T SqlConvert<T>(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Aggregate Functions - Extended

        /// <summary>
        /// Concatenates string values with a separator (SQL Server 2017+)
        /// </summary>
        public static string StringAgg(string value, string separator)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Concatenates string values with a separator and ordering (SQL Server 2017+)
        /// </summary>
        public static string StringAgg(string value, string separator, object orderBy)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Concatenates string values (MySQL GROUP_CONCAT)
        /// </summary>
        public static string GroupConcat(string value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Concatenates string values with separator (MySQL GROUP_CONCAT)
        /// </summary>
        public static string GroupConcat(string value, string separator)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the checksum aggregate of values in a group
        /// </summary>
        public static int ChecksumAgg(object value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the statistical standard deviation of all values
        /// </summary>
        public static double StDev(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the statistical standard deviation for the population
        /// </summary>
        public static double StDevP(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the statistical variance of all values
        /// </summary>
        public static double Var(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the statistical variance for the population
        /// </summary>
        public static double VarP(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the median value in a group of values (Oracle, PostgreSQL)
        /// </summary>
        public static decimal Median(decimal value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the specified percentile value (SQL Server, PostgreSQL)
        /// </summary>
        public static decimal PercentileCont(decimal value, double percentile)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the discrete percentile value (SQL Server, PostgreSQL)
        /// </summary>
        public static decimal PercentileDisc(decimal value, double percentile)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Window Functions - Extended

        /// <summary>
        /// Returns the value of the next row in the result set
        /// </summary>
        public static T Lead<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value of the next row with offset in the result set
        /// </summary>
        public static T Lead<T>(T value, int offset)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value of the next row with offset and default value
        /// </summary>
        public static T Lead<T>(T value, int offset, T defaultValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value of the previous row in the result set
        /// </summary>
        public static T Lag<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value of the previous row with offset in the result set
        /// </summary>
        public static T Lag<T>(T value, int offset)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value of the previous row with offset and default value
        /// </summary>
        public static T Lag<T>(T value, int offset, T defaultValue)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the first value in an ordered set of values
        /// </summary>
        public static T FirstValue<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the last value in an ordered set of values
        /// </summary>
        public static T LastValue<T>(T value)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the value at the specified position in an ordered set
        /// </summary>
        public static T NthValue<T>(T value, int position)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Divides the result set into a specified number of groups
        /// </summary>
        public static int Ntile(int numberOfGroups)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the relative rank of a row within a partition
        /// </summary>
        public static double PercentRank()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the cumulative distribution of a value in a group of values
        /// </summary>
        public static double CumeDist()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Conditional Functions - Extended

        /// <summary>
        /// Returns the first non-null value in the list (extended version)
        /// </summary>
        public static T Coalesce<T>(T value1, T value2, T value3)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the first non-null value in the list (extended version)
        /// </summary>
        public static T Coalesce<T>(T value1, T value2, T value3, T value4)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the first non-null value in the list (extended version)
        /// </summary>
        public static T Coalesce<T>(T value1, T value2, T value3, T value4, T value5)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns NULL if two expressions are equal, otherwise returns the first expression
        /// </summary>
        public static T NullIf<T>(T value1, T value2)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the greatest value from a list of values
        /// </summary>
        public static T Greatest<T>(params T[] values)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the least value from a list of values
        /// </summary>
        public static T Least<T>(params T[] values)
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion

        #region Utility Functions - Extended

        /// <summary>
        /// Returns the current database name
        /// </summary>
        public static string DatabaseName()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current user name
        /// </summary>
        public static string CurrentUser()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current session user name
        /// </summary>
        public static string SessionUser()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the current system user name
        /// </summary>
        public static string SystemUser()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the host name of the client
        /// </summary>
        public static string HostName()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the application name
        /// </summary>
        public static string AppName()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns information about the current connection
        /// </summary>
        public static int ConnectionId()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        /// <summary>
        /// Returns the last identity value inserted
        /// </summary>
        public static long LastInsertId()
        {
            throw new NotSupportedException("Can only be used in expressions");
        }

        #endregion
    }
}
