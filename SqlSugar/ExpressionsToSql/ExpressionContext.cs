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
        #region constructor
        private ExpressionContext()
        {

        }
        public ExpressionContext(Expression expression, ResolveExpressType resolveType)
        {
            this.ResolveType = resolveType;
            this.Expression = expression;
        }
        #endregion

        #region properties
        public IDbMethods DbMehtods { get; set; }
        public int Index { get; set; }
        public ResolveExpressType ResolveType { get; set; }
        public Expression Expression { get; set; }
        public object ResultObj { get; set; }
        public ExpressionResult Result
        {
            get
            {
                if (base._Result == null)
                {
                    this.Result = new ExpressionResult(this.ResolveType);
                }
                return base._Result;
            }
            set
            {
                this._Result = value;
            }
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
        public virtual string SqlParameterKeyWord
        {
            get
            {
                return "@";
            }
        }
        #endregion

        #region functions
        public virtual string GetaMppingColumnsName(string name)
        {
            return name;
        }
        #endregion
    }
}
