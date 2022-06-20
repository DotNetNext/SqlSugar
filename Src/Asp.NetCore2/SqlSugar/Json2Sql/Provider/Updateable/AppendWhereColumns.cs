using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {
        private void AppendWhereColumns(JToken item)
        {
            var columns = item.First().ToObject<string[]>();
            Check.ExceptionEasy(columns.IsNullOrEmpty(), "need WhereColumns", "WhereColumns 需要设置列名");
            this.sugarUpdateable.WhereColumns(columns);
        }

    }
}
