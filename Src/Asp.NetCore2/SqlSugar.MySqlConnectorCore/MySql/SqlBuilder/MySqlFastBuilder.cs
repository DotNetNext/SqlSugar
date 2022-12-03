using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar.MySqlConnector
{
   
    public class MySqlFastBuilder:FastBuilder,IFastBuilder
    {
        public override string UpdateSql { get; set; } = @"UPDATE  {1} TM    INNER JOIN {2} TE  ON {3} SET {0} ";
        public async Task<int> ExecuteBulkCopyAsync(DataTable dt)
        {

            var dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bulkcopyfiles");
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
            catch (MySqlException ex)
            {
                if (ex.Message == "The used command is not allowed with this MySQL version")
                {
                    Check.ExceptionEasy("connection string add : AllowLoadLocalInfile=true", "BulkCopy MySql连接字符串需要添加 AllowLoadLocalInfile=true; 添加后如果还不行Mysql数据库执行一下 SET GLOBAL local_infile=1 ");
                }
                else if (ex.Message == "Loading local data is disabled; this must be enabled on both the client and server sides")
                {
                    this.Context.Ado.ExecuteCommand("SET GLOBAL local_infile=1");
                    Check.ExceptionEasy(ex.Message, " 检测到你没有开启文件，已自动执行 SET GLOBAL local_infile=1 在试一次");
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
            var sql = queryable.AS(tableName).Where(it => false).ToSql().Key;
            await this.Context.Ado.ExecuteCommandAsync($"Create TEMPORARY  table {dt.TableName}({sql}) ");
        }
    }
}
