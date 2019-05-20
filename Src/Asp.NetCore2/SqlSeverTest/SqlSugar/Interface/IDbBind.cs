using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial interface IDbBind
    {
        SqlSugarProvider Context { get; set; }
        List<string> GuidThrow { get; }
        List<string> IntThrow { get; }
        List<string> StringThrow { get; }
        List<string> DecimalThrow { get; }
        List<string> DoubleThrow { get; }
        List<string> DateThrow { get; }
        List<string> ShortThrow { get; }
        string GetPropertyTypeName(string dbTypeName);
        string GetConvertString(string dbTypeName);
        string GetDbTypeName(string csharpTypeName);
        string GetCsharpTypeName(string dbTypeName);
        List<KeyValuePair<string, CSharpDataType>> MappingTypes { get; }
        List<T> DataReaderToList<T>(Type type, IDataReader reader);
        Task<List<T>> DataReaderToListAsync<T>(Type entityType, IDataReader dataReader);
        List<T> DataReaderToListNoUsing<T>(Type type, IDataReader reader);
        Task<List<T>> DataReaderToListNoUsingAsync<T>(Type type, IDataReader reader);

    }
}
