using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbBind
    {
        List<string> GuidThrow { get; }
        List<string> IntThrow { get; }
        List<string> StringThrow { get; }
        List<string> DecimalThrow { get; }
        List<string> DoubleThrow { get; }
        List<string> DateThrow { get; }
        List<string> ShortThrow { get; }
        SqlSugarClient Context { get; set; }
        List<T> DataReaderToList<T>(Type type, IDataReader reader, string fields);
        string ChangeDBTypeToCSharpType(string typeName);
    }
}
