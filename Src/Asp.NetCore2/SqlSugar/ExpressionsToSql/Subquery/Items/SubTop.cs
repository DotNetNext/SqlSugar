using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SqlSugar
{
    public class SubTop : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public Expression Expression
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "Top";
            }
        }

        public int Sort
        {
            get
            {
                if (this.Context is SqlServerExpressionContext)
                {
                    return 150;
                }
                else if (this.Context is OracleExpressionContext) {

                    return 401;
                }
                else
                {
                    return 490;
                }
            }
        }


        public string GetValue(Expression expression)
        {
            if (this.Context is SqlServerExpressionContext)
            {
                return "TOP 1";
            }
            else if (this.Context is OracleExpressionContext)
            {
                return (HasWhere?"AND":"WHERE")+ " ROWNUM=1";
            }
            else if (this.Context is PostgreSQLExpressionContext)
            {
                return "limit 1";
            }
            else
            {
                return "limit 0,1";
            }
        }
    }
}
