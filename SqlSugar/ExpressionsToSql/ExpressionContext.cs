using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    ///<summary>
    /// ** description：Expression to sql 
    /// ** author：sunkaixuan
    /// ** date：2017/1/14
    /// ** qq:610262374
    public class ExpressionContext : ExpResolveAccessory
    {

        #region properties
        public IDbMethods DbMehtods { get; set; }
        public int Index { get; set; }
        public int ParameterIndex { get; set; }
        public MappingColumnList MappingColumns { get; set; }
        public MappingTableList MappingTables { get; set; }
        public List<JoinQueryInfo> JoinQueryInfos { get; set; }
        public ResolveExpressType ResolveType { get; set; }
        public Expression Expression { get; set; }
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
                if (base._Parameters == null)
                    base._Parameters = new List<SugarParameter>();
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

        #region public functions
        public virtual string GetaMppingColumnsName(string name)
        {
            return name;
        }
        public virtual void Resolve(Expression expression, ResolveExpressType resolveType)
        {
            this.ResolveType = resolveType;
            this.Expression = expression;
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { Expression = this.Expression, Context = this });
            resolve.Start();
        }
        public virtual string GetAsString(string fieldName, string fieldValue)
        {
            return string.Format(" {0} {1} {2} ", fieldValue, "AS", fieldName);
        }
        public virtual void Clear()
        {
            base._Result = null;
        }
        #endregion
    }
}
