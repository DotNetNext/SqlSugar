using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    /// <summary>
    /// AppendGroupBy
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {

        private void AppendGroupBy(JToken item)
        {
            var value = item.First().ToString();
            var obj = context.Utilities.JsonToGroupByModels(value);
            sugarQueryable.GroupBy(obj);
        }
    }
}
