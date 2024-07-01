using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar 
{
    internal class DorisHelper
    {

        public static void UpdateDateParameter(MySqlConnector.MySqlParameter sqlParameter)
        {
            if (sqlParameter.DbType == System.Data.DbType.DateTime)
            {
                sqlParameter.DbType = System.Data.DbType.String;
                sqlParameter.Value = sqlParameter.Value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            if (sqlParameter.DbType == System.Data.DbType.DateTimeOffset)
            {
                sqlParameter.DbType = System.Data.DbType.String;
                sqlParameter.Value = sqlParameter.Value.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }
        public static List<DbColumnInfo> GetColumns(List<DbColumnInfo> colums)
        {
            foreach (var item in colums)
            {
                if (item.DataType?.Contains("(") == true)
                {
                    item.DataType = item.DataType.Substring(0, item.DataType.IndexOf("("));
                }
                if (item.DataType == "tinyint" && item.Length == 4)
                {
                    item.Length = 1;
                }
            }
            return colums;
        }

        public static  bool IsDoris(ISqlSugarClient context)
        {
            return context.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.Doris;
        }

        public static string UpdateDorisSql(ISqlBuilder sqlBuilder,List<DbColumnInfo> columns, string sql)
        {
            var pk = columns.FirstOrDefault(it => it.IsPrimarykey);
            if (pk != null)
            { 
                Check.ExceptionEasy(columns.Where(it => it.IsIdentity).Count() > 1, "Doris identity key no supported", "Doris不支持自增");
                Check.ExceptionEasy(columns.Where(it => it.IsPrimarykey).Count() > 1, "Doris Only one primary key is supported", "Doris只支持单主键");
                sql = sql.Replace("$PrimaryKey)", ")");
                var pkName = sqlBuilder.GetTranslationColumnName(pk.DbColumnName);
                sql += " \r\nENGINE=OLAP\r\nUNIQUE KEY(" + pkName + ")\r\nDISTRIBUTED BY HASH(" + pkName + ") BUCKETS 1\r\nPROPERTIES (\r\n    'replication_num' = '1',\r\n    'storage_format' = 'DEFAULT'\r\n);\r\n\r\n";
            }
            return sql;
        }
    }
}
