using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class GroupByModel 
    {
        public object FieldName { get; set; }
        public static List<GroupByModel> Create(params GroupByModel[] groupModels)
        {
            return groupModels.ToList();
        }
    }
}
