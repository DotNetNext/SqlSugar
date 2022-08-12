using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class JsonInsertableProvider : IJsonInsertableProvider<JsonInsertResult>
    {
        public SqlObjectResult ToSql()
        {
            return this.ToSqlList().First();
        }
        public JsonInsertableProvider(ISqlSugarClient context, JObject jObject)
        {
            this.jObject = jObject;
            this.context = context;
            this.jsonCommonProvider = new JsonCommonProvider(context);
        }
        public JsonInsertResult ToResult()
        {
            var result=new JsonInsertResult();   
            var sqlInfo = this.ToSqlList();
            var sqlInfoResult = sqlInfo.First();
            if (sqlInfoResult.JsonSqlType != JsonProviderType.InsertableIdentity)
            {
                result.InsertCount = this.context.Ado.ExecuteCommand(sqlInfoResult.Sql,sqlInfoResult.Parameters);
            }
            else
            {
                result.InsertCount = this.Count;
                result.IdentityValue = this.context.Ado.GetInt(sqlInfoResult.Sql, sqlInfoResult.Parameters);
            }
            return result;
        }
        public List<SqlObjectResult> ToSqlList()
        {
            return ToSqlHelper();
        }

        private  void AppendAll(JsonQueryParameter jsonQueryParameter, JToken item)
        {
            var name = item.Path.ToLower();
            if (IsName(name))
            {
                AppendName(item);
            }
            else if (IsIdentity(name))
            {
                AppendIdentity(item);
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
