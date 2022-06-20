using System;
using System.Collections.Generic;
using System.Text;

namespace  SqlSugar
{
    public class SqlObjectResult
    {


        public SqlObjectResult(KeyValuePair<string, List<SugarParameter>> keyValuePair, JsonProviderType jsonSqlType)
        {
            this.Sql = keyValuePair.Key;
            this.Parameters = keyValuePair.Value;
            this.JsonSqlType = jsonSqlType;
        }

        public JsonProviderType JsonSqlType { get; set; }
        public string Sql { get; set; }
        public List<SugarParameter> Parameters { get; set; }    
    }
}
