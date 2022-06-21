using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public partial class ContextMethods : IContextMethods
    {
        public List<GroupByModel> JsonToGroupByModels(string json)
        {
            List<GroupByModel> conditionalModels = new List<GroupByModel>();
            var jarray = this.Context.Utilities.DeserializeObject<JArray>(json);
            foreach (var item in jarray)
            {
                if (item.ObjToString().ToLower().Contains("fieldname"))
                {
                    var model = item.ToObject<GroupByModel>();
                    conditionalModels.Add(model);
                }
                else
                {
                    conditionalModels.Add(new GroupByModel() { FieldName = item.ObjToString().ToCheckField() });
                }
            }
            return conditionalModels;
        }

    }
}
