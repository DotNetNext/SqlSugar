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

        public override string ToString()
        {
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { Expression = this.Expression, Context = this });
            resolve.Start();
            if (resolve.SqlWhere == null) return string.Empty;
            return resolve.SqlWhere.ToString();
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
