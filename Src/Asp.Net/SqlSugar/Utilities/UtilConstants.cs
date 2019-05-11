using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    internal static class UtilConstants
    {
        public const string Dot = ".";
        public const char DotChar = '.';
        internal const string Space = " ";
        internal const char SpaceChar =' ';
        internal const string AssemblyName = "SqlSugar";
        internal const string ReplaceKey = "{662E689B-17A1-4D06-9D27-F29EAB8BC3D6}";
        internal const string ReplaceCommaKey = "{112A689B-17A1-4A06-9D27-A39EAB8BC3D5}";

        internal static Type IntType = typeof(int);
        internal static Type LongType = typeof(long);
        internal static Type GuidType = typeof(Guid);
        internal static Type BoolType = typeof(bool);
        internal static Type BoolTypeNull = typeof(bool?);
        internal static Type ByteType = typeof(Byte);
        internal static Type ObjType = typeof(object);
        internal static Type DobType = typeof(double);
        internal static Type FloatType = typeof(float);
        internal static Type ShortType = typeof(short);
        internal static Type DecType = typeof(decimal);
        internal static Type StringType = typeof(string);
        internal static Type DateType = typeof(DateTime);
        internal static Type DateTimeOffsetType = typeof(DateTimeOffset);
        internal static Type TimeSpanType = typeof(TimeSpan);
        internal static Type ByteArrayType = typeof(byte[]);
        internal static Type ModelType= typeof(ModelContext);
        internal static Type DynamicType = typeof(ExpandoObject);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicArraySS = typeof(Dictionary<string, string>);
        internal static Type DicArraySO = typeof(Dictionary<string, object>);

        public static Type SugarType = typeof(SqlSugarProvider);
    }
}
