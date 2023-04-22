using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SelectModel  
    {
        public object FiledName { get; set; }
        public string AsName { get; set; }

        public static List<SelectModel> Create(params SelectModel[] SelectModels) 
        {
            return SelectModels.ToList();
        }
    }
}
