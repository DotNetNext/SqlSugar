using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class JsonInsertableProvider : IJsonInsertableProvider<JsonInsertResult>
    {
        private static bool IsColumns(string name)
        {
            return name == "columns".ToLower();
        }

        private static bool IsName(string name)
        {
            return name == JsonProviderConfig.KeyInsertable.Get().ToLower();
        }

        private static bool IsIdentity(string name)
        {
            return name == "identity".ToLower();
        }
        private List<SqlObjectResult> ToSqlHelper()
        {
            List<SqlObjectResult> result = new List<SqlObjectResult>();
            JsonQueryParameter jsonQueryParameter = new JsonQueryParameter();
            var appendTypeNames = this.jObject.AsJEnumerable().ToList();
            foreach (JToken item in appendTypeNames.OrderBy(it => it.Path.EqualCase(JsonProviderConfig.KeyInsertable.Get()) ? 0 : 1))
            {
                AppendAll(jsonQueryParameter, item);
            }
            var addItem = this.sugarInsertable.ToSql();
            if (this.IdentityId.HasValue())
            {
                result.Add(new SqlObjectResult(addItem, JsonProviderType.InsertableIdentity));
            }
            else
            {
                result.Add(new SqlObjectResult(addItem, JsonProviderType.Insertable));
            }
            return result;
        }
    }
}
