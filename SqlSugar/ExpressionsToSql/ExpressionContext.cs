using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class ExpressionContext : ExpResolveAccessory
    {
        public ResolveExpressType Type { get; set; }
        public Expression Expression { get; set; }
        public StringBuilder SqlWhere { get; set; }
        public virtual string SqlParameterKeyWord
        {
            get
            {
                return "@";
            }
        }
        public virtual string GetaMppingColumnsName(string name)
        {
            return name;
        }
        public bool IsSingle { get { return this.Type == ResolveExpressType.Single; } }

        public ExpressionContext(Expression expression, ResolveExpressType type)
        {
            this.Type = type;
            this.Expression = expression;
        }

        public string ToSqlString()
        {
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { Expression = this.Expression, Context = this });
            resolve.Start();
            if (this.SqlWhere == null) return string.Empty;
            return this.SqlWhere.ToString();
        }

        public List<SugarParameter> Parameters
        {
            get
            {
                return base._Parameters;
            }
            set
            {
                base._Parameters = value;
            }
        }

        public int Index { get; set; }
    }
}
