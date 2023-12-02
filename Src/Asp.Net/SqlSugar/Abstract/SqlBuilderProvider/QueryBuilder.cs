using NetTaste;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        public string Hints { get; set; }
        internal AppendNavInfo AppendNavInfo { get; set; }
        public Type[] RemoveFilters { get; set; }
        public Dictionary<string, object> SubToListParameters { get; set; }
        internal List<QueryableAppendColumn> AppendColumns { get; set; }
        internal List<List<QueryableAppendColumn>> AppendValues { get; set; }
        public bool IsCrossQueryWithAttr { get;  set; }
        public Dictionary<string,string> CrossQueryItems { get; set; }
        public bool IsSelectSingleFiledJson { get; set; }
        public bool IsSelectSingleFiledArray { get; set; }
        public string TranLock { get; set; }
        public bool IsDisableMasterSlaveSeparation { get;  set; }
        public bool IsEnableMasterSlaveSeparation { get; set; }
        public bool IsQueryInQuery { get; set; }
        public List<object> Includes { get; set; }
        public List<string> IgnoreColumns { get; set; }
        public bool IsCount { get; set; }
        public bool IsSqlQuery { get; set; }
        public bool IsSqlQuerySelect { get; set; }
        public int? Skip { get; set; }
        public int ExternalPageIndex { get; set; }
        public int ExternalPageSize { get; set; }
        public int? Take { get; set; }
        public bool DisableTop { get; set; }
        public string SampleBy { get; set; }
        public string OrderByValue { get; set; }
        public object SelectValue { get; set; }
        public string SelectCacheKey { get; set; }
        public string EntityName { get; set; }
        public string OldSql { get; set; }

        public Type EntityType { get; set; }
        public Type ResultType { get; set; }
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
        public Dictionary<string, string> AsTables=new Dictionary<string, string>();
        public virtual string SqlTemplate
        {
            get
            {
                if (this.SampleBy.HasValue())
                {
                   return "SELECT {0} FROM {1}{2} "+ this.SampleBy + " {3}{4}";
                }
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
                return "ORDER BY " + this.Builder.SqlDateNow + " ";
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
        public virtual string EqualTemplate
        {
            get
            {
                return "{0} = {1} ";
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
            if (resolveType.IsIn(ResolveExpressType.FieldSingle, ResolveExpressType.FieldMultiple, ResolveExpressType.SelectSingle, ResolveExpressType.SelectMultiple) && (expression is LambdaExpression) && (expression as LambdaExpression).Body is BinaryExpression)
            {
                resolveType = resolveType.IsIn(ResolveExpressType.SelectSingle, ResolveExpressType.FieldSingle) ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple;
            }
            this.LambdaExpressions.Clear();
            if (this.Context.CurrentConnectionConfig.MoreSettings != null)
            {
                resolveExpress.TableEnumIsString = this.Context.CurrentConnectionConfig.MoreSettings.TableEnumIsString;
                resolveExpress.PgSqlIsAutoToLower = this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
            else
            {
                resolveExpress.PgSqlIsAutoToLower = true;
            }
            resolveExpress.SugarContext = new ExpressionOutParameter() { Context = this.Context, QueryBuilder = this } ;
            resolveExpress.RootExpression = expression;
            resolveExpress.JoinQueryInfos = Builder.QueryBuilder.JoinQueryInfos;
            resolveExpress.IsSingle = IsSingle()&& resolveType!= ResolveExpressType.WhereMultiple;
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
            this.Parameters.AddRange(resolveExpress.Parameters.Select(it => new SugarParameter(it.ParameterName, it.Value, it.DbType) {  Size=it.Size,TypeName=it.TypeName, IsNvarchar2=it.IsNvarchar2}));
            var result = resolveExpress.Result;
            var isSingleTableHasSubquery = IsSingle() && resolveExpress.SingleTableNameSubqueryShortName.HasValue();
            if (isSingleTableHasSubquery)
            {
                if (this.TableShortName != null && this.TableShortName.StartsWith("\""))
                {
                    Check.Exception(!string.IsNullOrEmpty(this.TableShortName) && resolveExpress.SingleTableNameSubqueryShortName != this.TableShortName.TrimEnd('\"').TrimStart('\"'), "{0} and {1} need same name", resolveExpress.SingleTableNameSubqueryShortName, this.TableShortName);
                    this.TableShortName = resolveExpress.SingleTableNameSubqueryShortName;
                }
                else
                {
                    Check.Exception(!string.IsNullOrEmpty(this.TableShortName) && resolveExpress.SingleTableNameSubqueryShortName != this.TableShortName, "{0} and {1} need same name", resolveExpress.SingleTableNameSubqueryShortName, this.TableShortName);
                    this.TableShortName = resolveExpress.SingleTableNameSubqueryShortName;
                }
            }
            return result;
        }

        internal string GetFilters(Type type)
        {
            var result = "";
            if (this.Context != null)
            {
                var db = Context;
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic| BindingFlags.Public;
                var index = 0;
                if (db.QueryFilter.GeFilterList != null)
                {
                    foreach (var item in db.QueryFilter.GeFilterList)
                    {
                        if (this.RemoveFilters != null && this.RemoveFilters.Length > 0) 
                        {
                            if (this.RemoveFilters.Contains(item.type)) 
                            {
                                continue;
                            }
                        }

                        PropertyInfo field = item.GetType().GetProperty("exp", flag);
                        if (field != null)
                        {
                            Type ChildType = item.type;
                            var isInterface = ChildType.IsInterface && type.GetInterfaces().Any(it => it == ChildType);
                            if (ChildType == type|| isInterface)
                            {
                                var entityInfo = db.EntityMaintenance.GetEntityInfo(ChildType);
                                var exp = field.GetValue(item, null) as Expression;
                                var whereStr = index==0 ? " " : " AND ";
                                index++;
                                result += (whereStr + GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
                                if (isInterface) 
                                {
                                    result = ReplaceFilterColumnName(result,type);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public virtual string ToSqlString()
        {
            string oldOrderBy = this.OrderByValue;
            string externalOrderBy = oldOrderBy;
            var isIgnoreOrderBy = this.IsCount && this.PartitionByValue.IsNullOrEmpty();
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
            if (isIgnoreOrderBy) { this.OrderByValue = oldOrderBy; return sql.ToString(); }
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
                if (this.RemoveFilters != null && this.RemoveFilters.Length > 0) 
                {
                    gobalFilterList = gobalFilterList.Where(it => !this.RemoveFilters.Contains(it.type)).ToList();
                }
                foreach (var item in gobalFilterList)
                {
                    if (item.GetType().Name.StartsWith("TableFilterItem"))
                    {
                        AppendTableFilter(item);
                    }
                }
                foreach (var item in gobalFilterList.Where(it=>it.GetType().Name=="SqlFilterItem").Where(it => it.IsJoinQuery == !IsSingle()))
                {
                    var filterResult = item.FilterValue(this.Context);
                    WhereInfos.Add(this.Builder.AppendWhereOrAnd(this.WhereInfos.IsNullOrEmpty(), filterResult.Sql + UtilConstants.Space));
                    var filterParamters = this.Context.Ado.GetParameters(filterResult.Parameters);
                    if (filterParamters.HasValue())
                    {
                        this.Parameters.AddRange(filterParamters);
                    }
                }
            }
        }

        private void AppendTableFilter(SqlFilterItem item)
        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic |BindingFlags.Public;
            Type type = item.GetType();
            PropertyInfo field = type.GetProperty("exp", flag);
            Type ChildType = item.type;
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfoWithAttr(ChildType);
            var exp = field.GetValue(item, null) as Expression;
            var isMain = ChildType == this.EntityType||(ChildType.IsInterface&& this.EntityType.GetInterfaces().Any(it => it == ChildType));
            var isSingle = IsSingle();
            var itName = (exp as LambdaExpression).Parameters[0].Name;
            itName = this.Builder.GetTranslationColumnName(itName) + ".";
            var isEasyJoin = this.EasyJoinInfos.Count > 0;

            string sql = "";
            if (isSingle)
            {
                if (ChildType.IsInterface&&this.EntityType.GetInterfaces().Any(it => it == ChildType)) 
                {
                    //future
                }
                else if (ChildType != this.EntityType && isSingle)
                {
                    return;
                } 
                sql = GetSql(exp, isSingle);
                if (ChildType.IsInterface)
                {
                    var filterType = this.EntityType;
                    sql = ReplaceFilterColumnName(sql, filterType);
                }
            }
            else if (isEasyJoin && ChildType.IsInterface && this.JoinExpression != null && (this.JoinExpression as LambdaExpression)?.Parameters?.Any(it => it.Type.GetInterfaces().Any(z => z == ChildType)) == true)
            {
                var parameters = (this.JoinExpression as LambdaExpression).Parameters.Where(it => it.Type.GetInterfaces().Any(z => z == ChildType)).ToList();
                foreach (var parameter in parameters)
                {
                    var shortName = this.Builder.GetTranslationColumnName(parameter.Name) + ".";
                    var mysql = GetSql(exp, isSingle);
                    sql += mysql.Replace(itName, shortName);
                    var filterType =parameter.Type;
                    sql = ReplaceFilterColumnName(sql, filterType);
                }
            }
            else if (isMain)
            {
                if (TableShortName == null)
                    return;

                var isSameName =!isSingle&& this.JoinQueryInfos.Count(it => it.TableName == entityInfo.DbTableName)>0;
                if (isSameName||ChildType.IsInterface)
                {
                    var mysql = GetSql(exp, isSingle);
                    if (ChildType.IsInterface)
                    {
                        foreach (var joinInfoItem in this.JoinQueryInfos.Where(it => it.EntityType.GetInterfaces().Any(z=>z==ChildType)))
                        {
                            var addSql = mysql.Replace(itName, this.Builder.GetTranslationColumnName(joinInfoItem.ShortName) + ".");
                            addSql = ReplaceFilterColumnName(addSql, joinInfoItem.EntityType,joinInfoItem.ShortName);
                            joinInfoItem.JoinWhere += (" AND " + Regex.Replace(addSql, "^ (WHERE|AND) ", ""));
                        }
                    }
                    else
                    {
                        foreach (var joinInfoItem in this.JoinQueryInfos.Where(it => it.TableName == entityInfo.DbTableName))
                        {
                            var addSql = mysql.Replace(itName, this.Builder.GetTranslationColumnName(joinInfoItem.ShortName) + ".");
                            addSql = ReplaceFilterColumnName(addSql, joinInfoItem.EntityType, joinInfoItem.ShortName);
                            joinInfoItem.JoinWhere += (" AND " + Regex.Replace(addSql, "^ (WHERE|AND) ", ""));
                        }
                    }
                    sql = mysql.Replace(itName, this.Builder.GetTranslationColumnName(TableShortName) + ".");
                }
                else
                {
                    var shortName = this.Builder.GetTranslationColumnName(TableShortName) + ".";
                    sql = GetSql(exp, isSingle);
                    sql = sql.Replace(itName, shortName);
                }

                if (ChildType.IsInterface)
                {
                    sql = ReplaceFilterColumnName(sql, this.EntityType);
                }
            }
            else if (isEasyJoin)
            {
                var easyInfo = EasyJoinInfos.FirstOrDefault(it =>
                   it.Value.Equals(entityInfo.DbTableName, StringComparison.CurrentCultureIgnoreCase) ||
                   it.Value.Equals(entityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase));
                if (easyInfo.Key == null)
                {
                    return;
                }
                var shortName = this.Builder.GetTranslationColumnName(easyInfo.Key.Trim()) + ".";
                sql = GetSql(exp, isSingle);
                sql = sql.Replace(itName, shortName);
            }
            else
            {
                var easyInfo = JoinQueryInfos.FirstOrDefault(it =>
                it.TableName.Equals(entityInfo.DbTableName, StringComparison.CurrentCultureIgnoreCase) ||
                it.TableName.Equals(entityInfo.EntityName, StringComparison.CurrentCultureIgnoreCase)||
                it.EntityType==ChildType);
                if (easyInfo == null)
                {
                    if (ChildType.IsInterface && JoinQueryInfos.Any(it =>it.EntityType!=null&&it.EntityType.GetInterfaces().Any(z => z == ChildType)))
                    {
                        easyInfo = JoinQueryInfos.FirstOrDefault(it => it.EntityType.GetInterfaces().Any(z => z == ChildType));
                    }
                    else
                    {
                        return;
                    }
                }
                var shortName = this.Builder.GetTranslationColumnName(easyInfo.ShortName.Trim()) + ".";
                sql = GetSql(exp, isSingle);
                sql = sql.Replace(itName, shortName);
            }
            if (item.IsJoinQuery == false||isMain||isSingle|| isEasyJoin)
            {
                WhereInfos.Add(sql);
            }
            else 
            {
                var isSameName = !isSingle && this.JoinQueryInfos.Count(it => it.TableName == entityInfo.DbTableName) > 1;
                foreach (var joinInfo in this.JoinQueryInfos)
                {
                    var isInterface = ChildType.IsInterface && joinInfo.EntityType != null && joinInfo.EntityType.GetInterfaces().Any(it => it == ChildType);
                    if (isInterface
                        && isSameName == false
                        &&this.JoinQueryInfos.Where(it=> it.EntityType!=null).Count(it => it.EntityType.GetInterfaces().Any(z=>z==ChildType)) > 1) 
                    {
                        sql = GetSql(exp, false);
                        var shortName = this.Builder.GetTranslationColumnName(joinInfo.ShortName.Trim()) + ".";
                        sql = sql.Replace(itName, shortName);
                    }
                    if (isInterface||joinInfo.TableName.EqualCase(entityInfo.EntityName)|| joinInfo.TableName.EqualCase(entityInfo.DbTableName)||joinInfo.EntityType==ChildType) 
                    {
                        var mysql = sql;
                        if (isSameName)
                        {
                            var firstShortName = this.JoinQueryInfos.First(it => it.TableName == entityInfo.DbTableName).ShortName;
                            mysql = mysql.Replace(Builder.GetTranslationColumnName(firstShortName), Builder.GetTranslationColumnName(joinInfo.ShortName));
                        }
                        if (mysql.StartsWith(" WHERE ")) 
                        {
                            mysql = Regex.Replace(mysql, $"^ WHERE ", " AND ");
                        }
                        if (isInterface) 
                        {
                            mysql = ReplaceFilterColumnName(mysql,joinInfo.EntityType,Builder.GetTranslationColumnName(joinInfo.ShortName));
                        }
                        joinInfo.JoinWhere=joinInfo.JoinWhere + mysql;
                    }
                }
            }
        }

        private string ReplaceFilterColumnName(string sql, Type filterType,string shortName=null)
        {
            foreach (var column in this.Context.EntityMaintenance.GetEntityInfoWithAttr(filterType).Columns.Where(it => it.IsIgnore == false))
            {
                if (shortName == null)
                {
                    sql = sql.Replace(Builder.GetTranslationColumnName(column.PropertyName), Builder.GetTranslationColumnName(column.DbColumnName));
                }
                else 
                {
                    sql = sql.Replace(Builder.GetTranslationColumnName(shortName) + "." + Builder.GetTranslationColumnName(column.PropertyName), Builder.GetTranslationColumnName(shortName) + "." + Builder.GetTranslationColumnName(column.DbColumnName));
                    sql = sql.Replace(shortName + "."+Builder.GetTranslationColumnName(column.PropertyName), shortName+"." +Builder.GetTranslationColumnName(column.DbColumnName));
                }
            }
            return sql;
        }

        private string GetSql(Expression exp, bool isSingle)
        {
            var expValue = GetExpressionValue(exp, isSingle ? ResolveExpressType.WhereSingle : ResolveExpressType.WhereMultiple);
            var sql = expValue.GetResultString();
            if (WhereInfos.Count == 0)
            {
                sql = (" WHERE " + sql);
            }
            else
            {
                sql = (" AND " + sql);
            }

            return sql;
        }

        public virtual string GetExternalOrderBy(string externalOrderBy)
        {
            return Regex.Replace(externalOrderBy, @"\[\w+\]\.", "");
        }

        public virtual string ToCountSql(string sql)
        {
            if (sql != null && sql.Contains("-- No table")) 
            {
                return "-- No table";
            }
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
            return string.Format(temp, sql.ToString(), (pageIndex - 1) * pageSize + 1, pageIndex * pageSize);
        }

        public virtual string GetSelectByItems(List<KeyValuePair<string, JoinMapper>> items)
        {
            var array = items.Select(it => {
                JoinMapper dynamicObj = it.Value;
                var dbName = Builder.GetTranslationColumnName(dynamicObj.DbName);
                var asName = Builder.GetTranslationColumnName(dynamicObj.AsName);
                return string.Format("{0}.{1} AS {2}", it.Key, dbName, asName);
            });
            return string.Join(",", array);
        }

        public virtual string ToJoinString(JoinQueryInfo joinInfo)
        {
            var name = joinInfo.TableName;
            if (this.AsTables.Any())
            {
                if (this.Context.MappingTables != null && this.Context.MappingTables.Any(it => it.DbTableName == name))
                {
                    name = this.Context.MappingTables.First(it => it.DbTableName == name).EntityName;
                }
                if (this.AsTables.Any(it => it.Key == name))
                {
                    name = this.AsTables.First(it => it.Key == name).Value;
                }
            }
            var isSubQuery = name!=null&& name.StartsWith("(") && name.EndsWith(")");
            var shortName = joinInfo.ShortName;
            if (shortName.HasValue()) 
            {
                shortName = this.Builder.GetTranslationColumnName(shortName);
            }
            var result= string.Format(
                this.JoinTemplate,
                joinInfo.JoinType.ToString() + UtilConstants.Space,
                Builder.GetTranslationTableName(name) + UtilConstants.Space,
                shortName + UtilConstants.Space + (TableWithString == SqlWith.Null|| isSubQuery ? " " : TableWithString),
                joinInfo.JoinWhere);
            if (joinInfo.EntityType!=null&&this.Context.EntityMaintenance.GetEntityInfoWithAttr(joinInfo.EntityType).Discrimator.HasValue()) 
            {
                var entityInfo = this.Context.EntityMaintenance.GetEntityInfoWithAttr(joinInfo.EntityType);
                result = $" {result} AND {shortName}.{UtilMethods.GetDiscrimator(entityInfo,this.Builder)}";
            }
            return result;
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
            return Regex.IsMatch(sql, @"AS \[\w+\.\w+\]")|| Regex.IsMatch(sql, @"AS \[\w+\.\w+\.\w+\]");
        }
        public string GetSqlQuerySql(string result)
        {
            if (this.Hints.HasValue())
            {
                result = ReplaceHints(result);
            }
            if (GetTableNameString == " (-- No table ) t  ")
            {
                result = "-- No table ";
                return result;
            }
            if (this.IsSqlQuerySelect == true) 
            {
                return result;
            }
            if (this.JoinQueryInfos.Count==0&&string.IsNullOrEmpty(OrderByValue)&&this.IsSqlQuery&&this.OldSql.HasValue() && (Skip == null && Take == null) && (this.WhereInfos == null || this.WhereInfos.Count == 0))
            {
                return this.OldSql;
            }
            else
            {
                return result;
            }
        }

        protected virtual string ReplaceHints(string result)
        {
            result = Regex.Replace(result, "^SELECT ", it =>
            {
                return $"{it} {Hints}  ";
            });
            return result;
        }

        protected string SubToListMethod(string result)
        {
            string oldResult = result;
            List<string> names = new List<string>();
            var allShortName = new List<string>();
            if (IsSingleSubToList())
            {
                this.TableShortName = (SelectValue as LambdaExpression).Parameters[0].Name;
            }
            allShortName.Add(this.Builder.SqlTranslationLeft + Builder.GetNoTranslationColumnName(this.TableShortName.ObjToString().ToLower()) + this.Builder.SqlTranslationRight + ".");
            if (this.JoinQueryInfos.HasValue())
            {
                foreach (var item in this.JoinQueryInfos)
                {
                    allShortName.Add(this.Builder.SqlTranslationLeft + Builder.GetNoTranslationColumnName(item.ShortName.ObjToString().ToLower()) + this.Builder.SqlTranslationRight + ".");
                }
            }
            else if (this.EasyJoinInfos != null && this.EasyJoinInfos.Any())
            {
                Check.ExceptionEasy("No Supprt Subquery.ToList(), Inner Join Or  Left Join", "Subquery.ToList请使用Inner方式联表");
            }
            if (this.TableShortName == null)
            {
                //Empty
            }
            else
            {
                var name = Builder.GetTranslationColumnName(this.TableShortName) + @"\.";
                foreach (var paramter in this.SubToListParameters)
                {
                    var regex = $@"\{Builder.SqlTranslationLeft}[\w]{{1,20}}?\{Builder.SqlTranslationRight}\.\{Builder.SqlTranslationLeft}.{{1,50}}?\{Builder.SqlTranslationRight}";
                    var matches = Regex
                        .Matches(paramter.Value.ObjToString(), regex, RegexOptions.IgnoreCase).Cast<Match>()
                        .Where(it => allShortName.Any(z => it.Value.ObjToString().ToLower().Contains(z)))
                        .Select(it => it.Value).ToList();
                    names.AddRange(matches);
                }
                int i = 0;
                names = names.Distinct().ToList();
                if (names.Any())
                {
                    List<QueryableAppendColumn> colums = new List<QueryableAppendColumn>();
                    foreach (var item in names)
                    {
                        result = (result + $",{item} as app_ext_col_{i}");
                        colums.Add(new QueryableAppendColumn() { AsName = $"app_ext_col_{i}", Name = item, Index = i });
                        i++;
                    }
                    this.AppendColumns = colums;
                }
            }
            if (HasAppText(oldResult))
            {
                return oldResult;
            }
            return result;
        }

        #endregion

        #region Get SQL Partial
        public string Offset { get; set; }
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
                if (IsDistinct&&result?.StartsWith("DISTINCT ")!=true)
                {
                    result = " DISTINCT " + result;
                }
                if (this.SubToListParameters!=null&& this.SubToListParameters.Any())
                {
                    result = SubToListMethod(result);
                }
                return result;
            }
        }

        public virtual string GetSelectValueByExpression()
        {
            var expression = this.SelectValue as Expression;
            string result = string.Empty;
            if (this.IgnoreColumns != null && this.IgnoreColumns.Any())
            {
                var expArray = GetExpressionValue(expression, this.SelectType).GetResultArray()
                    .Where(it=>
                      !this.IgnoreColumns.Any(z=>it.Contains(Builder.GetTranslationColumnName(z)))
                    ).ToArray();
                result =string.Join(",", expArray);
            }
            else
            {
                result= GetExpressionValue(expression, this.SelectType).GetResultString();
                if (result == null && ExpressionTool.GetMethodName(ExpressionTool.GetLambdaExpressionBody(expression)) == "End") 
                {
                    result = GetExpressionValue(expression, ResolveExpressType.FieldSingle).GetResultString();
                }
            }
            if (result == null&& this.AppendNavInfo?.AppendProperties==null)
            {
                return "*";
            }
            if (this.AppendNavInfo?.AppendProperties?.Any() ==true) 
            {
                if (result == null) 
                {
                    result = "*";
                }
                result += ",";
                var shortName = "";
                if (this.TableShortName.HasValue()) 
                {
                    shortName = $"{Builder.GetTranslationColumnName(this.TableShortName)}.";
                }
                if (this.GroupByValue.HasValue())
                {
                    result += string.Join(",",this.AppendNavInfo.AppendProperties.Select(it => "max(" + shortName + Builder.GetTranslationColumnName(it.Value) + ") AS SugarNav_" + it.Key));
                }
                else
                {
                    result += string.Join(",", this.AppendNavInfo.AppendProperties.Select(it => shortName + Builder.GetTranslationColumnName(it.Value) + " AS SugarNav_" + it.Key));
                }
            }
            if (result.Contains("/**/*")) 
            {
                return result.Replace("/**/*", "");
            }
            if (this.IsSingle() && this.SubToListParameters != null&& expression is LambdaExpression && this.SubToListParameters.Count > 0 && this.TableShortName == null) 
            {
                this.TableShortName =this.Builder.GetTranslationColumnName((expression as LambdaExpression).Parameters[0].Name);
            }
            if (result.Contains(".*") && this.IsSingle())
            {
                return "*";
            }
            else
            {
                if (expression is LambdaExpression && (expression as LambdaExpression).Body is MethodCallExpression && this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer && this.OrderByValue.HasValue())
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
                    columns = columns.Where(c => !this.IgnoreColumns.Any(i => c.PropertyName.Equals(i, StringComparison.CurrentCultureIgnoreCase) || c.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase))).ToList();
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
                var name = EntityName;
                if (this.AsTables.Any(it=>it.Key==EntityName))
                {
                    name = this.AsTables.FirstOrDefault(it => it.Key == EntityName).Value;
                    if (this.IsQueryInQuery && this.SelectValue != null && this.SelectValue is Expression) 
                    {
                        if (this.SelectValue.ToString().Contains("Subqueryable()")&& name.TrimStart().StartsWith("(")) 
                        {
                            var oldName = name;
                            name = Regex.Match(name, @"\(.+\)").Value;
                            if (name.IsNullOrEmpty()) 
                            {
                                name = oldName;
                            }
                        }
                    }
                }
                var result = Builder.GetTranslationTableName(name);
                result += UtilConstants.Space;
                if (IsSingle() && result.Contains("MergeTable") && result.Trim().EndsWith(" MergeTable") && TableShortName != null)
                {
                    result = result.Replace(") MergeTable  ", ") " + TableShortName+UtilConstants.Space);
                    TableShortName = null;
                }
                if (IsSingle() && result.Contains("unionTable") && result.Trim().EndsWith(" unionTable")&& TableShortName!=null) 
                {
                    result = result.Replace(" ) unionTable  ", ") "+TableShortName + UtilConstants.Space);
                    TableShortName = null;
                }
                if (this.TableShortName.HasValue()&&!IsSqlQuery)
                {
                    result += (Builder.GetTranslationColumnName(TableShortName) + UtilConstants.Space);
                }
                if (this.TableWithString.HasValue() && this.TableWithString != SqlWith.Null)
                {
                    if (!result.TrimStart().StartsWith("("))
                    {
                        result += TableWithString + UtilConstants.Space;
                    }
                }
                if (!this.IsSingle())
                {
                    result += GetJoinValueString + UtilConstants.Space;
                }
                if (this.EasyJoinInfos.IsValuable())
                {

                    if (this.TableWithString.HasValue() && this.TableWithString != SqlWith.Null)
                    {
                        result += "," + string.Join(",", this.EasyJoinInfos.Select(it => string.Format("{0} {1} {2} ", GetTableName(it.Value)," " +Builder.GetTranslationColumnName(it.Key.ObjToString().Trim()), TableWithString)));
                    }
                    else
                    {
                        result += "," + string.Join(",", this.EasyJoinInfos.Select(it => string.Format("{0} {1} ", GetTableName(it.Value), " " + Builder.GetTranslationColumnName(it.Key.ObjToString().Trim()))));
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
                if (IsCount && this.PartitionByValue.IsNullOrEmpty()) return null;
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
                if (this.GroupByValue.Last() != ' ')
                {
                    return this.GroupByValue + UtilConstants.Space;
                }
                return this.GroupByValue;
            }
        }


        #endregion

        #region NoCopy

        internal List<QueryableFormat> QueryableFormats { get; set; }
        internal bool IsClone { get; set; }
        public bool NoCheckInclude { get;  set; }
        public virtual bool IsSelectNoAll { get; set; } = false;
        public List<string> AutoAppendedColumns { get;  set; }
        public Dictionary<string, string> MappingKeys { get;  set; } 
        #endregion

        private string GetTableName(string entityName)
        {
            if (this.AsTables != null && this.AsTables.Any(it=>it.Key==entityName)) 
            {
                entityName = this.AsTables.First(it => it.Key == entityName).Value;
            }
            var result = this.Context.EntityMaintenance.GetTableName(entityName);
            return this.Builder.GetTranslationTableName(result);
        }

        public void CheckExpression(Expression expression, string methodName)
        {
            if (IsSingle() == false && this.JoinExpression != null)
            {
                var jsoinParameters = (this.JoinExpression as LambdaExpression).Parameters;
                var currentParametres = (expression as LambdaExpression).Parameters;
                if ((expression as LambdaExpression).Body.ToString() == "True")
                {
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
        public void CheckExpressionNew(Expression expression, string methodName)
        {
            if (IsSingle() == false && this.JoinExpression != null)
            {
                var jsoinParameters = (this.JoinExpression as LambdaExpression).Parameters;
                var currentParametres = (expression as LambdaExpression).Parameters;
                if ((expression as LambdaExpression).Body.ToString() == "True")
                {
                    return;
                }
                if (currentParametres != null && currentParametres.Count > 0)
                {
                    if (jsoinParameters.Count + 1 != currentParametres.Count) 
                    {
                        var str1 = "(" + string.Join(",", currentParametres.Select(it => it.Name)) + ")=>";
                        var str2 = "("+string.Join(",", jsoinParameters.Select(it => it.Name))+","+ currentParametres.Last().Type.Name + " )=>";
                        throw new Exception(ErrorMessage.GetThrowMessage($"Join {currentParametres.Last().Type.Name} error , Please change {str1} to {str2}.", $"Join {currentParametres.Last().Type.Name} 错误, 请把 {str1} 改成 {str2} "));
                    }
                    foreach (var item in currentParametres.Take(jsoinParameters.Count))
                    {
                        var index = currentParametres.IndexOf(item);
                        var name = item.Name;
                        var joinName = jsoinParameters[index].Name;
                        Check.Exception(name.ToLower() != joinName.ToLower(), ErrorMessage.ExpressionCheck, joinName, methodName, name);
                    }
                }
            }
        }
        private bool IsSingleSubToList()
        {
            return this.SubToListParameters != null
                             && this.TableShortName == null
                             && this.SelectValue is Expression
                             && this.IsSingle();
        }
        private static bool HasAppText(string result)
        {
            return result.HasValue() && result.Contains("app_ext_col_0");
        }
    }
}
