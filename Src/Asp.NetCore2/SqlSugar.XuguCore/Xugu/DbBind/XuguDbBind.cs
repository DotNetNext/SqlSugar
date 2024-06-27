using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace SqlSugar.Xugu
{
    public class XuguDbBind : DbBindProvider
    {
        public override string GetDbTypeName(string csharpTypeName) => csharpTypeName == nameof(DateTimeOffset) ? nameof(DateTimeOffset) : base.GetDbTypeName(csharpTypeName);
        public override List<KeyValuePair<string, CSharpDataType>> MappingTypes
        {
            get
            {
                var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
                if (extService != null && extService.AppendDataReaderTypeMappings.HasValue())
                    return extService.AppendDataReaderTypeMappings.Union(MappingTypesConst).ToList();
                else
                    return MappingTypesConst;
            }
        }
        public static List<KeyValuePair<string, CSharpDataType>> MappingTypesConst = new List<KeyValuePair<string, CSharpDataType>>()
                {
                  new KeyValuePair<string, CSharpDataType>("BIGINT",CSharpDataType.@long),
                  new KeyValuePair<string, CSharpDataType>("BINARY",CSharpDataType.@byteArray),
                  new KeyValuePair<string, CSharpDataType>("BLOB",CSharpDataType.@object),
                  new KeyValuePair<string, CSharpDataType>("BOOLEAN",CSharpDataType.@bool),
                  new KeyValuePair<string, CSharpDataType>("CHAR",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("CLOB",CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("DATE", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("DATETIME", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("DATETIME WITH TIME ZONE", CSharpDataType.DateTimeOffset),
                  new KeyValuePair<string, CSharpDataType>("DECIMAL", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("DOUBLE", CSharpDataType.@double),
                  new KeyValuePair<string, CSharpDataType>("FLOAT", CSharpDataType.@float),
                  new KeyValuePair<string, CSharpDataType>("GUID", CSharpDataType.Guid),
                  new KeyValuePair<string, CSharpDataType>("INT", CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("INTEGER", CSharpDataType.@int),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL YEAR", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL YEAR TO MONTH", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL MONTH", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL DAY", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL DAY TO HOUR", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL DAY TO MINUTE", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL DAY TO SECOND", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL HOUR", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL HOUR TO MINUTE", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL HOUR TO SECOND", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL MINUTE", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL MINUTE TO SECOND", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("INTERVAL SECOND", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("NCHAR", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("NUMERIC", CSharpDataType.@decimal),
                  new KeyValuePair<string, CSharpDataType>("NVARCHAR", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("ROWID", CSharpDataType.@string),
                  new KeyValuePair<string, CSharpDataType>("ROWVERSION", CSharpDataType.@object),//需要手动修改为System.Version
                  new KeyValuePair<string, CSharpDataType>("TIME", CSharpDataType.TimeSpan),
                  new KeyValuePair<string, CSharpDataType>("TIME WITH TIME ZONE", CSharpDataType.TimeSpan),
                  new KeyValuePair<string, CSharpDataType>("TIMESTAMP", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("TIMESTAMP AUTO UPDATE", CSharpDataType.DateTime),
                  new KeyValuePair<string, CSharpDataType>("TINYINT", CSharpDataType.@sbyte),
                  new KeyValuePair<string, CSharpDataType>("VARCHAR", CSharpDataType.@string),
        };
    };

    public class DateTimeOffsetConvert : ISugarDataConverter
    {
        public SugarParameter ParameterConverter<T>(object columnValue, int columnIndex)
        {
            string name = ":XuguParam" + columnIndex;
            Type underType = UtilMethods.GetUnderType(typeof(T));
            return new SugarParameter(name, columnValue, underType);
        }
        public T QueryConverter<T>(IDataRecord dr, int i)
        {
            var value = dr.GetString(i);
            if (DateTimeOffset.TryParse(value, out DateTimeOffset o)) return (T)UtilMethods.ChangeType2(o, typeof(T));
            else return default(T);
        }
    }
}
/*生成模型类后需要手动修改的地方。所以尽量少使用rowid\rowversion\blob\time\time with time zone\datetime with time zone  这几种类型
 * 
======================================================================================
//增加特性配置
[SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert))]
public TimeSpan? C_TIME { get; set; }

[SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert))]
public TimeSpan C_TIME_WITH_TIME_ZONE { get; set; }

[SugarColumn(SqlParameterDbType = typeof(DateTimeOffsetConvert))]
public DateTimeOffset C_DATETIME_WITH_TIME_ZONE { get; set; }
======================================================================================可以使用类型BINARY
//增加特性配置，写值可以写byte[]或者XGBlob
[SugarColumn(SqlParameterDbType = typeof(CommonPropertyConvert))]
public object C_BLOB { get; set; }
======================================================================================尽量不使用
//上传内容为ANSI编码读取
public string C_CLOB { get; set; }
//手动修改为类型Version
public Version C_ROWVERSION { get; set; }
*/