using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class EntityInfo
    {
        public string EntityName { get; set; }
        public string DbTableName { get; set; }
        public Type Type { get; set; }
        public List<EntityColumnInfo> Columns { get; set; }
    }
}
