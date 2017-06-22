using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    internal static class PubConst
    {
        internal const string AssemblyName = "SqlSugar";
        internal const string Space = " ";
        internal static Type StringType = typeof(string);
        internal static Type ShortType = typeof(short);
        internal static Type IntType = typeof(int);
        internal static Type LongType = typeof(long);
        internal static Type DecType = typeof(decimal);
        internal static Type GuidType = typeof(Guid);
        internal static Type DateType = typeof(DateTime);
        internal static Type ByteType = typeof(Byte);
        internal static Type ByteArrayType = typeof(byte[]);
        internal static Type BoolType = typeof(bool);
        internal static Type ObjType = typeof(object);
        internal static Type DobType = typeof(double);
        internal static Type FloatType=typeof(float);
        internal static Type ModelType= typeof(ModelContext);
        internal static Type DicSS = typeof(KeyValuePair<string, string>);
        internal static Type DicSi = typeof(KeyValuePair<string, int>);
        internal static Type Dicii = typeof(KeyValuePair<int, int>);
        internal static Type DicOO = typeof(KeyValuePair<object, object>);
        internal static Type DicSo = typeof(KeyValuePair<string, object>);
        internal static Type DicIS = typeof(KeyValuePair<int, string>);
        internal static Type DicArraySS = typeof(Dictionary<string, string>);
        internal static Type DicArraySO = typeof(Dictionary<string, object>);
    }
}
