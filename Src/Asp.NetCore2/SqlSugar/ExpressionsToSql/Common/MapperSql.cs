using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class MapperSql
    {
        public string Sql { get; set; }
    }

    public class MapperExpressionInfo
    {
        public Type Type { get; set; }
        public EntityInfo EntityInfo { get; set; }
        public string FieldName { get; set; }
        public string FieldString { get; set; }
    }
}
