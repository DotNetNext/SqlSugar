using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    /// <summary>
    /// AppendPage 
    /// </summary>
    public partial class JsonQueryableProvider : IJsonQueryableProvider<JsonQueryResult>
    {
        private int AppendPageSize(JToken item)
        {
            return Convert.ToInt32(item.First().ToString().ObjToInt());
        }

        private int AppendPageNumber(JToken item)
        {
            var result = Convert.ToInt32(item.First().ToString().ObjToInt());
            if (result == 0)
            {
                result = 1;
            }
            return result;
        }
    }
}
