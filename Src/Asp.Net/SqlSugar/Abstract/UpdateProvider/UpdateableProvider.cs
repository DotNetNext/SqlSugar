using System;
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
    public partial class UpdateableProvider<T> : IUpdateable<T> where T : class, new()  
    {
        #region Property
        public SqlSugarProvider Context { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public ISqlBuilder SqlBuilder { get; internal set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public T[] UpdateObjs { get; set; }
        /// <summary>
        /// true : by expression  update
        /// false: by object update
        /// </summary>
        public bool UpdateParameterIsNull { get; set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.UpdateObjs.Length == 1; } }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        private List<string> WhereColumnList { get; set; }
        private bool IsWhereColumns { get; set; }
        private bool IsOffIdentity { get; set; }
        private bool IsVersionValidation { get; set; }
        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        internal Action RemoveCacheFunc { get; set; }
        private int SetColumnsIndex { get; set; }
        private List<DbColumnInfo> columns { get; set; }
        #endregion

        #region Core
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
            PreToSql();
            string sql = UpdateBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, UpdateBuilder.Parameters);
        }
        public void AddQueue()
        {
            var sqlObj = this.ToSql();
            this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
        }
        public virtual int ExecuteCommandWithOptLockIF(bool? IsVersionValidation ,bool? IsOptLock=null) 
        {
            if (IsOptLock==true)
            {
                return ExecuteCommandWithOptLock(IsVersionValidation??false);
            }
            else 
            {
                return this.ExecuteCommand();
            }
        }
        public virtual int ExecuteCommandWithOptLock(bool IsVersionValidation=false)
        {
            Check.ExceptionEasy(UpdateObjs?.Length>1, " OptLock can only be used on a single object, and the argument cannot be List", "乐观锁只能用于单个对象,参数不能是List,如果是一对多操作请更新主表统一用主表验证");
            var updateData = UpdateObjs.FirstOrDefault();
            if (updateData == null) return 0;
            object oldValue = null;
            var name=_ExecuteCommandWithOptLock(updateData,ref oldValue);
            var result= this.ExecuteCommand();
            OptRollBack(result,updateData, oldValue, name);
            if (result == 0 && IsVersionValidation)
            {
                throw new VersionExceptions(string.Format("UpdateVersionValidation {0} Not the latest version ", name));
            }
            return result;
        }

        public virtual int ExecuteCommand()
        {
            //if (this.UpdateBuilder.UpdateColumns.HasValue())
            //{
            //    var columns = this.UpdateBuilder.UpdateColumns;
            //    this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => GetPrimaryKeys().Select(iit => iit.ToLower()).Contains(it.DbColumnName.ToLower()) || columns.Contains(it.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            //}
            if (this.IsTrakingDatas() || IsUpdateNullByList())
            {
                int trakRows = DatasTrackingExecommand();
                return trakRows;
            }
            string sql = _ExecuteCommand();
            if (this.UpdateBuilder.AppendWhere.HasValue()) 
            {
                sql += " AND "+ this.UpdateBuilder.AppendWhere;
            }
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            var result = this.Ado.ExecuteCommand(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
            After(sql);
            return result;
        }


        public bool ExecuteCommandHasChange()
        {
            return this.ExecuteCommand() > 0;
        }

        public virtual async Task<int> ExecuteCommandWithOptLockAsync(bool IsVersionValidation = false)
        {
            Check.ExceptionEasy(UpdateObjs?.Length > 1, " OptLock can only be used on a single object, and the argument cannot be List", "乐观锁只能用于单个对象,参数不能是List,如果是一对多操作请更新主表统一用主表验证");
            var updateData = UpdateObjs.FirstOrDefault();
            if (updateData == null) return 0;
            object oldValue = null;
            var name=_ExecuteCommandWithOptLock(updateData,ref oldValue);
            var result= await this.ExecuteCommandAsync();
            OptRollBack(result,updateData, oldValue, name);
            if (result == 0 && IsVersionValidation) 
            {
                throw new VersionExceptions(string.Format("UpdateVersionValidation {0} Not the latest version ", name));
            }
            return result;
        }
        public Task<int> ExecuteCommandAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteCommandAsync();
        }
        public virtual async Task<int> ExecuteCommandAsync()
        {
            //if (this.UpdateBuilder.UpdateColumns.HasValue())
            //{
            //    var columns = this.UpdateBuilder.UpdateColumns;
            //    this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => GetPrimaryKeys().Select(iit => iit.ToLower()).Contains(it.DbColumnName.ToLower()) || columns.Contains(it.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            //}
            if (this.IsTrakingDatas()||IsUpdateNullByList())
            {
                int trakRows =await DatasTrackingExecommandAsync();
                return trakRows;
            }
            string sql = _ExecuteCommand();
            if (this.UpdateBuilder.AppendWhere.HasValue())
            {
                sql += " AND " + this.UpdateBuilder.AppendWhere;
            }
            if (string.IsNullOrEmpty(sql))
            {
                return 0;
            }
            var result = await this.Ado.ExecuteCommandAsync(sql, UpdateBuilder.Parameters == null ? null : UpdateBuilder.Parameters.ToArray());
            After(sql);
            return result;
        }
        public Task<bool> ExecuteCommandHasChangeAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> ExecuteCommandHasChangeAsync()
        {
            return await this.ExecuteCommandAsync() > 0;
        }
        #endregion

        #region Common
        public UpdateablePage<T> PageSize(int pageSize) 
        {
            ThrowUpdateByExpressionByMesage(" PageSize(num) ");
            UpdateablePage<T> result = new UpdateablePage<T>();
            result.PageSize = pageSize;
            result.Context = this.Context;
            result.DataList = this.UpdateObjs;
            result.TableName = this.UpdateBuilder.TableName;
            result.IsEnableDiffLogEvent = this.IsEnableDiffLogEvent;
            result.WhereColumnList = this.WhereColumnList?.ToArray();
            result.DiffModel = this.diffModel; 
            if (this.UpdateBuilder.DbColumnInfoList.Any())
                result.UpdateColumns = this.UpdateBuilder.DbColumnInfoList.GroupBy(it => it.TableId).First().Select(it => it.DbColumnName).ToList();
            return result;
        }
        public IUpdateable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpress) 
        {
            UpdateableProvider<T, T2> result = new UpdateableProvider<T, T2>();
            result.updateableObj = this;
            var querybale=this.Context.Queryable<T>().LeftJoin<T2>(joinExpress);
            result.updateableObj.UpdateBuilder.JoinInfos = querybale.QueryBuilder.JoinQueryInfos;
            result.updateableObj.UpdateBuilder.ShortName = joinExpress.Parameters.FirstOrDefault()?.Name;
            return result;
        }
        public IUpdateable<T, T2> InnerJoin<T2>(Expression<Func<T, T2, bool>> joinExpress,string TableName)
        {
            UpdateableProvider<T, T2> result = new UpdateableProvider<T, T2>();
            result.updateableObj = this;
            var querybale = this.Context.Queryable<T>().LeftJoin<T2>(joinExpress);
            result.updateableObj.UpdateBuilder.JoinInfos = querybale.QueryBuilder.JoinQueryInfos;
            result.updateableObj.UpdateBuilder.ShortName = joinExpress.Parameters.FirstOrDefault()?.Name;
            result.updateableObj.UpdateBuilder.TableName = TableName;
            return result;
        }
        public IUpdateable<T> Clone() 
        {
            this.Context.SugarActionType = SugarActionType.Update;
            var result = InstanceFactory.GetUpdateableProvider<T>(this.Context.CurrentConnectionConfig);
            var sqlBuilder = InstanceFactory.GetSqlbuilder(this.Context.CurrentConnectionConfig); ;
            result.Context = this.Context;
            result.EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>();
            result.SqlBuilder = sqlBuilder;
            result.SqlBuilder.Context = this.Context;
            result.UpdateObjs = UpdateObjs;
            result.WhereColumnList= this.WhereColumnList;
            result.IsWhereColumns = this.IsWhereColumns;
            result.IgnoreColumnNameList= this.IgnoreColumnNameList;
            result.IsAs = this.IsAs;
            result.IsOffIdentity= this.IsOffIdentity;
            result.IsEnableDiffLogEvent= this.IsEnableDiffLogEvent;
            result.diffModel = this.diffModel;
            result.UpdateParameterIsNull= this.UpdateParameterIsNull;
            result.RemoveCacheFunc= this.RemoveCacheFunc;
            result.SetColumnsIndex = this.SetColumnsIndex;
            result.OldMappingTableList= this.OldMappingTableList;
            result.MappingColumnList= this.MappingColumnList;
            result.columns = this.columns.ToList();
            result.UpdateBuilder = InstanceFactory.GetUpdateBuilder(this.Context.CurrentConnectionConfig);
            result.UpdateBuilder.Builder = sqlBuilder;
            result.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            result.Context=this.Context;
            result.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.ToList();
            result.UpdateBuilder.TableName= this.UpdateBuilder.TableName;
            result.UpdateBuilder.WhereValues = this.UpdateBuilder?.WhereValues?.ToList();
            result.UpdateBuilder.Parameters= this.UpdateBuilder?.Parameters?.ToList();
            result.UpdateBuilder.IsListUpdate= this.UpdateBuilder.IsListUpdate;
            result.UpdateBuilder.IsWhereColumns= this.UpdateBuilder.IsWhereColumns;
            result.UpdateBuilder.WhereValues=this.UpdateBuilder?.WhereValues?.ToList();
            result.UpdateBuilder.IsNoUpdateDefaultValue = this.UpdateBuilder.IsNoUpdateDefaultValue;
            result.UpdateBuilder.IsNoUpdateNull= this.UpdateBuilder.IsNoUpdateNull;
            result.UpdateBuilder.SetValues= this.UpdateBuilder?.SetValues?.ToList();
            result.UpdateBuilder.UpdateColumns = this.UpdateBuilder?.UpdateColumns?.ToList();
            result.UpdateBuilder.Context = this.Context;
            return result;
        }
        public IUpdateable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.UpdateBuilder.TableWithString = lockString;
            return this;
        }
        public SplitTableUpdateProvider<T> SplitTable(Func<List<SplitTableInfo>, IEnumerable<SplitTableInfo>> getTableNamesFunc)
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            this.Context.MappingTables.Add(this.EntityInfo.EntityName, this.EntityInfo.DbTableName);
            SplitTableUpdateProvider<T> result = new SplitTableUpdateProvider<T>();
            result.Context = this.Context;
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.EntityInfo
            };
            var tables = getTableNamesFunc(helper.GetTables());
            result.Tables = tables;
            result.updateobj = this;
            return result;
        }
        public SplitTableUpdateByObjectProvider<T> SplitTable()
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            Check.ExceptionEasy(UpdateParameterIsNull, "SplitTable() not supported db.Updateable<T>(),use db.Updateable(list)", ".SplitTable()不支持 db.Updateable<T>()方式更新,请使用 db.Updateable(list) 对象方式更新, 或者使用 .SplitTable(+1)重载");
            SplitTableUpdateByObjectProvider<T> result = new SplitTableUpdateByObjectProvider<T>();
            result.Context = this.Context;
            result.UpdateObjects = this.UpdateObjs;
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.EntityInfo
            };
            result.updateobj = this;
            return result;
        }
        public IUpdateable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }
        public IUpdateable<T> RemoveDataCache(string likeString)
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCacheByLike(cacheService, likeString);
            };
            return this;
        }
        public IUpdateable<T> IsEnableUpdateVersionValidation()
        {
            this.IsVersionValidation = true;
            return this;
        }
        public IUpdateable<T> AsType(Type tableNameType) 
        {
            return AS(this.Context.EntityMaintenance.GetEntityInfo(tableNameType).DbTableName);
        }
        public IUpdateable<T> AS(string tableName)
        {
            //if (tableName == null) return this;
            //var entityName = typeof(T).Name;
            //IsAs = true;
            //OldMappingTableList = this.Context.MappingTables;
            //this.Context.MappingTables = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            //if (this.Context.MappingTables.Any(it => it.EntityName == entityName))
            //{
            //    this.Context.MappingTables.Add(this.Context.MappingTables.First(it => it.EntityName == entityName).DbTableName, tableName);
            //}
            //this.Context.MappingTables.Add(entityName, tableName);
            this.UpdateBuilder.TableName = tableName;
            if (tableName.IsNullOrEmpty())
                this.UpdateBuilder.TableName = this.EntityInfo.DbTableName;
            return this; ;
        }
        public IUpdateable<T> EnableDiffLogEventIF(bool isEnableDiffLog, object businessData = null) 
        {
            if (isEnableDiffLog) 
            {
                return EnableDiffLogEvent(businessData);
            }
            return this;
        }
        public IUpdateable<T> EnableDiffLogEvent(object businessData = null)
        {
            //Check.Exception(this.UpdateObjs.HasValue() && this.UpdateObjs.Count() > 1, "DiffLog does not support batch operations");
            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.update;
            return this;
        }



        public IUpdateable<T> IgnoreColumns(bool ignoreAllNullColumns, bool isOffIdentity = false, bool ignoreAllDefaultValue = false)
        {
            //Check.Exception(this.UpdateObjs.Count() > 1 && ignoreAllNullColumns, ErrorMessage.GetThrowMessage("ignoreNullColumn NoSupport batch insert", "ignoreNullColumn 不支持批量操作"));
            UpdateBuilder.IsOffIdentity = isOffIdentity;
            if (this.UpdateBuilder.LambdaExpressions == null)
                this.UpdateBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.UpdateBuilder.IsNoUpdateNull = ignoreAllNullColumns;
            this.UpdateBuilder.IsNoUpdateDefaultValue = ignoreAllDefaultValue;
            return this;
        }
        public IUpdateable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            var ignoreColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it).ToLower()).ToList();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName.ToLower())).ToList();
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.DbColumnName.ToLower())).ToList();
            return this;
        }
        public IUpdateable<T> IgnoreColumnsIF(bool IsIgnore, Expression<Func<T, object>> columns)
        {
            if (IsIgnore) this.IgnoreColumns(columns);
            return this;
        }
        public IUpdateable<T> IgnoreNullColumns(bool isIgnoreNull = true) 
        {
            if (isIgnoreNull)
            {
                return IgnoreColumns(isIgnoreNull);
            }
            else
            {
                return this;
            }
        }
        public IUpdateable<T> IgnoreColumns(string[] columns)
        {
            if (columns.HasValue())
            {
                var ignoreColumns = columns.Select(it => it.ToLower()).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.PropertyName.ToLower())).ToList();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Contains(it.DbColumnName.ToLower())).ToList();
            }
            return this;
        }


        public IUpdateable<T> ReSetValue(Action<T> setValueExpression)
        {
            ThrowUpdateByExpression();
            if (this.UpdateObjs.HasValue())
            {
                var oldColumns = this.UpdateBuilder.DbColumnInfoList.Select(it => it.PropertyName).ToList();
                foreach (var item in UpdateObjs)
                {
                    setValueExpression(item);
                }
                this.UpdateBuilder.DbColumnInfoList = new List<DbColumnInfo>();
                Init();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => oldColumns.Contains(it.PropertyName)).ToList();
            }
            return this;
        }
        public IUpdateable<T> PublicSetColumns(Expression<Func<T, object>> filedNameExpression, string computationalSymbol) 
        {
            if (UpdateParameterIsNull == true)
            {
                Check.Exception(UpdateParameterIsNull == true, ErrorMessage.GetThrowMessage("The PublicSetColumns(exp,string) overload can only be used to update entity objects: db.Updateable(object)", "PublicSetColumns(exp,string)重载只能用在实体对象更新：db.Updateable(对象)，请区分表达式更新和实体对象更不同用法 "));
            }
            else
            {
                var name = ExpressionTool.GetMemberName(filedNameExpression);
                if (name == null)
                {
                    Check.ExceptionEasy(filedNameExpression + " format error ", filedNameExpression + "参数格式错误");
                }
                //var value = this.UpdateBuilder.GetExpressionValue(ValueExpExpression, ResolveExpressType.WhereSingle).GetResultString();
                if (this.UpdateBuilder.ReSetValueBySqlExpList == null)
                {
                    this.UpdateBuilder.ReSetValueBySqlExpList = new Dictionary<string, ReSetValueBySqlExpListModel>();
                }
                if (!this.UpdateBuilder.ReSetValueBySqlExpList.ContainsKey(name))
                {
                    this.UpdateBuilder.ReSetValueBySqlExpList.Add(name, new ReSetValueBySqlExpListModel()
                    {
                        Type= ReSetValueBySqlExpListModelType.List,
                        Sql = computationalSymbol,
                        DbColumnName = this.SqlBuilder.GetTranslationColumnName(this.EntityInfo.Columns.First(it => it.PropertyName == name).DbColumnName)
                    });  
                }
            }
            return this;
        }

        public IUpdateable<T> PublicSetColumns(Expression<Func<T, object>> filedNameExpression, Expression<Func<T, object>> ValueExpExpression) 
        {
            if (UpdateParameterIsNull == true)
            {
                return SetColumns(filedNameExpression, ValueExpExpression);
            }
            else
            {
                var name = ExpressionTool.GetMemberName(filedNameExpression);
                if (name == null)
                {
                    Check.ExceptionEasy(filedNameExpression + " format error ", filedNameExpression + "参数格式错误");
                }
                var value = this.UpdateBuilder.GetExpressionValue(ValueExpExpression, ResolveExpressType.WhereSingle).GetResultString();
                if (this.UpdateBuilder.ReSetValueBySqlExpList == null)
                {
                    this.UpdateBuilder.ReSetValueBySqlExpList = new Dictionary<string, ReSetValueBySqlExpListModel>();
                }
                if (!this.UpdateBuilder.ReSetValueBySqlExpList.ContainsKey(name))
                {
                    this.UpdateBuilder.ReSetValueBySqlExpList.Add(name, new ReSetValueBySqlExpListModel()
                    {
                        Sql = value,
                        DbColumnName =this.SqlBuilder.GetTranslationColumnName(this.EntityInfo.Columns.First(it => it.PropertyName == name).DbColumnName)
                    }); ;
                }
            }
            return this;
        }
        #endregion

        #region Update by object
        public IUpdateable<T> CallEntityMethod(Expression<Action<T>> method)
        {
            ThrowUpdateByExpression();
            if (this.UpdateObjs.HasValue())
            {
                var oldColumns = this.UpdateBuilder.DbColumnInfoList.Select(it => it.PropertyName).ToList();
                var expression = (LambdaExpression.Lambda(method).Body as LambdaExpression).Body;
                Check.Exception(!(expression is MethodCallExpression), method.ToString() + " is not method");
                var callExpresion = expression as MethodCallExpression;
                UtilMethods.DataInoveByExpresson(this.UpdateObjs, callExpresion);
                this.UpdateBuilder.DbColumnInfoList = new List<DbColumnInfo>();
                Init();
                this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => oldColumns.Contains(it.PropertyName)).ToList();
            }
            return this;
        }

        public IUpdateable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            this.IsWhereColumns = true;
            UpdateBuilder.IsWhereColumns = true;
            Check.Exception(UpdateParameterIsNull == true, "Updateable<T>().Updateable is error,Use Updateable(obj).WhereColumns");
            var whereColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            foreach (var item in whereColumns)
            {
                _WhereColumn(item);
                var columnInfo=this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName.EqualCase(item));
                if (columnInfo != null)
                {
                    this.WhereColumnList.Add(columnInfo.DbColumnName);
                }
                else
                {
                    this.WhereColumnList.Add(item);
                }
            }
            return this;
        }
        public IUpdateable<T> WhereColumns(string columnName)
        {

            ThrowUpdateByExpression();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            _WhereColumn(columnName);
            this.WhereColumnList.Add(columnName);
            return this;
        }

        public IUpdateable<T> WhereColumns(string[] columnNames)
        {
            if (columnNames == null) return this;

            ThrowUpdateByExpression();
            if (this.WhereColumnList == null) this.WhereColumnList = new List<string>();
            foreach (var columnName in columnNames)
            {
                _WhereColumn(columnName);
                this.WhereColumnList.Add(columnName);
            }
            return this;
        }
        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns, bool appendColumnsByDataFilter) 
        {
            ThrowUpdateByExpression();
            var updateColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            return UpdateColumns(updateColumns.ToArray(), appendColumnsByDataFilter);
        }
        public IUpdateable<T> UpdateColumns(Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            var updateColumns = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            if (this.UpdateBuilder.UpdateColumns == null)
            {
                this.UpdateBuilder.UpdateColumns = new List<string>();
            }
            this.UpdateBuilder.UpdateColumns.AddRange(updateColumns);
            //List<string> primaryKeys = GetPrimaryKeys();
            //foreach (var item in this.UpdateBuilder.DbColumnInfoList)
            //{
            //    var mappingInfo = primaryKeys.SingleOrDefault(i => item.DbColumnName.Equals(i, StringComparison.CurrentCultureIgnoreCase));
            //    if (mappingInfo != null && mappingInfo.Any())
            //    {
            //        item.IsPrimarykey = true;
            //    }
            //}
            //this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => updateColumns.Any(uc => uc.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase) || uc.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey || it.IsIdentity).ToList();
            return this;
        }
        public IUpdateable<T> UpdateColumns(string[] columns, bool appendColumnsByDataFilter) 
        {
            List<string> updateColumns = new List<string>();
            if (appendColumnsByDataFilter)
            {
                var newData = new T() { };
                UtilMethods.ClearPublicProperties(newData, this.EntityInfo);
                var data = ((UpdateableProvider<T>)this.Context.Updateable(newData)).UpdateObjs.First();
                foreach (var item in this.EntityInfo.Columns.Where(it => !it.IsPrimarykey && !it.IsIgnore && !it.IsOnlyIgnoreUpdate))
                {
                    var value = item.PropertyInfo.GetValue(data);
                    if (value != null && !value.Equals(""))
                    {
                        if (!value.Equals(UtilMethods.GetDefaultValue(item.UnderType)))
                        {
                            updateColumns.Add(item.PropertyName);
                        }
                    }
                }
            }
            updateColumns.AddRange(columns);
            return UpdateColumns(updateColumns.ToArray());
        }
        public IUpdateable<T> UpdateColumns(string[] columns)
        {
            if (columns == null||columns.Length==0) return this;
            ThrowUpdateByExpression();
            if (this.UpdateBuilder.UpdateColumns == null) 
            {
                this.UpdateBuilder.UpdateColumns = new List<string>();
            }
            this.UpdateBuilder.UpdateColumns.AddRange(columns);
            //if (columns.HasValue())
            //{
              
            //    //this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => GetPrimaryKeys().Select(iit => iit.ToLower()).Contains(it.DbColumnName.ToLower()) || columns.Contains(it.PropertyName, StringComparer.OrdinalIgnoreCase)).ToList();
            //}
            return this;
        }
        public IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, Expression<Func<T, object>> columns)
        {
            ThrowUpdateByExpression();
            if (isUpdateColumns)
                UpdateColumns(columns);
            return this;
        }
        public IUpdateable<T> UpdateColumnsIF(bool isUpdateColumns, string[] columns)
        {
            ThrowUpdateByExpression();
            if (isUpdateColumns)
                UpdateColumns(columns);
            return this;
        }
        #endregion

        #region Update by expression
        public IUpdateable<T> EnableQueryFilter() 
        {
            try
            {
                ThrowUpdateByObject();
            }
            catch 
            {
                Check.ExceptionEasy("Updateable<T>(obj) no support, use Updateable<T>().SetColumn ", "更新过滤器只能用在表达式方式更新 ,更新分为实体更新和表达式更新 。正确用法 Updateable<T>().SetColum(..).Where(..)");
            }
            var queryable = this.Context.Queryable<T>();
            queryable.QueryBuilder.LambdaExpressions.ParameterIndex = 1000;
            var sqlable = queryable.ToSql();
            var whereInfos = Regex.Split(sqlable.Key, " Where ", RegexOptions.IgnoreCase);
            if (whereInfos.Length > 1)
            {
                this.Where(whereInfos.Last(), sqlable.Value);
            }
            return this;
        }
        public IUpdateable<T> SetColumns(string fieldName, object fieldValue) 
        {
            ThrowUpdateByObject();
            var columnInfo = this.EntityInfo.Columns.FirstOrDefault(it => it.PropertyName.EqualCase(fieldName));
            if (columnInfo != null) 
            {
                fieldName = columnInfo.DbColumnName;
            }
            var parameterName =this.SqlBuilder.SqlParameterKeyWord+ "Const" + this.UpdateBuilder.LambdaExpressions.ParameterIndex;
            this.UpdateBuilder.LambdaExpressions.ParameterIndex = this.UpdateBuilder.LambdaExpressions.ParameterIndex+1;
            if (UpdateBuilder.Parameters == null) 
            {
                UpdateBuilder.Parameters = new List<SugarParameter>();
            }
            UpdateBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            if (columnInfo?.UpdateServerTime == true)
            {
                var nowTime= this.Context.Queryable<object>().QueryBuilder.LambdaExpressions.DbMehtods.GetDate();
                UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(fieldName), $"{SqlBuilder.GetTranslationColumnName(fieldName)}={nowTime}"));
            }
            else
            {
                UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(fieldName), $"{SqlBuilder.GetTranslationColumnName(fieldName)}={parameterName}"));
            }
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            if (!this.UpdateBuilder.DbColumnInfoList.Any(it => it.DbColumnName.EqualCase(fieldName))) 
            {
                this.UpdateBuilder.DbColumnInfoList.Add(new DbColumnInfo()
                {
                     DbColumnName=fieldName,
                      Value=fieldValue,
                      PropertyName=fieldName,
                      PropertyType=fieldValue?.GetType()
                });
            }
            AppendSets();
            return this;
        }
        public IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, object>> filedNameExpression, object fieldValue) 
        {
            if (isUpdateColumns)
            {
                return SetColumns(filedNameExpression, fieldValue);
            }
            else
            {
                return this;
            }
        }
        public IUpdateable<T> SetColumns(Expression<Func<T, object>> filedNameExpression, Expression<Func<T, object>> valueExpression) 
        {
            if (valueExpression == null) 
            {
                return SetColumns(filedNameExpression,(object)null);
            }
            var name = UpdateBuilder.GetExpressionValue(filedNameExpression, ResolveExpressType.FieldSingle).GetString();
            name = UpdateBuilder.Builder.GetNoTranslationColumnName(name);
            var value = UpdateBuilder.GetExpressionValue(valueExpression, ResolveExpressType.FieldSingle).GetString();
            this.UpdateBuilder.DbColumnInfoList.Add(new DbColumnInfo()
            {
                DbColumnName = name,
                Value = value,
                PropertyName = name ,
                SqlParameterDbType=typeof(SqlSugar.DbConvert.NoParameterCommonPropertyConvert)
            });
            this.UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(name,value));
            return this;
        }
        public IUpdateable<T> SetColumns(Expression<Func<T, object>> filedNameExpression, object fieldValue) 
        {
            var name= UpdateBuilder.GetExpressionValue(filedNameExpression,ResolveExpressType.FieldSingle).GetString();
            name = UpdateBuilder.Builder.GetNoTranslationColumnName(name);
            return SetColumns(name, fieldValue);
        }
        public IUpdateable<T> SetColumns(Expression<Func<T, T>> columns)
        {
            ThrowUpdateByObject();
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update);
            var resultArray = expResult.GetResultArray();
            Check.ArgumentNullException(resultArray, "UpdateColumns Parameter error, UpdateColumns(it=>new T{ it.id=1}) is valid, UpdateColumns(it=>T) is error");
            var keys= ExpressionTool.GetNewExpressionItemList(columns).ToArray();
            if (resultArray.HasValue())
            {
                int i = 0;
                foreach (var item in resultArray)
                {
                    string key = key = keys[i].Key;
                    i++;
                    var value = item;
                    if (value.Contains("= \"SYSDATE\""))
                    {
                        value = value.Replace("= \"SYSDATE\"", "= SYSDATE");
                    }
                    UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), value));
                }
            }
            this.UpdateBuilder.DbColumnInfoList = UpdateBuilder.DbColumnInfoList.Where(it =>it.UpdateServerTime==true||it.UpdateSql.HasValue()|| (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            CheckTranscodeing(false);
            AppendSets();
            return this;
        }


        public IUpdateable<T> SetColumns(Expression<Func<T, T>> columns, bool appendColumnsByDataFilter) 
        {
            ThrowUpdateByObject();
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.Update);
            var resultArray = expResult.GetResultArray();
            Check.ArgumentNullException(resultArray, "UpdateColumns Parameter error, UpdateColumns(it=>new T{ it.id=1}) is valid, UpdateColumns(it=>T) is error");
            if (resultArray.HasValue())
            {
                foreach (var item in resultArray)
                {
                    string key = SqlBuilder.GetNoTranslationColumnName(item);
                    var value = item;
                    if (value.Contains("= \"SYSDATE\""))
                    {
                        value = value.Replace("= \"SYSDATE\"", "= SYSDATE");
                    }
                    UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), value));
                }
            }
            if (appendColumnsByDataFilter)
            {
                var newData = new T() { };
                UtilMethods.ClearPublicProperties(newData, this.EntityInfo);
                var data = ((UpdateableProvider<T>)this.Context.Updateable(newData)).UpdateObjs.First();
                foreach (var item in this.EntityInfo.Columns.Where(it => !it.IsPrimarykey && !it.IsIgnore && !it.IsOnlyIgnoreUpdate))
                {
                    var value = item.PropertyInfo.GetValue(data);
                    if (value != null && !value.Equals(""))
                    {
                        if (!value.Equals(UtilMethods.GetDefaultValue(item.UnderType))) 
                        {
                            var pName = this.SqlBuilder.SqlParameterKeyWord + item.PropertyName + 1000;
                            var p = new SugarParameter(pName, value);
                            this.UpdateBuilder.Parameters.Add(p);
                            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(item.DbColumnName), SqlBuilder.GetTranslationColumnName(item.DbColumnName)+"="+pName));
                        }
                    }
                }
            }
            if (this.EntityInfo.Columns.Any(it => it.UpdateServerTime || it.UpdateSql.HasValue())) 
            {
                var appendColumns = this.EntityInfo.Columns.Where(it => it.UpdateServerTime || it.UpdateSql.HasValue());
                foreach (var item in appendColumns)
                {
                    if (item.UpdateServerTime)
                    {
                        UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(item.DbColumnName), SqlBuilder.GetTranslationColumnName(item.DbColumnName) + "=" + UpdateBuilder.LambdaExpressions.DbMehtods.GetDate()));
                    }
                    else if (item.UpdateSql.HasValue())
                    {
                        UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(item.DbColumnName), SqlBuilder.GetTranslationColumnName(item.DbColumnName) + "=" + item.UpdateSql));
                    }
                }
            }
            this.UpdateBuilder.DbColumnInfoList = UpdateBuilder.DbColumnInfoList.Where(it =>it.UpdateServerTime||it.UpdateSql.HasValue()|| (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            CheckTranscodeing(false);
            AppendSets();
            return this;
        }
        public IUpdateable<T> SetColumns(Expression<Func<T, bool>> columns)
        {
            ThrowUpdateByObject();

            var binaryExp = columns.Body as BinaryExpression;
            Check.Exception(!binaryExp.NodeType.IsIn(ExpressionType.Equal), "No support {0}", columns.ToString());
            Check.Exception(!(binaryExp.Left is MemberExpression) && !(binaryExp.Left is UnaryExpression), "No support {0}", columns.ToString());
            Check.Exception(ExpressionTool.IsConstExpression(binaryExp.Left as MemberExpression), "No support {0}", columns.ToString());
            var expResult = UpdateBuilder.GetExpressionValue(columns, ResolveExpressType.WhereSingle).GetResultString().Replace(")", " )").Replace("(", "( ").Trim().TrimStart('(').TrimEnd(')').Replace("= =","=");
            if (expResult.EndsWith(" IS NULL  ")) 
            {
                expResult = Regex.Split(expResult, " IS NULL  ")[0]+" = NULL ";
            }
            else if (!expResult.Contains("=")&&expResult.Contains("IS  NULL  "))
            {
                expResult = Regex.Split(expResult, "IS  NULL  ")[0] + " = NULL ";
            }
            string key = SqlBuilder.GetNoTranslationColumnName(expResult);

            if (EntityInfo.Columns.Where(it=>it.IsJson||it.IsTranscoding).Any(it => it.DbColumnName.EqualCase(key) || it.PropertyName.EqualCase(key)))
            {
                CheckTranscodeing();
            }

            if (columns.ToString().Contains("Subqueryable()."))
            {
                expResult= expResult.Replace(this.SqlBuilder.GetTranslationColumnName((binaryExp.Left as MemberExpression).Expression+"") +".",this.UpdateBuilder.GetTableNameString.TrimEnd()+".");
            }

            UpdateBuilder.SetValues.Add(new KeyValuePair<string, string>(SqlBuilder.GetTranslationColumnName(key), expResult));
            this.UpdateBuilder.DbColumnInfoList = this.UpdateBuilder.DbColumnInfoList.Where(it => (UpdateParameterIsNull == false && IsPrimaryKey(it)) || UpdateBuilder.SetValues.Any(v => SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase) || SqlBuilder.GetNoTranslationColumnName(v.Key).Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || it.IsPrimarykey == true).ToList();
            AppendSets();
            return this;
        }
        public IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, bool>> columns)
        {
            ThrowUpdateByObject();
            if (isUpdateColumns)
                SetColumns(columns);
            return this;
        }
        public IUpdateable<T> SetColumnsIF(bool isUpdateColumns, Expression<Func<T, T>> columns)
        {
            ThrowUpdateByObject();
            if (isUpdateColumns)
                SetColumns(columns);
            return this;
        }
        public IUpdateable<T> WhereIF(bool isWhere, Expression<Func<T, bool>> expression) 
        {
            Check.ExceptionEasy(!StaticConfig.EnableAllWhereIF, "Need to program startup configuration StaticConfig. EnableAllWhereIF = true; Tip: This operation is very risky if there are no conditions it is easy to update the entire table", " 需要程序启动时配置StaticConfig.EnableAllWhereIF=true; 提示：该操作存在很大的风险如果没有条件很容易将整个表全部更新");
            if (isWhere) 
            {
                return Where(expression);
            }
            return this;
        }
        public IUpdateable<T> Where(Expression<Func<T, bool>> expression)
        {
            Check.Exception(UpdateObjectNotWhere()&&UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where","集合更新不支持Where请使用WhereColumns"));
            var expResult = UpdateBuilder.GetExpressionValue(expression, ResolveExpressType.WhereSingle);
            var whereString = expResult.GetResultString();
            if (expression.ToString().Contains("Subqueryable()"))
            {
                var entityTableName = this.EntityInfo.DbTableName;
                if (UpdateBuilder.TableName.HasValue()) 
                {
                    entityTableName = UpdateBuilder.TableName;
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
                if (this.UpdateBuilder.TableName.HasValue())
                {
                    entityTableName2 = this.UpdateBuilder.TableName;
                }
                whereString = whereString.Replace(expression.Parameters.First().Name + ".", this.SqlBuilder.GetTranslationTableName(entityTableName2) + ".");
                whereString = whereString.Replace(this.SqlBuilder.GetTranslationColumnName(expression.Parameters.First().Name) + ".", this.SqlBuilder.GetTranslationTableName(entityTableName2) + ".");
            }
            UpdateBuilder.WhereValues.Add(whereString);
            return this;
        }
        public IUpdateable<T> Where(string whereSql, object parameters = null)
        {
            Check.Exception(UpdateObjectNotWhere() && UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where", "集合更新不支持Where请使用WhereColumns"));
            if (whereSql.HasValue())
            {
                UpdateBuilder.WhereValues.Add(whereSql);
            }
            if (parameters != null)
            {
                UpdateBuilder.Parameters.AddRange(Context.Ado.GetParameters(parameters));
            }
            return this;
        }
        public IUpdateable<T> Where(string fieldName, string conditionalType, object fieldValue)
        {
            Check.Exception(UpdateObjectNotWhere() && UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where", "集合更新不支持Where请使用WhereColumns"));
            var whereSql = this.SqlBuilder.GetWhere(fieldName, conditionalType, 0);
            this.Where(whereSql);
            string parameterName = this.SqlBuilder.SqlParameterKeyWord + fieldName + "0";
            this.UpdateBuilder.Parameters.Add(new SugarParameter(parameterName, fieldValue));
            return this;
        }
        public IUpdateable<T> Where(List<IConditionalModel> conditionalModels)
        {
            Check.Exception(UpdateObjectNotWhere() && UpdateObjs.Length > 1, ErrorMessage.GetThrowMessage("update List no support where", "集合更新不支持Where请使用WhereColumns"));
            var sql = this.Context.Queryable<T>().SqlBuilder.ConditionalModelToSql(conditionalModels);
            var result = this;
            result.Where(sql.Key, sql.Value);
            return result;
        }

        public IUpdateable<T> In(object[] ids) 
        {
            ThrowUpdateByObjectByMesage(" In(object[] ids) ");
            List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
            var column = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            Check.ExceptionEasy(column == null, "In need primary key", "In需要实体有主键");
            conditionalModels.Add(new ConditionalModel() { FieldName= column.DbColumnName, ConditionalType= ConditionalType.In,FieldValue=string.Join(",",ids),CSharpTypeName=column.UnderType?.Name } );
            return this.Where(conditionalModels);
        }
        #endregion

    }
}
