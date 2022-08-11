using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {
        private ISqlSugarClient context;
        private JObject jObject;
        private JsonCommonProvider jsonCommonProvider;
        private string TableName { get; set; }
        private bool isList { get; set; }
        private IUpdateable<Dictionary<string, object>> sugarUpdateable;
        public JsonUpdateableProvider(ISqlSugarClient context, JObject jObject)
        {
            this.jObject = jObject;
            this.context = context;
            this.jsonCommonProvider = new JsonCommonProvider(context);
        }
        public JsonUpdateResult ToResult()
        {
            var result = new JsonUpdateResult();
            var sqlInfo = this.ToSqlList();
            var sqlInfoResult = sqlInfo.First();
            result.UpdateRows = this.context.Ado.ExecuteCommand(sqlInfoResult.Sql, sqlInfoResult.Parameters);
            return result;
        }
        public SqlObjectResult ToSql()
        {
            return this.ToSqlList().First();
        }
        public List<SqlObjectResult> ToSqlList()
        {
            List<SqlObjectResult> result = new List<SqlObjectResult>();
            JsonQueryParameter jsonQueryParameter = new JsonQueryParameter();
            List<JToken> appendTypeNames = GetAppendTypes();
            foreach (JToken item in appendTypeNames)
            {
                AppendAll(jsonQueryParameter, item);
            }
            var addItem = this.sugarUpdateable.ToSql();
            result.Add(new SqlObjectResult(addItem,JsonProviderType.Updateable));
            return result;
        }

        private List<JToken> GetAppendTypes()
        {
            var appendTypeNames = this.jObject.AsJEnumerable().ToList();
            appendTypeNames = appendTypeNames.OrderBy(it => 
            {
                if (it.Path.EqualCase(JsonProviderConfig.KeyUpdateable.Get())) return 0;
                if (it.Path.EqualCase("Columns")) return 1;
                else return 3;

           } ).ToList();
            return appendTypeNames;
        }

        private void AppendAll(JsonQueryParameter jsonQueryParameter, JToken item)
        {
            var name = item.Path.ToLower();
            if (IsTable(name))
            {
                AppendTable(item);
            }
            else if (IsWhereColumns(name))
            {
                AppendWhereColumns(item);
            }
            else if (IsWhere(name))
            {
                AppendWhere(item);
            }
            else if (IsColumns(name))
            {
                AppendRow(item);
            } 
        }
        public List<string> ToSqlString()
        {
            throw new NotImplementedException();
        } 
    }
}
