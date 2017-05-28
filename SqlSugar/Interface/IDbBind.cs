using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbBind
    {
        SqlSugarClient Context { get; set; }
        List<string> GuidThrow { get; }
        List<string> IntThrow { get; }
        List<string> StringThrow { get; }
        List<string> DecimalThrow { get; }
        List<string> DoubleThrow { get; }
        List<string> DateThrow { get; }
        List<string> ShortThrow { get; }
        string ChangeDBTypeToCSharpType(string typeName);
        List<T> DataReaderToList<T>(Type type, IDataReader reader, string fields);
    }
}
