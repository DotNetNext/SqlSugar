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
        public List<KeyValuePair<string, BinaryExpressionInfo>> BinaryExpressionInfoList { get; set; }
        public object TempDate { get; set; }
        public ExpressionParameter BaseParameter { get; set; }
        public int SwitchCaseNumber { get; set; }
    }
}
