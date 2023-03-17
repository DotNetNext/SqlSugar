using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public partial class JsonInsertableProvider : IJsonInsertableProvider<JsonInsertResult>
    {
        private void AppendRow(JToken item)
        {
            var value = item.First().ToString();
            var dics = context.Utilities.JsonToColumnsModels(value); 
            sugarInsertable =this.context.Insertable(dics).AS(this.TableName);
        }
    }
}
