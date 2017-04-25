using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public abstract class LambadaQueryBuilder : IDMLBuilder
    {

        public LambadaQueryBuilder()
        {
            this.QueryPars = new List<SugarParameter>();
        }

        #region Private Fileds
        private List<JoinQueryInfo> _JoinQueryInfos;
        private List<string> _WhereInfos;
        private string _TableNameString;
        #endregion

        #region Service object
        public StringBuilder Sql { get; set; }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        #endregion

        #region Splicing basic
        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string OrderByValue { get; set; }
        public object SelectValue { get; set; }
        public string SelectCacheKey { get; set; }
        public string EntityName { get; set; }
        public string TableWithString { get; set; }
        public string GroupByValue { get; set; }
        public int WhereIndex { get; set; }
        public int JoinIndex { get; set; }
        public virtual List<SugarParameter> QueryPars { get; set; }
        public virtual List<JoinQueryInfo> JoinQueryInfos
        {
            get
            {
                _JoinQueryInfos = PubMethod.IsNullReturnNew(_JoinQueryInfos);
                return _JoinQueryInfos;
            }
            set { _JoinQueryInfos = value; }
        }
        public virtual string TableShortName { get; set; }
        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = PubMethod.IsNullReturnNew(_WhereInfos);
                return _WhereInfos;
            }
            set { _WhereInfos = value; }
        }
        #endregion

        #region Lambada Type
        public ResolveExpressType SelectType
        {
            get
            {
                return this.IsSingle() ? ResolveExpressType.SelectSingle : ResolveExpressType.SelectMultiple;
            }
        }
        public ResolveExpressType WheretType
        {
            get
            {
                return this.IsSingle() ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
            }
        }
        #endregion

        #region Sql Template
        public virtual string SqlTemplate
        {
            get
            {
                return "SELECT {0} FROM {1} {2}";
            }
        }
        public virtual string JoinTemplate
        {
            get
            {
                return " {0} JOIN {1} {2} ON {3} ";
            }
        }
        #endregion

        #region Common Methods
        public virtual bool IsSingle()
        {
            var isSingle = Builder.LambadaQueryBuilder.JoinQueryInfos.IsNullOrEmpty();
            return isSingle;
        }
        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.JoinQueryInfos = Builder.LambadaQueryBuilder.JoinQueryInfos;
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreComumns;
            resolveExpress.Resolve(expression, resolveType);
            this.QueryPars = new List<SugarParameter>();
            this.QueryPars.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }
        public virtual string ToSqlString()
        {
            Sql = new StringBuilder();
            var tableString = GetTableNameString;
            if (this.JoinQueryInfos.IsValuable())
            {
                tableString = tableString + " " + GetJoinValueString;
            }
            Sql.AppendFormat(SqlTemplate, GetSelectValue, tableString, GetWhereValueString);
            return Sql.ToString();
        }
        public virtual string ToJoinString(JoinQueryInfo joinInfo)
        {
            return string.Format(
                this.JoinTemplate,
                joinInfo.JoinType.ToString(),
                joinInfo.TableName,
                joinInfo.ShortName + " " + joinInfo.TableWithString,
                joinInfo.JoinWhere);
        }
        public virtual void Clear()
        {
            this.Skip = 0;
            this.Take = 0;
            this.Sql = null;
            this.WhereIndex = 0;
            this.QueryPars = null;
            this.GroupByValue = null;
            this._TableNameString = null;
            this.WhereInfos = null;
            this.JoinQueryInfos = null;
        }
        #endregion

        #region Get SQL Partial
        public virtual string GetSelectValue
        {
            get
            {
                string reval = string.Empty;
                if (this.SelectValue == null || this.SelectValue is string)
                {
                    reval = GetSelectValueByString();
                }
                else
                {
                    reval = GetSelectValueByExpression();
                }
                if (this.SelectType == ResolveExpressType.SelectMultiple)
                {
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this._JoinQueryInfos.Select(it => it.TableName));
                }
                return reval;
            }
        }
        public virtual string GetSelectValueByExpression()
        {
            var expression = this.SelectValue as Expression;
            var reval = GetExpressionValue(expression, this.SelectType).GetResultString();
            this.SelectCacheKey = reval;
            return reval;
        }
        public virtual string GetSelectValueByString()
        {
            string reval;
            if (this.SelectValue.IsNullOrEmpty())
            {
                string pre = null;
                if (this.JoinQueryInfos.IsValuable() && this.JoinQueryInfos.Any(it => TableShortName.IsValuable()))
                {
                    pre = Builder.GetTranslationColumnName(TableShortName) + ".";
                }
                reval = string.Join(",", this.Context.Database.DbMaintenance.GetColumnInfosByTableName(this.EntityName).Select(it => pre + Builder.GetTranslationColumnName(it.ColumnName)));
            }
            else
            {
                reval = this.SelectValue.ObjToString();
                this.SelectCacheKey = reval;
            }

            return reval;
        }
        public virtual string GetWhereValueString
        {
            get
            {
                if (this.WhereInfos == null) return null;
                else
                {
                    return string.Join(" ", this.WhereInfos);
                }
            }
        }
        public virtual string GetJoinValueString
        {
            get
            {
                if (this.JoinQueryInfos.IsNullOrEmpty()) return null;
                else
                {
                    return string.Join(" ", this.JoinQueryInfos.Select(it => this.ToJoinString(it)));
                }
            }
        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityName) + TableWithString;
                if (this.TableShortName.IsValuable())
                {
                    result += " " + TableShortName;
                }
                return result;
            }
        }
        #endregion
    }
}
