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
        public bool IsSingle { get { return this.Type == ResolveExpressType.Single; } }

        public ExpressionContext(Expression expression, ResolveExpressType type)
        {
            this.Type = type;
            this.Expression = expression;
        }

        public override string ToString()
        {
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { Expression=this.Expression, Context=this });
            resolve.Start();
            return resolve.SqlWhere;
        }

        public string GetSelect() { return ""; }

        public List<DbParameter> Parameters
        {
            get
            {
                return PubMethod.IsNullReturnNew(base._Parameters);
            }
            set
            {
                base._Parameters = value;
            }
        }

        public int Index { get; set; }
    }
}
