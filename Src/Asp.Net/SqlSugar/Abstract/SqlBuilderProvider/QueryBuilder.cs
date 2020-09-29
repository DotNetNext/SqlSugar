using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
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
            this.Parameters = new List<SugarParameter>();
        }

        #region Private Fileds
        protected List<JoinQueryInfo> _JoinQueryInfos;
        protected Dictionary<string, string> _EasyJoinInfos;
        private List<string> _WhereInfos;
        private string _HavingInfos;
        protected string _TableNameString;
        #endregion

        #region Service object
        public StringBuilder sql { get; set; }
        public SqlSugarProvider Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        #endregion

        #region Splicing basic
        public List<string> IgnoreColumns { get; set; }
        public bool IsCount { get; set; }
        public int? Skip { get; set; }
        public int ExternalPageIndex { get; set; }
        public int ExternalPageSize { get; set; }
        public int? Take { get; set; }
        public string OrderByValue { get; set; }
        public object SelectValue { get; set; }
        public string SelectCacheKey { get; set; }
        public string EntityName { get; set; }


        public Type EntityType { get; set; }
        public string TableWithString { get; set; }
        public string GroupByValue { get; set; }
        public string PartitionByValue { get; set; }
        public int WhereIndex { get; set; }
        public bool IsDistinct { get; set; }
        public int JoinIndex { get; set; }
        public bool IsDisabledGobalFilter { get; set; }
        public virtual List<SugarParameter> Parameters { get; set; }
        public Expression JoinExpression { get; set; }
        public Dictionary<string, string> EasyJoinInfos
        {
            get
            {
                _EasyJoinInfos = UtilMethods.IsNullReturnNew(_EasyJoinInfos);
                return _EasyJoinInfos;
            }
            set { _EasyJoinInfos = value; }
        }
        public virtual List<JoinQueryInfo> JoinQueryInfos
        {
            get
            {
                _JoinQueryInfos = UtilMethods.IsNullReturnNew(_JoinQueryInfos);
                return _JoinQueryInfos;
            }
            set { _JoinQueryInfos = value; }
        }
        public virtual string TableShortName { get; set; }
        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = UtilMethods.IsNullReturnNew(_WhereInfos);
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
                return @"SELECT * FROM ({0}) T WHERE RowIndex BETWEEN {1} AND {2}";
            }
        }
        public virtual string ExternalPageTempalte
        {
            get
            {
                return @"SELECT * FROM ({0}) T WHERE RowIndex2 BETWEEN {1} AND {2}";
            }
        }
        public virtual string DefaultOrderByTemplate
        {
            get
            {
                return "ORDER BY "+this.Builder.SqlDateNow+" ";
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
        public virtual string PartitionByTemplate
        {
            get
            {
                return "PARTITION BY ";
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
            var isSingle = Builder.QueryBuilder.JoinQueryInfos.IsNullOrEmpty() && !EasyJoinInfos.Any();
            return isSingle;
        }
        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            if (resolveType.IsIn(ResolveExpressType.FieldSingle,ResolveExpressType.FieldMultiple,ResolveExpressType.SelectSingle, ResolveExpressType.SelectMultiple) &&(expression is LambdaExpression)&& (expression as LambdaExpression).Body is BinaryExpression) {
                resolveType = resolveType.IsIn(ResolveExpressType.SelectSingle, ResolveExpressType.FieldSingle) ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
            }
            this.LambdaExpressions.Clear();
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                resolveExpress.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                resolveExpress.PgSqlIsAutoToLower = true;
            }
            resolveExpress.JoinQueryInfos = Builder.QueryBuilder.JoinQueryInfos;
            resolveExpress.IsSingle = IsSingle();
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            resolveExpress.InitMappingInfo = this.Context.InitMappingInfo;
            resolveExpress.RefreshMapping = () =>
            {
                resolveExpress.MappingColumns = Context.MappingColumns;
                resolveExpress.MappingTables = Context.MappingTables;
                resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
                resolveExpress.SqlFuncServices = Context.CurrentConnectionConfig.ConfigureExternalServices == null ? null : Context.CurrentConnectionConfig.ConfigureExternalServices.SqlFuncServices;
            };
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters.AddRange(resolveExpress.Parameters);
            var result = resolveExpress.Result;
            var isSingleTableHasSubquery = IsSingle() && resolveExpress.SingleTableNameSubqueryShortName.HasValue();
            if (isSingleTableHasSubquery) {
                Check.Exception(!string.IsNullOrEmpty(this.TableShortName) && resolveExpress.SingleTableNameSubqueryShortName != this.TableShortName, "{0} and {1} need same name");
                this.TableShortName = resolveExpress.SingleTableNameSubqueryShortName;
            }
            return result;
        }
        public virtual string ToSqlString()
        {
            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount&&this.PartitionByValue.IsNullOrEmpty();
            AppendFilter();
            sql = new StringBuilder();
            if (this.OrderByValue == null && (Skip != null || Take != null)) this.OrderByValue = " ORDER BY GetDate() ";
            if (this.PartitionByValue.HasValue())
            {
                this.OrderByValue = this.PartitionByValue + this.OrderByValue;
            }
            var isRowNumber = Skip != null || Take != null;
            var rowNumberString = string.Format(",ROW_NUMBER() OVER({0}) AS RowIndex ", GetOrderByString);
            string groupByValue = GetGroupByString + HavingInfos;
            string orderByValue = (!isRowNumber && this.OrderByValue.HasValue()) ? GetOrderByString : null;
            if (isIgnoreOrderBy) { orderByValue = null; }
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, groupByValue, orderByValue);
            sql.Replace(UtilConstants.ReplaceKey, isRowNumber ? (isIgnoreOrderBy ? null : rowNumberString) : null);
            if (isIgnoreOrderBy) { this.OrderByValue = oldOrderBy; return sql.ToString();  }
            var result = ToPageSql(sql.ToString(), this.Take, this.Skip);
            if (ExternalPageIndex > 0)
            {
                if (externalOrderBy.IsNullOrEmpty())
                {
                    externalOrderBy = " ORDER BY GetDate() ";
                }
                result = string.Format("SELECT *,ROW_NUMBER() OVER({0}) AS RowIndex2 FROM ({1}) ExternalTable ", GetExternalOrderBy(externalOrderBy), result);
                result = ToPageSql2(result, ExternalPageIndex, ExternalPageSize, true);
            }
            this.OrderByValue = oldOrderBy;
            return result;
        }

        public virtual void AppendFilter()
        {
            if (!IsDisabledGobalFilter && this.Context.QueryFilter.GeFilterList.HasValue())
            {
                var gobalFilterList = this.Context.QueryFilter.GeFilterList.Where(it => it.FilterName.IsNullOrEmpty()).ToList();
                foreach (var item in gobalFilterList.Where(it => it.IsJoinQuery == !IsSingle()))
                {
                    var filterResult = item.FilterValue(this.Context);
                    WhereInfos.Add(this.Builder.AppendWhereOrAnd(this.WhereInfos.IsNullOrEmpty(), filterResult.Sql+UtilConstants.Space));
                    var filterParamters = this.Context.Ado.GetParameters(filterResult.Parameters);
                    if (filterParamters.HasValue())
                    {
                        this.Parameters.AddRange(filterParamters);
                    }
                }
            }
        }

        public virtual string GetExternalOrderBy(string externalOrderBy)
        {
            return Regex.Replace(externalOrderBy, @"\[\w+\]\.", "");
        }

        public virtual string ToCountSql(string sql)
        {

            return string.Format(" SELECT COUNT(1) FROM ({0}) CountTable ", sql);
        }

        public virtual string ToPageSql(string sql, int? take, int? skip, bool isExternal = false)
        {
            string temp = isExternal ? ExternalPageTempalte : PageTempalte;
            if (skip != null && take == null)
            {
                return string.Format(temp, sql.ToString(), skip.ObjToInt() + 1, long.MaxValue);
            }
            else if (skip == null && take != null)
            {
                return string.Format(temp, sql.ToString(), 1, take.ObjToInt());
            }
            else if (skip != null && take != null)
            {
                return string.Format(temp, sql.ToString(), skip.ObjToInt() + 1, skip.ObjToInt() + take.ObjToInt());
            }
            else
            {
                return sql.ToString();
            }
        }

        public virtual string ToPageSql2(string sql, int? pageIndex, int? pageSize, bool isExternal = false)
        {
            string temp = isExternal ? ExternalPageTempalte : PageTempalte;
            return string.Format(temp, sql.ToString(), (pageIndex - 1) * pageSize+1, pageIndex * pageSize);
        }

        public virtual string GetSelectByItems(List<KeyValuePair<string, object>> items)
        {
            var array = items.Select(it => {
                dynamic dynamicObj = this.Context.Utilities.DeserializeObject<dynamic>(this.Context.Utilities.SerializeObject(it.Value));
                var dbName =Builder.GetTranslationColumnName( (string)(dynamicObj.dbName));
                var asName = Builder.GetTranslationColumnName((string)(dynamicObj.asName));
                return string.Format("{0}.{1} AS {2}",it.Key,dbName,asName);
            });
            return  string.Join(",",array);
        }

        public virtual string ToJoinString(JoinQueryInfo joinInfo)
        {
            return string.Format(
                this.JoinTemplate,
                joinInfo.JoinType.ToString() + UtilConstants.Space,
                Builder.GetTranslationTableName(joinInfo.TableName) + UtilConstants.Space,
                joinInfo.ShortName + UtilConstants.Space + TableWithString,
                joinInfo.JoinWhere);
        }
        public virtual void Clear()
        {
            this.Skip = 0;
            this.Take = 0;
            this.sql = null;
            this.WhereIndex = 0;
            this.Parameters = null;
            this.GroupByValue = null;
            this._TableNameString = null;
            this.WhereInfos = null;
            this.JoinQueryInfos = null;
            this.IsDistinct = false;
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
                string result = string.Empty;
                if (this.SelectValue == null || this.SelectValue is string)
                {
                    result = GetSelectValueByString();
                }
                else
                {
                    result = GetSelectValueByExpression();
                }
                if (this.SelectType == ResolveExpressType.SelectMultiple)
                {
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this._JoinQueryInfos.Select(it => it.TableName));
                }
                if (IsDistinct)
                {
                    result = " DISTINCT " + result;
                }
                return result;
            }
        }
        public virtual string GetSelectValueByExpression()
        {
            var expression = this.SelectValue as Expression;
            var result = GetExpressionValue(expression, this.SelectType).GetResultString();
            if (result == null)
            {
                return "*";
            }
            if (result.Contains(".*") && this.IsSingle())
            {
                return "*";
            }
            else
            {
                if (expression is LambdaExpression && (expression as LambdaExpression).Body is MethodCallExpression&&this.Context.CurrentConnectionConfig.DbType==DbType.SqlServer&&this.OrderByValue.HasValue())
                {
                    result = result + " AS columnName";
                }
                this.SelectCacheKey = result;
                return result;
            }
        }
        public virtual string GetSelectValueByString()
        {
            string result;
            if (this.SelectValue.IsNullOrEmpty())
            {
                string pre = null;
                if (this.JoinQueryInfos.HasValue() && this.JoinQueryInfos.Any(it => TableShortName.HasValue()))
                {
                    pre = Builder.GetTranslationColumnName(TableShortName) + ".";
                }
                var columns = this.Context.EntityMaintenance.GetEntityInfo(this.EntityType).Columns.Where(it => !it.IsIgnore);
                if (this.IgnoreColumns.HasValue())
                {
                    columns = columns.Where(c => !this.IgnoreColumns.Any(i=>c.PropertyName.Equals(i,StringComparison.CurrentCultureIgnoreCase)||c.DbColumnName.Equals(i,StringComparison.CurrentCultureIgnoreCase))).ToList();
                }
                result = string.Join(",", columns.Select(it => pre + Builder.GetTranslationColumnName(it.EntityName, it.PropertyName)));
            }
            else
            {
                result = this.SelectValue.ObjToString();
                this.SelectCacheKey = result;
            }
            if (result.IsNullOrEmpty())
            {
                result = "*";
            }
            return result;
        }
        public virtual string GetWhereValueString
        {
            get
            {
                if (this.WhereInfos == null) return null;
                else
                {
                    return string.Join(UtilConstants.Space, this.WhereInfos);
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
                    return string.Join(UtilConstants.Space, this.JoinQueryInfos.Select(it => this.ToJoinString(it)));
                }
            }
        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityName);
                result += UtilConstants.Space;
                if (this.TableShortName.HasValue())
                {
                    result += (TableShortName + UtilConstants.Space);
                }
                if (this.TableWithString.HasValue()&&this.TableWithString!= SqlWith.Null)
                {
                    result += TableWithString + UtilConstants.Space;
                }
                if (!this.IsSingle())
                {
                    result += GetJoinValueString + UtilConstants.Space;
                }
                if (this.EasyJoinInfos.IsValuable())
                {

                    if (this.TableWithString.HasValue() && this.TableWithString != SqlWith.Null)
                    {
                        result += "," + string.Join(",", this.EasyJoinInfos.Select(it => string.Format("{0} {1} {2} ", GetTableName(it.Value), it.Key, TableWithString)));
                    }
                    else
                    {
                        result += "," + string.Join(",", this.EasyJoinInfos.Select(it => string.Format("{0} {1} ", GetTableName(it.Value), it.Key)));
                    }
                }
                return result;
            }
        }
        public virtual string GetOrderByString
        {
            get
            {
                if (this.OrderByValue == null) return null;
                if (IsCount&&this.PartitionByValue.IsNullOrEmpty()) return null;
                else
                {
                    return this.OrderByValue;
                }
            }
        }
        public virtual string GetGroupByString
        {
            get
            {
                if (this.GroupByValue == null) return null;
                if (this.GroupByValue.Last() != ' ' )
                {
                    return this.GroupByValue + UtilConstants.Space;
                }
                return this.GroupByValue;
            }
        }

        #endregion

        private string GetTableName(string entityName)
        {
            var result = this.Context.EntityMaintenance.GetTableName(entityName);
            return this.Builder.GetTranslationTableName(result);
        }

        public void CheckExpression(Expression expression, string methodName)
        {
            if (IsSingle() == false&& this.JoinExpression!=null)
            {
                var jsoinParameters = (this.JoinExpression as LambdaExpression).Parameters;
                var currentParametres = (expression as LambdaExpression).Parameters;
                if ((expression as LambdaExpression).Body.ToString() == "True") {
                    return;
                }
                if (currentParametres != null && currentParametres.Count > 0)
                {
                    foreach (var item in currentParametres)
                    {
                        var index = currentParametres.IndexOf(item);
                        var name = item.Name;
                        var joinName = jsoinParameters[index].Name;
                        Check.Exception(name.ToLower() != joinName.ToLower(), ErrorMessage.ExpressionCheck, joinName, methodName, name);
                    }
                }
            }
        }
    }
}
