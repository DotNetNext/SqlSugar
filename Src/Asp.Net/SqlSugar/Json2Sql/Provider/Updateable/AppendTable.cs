using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {
        private void AppendTable(JToken item)
        {
            var tableInfo = jsonCommonProvider.GetTableName(item);
            this.TableName = tableInfo.TableName.ToCheckField();
            if (tableInfo.ShortName.HasValue())
            {
                this.TableName = tableInfo.ShortName + "." + tableInfo.TableName;
            }
        }
    }
}
