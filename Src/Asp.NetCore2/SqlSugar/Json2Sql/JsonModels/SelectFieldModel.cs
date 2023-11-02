using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SelectModel  
    {
        public object FieldName { get; set; }

        [Obsolete("名字拼错使用FieldName")]
        public object FiledName { get { return FieldName; }  set { FieldName = value; }  }

        public string AsName { get; set; }

        public static List<SelectModel> Create(params SelectModel[] SelectModels) 
        {
            return SelectModels.ToList();
        }
    }
}
