using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class JsonInsertableProvider : IJsonInsertableProvider<JsonInsertResult>
    {
        private ISqlSugarClient context;
        private JObject jObject;
        private JsonCommonProvider jsonCommonProvider;
        private string TableName { get; set; }
        private string IdentityId { get; set; }
        private int Count { get; set; }
        private IInsertable<Dictionary<string, object>> sugarInsertable;

    }
}
