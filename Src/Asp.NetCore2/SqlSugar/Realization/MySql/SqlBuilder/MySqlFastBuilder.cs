using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data; 
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
   
    public partial class MySqlFastBuilder:FastBuilder,IFastBuilder
    {
        public override string UpdateSql { get; set; } = @"UPDATE  {1} TM    INNER JOIN {2} TE  ON {3} SET {0} ";
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        { 
            if (dt.Columns.Cast<DataColumn>().Any(it => it.DataType == UtilConstants.ByteArrayType)) 
            {
                return await MySqlConnectorBulkCopy(dt);
            } 
            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bulkcopyfiles");
            if (StaticConfig.BulkCopy_MySqlCsvPath.HasValue()) 
            {
                dllPath = StaticConfig.BulkCopy_MySqlCsvPath;
            }
            DirectoryInfo dir = new DirectoryInfo(dllPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            var fileName = Path.Combine(dllPath, Guid.NewGuid().ToString() + ".csv");
            var dataTableToCsv =new MySqlBlukCopy<object>(this.Context.Context,null,null).DataTableToCsvString(dt);
            File.WriteAllText(fileName, dataTableToCsv, new UTF8Encoding(false));
            MySqlConnection conn = this.Context.Ado.Connection as MySqlConnection;
            int result = 0;
            try
            {
                this.Context.Ado.Open();
                // IsolationLevel.Parse
                MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                {
                    CharacterSet = "utf8mb4",
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = Environment.NewLine,
                    FileName = fileName,
                    NumberOfLinesToSkip = 0,
                    TableName = dt.TableName,
                    Local = true,
                };
                if (this.CharacterSet.HasValue()) 
                {
                    bulk.CharacterSet = this.CharacterSet;
                }
                bulk.Columns.AddRange(dt.Columns.Cast<DataColumn>().Select(colum =>new MySqlBuilder().GetTranslationColumnName(colum.ColumnName)).Distinct().ToArray());
                result= await bulk.LoadAsync();
                //执行成功才删除文件
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch (Exception ex)
            {
                if (ex.Message == "The used command is not allowed with this MySQL version")
                {
                    Check.ExceptionEasy("connection string add : AllowLoadLocalInfile=true", "BulkCopy MySql连接字符串需要添加 AllowLoadLocalInfile=true; 添加后如果还不行Mysql数据库执行一下 SET GLOBAL local_infile=1 ");
                }
                else if (ex.Message.Contains("To use MySqlBulkLoader.Local=true, set Allo")) 
                {
                    Check.ExceptionEasy("connection string add : AllowLoadLocalInfile=true", "BulkCopy MySql连接字符串需要添加 AllowLoadLocalInfile=true; 添加后如果还不行Mysql数据库执行一下 SET GLOBAL local_infile=1 ");
                }
                else if (ex.Message == "Loading local data is disabled; this must be enabled on both the client and server sides")
                {
                    this.Context.Ado.ExecuteCommand("SET GLOBAL local_infile=1");
                    Check.ExceptionEasy(ex.Message, " 检测到你没有开启文件，AllowLoadLocalInfile=true加到自符串上，已自动执行 SET GLOBAL local_infile=1 在试一次");
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                CloseDb();
            }
            return result;
        }
        public override async Task CreateTempAsync<T>(DataTable dt) 
        {
            var queryable = this.Context.Queryable<T>();
            var tableName = queryable.SqlBuilder.GetTranslationTableName(dt.TableName);
            dt.TableName = "temp"+SnowFlakeSingle.instance.getID();
            
            // 检查是否为TiDB，TiDB不支持CREATE TEMPORARY TABLE ... (SELECT ...)语法
            if (this.Context?.CurrentConnectionConfig?.MoreSettings?.DatabaseModel == DbType.Tidb)
            {
                await CreateTempTableForTiDB(dt, tableName);
            }
            else
            {
                // 原有的MySQL逻辑
                var sql = string.Empty;
                if (dt.Columns.Cast<DataColumn>().Any(it => it.DataType == UtilConstants.ByteArrayType))
                {
                    sql = queryable.AS(tableName).Where(it => false)
                        .Select(string.Join(",", dt.Columns.Cast<DataColumn>().Select(it => queryable.SqlBuilder.GetTranslationTableName(it.ColumnName)))).ToSql().Key;
                }
                else 
                {
                    sql=queryable.AS(tableName).Where(it => false).ToSql().Key;
                }
                await this.Context.Ado.ExecuteCommandAsync($"Create TEMPORARY  table {dt.TableName}({sql}) ");
            }
        }

        /// <summary>
        /// 为TiDB创建临时表，使用标准的CREATE TEMPORARY TABLE语法
        /// </summary>
        private async Task CreateTempTableForTiDB(DataTable dt, string originalTableName)
        {
            var columnDefinitions = new List<string>();
            
            foreach (DataColumn column in dt.Columns)
            {
                var columnName = new MySqlBuilder().GetTranslationColumnName(column.ColumnName);
                var sqlType = GetTiDBColumnType(column);
                columnDefinitions.Add($"{columnName} {sqlType}");
            }
            
            var createTableSql = $"CREATE TEMPORARY TABLE {dt.TableName} ({string.Join(", ", columnDefinitions)})";
            await this.Context.Ado.ExecuteCommandAsync(createTableSql);
        }

        /// <summary>
        /// 将.NET DataColumn类型映射为TiDB支持的SQL类型
        /// </summary>
        private string GetTiDBColumnType(DataColumn column)
        {
            var dataType = column.DataType;
            
            if (dataType == typeof(int) || dataType == typeof(int?))
                return "INT";
            if (dataType == typeof(long) || dataType == typeof(long?))
                return "BIGINT";
            if (dataType == typeof(short) || dataType == typeof(short?))
                return "SMALLINT";
            if (dataType == typeof(byte) || dataType == typeof(byte?))
                return "TINYINT";
            if (dataType == typeof(bool) || dataType == typeof(bool?))
                return "BOOLEAN";
            if (dataType == typeof(decimal) || dataType == typeof(decimal?))
                return "DECIMAL(18,2)";
            if (dataType == typeof(double) || dataType == typeof(double?))
                return "DOUBLE";
            if (dataType == typeof(float) || dataType == typeof(float?))
                return "FLOAT";
            if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
                return "DATETIME";
            if (dataType == typeof(DateTimeOffset) || dataType == typeof(DateTimeOffset?))
                return "TIMESTAMP";
            if (dataType == typeof(TimeSpan) || dataType == typeof(TimeSpan?))
                return "TIME";
            if (dataType == typeof(Guid) || dataType == typeof(Guid?))
                return "VARCHAR(36)";
            if (dataType == typeof(byte[]))
                return "LONGBLOB";
            if (dataType == typeof(string))
            {
                // 根据MaxLength决定使用VARCHAR还是TEXT
                if (column.MaxLength > 0 && column.MaxLength <= 65535)
                    return $"VARCHAR({column.MaxLength})";
                else
                    return "TEXT";
            }
            
            // 默认使用TEXT类型
            return "TEXT";
        }
    }
}
