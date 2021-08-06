using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ParameterInsertable<T> : IParameterInsertable<T> where T:class,new()
    {
        internal IInsertable<T>   Inserable { get; set; }
        internal SqlSugarProvider Context { get; set; }
        public int ExecuteCommand()
        {
            if (this.Context.CurrentConnectionConfig.DbType.IsIn(DbType.Oracle, DbType.Dm))
            {
                return DefaultExecuteCommand();
            }
            else
            {
                return ValuesExecuteCommand();
            }
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (this.Context.CurrentConnectionConfig.DbType.IsIn(DbType.Oracle, DbType.Dm))
            {
                return await DefaultExecuteCommandAsync();
            }
            else
            {
                return await ValuesExecuteCommandAsync();
            }
        }
        public int DefaultExecuteCommand() 
        {
            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns= inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it=>it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            this.Context.Utilities.PageEach(objects, 60, pagelist =>
            {
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("begin");
                foreach (var item in pagelist)
                {
                    var itemable = this.Context.Insertable(item);
                    itemable.InsertBuilder.DbColumnInfoList = itemable.InsertBuilder.DbColumnInfoList.Where(it => columns.Contains(it.DbColumnName)).ToList();
                    itemable.InsertBuilder.TableWithString = tableWithString;
                    (itemable as InsertableProvider<T>).RemoveCacheFunc = removeCacheFunc;
                    itemable.AddQueue();
                }
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("end \r\n");
                result +=this.Context.SaveQueues(false);
            });
            if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                return objects.Length;
            return result;
        }
        public async Task<int> DefaultExecuteCommandAsync()
        {
            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns = inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it => it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            await this.Context.Utilities.PageEachAsync<T,int>(objects, 60,async pagelist =>
            {
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    this.Context.AddQueue("Begin");
                foreach (var item in pagelist)
                {
                    var itemable = this.Context.Insertable(item);
                    itemable.InsertBuilder.DbColumnInfoList = itemable.InsertBuilder.DbColumnInfoList.Where(it => columns.Contains(it.DbColumnName)).ToList();
                    itemable.InsertBuilder.TableWithString = tableWithString;
                    (itemable as InsertableProvider<T>).RemoveCacheFunc = removeCacheFunc;
                    itemable.AddQueue();
                }
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle) 
                    this.Context.AddQueue("End");
                result += await this.Context.SaveQueuesAsync(false);
                if (this.Context.CurrentConnectionConfig.DbType == DbType.Oracle)
                    return objects.Length;
                return result;
            });
            return result;
        }
        public int ValuesExecuteCommand()
        {

            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns = inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it => it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            var identityList = inserable.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.PropertyName).ToArray();
            this.Context.Utilities.PageEach(objects, 100, pagelist =>
            {
 
                    StringBuilder batchInsetrSql;
                    List<SugarParameter> allParamter=new List<SugarParameter>();
                    GetInsertValues(identityList,columns, tableWithString, removeCacheFunc, pagelist, out batchInsetrSql, allParamter);
                    result += this.Context.Ado.ExecuteCommand(batchInsetrSql.ToString(), allParamter);

            });
            return result;

        }
        public  async Task<int> ValuesExecuteCommandAsync()
        {
            int result = 0;
            var inserable = Inserable as InsertableProvider<T>;
            var columns = inserable.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it => it.Key).Distinct().ToList();
            var tableWithString = inserable.InsertBuilder.TableWithString;
            var removeCacheFunc = inserable.RemoveCacheFunc;
            var objects = inserable.InsertObjs;
            var identityList = inserable.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.PropertyName).ToArray();
            await this.Context.Utilities.PageEachAsync(objects, 100,async pagelist =>
            {

                StringBuilder batchInsetrSql;
                List<SugarParameter> allParamter = new List<SugarParameter>();
                GetInsertValues(identityList, columns, tableWithString, removeCacheFunc, pagelist, out batchInsetrSql, allParamter);
                result +=await this.Context.Ado.ExecuteCommandAsync(batchInsetrSql.ToString(), allParamter);

            });
            return result;
        }
        #region Values Helper
        private void GetInsertValues(string[] identitys, List<string> columns, string tableWithString, Action removeCacheFunc, List<T> items, out StringBuilder batchInsetrSql, List<SugarParameter> allParamter)
        {
            var itemable = this.Context.Insertable(items);
            itemable.InsertBuilder.DbColumnInfoList = itemable.InsertBuilder.DbColumnInfoList.Where(it => columns.Contains(it.DbColumnName)).ToList();
            itemable.InsertBuilder.TableWithString = tableWithString;
            (itemable as InsertableProvider<T>).RemoveCacheFunc = removeCacheFunc;
            batchInsetrSql = new StringBuilder();
            batchInsetrSql.Append("INSERT INTO " + itemable.InsertBuilder.GetTableNameString + " ");
            batchInsetrSql.Append("(");
            var groupList = itemable.InsertBuilder.DbColumnInfoList.Where(it => !identitys.Contains(it.PropertyName)).GroupBy(it => it.TableId).ToList();
            string columnsString = string.Join(",", groupList.First().Select(it => itemable.InsertBuilder.Builder.GetTranslationColumnName(it.DbColumnName)));
            batchInsetrSql.Append(columnsString);
            batchInsetrSql.Append(") VALUES");
            string insertColumns = "";
            foreach (var gitem in groupList)
            {
                batchInsetrSql.Append("(");
                insertColumns = string.Join(",", gitem.Select(it => FormatValue(it.DbColumnName, it.Value, allParamter, itemable.InsertBuilder.Builder.SqlParameterKeyWord)));
                batchInsetrSql.Append(insertColumns);
                if (groupList.Last() == gitem)
                {
                    batchInsetrSql.Append(") ");
                }
                else
                {
                    batchInsetrSql.Append("),  ");
                }
            }
        }
        private string FormatValue(string name, object value, List<SugarParameter> allParamter, string keyword)
        {
            var result = keyword + name + allParamter.Count;
            allParamter.Add(new SugarParameter(result, value));
            return result;
        } 
        #endregion
    }
}
