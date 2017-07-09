using Microsoft.Data.Sqlite;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace System.Data.SQLite
{
    public static class AdoCore
    {
        public static List<DbColumnInfo> GetColumnInfosByTableName(string tableName, DbDataReader dataReader)
        {
            List<DbColumnInfo> result = new List<DbColumnInfo>();
            var columns = dataReader.GetColumnSchema();
            foreach (var row in columns)
            {
                DbColumnInfo column = new DbColumnInfo()
                {
                    TableName = tableName,
                    DataType = row.DataTypeName,
                    IsNullable = row.AllowDBNull.ObjToBool(),
                    IsIdentity = row.IsAutoIncrement.ObjToBool(),
                    ColumnDescription = null,
                    DbColumnName = row.ColumnName,
                    DefaultValue = null,
                    IsPrimarykey = row.IsKey.ObjToBool(),
                    Length = row.ColumnSize.ObjToInt()
                };
                result.Add(column);
            }
            return result;
        }
    }
    public class SQLiteConnection : SqliteConnection
    {
        public SQLiteConnection(string connectionString) : base(connectionString)
        {

        }
    }

    public class SQLiteCommand : SqliteCommand
    {
        public SQLiteCommand(string sql, SQLiteConnection connection) : base(sql, connection)
        {

        }
    }

    public class SQLiteParameter : SqliteParameter
    {
        public SQLiteParameter() { }
        public SQLiteParameter(string name, string parameter) :base(name,parameter){

        }
    }
}
