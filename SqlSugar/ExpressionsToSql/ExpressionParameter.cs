using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public class ExpressionParameter
    {
        public Expression Expression { get; set; }
        public ExpressionContext Context { get; set; }
        public bool? IsLeft { get; set; }
        public Expression BaseExpression { get; set; }
        public int Index { get; set; }
        public Dictionary<string, object> TempData { get; set; }
        public ExpressionParameter BaseParameter { get; set; }
    }
}
