using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    /// <summary>
    /// AppendOrderBy
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private void AppendOrderBy(JToken item)
        {
            var value = item.First().ToString();
            var obj = context.Utilities.JsonToOrderByModels(value);
            sugarQueryable.OrderBy(obj);
        }

    }
}
