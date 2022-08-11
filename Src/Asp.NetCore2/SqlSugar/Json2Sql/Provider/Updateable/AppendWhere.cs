using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {
        private void AppendWhere(JToken item)
        {
            Check.Exception(isList, "Batch updates cannot use Where, only WhereColumns can set columns", "批量更新不能使用Where，只能通过WhereColumns设置列");
            var sqlObj = jsonCommonProvider.GetWhere(item, sugarUpdateable.UpdateBuilder.Context);
            sugarUpdateable.Where(sqlObj.Key, sqlObj.Value);
        }

    }
}
