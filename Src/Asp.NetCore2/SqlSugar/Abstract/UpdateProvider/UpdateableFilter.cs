using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateableFilter<T> where T : class, new()
    {
        public T[] DataList { get; set; }
        public SqlSugarProvider Context { get; set; }
        public int PageSize { get; internal set; }
        public string TableName { get; internal set; }
        public bool IsEnableDiffLogEvent { get; internal set; }
        public DiffLogModel DiffModel { get; internal set; }
        public List<string> UpdateColumns { get; internal set; }
        public int ExecuteCommand()
        {
            if (DataList.Count() == 1 && DataList.First() == null)
            {
                return 0;
            }
            PageSize = 1;
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    this.Context.Ado.BeginTran();
                }
                this.Context.Utilities.PageEach(DataList, PageSize, pageItem =>
                {
                    result += SetFilterSql(this.Context.Updateable(pageItem.First()).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).UpdateColumns(UpdateColumns.ToArray())).ExecuteCommand();
                });
                if (isNoTran)
                {
                    this.Context.Ado.CommitTran();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                    this.Context.Ado.RollbackTran();
                }
                throw;
            }
            return result;
        }



        public async Task<int> ExecuteCommandAsync()
        {
            if (DataList.Count() == 1 && DataList.First() == null)
            {
                return 0;
            }
            PageSize = 1;
            var result = 0;
            var isNoTran = this.Context.Ado.IsNoTran();
            try
            {
                if (isNoTran)
                {
                    await this.Context.Ado.BeginTranAsync();
                }
                await this.Context.Utilities.PageEachAsync(DataList, PageSize, async pageItem =>
                {
                    result += await SetFilterSql(this.Context.Updateable(pageItem.First()).AS(TableName).EnableDiffLogEventIF(IsEnableDiffLogEvent, DiffModel).UpdateColumns(UpdateColumns.ToArray())).ExecuteCommandAsync();
                });
                if (isNoTran)
                {
                    await this.Context.Ado.CommitTranAsync();
                }
            }
            catch (Exception)
            {
                if (isNoTran)
                {
                    await this.Context.Ado.RollbackTranAsync();
                }
                throw;
            }
            return result;
        }


        private IUpdateable<T> SetFilterSql(IUpdateable<T> updateable)
        {
            var queryable = this.Context.Queryable<T>();
            queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 10000;
            var sqlobj=queryable.ToSql();
            var sql= UtilMethods.RemoveBeforeFirstWhere(sqlobj.Key);
            if (sql!=sqlobj.Key)
            {
                updateable.UpdateBuilder.AppendWhere = sql;
            }
            if (sqlobj.Value != null) 
            {
                updateable.UpdateBuilder.Parameters.AddRange(sqlobj.Value);
            }
            return updateable;
        }
    }
}
