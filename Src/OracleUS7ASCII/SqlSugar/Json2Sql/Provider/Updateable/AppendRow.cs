using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {
        private void AppendRow(JToken item)
        {
            var itemFirst = item.First();
            var isObject = itemFirst.Type == JTokenType.Object;
            var value = itemFirst.ToString();
            var dics = context.Utilities.JsonToColumnsModels(value);
            if (isObject)
                sugarUpdateable = this.context.Updateable(dics.First()).AS(this.TableName);
            else
            {
                sugarUpdateable = this.context.Updateable(dics).AS(this.TableName);
                isList = dics.Take(2).Count() > 0;
            }
        }
    }
}
