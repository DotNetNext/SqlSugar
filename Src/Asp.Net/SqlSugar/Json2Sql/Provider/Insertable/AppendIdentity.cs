using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public partial class JsonInsertableProvider : IJsonInsertableProvider<JsonInsertResult>
    {
        private void AppendIdentity(JToken item)
        {
            var tableInfo = jsonCommonProvider.GetTableName(item);
            this.IdentityId = tableInfo.TableName;
        }
    }
}
