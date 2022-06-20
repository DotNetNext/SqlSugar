using System;
using System.Collections.Generic;
using System.Text;

namespace SqlSugar
{
 
    public class JsonTableConfig
    {
        public string TableName { get; set; }
        public string TableDescription { get; set; }
        public List<IConditionalModel> Conditionals { get; set; }
        public bool? AllowQuery { get; set; }
        public bool? AllowUpdate { get; set; }
        public bool? AllowDelete { get; set; }
        public bool? AllowInsert { get; set; }
        public List<JsonColumnConfig>  Columns { get; set; } 
    }
    public class JsonColumnConfig
    {
        public string Name { get; set; }
        public string  Description { get; set; }
        public string ValidateMessage { get; set; }
        public object Validate { get; set; }
        bool? AllowEdit { get; set; }
    }    
}
