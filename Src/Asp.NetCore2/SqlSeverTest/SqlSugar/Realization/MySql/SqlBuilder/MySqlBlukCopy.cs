using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class MySqlBlukCopy<T> 
    {
        internal SqlSugarProvider Context { get; set; }
        internal ISqlBuilder Builder { get; set; }
        internal T[] Entitys { get; set; }
        internal string Chara { get; set; }
        private MySqlBlukCopy()
        {

        }
        public MySqlBlukCopy(SqlSugarProvider context, ISqlBuilder builder, T []entitys)
        {
            this.Context = context;
            this.Builder = builder;
            this.Entitys = entitys;
        }
        public bool ExecuteBulkCopy(string characterSet) 
        {
            this.Chara = characterSet;
            return ExecuteBulkCopy();
        }

        public bool ExecuteBulkCopy()
        {
            var IsBulkLoad = false;
            if (Entitys == null || Entitys.Length <= 0)
                return IsBulkLoad;
            if (Entitys.First() == null && Entitys.Length ==1)
                return IsBulkLoad;
            DataTable dt = new DataTable();
            Type type = typeof(T);
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            dt.TableName = this.Builder.GetTranslationColumnName(entity.DbTableName);
            //if (this.Context.MappingTables != null && this.Context.MappingTables.Any(it => it.EntityName == it.EntityName)) 
            //{
            //    dt.TableName = this.Builder.GetTranslationColumnName(this.Context.MappingTables.First(it => it.EntityName == it.EntityName).DbTableName);
            //}
            //创建属性的集合    
            List<PropertyInfo> pList = new List<PropertyInfo>();
            //把所有的public属性加入到集合 并添加DataTable的列    
            Array.ForEach(entity.Columns.ToArray(), p => {
                if (!p.IsIgnore&& !p.IsOnlyIgnoreInsert)
                {
                    pList.Add(p.PropertyInfo); dt.Columns.Add(p.DbColumnName);
                }
            });
            DataRow row = null;
            foreach (T item in Entitys)
            {
                row = dt.NewRow();
                pList.ForEach(p =>
                {
                    var name = p.Name;
                    if (entity.Columns.Any(it => it.PropertyName == name))
                    {
                        name = entity.Columns.First(it => it.PropertyName == name).DbColumnName;
                    }
                    row[name] = GetValue(p, item);
                });
                dt.Rows.Add(row);
            }
            var dllPath =Path.Combine(AppDomain.CurrentDomain.BaseDirectory , "failFiles");
            DirectoryInfo dir = new DirectoryInfo(dllPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            var fileName =Path.Combine( dllPath , Guid.NewGuid().ToString() + ".csv");
            var dataTableToCsv = DataTableToCsvString(dt);
            File.WriteAllText(fileName, dataTableToCsv, new UTF8Encoding(false));
            MySqlConnection conn = this.Context.Ado.Connection as MySqlConnection;
            try
            {
                this.Context.Ado.Open();
                // IsolationLevel.Parse
                MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                {
                    CharacterSet = GetChara(),
                    FieldTerminator = ",",
                    FieldQuotationCharacter = '"',
                    EscapeCharacter = '"',
                    LineTerminator = Environment.NewLine,
                    FileName = fileName,
                    NumberOfLinesToSkip = 0,
                    TableName = dt.TableName,
                    Local = true,
                };
                bulk.Columns.AddRange(dt.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).Distinct().ToArray());
                IsBulkLoad = bulk.Load() > 0;
                //执行成功才删除文件
                if (IsBulkLoad && File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
            }
            catch (MySqlException ex)
            {
                throw ex;
            }
            finally
            {
                CloseDb();
            }
            return IsBulkLoad; ;
        }

        public Task<bool> ExecuteBulkCopyAsync()
        {
            return Task.FromResult(ExecuteBulkCopy());
        }

        public Task<bool> ExecuteBulkCopyAsync(string characterSet)
        {
            this.Chara = characterSet;
            return Task.FromResult(ExecuteBulkCopy());
        }

        #region  Helper
        private string GetChara()
        {
            if (this.Chara == null)
            {
                return "UTF8";
            }
            else
            {
                return this.Chara;
            }
        }

        private void CloseDb()
        {
            if (this.Context.CurrentConnectionConfig.IsAutoCloseConnection && this.Context.Ado.Transaction == null)
            {
                this.Context.Ado.Connection.Close();
            }
        }

        /// <summary>
        ///DataTable to CSV
        /// </summary>
        /// <param name="table">datatable</param>
        /// <returns>CSV</returns>
        public string DataTableToCsvString(DataTable table)
        {
            if (table.Rows.Count == 0)
                return "";
            StringBuilder sb = new StringBuilder();
            DataColumn colum;
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    colum = table.Columns[i];
                    if (i != 0) sb.Append(",");
                    if (colum.DataType == typeof(string) && (row[colum].ToString().Contains(",") || row[colum].ToString().Contains("\r") || row[colum].ToString().Contains("\"")))
                    {
                        sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                    }
                    else if (colum.DataType == typeof(bool))
                    {
                        sb.Append(row[colum].ObjToBool() ? 1 : 0);
                    }
                    else if (colum.DataType == UtilConstants.DateType&& row[colum] != null && row[colum] != DBNull.Value) 
                    {
                        sb.Append(row[colum].ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    }
                    else sb.Append(row[colum].ToString());
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }


        private static object GetValue(PropertyInfo p, T item)
        {
            var result= p.GetValue(item, null);
            if (result != null && UtilMethods.GetUnderType(p.PropertyType) == UtilConstants.BoolType) 
            {
                if (result.ObjToBool() == false) 
                {
                    result = null;
                }
            }
            return result;
        }

        #endregion
    }
}