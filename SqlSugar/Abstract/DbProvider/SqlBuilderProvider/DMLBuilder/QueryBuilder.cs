using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public abstract class QueryBuilder : IDMLBuilder
    {

        public QueryBuilder()
        {
            this.QueryPars = new List<SugarParameter>();
        }

        #region Private Fileds
        private List<JoinQueryInfo> _JoinQueryInfos;
        private List<string> _WhereInfos;
        private string _HavingInfos;
        private string _TableNameString;
        #endregion

        #region Service object
        public StringBuilder sql { get; set; }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        #endregion

        #region Splicing basic
        public bool IsCount { get; set; }
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
        public virtual string HavingInfos
        {
            get
            {
                return _HavingInfos;
            }
            set
            {
                _HavingInfos = value;
            }
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
                return "SELECT {0} FROM {1}{2}{3}{4} ";
            }
        }
        public virtual string JoinTemplate
        {
            get
            {
                return "{0}JOIN {1}{2}ON {3} ";
            }
        }
        public virtual string PageTempalte
        {
            get
            {
                return @"WITH PageTable AS(
                          {0}
                  )
                  SELECT * FROM (SELECT *,ROW_NUMBER() OVER({1}) AS RowIndex FROM PageTable ) T WHERE RowIndex BETWEEN {2} AND {3}";
            }
        }
        public virtual string DefaultOrderByTemplate
        {
            get
            {
                return "ORDER BY GETDATE() ";
            }
        }
        public virtual string OrderByTemplate
        {
            get
            {
                return "ORDER BY ";
            }
        }
        public virtual string GroupByTemplate
        {
            get
            {
                return "GROUP BY ";
            }
        }

        public virtual string MaxTemplate
        {
            get
            {
                return "MAX({0})";
            }
        }
        public virtual string MinTemplate
        {
            get
            {
                return "MIN({0})";
            }
        }
        public virtual string SumTemplate
        {
            get
            {
                return "SUM({0})";
            }
        }
        public virtual string AvgTemplate
        {
            get
            {
                return "AVG({0})";
            }
        }
        public virtual string InTemplate
        {
            get
            {
                return "{0} IN ({1}) ";
            }
        }
        #endregion

        #region Common Methods
        public virtual bool IsSingle()
        {
            var isSingle = Builder.QueryBuilder.JoinQueryInfos.IsNullOrEmpty();
            return isSingle;
        }
        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.JoinQueryInfos = Builder.QueryBuilder.JoinQueryInfos;
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
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString+HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                return string.Format(PageTempalte, sql.ToString(), GetOrderByString, Skip.ObjToInt() + 1, long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                return string.Format(PageTempalte, sql.ToString(), GetOrderByString, 1, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                return string.Format(PageTempalte, sql.ToString(), GetOrderByString, Skip.ObjToInt() + 1, Take);
            }
            else
            {
                return sql.ToString();
            }

        }
        public virtual string ToJoinString(JoinQueryInfo joinInfo)
        {
            return string.Format(
                this.JoinTemplate,
                joinInfo.JoinType.ToString() + PubConst.Space,
                joinInfo.TableName + PubConst.Space,
                joinInfo.ShortName + PubConst.Space + joinInfo.TableWithString,
                joinInfo.JoinWhere);
        }
        public virtual void Clear()
        {
            this.Skip = 0;
            this.Take = 0;
            this.sql = null;
            this.WhereIndex = 0;
            this.QueryPars = null;
            this.GroupByValue = null;
            this._TableNameString = null;
            this.WhereInfos = null;
            this.JoinQueryInfos = null;
        }
        public virtual bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS \[\w+\.\w+\]");
        }
        #endregion

        #region Get SQL Partial
        public virtual string GetSelectValue
        {
            get
            {
                if (this.IsCount) return "COUNT(1) AS [Count] ";
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
                    return string.Join(PubConst.Space, this.WhereInfos);
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
                    return string.Join(PubConst.Space, this.JoinQueryInfos.Select(it => this.ToJoinString(it)));
                }
            }
        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                if (this.TableShortName.IsValuable())
                {
                    result += (TableShortName + PubConst.Space);
                }
                if (!this.IsSingle())
                {
                    result += GetJoinValueString + PubConst.Space;
                }
                return result;
            }
        }
        public virtual string GetOrderByString
        {
            get
            {
                return this.OrderByValue;
            }
        }
        public virtual string GetGroupByString
        {
            get
            {
                return this.GroupByValue;
            }
        }
        #endregion
    }
}
