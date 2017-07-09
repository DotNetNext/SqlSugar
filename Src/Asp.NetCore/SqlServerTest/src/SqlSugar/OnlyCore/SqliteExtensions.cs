using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.SQLite
{
    public static class AdoCore
    {
        public static List<DbColumnInfo> GetColumnInfosByTableName(string tableName, DbDataReader dataReader)
        {
            List<DbColumnInfo> result = new List<DbColumnInfo>();
            while (dataReader.Read())
            {
                var type = dataReader.GetValue(2).ObjToString();
                var length = 0;
                if (type.Contains("("))
                {
                    type = type.Split('(').First();
                    length = type.Split('(').Last().TrimEnd(')').ObjToInt();
                }
                DbColumnInfo column = new DbColumnInfo()
                {
                    TableName = tableName,
                    DataType = type,
                    IsNullable = !dataReader.GetBoolean(3),
                    IsIdentity = dataReader.GetBoolean(3)&&dataReader.GetBoolean(5).ObjToBool() && (type.IsIn("integer", "int", "int32", "int64", "long")),
                    ColumnDescription = null,
                    DbColumnName = dataReader.GetString(1),
                    DefaultValue =dataReader.GetValue(4).ObjToString(),
                    IsPrimarykey = dataReader.GetBoolean(5).ObjToBool(),
                    Length = length
                };
                result.Add(column);
            }
            return result;
        }
    }
}
