using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class DeleteableProvider<T> : IDeleteable<T> where T : class, new()
    {
        public ISqlSugarClient Context { get; set; }
        public IAdo Db { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        public List<string> tempPrimaryKeys { get; set; }
        internal Action RemoveCacheFunc { get; set; }
        internal List<T> DeleteObjects { get; set; }
        public EntityInfo EntityInfo
        {
            get
            {
                return this.Context.EntityMaintenance.GetEntityInfo<T>();
            }
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public int ExecuteCommand()
        {
            string sql;
            SugarParameter[] paramters;
            _ExecuteCommand(out sql, out paramters);
            var result = Db.ExecuteCommand(sql, paramters);
            After(sql);
            return result;
        }
        public bool ExecuteCommandHasChange()
        {
            return ExecuteCommand() > 0;
        }
        public Task<int> ExecuteCommandAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteCommandAsync();
        }
        public async Task<int> ExecuteCommandAsync()
        {
            string sql;
            SugarParameter[] paramters;
            _ExecuteCommand(out sql, out paramters);
            var result = await Db.ExecuteCommandAsync(sql, paramters);
            After(sql);
            return result;
        }
        public async Task<bool> ExecuteCommandHasChangeAsync()
        {
            return await ExecuteCommandAsync() > 0;
        }
        public IDeleteable<T> AsType(Type tableNameType)
        {
            return AS(this.Context.EntityMaintenance.GetEntityInfo(tableNameType).DbTableName);
        }
        public IDeleteable<T> AS(string tableName)
        {
            if (tableName == null) return this;
            //var entityName = typeof(T).Name;
            //IsAs = true;
            //OldMappingTableList = this.Context.MappingTables;
            //this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            //if (this.Context.MappingTables.Any(it => it.EntityName == entityName))
            //{
            //    this.Context.MappingTables.Add(this.Context.MappingTables.First(it => it.EntityName == entityName).DbTableName, tableName);
            //}
            //this.Context.MappingTables.Add(entityName, tableName);
            this.DeleteBuilder.AsName = tableName;
            return this; ;
        }
        public IDeleteable<T> EnableDiffLogEventIF(bool isEnableDiffLogEvent, object businessData = null)
        {
            if (isEnableDiffLogEvent)
            {
                return EnableDiffLogEvent(businessData);
            }
            else
            {
                return this;
            }
        }
        public IDeleteable<T> EnableDiffLogEvent(object businessData = null)
        {

            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.delete;
            return this;
        }

        public IDeleteable<T> Where(List<T> deleteObjs)
        {
            this.DeleteObjects = deleteObjs;
            if (deleteObjs == null || deleteObjs.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            DataAop(deleteObjs);
            string tableName = this.Context.EntityMaintenance.GetTableName<T>();
            var primaryFields = this.GetPrimaryKeys();
            var isSinglePrimaryKey = primaryFields.Count == 1;
            Check.Exception(primaryFields.IsNullOrEmpty(), string.Format("Table {0} with no primarykey", tableName));
            if (isSinglePrimaryKey)
            {
                List<object> primaryKeyValues = new List<object>();
                var primaryField = primaryFields.Single();
                foreach (var deleteObj in deleteObjs)
                {
                    var entityPropertyName = this.Context.EntityMaintenance.GetPropertyName<T>(primaryField);
                    var columnInfo = EntityInfo.Columns.Single(it => it.PropertyName.Equals(entityPropertyName, StringComparison.CurrentCultureIgnoreCase));
                    var value = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                    value = UtilMethods.GetConvertValue(value);
                    if (this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString!=true&&
                        columnInfo.SqlParameterDbType==null&& 
                        columnInfo.PropertyInfo.PropertyType.IsEnum()) 
                    {
                        value = Convert.ToInt64(value);
                    }
                    primaryKeyValues.Add(value);
                }
                if (primaryKeyValues.Count < 10000)
                {
                    var inValueString = primaryKeyValues.ToArray().ToJoinSqlInVals();
                    Where(string.Format(DeleteBuilder.WhereInTemplate, SqlBuilder.GetTranslationColumnName(primaryFields.Single()), inValueString));
                }
                else
                {
                    if (DeleteBuilder.BigDataInValues == null)
                        DeleteBuilder.BigDataInValues = new List<object>();
                    DeleteBuilder.BigDataInValues.AddRange(primaryKeyValues);
                    DeleteBuilder.BigDataFiled = primaryField;
                }
            }
            else
            {
                StringBuilder whereInSql = new StringBuilder();
                foreach (var deleteObj in deleteObjs)
                {
                    StringBuilder orString = new StringBuilder();
                    var isFirst = deleteObjs.IndexOf(deleteObj) == 0;
                    if (!isFirst)
                    {
                        orString.Append(DeleteBuilder.WhereInOrTemplate + UtilConstants.Space);
                    }
                    int i = 0;
                    StringBuilder andString = new StringBuilder();
                    foreach (var primaryField in primaryFields)
                    {
                        if (i != 0)
                            andString.Append(DeleteBuilder.WhereInAndTemplate + UtilConstants.Space);
                        //var entityPropertyName = this.EntityInfo.Columns.Single(it=>it.PropertyName.EqualCase(primaryField)||it.DbColumnName.EqualCase(primaryField)).PropertyName;
                        var columnInfo = EntityInfo.Columns.Single(t => t.PropertyName.EqualCase(primaryField) || t.DbColumnName.EqualCase(primaryField));
                        var entityValue = columnInfo.PropertyInfo.GetValue(deleteObj, null);
                        if (this.Context.CurrentConnectionConfig?.MoreSettings?.TableEnumIsString != true &&
                        columnInfo.SqlParameterDbType == null &&
                        columnInfo.PropertyInfo.PropertyType.IsEnum())
                         {
                            entityValue = Convert.ToInt64(entityValue);
                         }
                        var tempequals = DeleteBuilder.WhereInEqualTemplate;
                        if (this.Context.CurrentConnectionConfig.MoreSettings != null && this.Context.CurrentConnectionConfig.MoreSettings.DisableNvarchar == true)
                        {
                            tempequals = $"{SqlBuilder.SqlTranslationLeft}{{0}}{SqlBuilder.SqlTranslationRight}='{{1}}' ";
                        }
                        if (SqlBuilder.SqlParameterKeyWord==":")
                        {
                            var isAutoToUpper =this.Context.CurrentConnectionConfig?.MoreSettings?.IsAutoToUpper??true;
                            if (entityValue != null && UtilMethods.GetUnderType(entityValue.GetType()) == UtilConstants.DateType)
                            {
                                andString.AppendFormat("\"{0}\"={1} ", primaryField.ToUpper(isAutoToUpper), "to_date('" + entityValue.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss") + "', 'YYYY-MM-DD HH24:MI:SS') ");
                            }
                            else
                            {
                                andString.AppendFormat(tempequals.Replace("N","")+" ", primaryField.ToUpper(isAutoToUpper), entityValue);
                            }
                        }
                        else if (this.Context.CurrentConnectionConfig.DbType == DbType.PostgreSQL && (this.Context.CurrentConnectionConfig.MoreSettings == null || this.Context.CurrentConnectionConfig.MoreSettings?.PgSqlIsAutoToLower == true))
                        {
                            andString.AppendFormat("\"{0}\"={1} ", primaryField.ToLower(), new PostgreSQLExpressionContext().GetValue(entityValue));
                        }
                        else if ( entityValue != null &&UtilMethods.IsNumber( UtilMethods.GetUnderType(entityValue.GetType()).Name))
                        {
                            andString.AppendFormat("{0}={1} ", this.SqlBuilder.GetTranslationColumnName(primaryField), $"{entityValue}");
                        }
                        else if (entityValue != null && UtilMethods.GetUnderType(entityValue.GetType()) == UtilConstants.DateType)
                        {
                            andString.AppendFormat("{0}={1} ", this.SqlBuilder.GetTranslationColumnName(primaryField), this.DeleteBuilder.LambdaExpressions.DbMehtods.ToDate(new MethodCallExpressionModel()
                            {
                                Args = new List<MethodCallExpressionArgs>()
                             {
                                 new MethodCallExpressionArgs()
                                 {
                                      IsMember=false,
                                      MemberName="'"+entityValue.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss.fff")+"'"
                                 }
                             }
                            })); ;
                        }
                        else
                        {
                            if ((columnInfo.SqlParameterDbType.ObjToString() == System.Data.DbType.AnsiString.ObjToString()) || !(entityValue is string) || this.Context.CurrentConnectionConfig?.MoreSettings?.DisableNvarchar == true)
                            {
                                tempequals = tempequals.Replace("=N'", "='");
                            }
                            else 
                            {
                                tempequals = SqlBuilder.RemoveN(tempequals);
                            }
                            entityValue = UtilMethods.GetConvertValue(entityValue);
                            andString.AppendFormat(tempequals, primaryField, entityValue);
                        }
                        ++i;
                    }
                    orString.AppendFormat(DeleteBuilder.WhereInAreaTemplate, andString);
                    whereInSql.Append(orString);
                }
                Where(string.Format(DeleteBuilder.WhereInAreaTemplate, whereInSql.ToString()));
            }
            return this;
        }
        public IDeleteable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression)
        {
            Check.ExceptionEasy(!StaticConfig.EnableAllWhereIF, "Need to program startup configuration StaticConfig. EnableAllWhereIF = true; Tip: This operation is very risky if there are no conditions it is easy to update the entire table", " 需要程序启动时配置StaticConfig.EnableAllWhereIF=true; 提示：该操作存在很大的风险如果没有条件很容易将整个表全部更新");
            if (isWhere)
            {
                return Where(expression);
            }
            return this;
        }
        public IDeleteable<T> Where(Expression<Func<T, bool>> expression)
        {
            var expResult = DeleteBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            var whereString = expResult.GetResultString();
            if (expression.ToString().Contains("Subqueryable()")) {
                var entityTableName = this.EntityInfo.DbTableName;
                if (this.DeleteBuilder.AsName.HasValue()) 
                {
                    entityTableName = this.DeleteBuilder.AsName;
                }
                if (ExpressionTool.GetParameters(expression).First().Type == typeof(T))
                {
                    var tableName = this.SqlBuilder.GetTranslationColumnName(entityTableName);
                    whereString = whereString.Replace(tableName, $"( SELECT * FROM {tableName})  ");
                }
                whereString = whereString.Replace(this.SqlBuilder.GetTranslationColumnName(expression.Parameters.First().Name) + ".", this.SqlBuilder.GetTranslationTableName(entityTableName) + ".");
            }
            else if (expResult.IsNavicate)
            {
                var entityTableName2 = this.EntityInfo.DbTableName;
                if (this.DeleteBuilder.AsName.HasValue())
                {
                    entityTableName2 = this.DeleteBuilder.AsName;
                }
                whereString = whereString.Replace(expression.Parameters.First().Name + ".", this.SqlBuilder.GetTranslationTableName(entityTableName2) + ".");
                whereString = whereString.Replace(this.SqlBuilder.GetTranslationColumnName(expression.Parameters.First().Name) + ".", this.SqlBuilder.GetTranslationTableName(entityTableName2) + ".");
               
            }
            DeleteBuilder.WhereInfos.Add(whereString);
            return this;
        }

        public IDeleteable<T> Where(T deleteObj)
        {
            Check.Exception(GetPrimaryKeys().IsNullOrEmpty(), "Where(entity) Primary key required");
            Where(new List<T>() { deleteObj });
            return this;
        }

        public IDeleteable<T> Where(string whereString, object parameters = null)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (parameters != null)
            {
                if (DeleteBuilder.Parameters == null)
                {
                    DeleteBuilder.Parameters = new List<SugarParameter>();
                }
                DeleteBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            }
            return this;
        }

        public IDeleteable<T> Where(string whereString, SugarParameter parameter)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.Add(parameter);
            return this;
        }
        public IDeleteable<T> Where(string whereString, SugarParameter[] parameters)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public IDeleteable<T> Where(string whereString, List<SugarParameter> parameters)
        {
            DeleteBuilder.WhereInfos.Add(whereString);
            if (DeleteBuilder.Parameters == null)
            {
                DeleteBuilder.Parameters = new List<SugarParameter>();
            }
            DeleteBuilder.Parameters.AddRange(parameters);
            return this;
        }
        public IDeleteable<T> Where(List<IConditionalModel> conditionalModels)
        {
            if (conditionalModels.Count == 0)
            {
                return Where("1=2");
            }
            var sql = this.Context.Queryable<T>().SqlBuilder.ConditionalModelToSql(conditionalModels);
            var result = this;
            result.Where(sql.Key, sql.Value);
            return result;
        }
        public IDeleteable<T> WhereColumns(T data, Expression<Func<T, object>> columns)
        {
            return WhereColumns(new List<T>() { data },columns);
        }
        public IDeleteable<T> WhereColumns(List<T> list,Expression<Func<T, object>> columns)
        {
            if (columns!=null)
            {
                tempPrimaryKeys = DeleteBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            }
            //else if (columns != null && tempPrimaryKeys.IsNullOrEmpty())
            //{
            //    tempPrimaryKeys = DeleteBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            //}
            this.Where(list);
   
            return this;
        }
        public IDeleteable<T> WhereColumns(List<Dictionary<string, object>> list) 
        {
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            foreach (var model in list)
            {
                int i = 0;
                var clist = new List<KeyValuePair<WhereType, ConditionalModel>>();
                foreach (var item in model.Keys)
                {
                    clist.Add(new KeyValuePair<WhereType, ConditionalModel>(i == 0 ? WhereType.Or : WhereType.And, new ConditionalModel()
                    {
                        FieldName =item,
                        ConditionalType = ConditionalType.Equal,
                        FieldValue = model[item].ObjToString(),
                        CSharpTypeName = model[item]==null?null : model[item].GetType().Name
                    }));
                    i++;
                }
                conditionalModels.Add(new ConditionalCollections()
                {
                    ConditionalList = clist
                });
            }
            return this.Where(conditionalModels);
        }
        public IDeleteable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }
        public IDeleteable<T> EnableQueryFilter()
        {
            var queryable = this.Context.Queryable<T>();
            queryable.QueryBuilder.LambdaExpressions.ParameterIndex= 1000;
            var sqlable= queryable.ToSql();
            var whereInfos = Regex.Split(sqlable.Key, " Where ", RegexOptions.IgnoreCase);
            if (whereInfos.Length > 1)
            {
                this.Where(whereInfos.Last(), sqlable.Value);
            }
            return this;
        }
        public IDeleteable<T> EnableQueryFilter(Type type)
        {
            var queryable = this.Context.Queryable<T>().Filter(type);
            queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 1000;
            var sqlable = queryable.ToSql();
            var whereInfos = Regex.Split(sqlable.Key, " Where ", RegexOptions.IgnoreCase);
            if (whereInfos.Length > 1)
            {
                this.Where(whereInfos.Last(), sqlable.Value);
            }
            return this;
        }
        public SplitTableDeleteProvider<T> SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc) 
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            this.Context.MappingTables.Add(this.EntityInfo.EntityName, this.EntityInfo.DbTableName);
            SplitTableDeleteProvider<T> result = new SplitTableDeleteProvider<T>();
            result.Context = this.Context;
            SplitTableContext helper = new SplitTableContext((SqlSugarProvider)Context)
            {
                EntityInfo = this.EntityInfo
            };
            var tables = getTableNamesFunc(helper.GetTables());
            result.Tables = tables;
            result.deleteobj = this;
            return result;
        }
        public SplitTableDeleteByObjectProvider<T> SplitTable()
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            SplitTableDeleteByObjectProvider<T> result = new SplitTableDeleteByObjectProvider<T>();
            result.Context = this.Context;
            Check.ExceptionEasy(this.DeleteObjects == null, "SplitTable() +0  only List<T> can be deleted", "SplitTable()无参数重载只支持根据实体集合删除");
            result.deleteObjects = this.DeleteObjects.ToArray();
            SplitTableContext helper = new SplitTableContext((SqlSugarProvider)Context)
            {
                EntityInfo = this.EntityInfo
            };
            result.deleteobj = this;
            return result;
        }
        public LogicDeleteProvider<T> IsLogic() 
        {
            LogicDeleteProvider<T> result = new LogicDeleteProvider<T>();
            result.DeleteBuilder = this.DeleteBuilder;
            result.Deleteable = this;
            return result;
        }
        public IDeleteable<T> RemoveDataCache(string likeString)
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCacheByLike(cacheService, likeString);
            };
            return this;
        }
        public IDeleteable<T> In<PkType>(List<PkType> primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            return In<PkType>(primaryKeyValues.ToArray());
        }

        public IDeleteable<T> In<PkType>(PkType[] primaryKeyValues)
        {
            if (primaryKeyValues == null || primaryKeyValues.Count() == 0)
            {
                Where(SqlBuilder.SqlFalse);
                return this;
            }
            string tableName = this.Context.EntityMaintenance.GetTableName<T>();
            string primaryField = null;
            primaryField = GetPrimaryKeys().FirstOrDefault();
            Check.ArgumentNullException(primaryField, "Table " + tableName + " with no primarykey");
            if (primaryKeyValues.Length < 10000)
            {
                Where(string.Format(DeleteBuilder.WhereInTemplate, SqlBuilder.GetTranslationColumnName(primaryField), primaryKeyValues.ToJoinSqlInVals()));
            }
            else
            {
                if (DeleteBuilder.BigDataInValues == null)
                    DeleteBuilder.BigDataInValues = new List<object>();
                DeleteBuilder.BigDataInValues.AddRange(primaryKeyValues.Select(it => (object)it));
                DeleteBuilder.BigDataFiled = primaryField;
            }
            return this;
        }

        public IDeleteable<T> In<PkType>(PkType primaryKeyValue)
        {
            if (typeof(PkType).FullName.IsCollectionsList())
            {
                var newValues = new List<object>();
                foreach (var item in primaryKeyValue as IEnumerable)
                {
                    newValues.Add(item);
                }
                return In(newValues);
            }


            In(new PkType[] { primaryKeyValue });
            return this;
        }

        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, PkType primaryKeyValue)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValue);
            tempPrimaryKeys = null;
            return this;
        }
        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, PkType[] primaryKeyValues)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValues);
            tempPrimaryKeys = null;
            return this;
        }
        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, List<PkType> primaryKeyValues)
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            tempPrimaryKeys = new List<string>() { fieldName };
            var result = In(primaryKeyValues);
            tempPrimaryKeys = null;
            return this;
        }

        public IDeleteable<T> In<PkType>(Expression<Func<T, object>> inField, ISugarQueryable<PkType> childQueryExpression) 
        {
            var lamResult = DeleteBuilder.GetExpressionValue(inField, ResolveExpressType.FieldSingle);
            var fieldName = lamResult.GetResultString();
            var sql= childQueryExpression.ToSql();
            Where($" {fieldName} IN ( SELECT {fieldName} FROM ( {sql.Key} ) SUBDEL) ",sql.Value);
            return this;
        }
        public IDeleteable<T> In<PkType>(string inField, List<PkType> primaryKeyValues)
        {
            tempPrimaryKeys = new List<string>() { inField };
            var result = In(primaryKeyValues);
            tempPrimaryKeys = null;
            return this;
        }

        public DeleteablePage<T> PageSize(int pageSize) 
        {
            Check.ExceptionEasy(this.DeleteObjects == null, "PageSize can only be deleted as a List<Class> entity collection", "Deleteable.PageSize()只能是List<Class>实体集合方式删除,并且集合不能为null");
            DeleteablePage<T> result = new DeleteablePage<T>();
            result.DataList = this.DeleteObjects.ToArray();
            result.Context = this.Context;
            result.DiffModel = this.diffModel;
            result.IsEnableDiffLogEvent= this.IsEnableDiffLogEvent;
            result.TableName = this.DeleteBuilder.AsName;
            result.PageSize =  pageSize;
            return result;
        }
        public IDeleteable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                DeleteBuilder.TableWithString = lockString;
            return this;
        }
        public virtual string ToSqlString()
        {
            var sqlObj = this.ToSql();
            var result = sqlObj.Key;
            if (result == null) return null;
            result = UtilMethods.GetSqlString(this.Context.CurrentConnectionConfig, sqlObj);
            return result;
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            string sql = DeleteBuilder.ToSqlString();
            var paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToList();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, paramters);
        }

        private List<string> GetPrimaryKeys()
        {
            if (tempPrimaryKeys.HasValue())
            {
                return tempPrimaryKeys;
            }
            else if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetPrimaries(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsPrimarykey).Select(it => it.DbColumnName).ToList();
            }
        }
        private void _ExecuteCommand(out string sql, out SugarParameter[] paramters)
        {
            DeleteBuilder.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            sql = DeleteBuilder.ToSqlString();
            paramters = DeleteBuilder.Parameters == null ? null : DeleteBuilder.Parameters.ToArray();
            RestoreMapping();
            AutoRemoveDataCache();
            Before(sql);
        }

        protected virtual List<string> GetIdentityKeys()
        {
            if (this.Context.IsSystemTablesConfig)
            {
                return this.Context.DbMaintenance.GetIsIdentities(this.Context.EntityMaintenance.GetTableName(this.EntityInfo.EntityName));
            }
            else
            {
                return this.EntityInfo.Columns.Where(it => it.IsIdentity).Select(it => it.DbColumnName).ToList();
            }
        }

        private void RestoreMapping()
        {
            if (IsAs)
            {
                this.Context.MappingTables = OldMappingTableList;
            }
        }

        //private void TaskStart<Type>(Task<Type> result)
        //{
        //    if (this.Context.CurrentConnectionConfig.IsShardSameThread)
        //    {
        //        Check.Exception(true, "IsShardSameThread=true can't be used async method");
        //    }
        //    result.Start();
        //}

        private void AutoRemoveDataCache()
        {
            var moreSetts = this.Context.CurrentConnectionConfig.MoreSettings;
            var extService = this.Context.CurrentConnectionConfig.ConfigureExternalServices;
            if (moreSetts != null && moreSetts.IsAutoRemoveDataCache && extService != null && extService.DataInfoCacheService != null)
            {
                this.RemoveDataCache();
            }
        }


        private void After(string sql)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = DeleteBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.AfterData = null;
                diffModel.Time = this.Context.Ado.SqlExecutionTime;
                if (this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent != null)
                    this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent(diffModel);
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
            if (this.RemoveCacheFunc != null) {
                this.RemoveCacheFunc();
            }
        }

        private void Before(string sql)
        {
            if (this.IsEnableDiffLogEvent)
            {
                var isDisableMasterSlaveSeparation = this.Context.Ado.IsDisableMasterSlaveSeparation;
                this.Context.Ado.IsDisableMasterSlaveSeparation = true;
                var parameters = DeleteBuilder.Parameters;
                if (parameters == null)
                    parameters = new List<SugarParameter>();
                diffModel.BeforeData = GetDiffTable(sql, parameters);
                diffModel.Sql = sql;
                diffModel.Parameters = parameters.ToArray();
                this.Context.Ado.IsDisableMasterSlaveSeparation = isDisableMasterSlaveSeparation;
            }
        }

        private List<DiffLogTableInfo> GetDiffTable(string sql, List<SugarParameter> parameters)
        {
            List<DiffLogTableInfo> result = new List<DiffLogTableInfo>();
            var whereSql = Regex.Replace(sql, ".* WHERE ", "", RegexOptions.Singleline);
            var dt = this.Context.Queryable<T>().AS(this.DeleteBuilder.AsName).Filter(null, true).Where(whereSql).AddParameters(parameters).ToDataTable();
            if (dt.Rows != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    DiffLogTableInfo item = new DiffLogTableInfo();
                    item.TableDescription = this.EntityInfo.TableDescription;
                    item.TableName = this.EntityInfo.DbTableName;
                    item.Columns = new List<DiffLogColumnInfo>();
                    foreach (DataColumn col in dt.Columns)
                    {
                        var sugarColumn = this.EntityInfo.Columns.Where(it => it.DbColumnName != null).First(it =>
                            it.DbColumnName.Equals(col.ColumnName, StringComparison.CurrentCultureIgnoreCase));
                        DiffLogColumnInfo addItem = new DiffLogColumnInfo();
                        addItem.Value = row[col.ColumnName];
                        addItem.ColumnName = col.ColumnName;
                        addItem.IsPrimaryKey = sugarColumn.IsPrimarykey;
                        addItem.ColumnDescription = sugarColumn.ColumnDescription;
                        item.Columns.Add(addItem);
                    }
                    result.Add(item);
                }
            }
            return result;
        }
        private void DataAop(object deleteObj)
        {
            var dataEvent = this.Context.CurrentConnectionConfig.AopEvents?.DataExecuting;
            if (deleteObj != null&& dataEvent!=null)
            {
                var model = new DataFilterModel()
                {
                    OperationType = DataFilterType.DeleteByObject,
                    EntityValue = deleteObj,
                    EntityColumnInfo=this.EntityInfo.Columns.FirstOrDefault() 
                };
                dataEvent(deleteObj,model);
            }
        }
    }
}
