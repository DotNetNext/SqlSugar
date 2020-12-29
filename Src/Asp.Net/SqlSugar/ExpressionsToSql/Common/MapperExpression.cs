using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class MapperExpression
    {
        public MapperExpressionType Type { get; set; }
        public Expression FillExpression { get; set; }
        public Expression MappingField1Expression { get; set; }
        public Expression MappingField2Expression { get; set; }
        public SqlSugarProvider Context { get; set; }
        public QueryBuilder QueryBuilder { get;  set; }
        public ISqlBuilder SqlBuilder { get;  set; }
    }

    public enum MapperExpressionType
    {
        oneToOne=1,
        oneToN=2
    }
}
