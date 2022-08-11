using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public partial class ContextMethods : IContextMethods
    {
        public List<Dictionary<string, object>> JsonToColumnsModels(string json)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            List<SelectModel> conditionalModels = new List<SelectModel>();
            if (IsArray(json))
            {
                return GetColumnsByArray(json);
            }
            else 
            {
                return GetColumnsByObject(json);
            }
        }
        private List<Dictionary<string, object>> GetColumnsByObject(string json)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var dic = this.Context.Utilities.DeserializeObject<Dictionary<string,object>>(json);
            result.Add( GetColumns(dic));
            return result;
        }
        private List<Dictionary<string, object>> GetColumnsByArray(string json)
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var jarray = this.Context.Utilities.DeserializeObject<List<Dictionary<string,object>>>(json);
            foreach (var item in jarray)
            {
                result.Add(GetColumns(item));
            }
            return result;
        }

        private Dictionary<string, object> GetColumns(Dictionary<string, object> dictionary)
        {
            Dictionary<string, object> result= new Dictionary<string, object>();
            foreach (var item in dictionary)
            {
                var value = GetValue(item);
                result.Add(item.Key, value);
            }
            return result;
        }

        private static object GetValue(KeyValuePair<string, object> item)
        {
            if (item.Value == null)
                return null;
            var valueString = item.Value.ToString();
            var vallue = Json2SqlHelper.GetValue(valueString);
            var type = Json2SqlHelper.GetType(valueString);
            return UtilMethods.ConvertDataByTypeName(type,vallue);
        }
    }
}
