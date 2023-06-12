using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar 
{
    public class QueryMethodInfo
    {
        public object QueryableObj { get; internal set; }
        public SqlSugarProvider Context { get; internal set; }
        public Type EntityType { get;  set; }


        #region Json 2 sql api
        #endregion

        #region Sql API
        public QueryMethodInfo AS(string tableName)
        {
            string shortName = $"{tableName}_1";
            var method = QueryableObj.GetType().GetMyMethod("AS", 2, typeof(string), typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { tableName, shortName });
            return this;
        }
        public QueryMethodInfo AS(string tableName, string shortName)
        {
            var method = QueryableObj.GetType().GetMyMethod("AS", 2, typeof(string), typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { tableName, shortName });
            return this;
        }
        public QueryMethodInfo OrderBy(string orderBySql)
        {
            var method = QueryableObj.GetType().GetMyMethod("OrderBy", 1, typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { orderBySql });
            return this;
        }
        public QueryMethodInfo AddJoinInfo(string tableName, string shortName,string onWhere, JoinType type = JoinType.Left) 
        {
            var method = QueryableObj.GetType().GetMyMethod("AddJoinInfo", 4, typeof(string),typeof(string),typeof(string),typeof(JoinType));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { tableName,shortName,onWhere,type });
            return this;
        }
        public QueryMethodInfo AddJoinInfo(Type joinEntityType, string shortName, string onWhere, JoinType type = JoinType.Left)
        {
            var method = QueryableObj.GetType().GetMyMethod("AddJoinInfo", 4, typeof(string), typeof(string), typeof(string), typeof(JoinType));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { this.Context.EntityMaintenance.GetTableName(joinEntityType), shortName, onWhere, type });
            return this;
        }
        public QueryMethodInfo GroupBy(string groupBySql)
        {
            var method = QueryableObj.GetType().GetMyMethod("GroupBy", 1, typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { groupBySql });
            return this;
        }
        public QueryMethodInfo Where(string sql, object parameters = null)
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 2, typeof(string), typeof(object));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { sql, parameters });
            return this;
        }
        public QueryMethodInfo Having(string sql, object parameters = null)
        {
            var method = QueryableObj.GetType().GetMyMethod("Having", 2, typeof(string), typeof(object));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { sql, parameters });
            return this;
        }
        public QueryMethodInfo SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc)
        {
            var method = QueryableObj.GetType().GetMyMethod("SplitTable", 1, typeof(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>>));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { getTableNamesFunc });
            return this;
        }
        public QueryMethodInfo SplitTable(DateTime begintTime, DateTime endTime)
        {
            var method = QueryableObj.GetType().GetMyMethod("SplitTable", 2, typeof(DateTime), typeof(DateTime));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { begintTime, endTime });
            return this;
        }
        public QueryMethodInfo SplitTable()
        {
            var method = QueryableObj.GetType().GetMyMethod("SplitTable", 0);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { });
            return this;
        }

        public QueryMethodInfo Select(string selectorSql)
        {
            var method = QueryableObj.GetType().GetMyMethod("Select", 1, typeof(string))
             .MakeGenericMethod(EntityType);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { selectorSql });
            return this;
        }

        public QueryMethodInfo Select(string selectorSql, Type selectType)
        {
            var method = QueryableObj.GetType().GetMyMethod("Select", 1, typeof(string))
             .MakeGenericMethod(selectType);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { selectorSql });
            return this;
        }

        #endregion

        #region Result
        public object ToPageList(int pageNumber, int pageSize)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageList", 2, typeof(int), typeof(int));
            var reslt = method.Invoke(QueryableObj, new object[] { pageNumber, pageSize });
            return reslt;
        }
        public object ToPageList(int pageNumber, int pageSize, ref int count)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageList", 3, typeof(int), typeof(int), typeof(int).MakeByRefType());
            var parameters = new object[] { pageNumber, pageSize, count };
            var reslt = method.Invoke(QueryableObj, parameters);
            count = parameters.Last().ObjToInt();
            return reslt;
        }
        public object ToList()
        {
            var method = QueryableObj.GetType().GetMyMethod("ToList", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return reslt;
        }
        public object First()
        {
            var method = QueryableObj.GetType().GetMyMethod("First", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return reslt;
        }
        public object Any()
        {
            var method = QueryableObj.GetType().GetMyMethod("Any", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return reslt;
        }
        #endregion

        #region Result Async
        public async Task<object> ToPageListAsync(int pageNumber, int pageSize)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageListAsync", 2, typeof(int), typeof(int));
            Task task = (Task)method.Invoke(QueryableObj, new object[] { pageNumber, pageSize });
            return await GetTask(task).ConfigureAwait(false);
        }
        public async Task<object> ToPageListAsync(int pageNumber, int pageSize, RefAsync<int> count)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToPageListAsync", 3, typeof(int), typeof(int),typeof( RefAsync<int>));
            var parameters = new object[] { pageNumber, pageSize, count };
            var task = (Task)method.Invoke(QueryableObj, parameters);
            return await GetTask(task).ConfigureAwait(false);
        }
        public async Task<object> ToListAsync()
        {
            var method = QueryableObj.GetType().GetMyMethod("ToListAsync", 0);
            var task = (Task)method.Invoke(QueryableObj, new object[] { });
            return await GetTask(task).ConfigureAwait(false);
        }
        public async Task<object> FirstAsync()
        {
            var method = QueryableObj.GetType().GetMyMethod("FirstAsync", 0);
            var task = (Task)method.Invoke(QueryableObj, new object[] { });
            return await GetTask(task).ConfigureAwait(false);
        }
        public async Task<bool> AnyAsync()
        {
            var method = QueryableObj.GetType().GetMyMethod("AnyAsync", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return await (Task<bool>) reslt;
        } 
        #endregion

        #region Helper
        private static async Task<object> GetTask(Task task)
        {
            await task.ConfigureAwait(false); // 等待任务完成
            var resultProperty = task.GetType().GetProperty("Result");
            var result = resultProperty.GetValue(task);
            return result;
        }
        #endregion
    }
}
