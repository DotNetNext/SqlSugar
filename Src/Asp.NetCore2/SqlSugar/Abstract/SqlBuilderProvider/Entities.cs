using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class ReSetValueBySqlExpListModel
    {
        public string DbColumnName { get; set; }
        public string Sql { get; set; }
        public ReSetValueBySqlExpListModelType? Type { get; set; }
    }
    internal enum ReSetValueBySqlExpListModelType 
    {
        Default,
        List
    }
}
