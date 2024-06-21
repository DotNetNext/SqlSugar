using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class ParameterUpdateable<T> where T : class, new()
    {
        internal UpdateableProvider<T> Updateable { get; set; }
        internal SqlSugarProvider Context { get; set; }
        public int ExecuteCommand()
        {
            var result = 0;
            var list = Updateable.UpdateObjs;
            var count = list.Length;
            var size = GetPageSize(20, count);
            Context.Utilities.PageEach(list.ToList(), size, item =>
            { 
                Before(item.ToList());
                List<SugarParameter> allParamter = new List<SugarParameter>();
                var sql=GetSql(item);
                result+=Context.Ado.ExecuteCommand(sql.Key, sql.Value);
                After(item.ToList());
            });
            return result<0?count:result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            var result = 0;
            var list = Updateable.UpdateObjs;
            var count = list.Length;
            var size = GetPageSize(20, count);
            await Context.Utilities.PageEachAsync(list.ToList(), size,async item =>
            {
                Before(item.ToList());
                List<SugarParameter> allParamter = new List<SugarParameter>();
                var sql = GetSql(item);
                result +=await Context.Ado.ExecuteCommandAsync(sql.Key, sql.Value);
                After(item.ToList());
            });
            return result < 0 ? count : result;
        }


        private void Before(List<T> updateObjects)
        {
            if (this.Updateable.IsEnableDiffLogEvent && updateObjects.Count > 0)
            {
                var isDisableMasterSlaveSeparation = this.Updateable.Ado.IsDisableMasterSlaveSeparation;
                this.Updateable.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = Updateable.UpdateBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                Updateable.diffModel.BeforeData = GetDiffTable(updateObjects);
                Updateable.diffModel.Sql = this.Updateable.UpdateBuilder.ToSqlString();
                Updateable.diffModel.Parameters = parameters.ToArray();
                this.Updateable.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
        }

        protected void After(List<T> updateObjects)
        {
            if (this.Updateable.IsEnableDiffLogEvent && updateObjects.Count > 0)
            {
                var isDisableMasterSlaveSeparation = this.Updateable.Ado.IsDisableMasterSlaveSeparation;
                this.Updateable.Ado.IsDisableMasterSlaveSeparation = true;
                Updateable.diffModel.AfterData = GetDiffTable(updateObjects);
                Updateable.diffModel.Time = this.Context.Ado.SqlExecutionTime;
                if (this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent != null)
                    this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent(Updateable.diffModel);
                this.Updateable.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
            if (this.Updateable.RemoveCacheFunc != null)
            {
                this.Updateable.RemoveCacheFunc();
            }
        }
        private List<DiffLogTableInfo> GetDiffTable(List<T> updateObjects)
        {
            var builder = Updateable.UpdateBuilder.Builder;
            var tableWithString = builder.GetTranslationColumnName(Updateable.UpdateBuilder.TableName);
            var wheres = Updateable.WhereColumnList ?? Updateable.UpdateBuilder.PrimaryKeys;
            if (wheres == null)
            {
                wheres = Updateable.UpdateBuilder.DbColumnInfoList
                    .Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).Distinct().ToList();
            }
            var sqlDb = this.Context.CopyNew();
            sqlDb.Aop.DataExecuting = null;
            var dataColumns = sqlDb.Updateable(updateObjects).UpdateBuilder.DbColumnInfoList;
            List<SugarParameter> parameters = new List<SugarParameter>();
            StringBuilder allWhereString = new StringBuilder();
            string columnStr = string.Join(",", dataColumns.Select(x => x.DbColumnName).Distinct().ToList());
            foreach (var item in dataColumns.GroupBy(it => it.TableId))
            {
                StringBuilder whereString = new StringBuilder();
                foreach (var whereItem in wheres)
                {
                    var pk = item.FirstOrDefault(it => it.DbColumnName.EqualCase(whereItem));
                    var paraterName = FormatValue(pk.PropertyType, pk.DbColumnName, pk.Value, parameters);
                    whereString.Append($" {pk.DbColumnName} = {paraterName} AND");
                }
                allWhereString.Append($" {Regex.Replace(whereString.ToString(), "AND$", "")} OR");
            }
            string key = $"SELECT {columnStr} FROM {tableWithString} WHERE {Regex.Replace(allWhereString.ToString(), "OR$", "")}";

            var dt = sqlDb.Ado.GetDataTable(key, parameters);
            return Updateable.GetTableDiff(dt);
        }

        #region Values Helper

        public KeyValuePair<string, SugarParameter[]> GetSql(List<T> updateObjects)
        {
            var inserable = Updateable as UpdateableProvider<T>;
            var builder = inserable.UpdateBuilder.Builder;
            var columns = inserable.UpdateBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(it => it.Key).Distinct().ToList();
            var tableWithString = builder.GetTranslationColumnName(inserable.UpdateBuilder.TableName);
            var wheres = inserable.WhereColumnList ?? inserable.UpdateBuilder.PrimaryKeys;
            if (wheres == null)
            {
                wheres = inserable.UpdateBuilder.DbColumnInfoList
                    .Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).Distinct().ToList();
            }
            StringBuilder sbAllSql = new StringBuilder();
            var sqlTemp = ($" UPDATE {tableWithString} SET {{0}}  WHERE {{1}};\r\n");
            List<SugarParameter> parameters = new List<SugarParameter>();
            Check.ExceptionEasy(wheres?.Any() != true, "Updates cannot be without a primary key or condition", "更新不能没有主键或者条件");
            var sqlDb = this.Context.CopyNew();
            sqlDb.Aop.DataExecuting = null;
            foreach (var item in sqlDb.Updateable(updateObjects).UpdateBuilder.DbColumnInfoList.GroupBy(it => it.TableId))
            {
                Check.ExceptionEasy(item?.ToList()?.Any() != true, "Set has no columns", "更新Set没有列");
                StringBuilder setString = new StringBuilder();
                foreach (var setItem in item.ToList())
                {
                    if (setItem.IsPrimarykey) { continue; }
                    if (Updateable.UpdateBuilder.UpdateColumns?.Any() == true)
                    {
                        if (!Updateable.UpdateBuilder.UpdateColumns.Any(it => it.EqualCase(setItem.DbColumnName)))
                        {
                            continue;
                        }
                    }
                    if (Updateable.UpdateBuilder.IgnoreColumns?.Any() == true)
                    {
                        if (Updateable.UpdateBuilder.IgnoreColumns.Any(it => it.EqualCase(setItem.DbColumnName)))
                        {
                            continue;
                        }
                    }
                    var paraterName = FormatValue(setItem.PropertyType, setItem.DbColumnName, setItem.Value, parameters);
                    setString.Append($" {builder.GetTranslationColumnName(setItem.DbColumnName)} = {paraterName} ,");
                }
                StringBuilder whereString = new StringBuilder();
                foreach (var whereItem in wheres)
                {
                    var pk = item.FirstOrDefault(it => it.DbColumnName.EqualCase(whereItem));
                    var paraterName = FormatValue(pk.PropertyType, pk.DbColumnName, pk.Value, parameters);
                    whereString.Append($" {pk.DbColumnName} = {paraterName} AND");
                }
                var builderItem = string.Format(sqlTemp, setString.ToString().TrimEnd(','), whereString.ToString().TrimEnd('D').TrimEnd('N').TrimEnd('A'));
                sbAllSql.Append(builderItem);
            }
            builder.FormatSaveQueueSql(sbAllSql);
            return new KeyValuePair<string, SugarParameter[]>(sbAllSql.ToString(), parameters.ToArray());
        }

        private  int GetPageSize(int pageSize, int count)
        {
            if (pageSize * count > 2100)
            {
                pageSize = 50;
            }
            if (pageSize * count > 2100)
            {
                pageSize = 20;
            }
            if (pageSize * count > 2100)
            {
                pageSize = 10;
            }

            return pageSize;
        }
        private string FormatValue(Type type, string name, object value, List<SugarParameter> allParamter)
        {
            var keyword=this.Updateable.UpdateBuilder.Builder.SqlParameterKeyWord;
            var result = keyword + name + allParamter.Count;
            var addParameter = new SugarParameter(result, value, type);
            allParamter.Add(addParameter);
            return result;
        }
        #endregion
    }
}
