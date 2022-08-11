using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    /// <summary>
    /// AppendHaving
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private void AppendHaving(JToken item)
        {
            var value = item.First().ToString();
            var obj = context.Utilities.JsonToSqlFuncModels(value);
            sugarQueryable.Having(obj);
        }

    }
}
