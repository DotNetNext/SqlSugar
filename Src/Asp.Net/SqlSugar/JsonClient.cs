using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;
namespace SqlSugar
{
    public class JsonClient : IJsonClient
    {
        public ISqlSugarClient Context { get; set; }

        public IJsonQueryableProvider<JsonQueryResult> Queryable(string json)
        {
            var iJsonToSql = new JsonQueryableProvider(Context, JObject.Parse(json));
            return iJsonToSql;
        }
        public IJsonProvider<JsonInsertResult> Insertable(string json)
        {
            var iJsonToSql = new JsonInsertableProvider(Context, JObject.Parse(json));
            return iJsonToSql;
        }
        public IJsonProvider<JsonUpdateResult> Updateable(string json)
        {
            var iJsonToSql = new JsonUpdateableProvider(Context, JObject.Parse(json));
            return iJsonToSql;
        }
        public IJsonProvider<JsonDeleteResult> Deleteable(string json)
        {
            var iJsonToSql = new JsonDeleteableProvider(Context, JObject.Parse(json));
            return iJsonToSql;
        }
        public List<string> GetTableNameList(string json)
        {
            List<string> result = new List<string>();
            var mainTable = JObject.Parse(json).AsJEnumerable().Where(it =>
              it.Path.ToLower().IsIn(
                  JsonProviderConfig.KeyInsertable.Get().ToLower(),
                  JsonProviderConfig.KeyUpdateable.Get().ToLower(),
                  JsonProviderConfig.KeyDeleteable.Get().ToLower(),
                  JsonProviderConfig.KeyQueryable.Get().ToLower()
              )).FirstOrDefault();
            if (mainTable != null)
                result.Add(mainTable.First().ToString());
            return result;
        }
    }
}
