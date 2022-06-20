using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
    public partial class JsonUpdateableProvider : IJsonUpdateableProvider<JsonUpdateResult>
    {

        private static bool IsColumns(string name)
        {
            return name == "Columns".ToLower();
        }

        private static bool IsWhere(string name)
        {
            return name == "Where".ToLower();
        }

        private static bool IsWhereColumns(string name)
        {
            return name == "WhereColumns".ToLower();
        }

        private static bool IsTable(string name)
        {
            return name == JsonProviderConfig.KeyUpdateable.Get().ToLower();
        } 
    }
}
