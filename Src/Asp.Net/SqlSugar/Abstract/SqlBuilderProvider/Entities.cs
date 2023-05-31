using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class ReSetValueBySqlExpListModel
    {
        public string DbColumnName { get; set; }
        public string Sql { get; set; }
        public ReSetValueBySqlExpListModelType? Type { get; set; }
    }
    public enum ReSetValueBySqlExpListModelType 
    {
        Default,
        List
    }
}
