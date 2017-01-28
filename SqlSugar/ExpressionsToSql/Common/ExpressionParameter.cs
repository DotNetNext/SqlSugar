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
        public ExpressionContext Context { get; set; }
        public ExpressionParameter BaseParameter { get; set; }
        public Expression BaseExpression { get; set; }
        public Expression LeftExpression { get; set; }
        public Expression RightExpression { get; set; }
        public Expression Expression { get; set; }
        public bool? IsLeft { get; set; }
        public int Index { get; set; }
        public object CommonTempData { get; set; }
        public ExpressionResultAppendType AppendType { get; set; }
        public void IsAppendResult()
        {
            this.AppendType = ExpressionResultAppendType.AppendResult;
        }
        public void IsAppendTempDate()
        {
            this.AppendType = ExpressionResultAppendType.AppendTempDate;
        }
    }
}
