using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public QueryMethodInfo OrderBy(List<OrderByModel> models) 
        {
            var method = QueryableObj.GetType().GetMyMethod("OrderBy", 1, typeof(List<OrderByModel>));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { models });
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
        public QueryMethodInfo AddJoinInfo(string tableName, string shortName, IFuncModel onFunc, JoinType type = JoinType.Left)
        {
            var method = QueryableObj.GetType().GetMyMethod("AddJoinInfo", 4, typeof(string), typeof(string), typeof(IFuncModel), typeof(JoinType));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { tableName, shortName, onFunc, type });
            return this;
        }
        public QueryMethodInfo AddJoinInfo(List<JoinInfoParameter> joinInfoParameters) 
        {
            foreach (var item in joinInfoParameters)
            {
                AddJoinInfo(item.TableName,item.ShortName,item.Models,item.Type);
            }
            return this;
        }
        public QueryMethodInfo AddJoinInfo(Type joinEntityType,Dictionary<string,Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expOnWhere, JoinType type = JoinType.Left)
        {
            var method = QueryableObj.GetType().GetMyMethod("AddJoinInfo", 4,typeof(Type), typeof(Dictionary<string, Type>), typeof(FormattableString), typeof(JoinType));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { joinEntityType, keyIsShortName_ValueIsType_Dictionary, expOnWhere, type });
            return this;
        }
        public QueryMethodInfo AddJoinInfo(Type joinEntityType, string shortName, string onWhere, JoinType type = JoinType.Left)
        {
            var method = QueryableObj.GetType().GetMyMethod("AddJoinInfo", 4, typeof(string), typeof(string), typeof(string), typeof(JoinType));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { this.Context.EntityMaintenance.GetTableName(joinEntityType), shortName, onWhere, type });
            return this;
        }
        public QueryMethodInfo GroupBy(List<GroupByModel> models) 
        {
            var method = QueryableObj.GetType().GetMyMethod("GroupBy", 1, typeof(List<GroupByModel>));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { models });
            return this;
        }
        public QueryMethodInfo GroupBy(string groupBySql)
        {
            var method = QueryableObj.GetType().GetMyMethod("GroupBy", 1, typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { groupBySql });
            return this;
        }
        public QueryMethodInfo Where(string expShortName, FormattableString expressionString) 
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 2, typeof(string),typeof(FormattableString));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { expShortName, expressionString });
            return this;
        }
        public QueryMethodInfo Where(Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expressionString)
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 2, typeof(Dictionary<string, Type>), typeof(FormattableString));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { keyIsShortName_ValueIsType_Dictionary, expressionString });
            return this;
        }
        public QueryMethodInfo Where(List<IConditionalModel> conditionalModels) 
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 1, typeof(List<IConditionalModel>));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { conditionalModels });
            return this;
        }

        public QueryMethodInfo Where(IFuncModel model)
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 1, typeof(IFuncModel));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { model });
            return this;
        }

        public QueryMethodInfo Where(List<IConditionalModel> conditionalModels, bool isWrap)
        {
            var method = QueryableObj.GetType().GetMyMethod("Where", 2, typeof(List<IConditionalModel>),typeof(bool));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { conditionalModels,isWrap });
            return this;
        }
        public QueryMethodInfo Where(string sql, object parameters = null)
        {
            var method = QueryableObj.GetType().GetMyMethodNoGen("Where", 2, typeof(string), typeof(object));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { sql, parameters });
            return this;
        }
        public QueryMethodInfo Having(IFuncModel model) 
        {
            var method = QueryableObj.GetType().GetMyMethod("Having", 1, typeof(IFuncModel));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] {model});
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

        public QueryMethodInfo Select(List<SelectModel> models) 
        {
            var method = QueryableObj.GetType().GetMyMethod("Select", 1, typeof(List<SelectModel>));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { models });
            return this;
        }
        public QueryMethodInfo Select(string expShortName, FormattableString expSelect, Type resultType)
        {
            var method = QueryableObj.GetType().GetMyMethod("Select", 3, typeof(string),typeof(FormattableString),typeof(Type));
            method= method.MakeGenericMethod(resultType);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { expShortName, expSelect, resultType });
            return this;
        }
        public QueryMethodInfo Select(Dictionary<string, Type> keyIsShortName_ValueIsType_Dictionary, FormattableString expSelect, Type resultType)
        {
            var method = QueryableObj.GetType().GetMyMethod("Select", 3, typeof(Dictionary<string, Type>), typeof(FormattableString), typeof(Type));
            method = method.MakeGenericMethod(resultType);
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { keyIsShortName_ValueIsType_Dictionary, expSelect, resultType });
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

        #region Nav
        
        public QueryMethodInfo IncludesAllFirstLayer(params string[] ignoreNavPropertyNames)
        {
            var method = QueryableObj.GetType().GetMyMethod("IncludesAllFirstLayer",1,typeof(string[]));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { ignoreNavPropertyNames });
            return this;
        }
        public QueryMethodInfo Includes(string navProperyName)
        {
            var method = QueryableObj.GetType().GetMyMethod("IncludesByNameString", 1, typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { navProperyName });
            return this;
        }
        public QueryMethodInfo Includes(string navProperyName,string thenNavProperyName2)
        {
            var method = QueryableObj.GetType().GetMyMethod("IncludesByNameString", 2, typeof(string),typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { navProperyName , thenNavProperyName2 });
            return this;
        }
        public QueryMethodInfo Includes(string navProperyName, string thenNavProperyName2, string thenNavProperyName3)
        {
            var method = QueryableObj.GetType().GetMyMethod("IncludesByNameString", 3, typeof(string), typeof(string),typeof(string));
            this.QueryableObj = method.Invoke(QueryableObj, new object[] { navProperyName, thenNavProperyName2 , thenNavProperyName3 });
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
        public string ToSqlString()
        {
            var method = QueryableObj.GetType().GetMyMethod("ToSqlString", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return (string)reslt;
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            var method = QueryableObj.GetType().GetMyMethod("ToSql", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return (KeyValuePair<string, List<SugarParameter>>)reslt;
        }
        public object InSingle(object pkValue)
        {
            var method = QueryableObj.GetType().GetMyMethod("InSingle", 1);
            var reslt = method.Invoke(QueryableObj, new object[] { pkValue });
            return reslt;
        }
        public bool CreateView(string viewNameFomat)
        {
            if (viewNameFomat?.Contains("{0}")!=true) 
            {
                Check.ExceptionEasy("need{0}", "需要{0}表名的占位符");
            }
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(EntityType);
            var viewName = string.Format(viewNameFomat, entityInfo.DbTableName);
            if (!this.Context.DbMaintenance.GetViewInfoList().Any(it => it.Name.EqualCase(viewName))) {
                var method = QueryableObj.GetType().GetMyMethod("ToSqlString", 0);
                var reslt = (string)method.Invoke(QueryableObj, new object[] { });
                var sql = $"CREATE  VIEW  {viewName} AS {Environment.NewLine} {reslt}";
                this.Context.Ado.ExecuteCommand(sql);
                return true;
            }
            else 
            {
                return false;
            }
        }
        public object First()
        {
            var method = QueryableObj.GetType().GetMyMethod("First", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return reslt;
        }
        public bool Any()
        {
            var method = QueryableObj.GetType().GetMyMethod("Any", 0);
            var reslt = method.Invoke(QueryableObj, new object[] { });
            return Convert.ToBoolean(reslt);
        }

        public object ToTree(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToTree", 4,typeof(string),typeof(string),typeof(object),typeof(string));
            var reslt = method.Invoke(QueryableObj, new object[] {childPropertyName,parentIdPropertyName,rootValue,primaryKeyPropertyName });
            return  reslt;
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
        public async Task<object> InSingleAsync(object pkValue)
        {
            var method = QueryableObj.GetType().GetMyMethod("InSingleAsync", 1);
            var task = (Task)method.Invoke(QueryableObj, new object[] { pkValue });
            return await GetTask(task).ConfigureAwait(false);
        }

        public async Task<object> ToTreeAsync(string childPropertyName, string parentIdPropertyName, object rootValue, string primaryKeyPropertyName)
        {
            var method = QueryableObj.GetType().GetMyMethod("ToTreeAsync", 4, typeof(string), typeof(string), typeof(object), typeof(string));
            var task =(Task)method.Invoke(QueryableObj, new object[] { childPropertyName, parentIdPropertyName, rootValue, primaryKeyPropertyName });
            return await GetTask(task).ConfigureAwait(false);
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
