using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public IAdo Ado { get { return Context.Ado; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }

        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public bool IsSingle { get { return this.InsertObjs.Length == 1; } }

        public EntityInfo EntityInfo { get; set; }
        public List<MappingColumn> MappingColumnList { get; set; }
        private List<string> IgnoreColumnNameList { get; set; }
        internal bool IsOffIdentity { get; set; }
        public T[] InsertObjs { get; set; }

        public MappingTableList OldMappingTableList { get; set; }
        public bool IsAs { get; set; }
        public bool IsEnableDiffLogEvent { get; set; }
        public DiffLogModel diffModel { get; set; }
        internal Action RemoveCacheFunc { get; set; }


        #region Core
        public void AddQueue()
        {
            if (this.InsertObjs != null && this.InsertObjs.Length > 0 && this.InsertObjs[0] != null)
            {
                var sqlObj = this.ToSql();
                this.Context.Queues.Add(sqlObj.Key, sqlObj.Value);
            }
        }
        public virtual int ExecuteCommand()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteCommand();
            var result = Ado.ExecuteCommand(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, null);
            if (result == -1) return this.InsertObjs.Count();
            return result;
        }
        public virtual string ToSqlString()
        {
            var sqlObj = this.ToSql();
            var result = sqlObj.Key;
            if (result == null) return null;
            result = UtilMethods.GetSqlString(this.Context.CurrentConnectionConfig, sqlObj);
            return result;
        }
        public virtual KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            InsertBuilder.IsReturnIdentity = true;
            PreToSql();
            AutoRemoveDataCache();
            string sql = InsertBuilder.ToSqlString();
            RestoreMapping();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }
        public async Task<List<Type>> ExecuteReturnPkListAsync<Type>()
        {
            return await Task.Run(() => ExecuteReturnPkList<Type>());
        }
        public virtual List<Type> ExecuteReturnPkList<Type>()
        {
            var pkInfo = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            Check.ExceptionEasy(pkInfo == null, "ExecuteReturnPkList need primary key", "ExecuteReturnPkList需要主键");
            Check.ExceptionEasy(this.EntityInfo.Columns.Count(it => it.IsPrimarykey == true) > 1, "ExecuteReturnPkList ，Only support technology single primary key", "ExecuteReturnPkList只支技单主键");
            var isIdEntity = pkInfo.IsIdentity || (pkInfo.OracleSequenceName.HasValue() && this.Context.CurrentConnectionConfig.DbType == DbType.Oracle);
            if (isIdEntity && this.InsertObjs.Length == 1)
            {
                return InsertPkListIdentityCount1<Type>(pkInfo);
            }
            else if (isIdEntity && this.InsertBuilder.ConvertInsertReturnIdFunc == null)
            {
                return InsertPkListNoFunc<Type>(pkInfo);
            }
            else if (isIdEntity && this.InsertBuilder.ConvertInsertReturnIdFunc != null)
            {
                return InsertPkListWithFunc<Type>(pkInfo);
            }
            else if (pkInfo.UnderType == UtilConstants.LongType)
            {
                return InsertPkListLong<Type>();
            }
            else
            {
                return InsertPkListGuid<Type>(pkInfo);
            }
        }

        public virtual int ExecuteReturnIdentity()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnIdentity();
            var result = 0;
            if (InsertBuilder.IsOleDb)
            {
                var isAuto = false;
                if (this.Context.Ado.IsAnyTran() == false && this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
                {
                    isAuto = this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
                }
                result = Ado.GetInt(sql.Split(';').First(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                result = Ado.GetInt(sql.Split(';').Last(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                if (this.Context.Ado.IsAnyTran() == false && isAuto)
                {
                    this.Ado.Close();
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
                }
            }
            else
            {
                result = Ado.GetInt(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            }
            After(sql, result);
            return result;
        }

        public virtual long ExecuteReturnBigIdentity()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnBigIdentity();
            long result = 0;
            if (InsertBuilder.IsOleDb)
            {
                var isAuto = false;
                if (this.Context.Ado.IsAnyTran() == false && this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
                {
                    isAuto = this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
                }
                result = Convert.ToInt64(Ado.GetScalar(sql.Split(';').First(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
                result = Convert.ToInt64(Ado.GetScalar(sql.Split(';').Last(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
                if (this.Context.Ado.IsAnyTran() == false && isAuto)
                {
                    this.Ado.Close();
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
                }
            }
            else
            {
                result = Convert.ToInt64(Ado.GetScalar(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            }
            After(sql, result);
            return result;
        }

        public virtual long ExecuteReturnSnowflakeId()
        {
            if (this.InsertObjs.Length > 1)
            {
                return this.ExecuteReturnSnowflakeIdList().First();
            }

            var id = SnowFlakeSingle.instance.getID();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var snowProperty = entity.Columns.FirstOrDefault(it => it.IsPrimarykey && it.PropertyInfo.PropertyType == UtilConstants.LongType);
            Check.Exception(snowProperty == null, "The entity sets the primary key and is long");
            Check.Exception(snowProperty.IsIdentity == true, "SnowflakeId IsIdentity can't true");
            foreach (var item in this.InsertBuilder.DbColumnInfoList.Where(it => it.PropertyName == snowProperty.PropertyName))
            {
                item.Value = id;
                snowProperty?.PropertyInfo.SetValue(this.InsertObjs.First(), id);
            }
            this.ExecuteCommand();
            return id;
        }
        public List<long> ExecuteReturnSnowflakeIdList()
        {
            List<long> result = new List<long>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var snowProperty = entity.Columns.FirstOrDefault(it => it.IsPrimarykey && it.PropertyInfo.PropertyType == UtilConstants.LongType);
            Check.Exception(snowProperty == null, "The entity sets the primary key and is long");
            Check.Exception(snowProperty.IsIdentity == true, "SnowflakeId IsIdentity can't true");
            foreach (var item in this.InsertBuilder.DbColumnInfoList.Where(it => it.PropertyName == snowProperty.PropertyName))
            {
                var id = SnowFlakeSingle.instance.getID();
                item.Value = id;
                result.Add(id);
                var obj = this.InsertObjs.ElementAtOrDefault(item.TableId);
                if (obj != null)
                {
                    snowProperty?.PropertyInfo.SetValue(obj, id);
                }
            }
            this.ExecuteCommand();
            return result;
        }
        public Task<long> ExecuteReturnSnowflakeIdAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteReturnSnowflakeIdAsync();
        }
        public async Task<long> ExecuteReturnSnowflakeIdAsync() 
        {
            var id = SnowFlakeSingle.instance.getID();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var snowProperty = entity.Columns.FirstOrDefault(it => it.IsPrimarykey && it.PropertyInfo.PropertyType == UtilConstants.LongType);
            Check.Exception(snowProperty == null, "The entity sets the primary key and is long");
            Check.Exception(snowProperty.IsIdentity == true, "SnowflakeId IsIdentity can't true");
            foreach (var item in this.InsertBuilder.DbColumnInfoList.Where(it => it.PropertyName == snowProperty.PropertyName))
            {
                item.Value = id;
                snowProperty?.PropertyInfo.SetValue(this.InsertObjs.First(), id);
            }
            await this.ExecuteCommandAsync();
            return id;
        }
        public async Task<List<long>> ExecuteReturnSnowflakeIdListAsync() 
        {
            List<long> result = new List<long>();
            var entity = this.Context.EntityMaintenance.GetEntityInfo<T>();
            var snowProperty = entity.Columns.FirstOrDefault(it => it.IsPrimarykey && it.PropertyInfo.PropertyType == UtilConstants.LongType);
            Check.Exception(snowProperty == null, "The entity sets the primary key and is long");
            Check.Exception(snowProperty.IsIdentity == true, "SnowflakeId IsIdentity can't true");
            foreach (var item in this.InsertBuilder.DbColumnInfoList.Where(it => it.PropertyName == snowProperty.PropertyName))
            {
                var id = SnowFlakeSingle.instance.getID();
                item.Value = id;
                result.Add(id);
                var obj = this.InsertObjs.ElementAtOrDefault(item.TableId);
                if (obj != null)
                {
                    snowProperty?.PropertyInfo.SetValue(obj, id);
                }
            }
            await this.ExecuteCommandAsync();
            return result;
        }

        public Task<List<long>> ExecuteReturnSnowflakeIdListAsync(CancellationToken token) 
        {
            this.Ado.CancellationToken= token;
            return ExecuteReturnSnowflakeIdListAsync();
        }

        public virtual T ExecuteReturnEntity()
        {
            ExecuteCommandIdentityIntoEntity();
            return InsertObjs.First();
        }
        public virtual bool ExecuteCommandIdentityIntoEntity()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0)
            {
                var snowColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey && it.UnderType == UtilConstants.LongType);
                if (snowColumn!=null)
                {
                    if (Convert.ToInt64(snowColumn.PropertyInfo.GetValue(result)) == 0)
                    {
                        var id = this.ExecuteReturnSnowflakeId();
                        snowColumn.PropertyInfo.SetValue(result, id);
                    }
                    else 
                    {
                        ExecuteCommand();
                    }
                    return true;
                }
                else
                {
                    return this.ExecuteCommand() > 0;
                }
            }
            var idValue = ExecuteReturnBigIdentity();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue = 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(uint))) 
            {
                setValue = Convert.ToUInt32(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(ulong)))
            {
                setValue = Convert.ToUInt64(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(ushort)))
            {
                setValue = Convert.ToUInt16(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(short)))
            {
                setValue = Convert.ToInt16(idValue);
            }
            else
                setValue = Convert.ToInt32(idValue);
            this.Context.EntityMaintenance.GetProperty<T>(identityKey).SetValue(result, setValue, null);
            return idValue > 0;
        }
        public Task<int> ExecuteCommandAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteCommandAsync();
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteCommand();
            var result =await Ado.ExecuteCommandAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            After(sql, null);
            if (result == -1) return this.InsertObjs.Count();
            return result;
        }
        public Task<int> ExecuteReturnIdentityAsync(CancellationToken token) 
        {
            this.Ado.CancellationToken= token;
            return ExecuteReturnIdentityAsync();
        }
        public virtual async Task<int> ExecuteReturnIdentityAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnIdentity();
            var result = 0;
            if (InsertBuilder.IsOleDb)
            {
                var isAuto = false;
                if (this.Context.Ado.IsAnyTran() == false && this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
                {
                    isAuto = this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
                }
                result = Ado.GetInt(sql.Split(';').First(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                result = Ado.GetInt(sql.Split(';').Last(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                if (this.Context.Ado.IsAnyTran() == false && isAuto)
                {
                    this.Ado.Close();
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
                }
            }
            else
            {
                result = await Ado.GetIntAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
            }
            After(sql, result);
            return result;
        }
        public T ExecuteReturnEntity(bool isIncludesAllFirstLayer)
        {
            var data =  ExecuteReturnEntity();
            return  this.Context.Queryable<T>().WhereClassByPrimaryKey(data).IncludesAllFirstLayer().First();
        }
        public async Task<T> ExecuteReturnEntityAsync()
        {
            await ExecuteCommandIdentityIntoEntityAsync();
            return InsertObjs.First();
        }
        public async Task<T> ExecuteReturnEntityAsync(bool isIncludesAllFirstLayer) 
        {
            var data=await ExecuteReturnEntityAsync();
            return await this.Context.Queryable<T>().WhereClassByPrimaryKey(data).IncludesAllFirstLayer().FirstAsync();
        }
        public async Task<bool> ExecuteCommandIdentityIntoEntityAsync()
        {
            var result = InsertObjs.First();
            var identityKeys = GetIdentityKeys();
            if (identityKeys.Count == 0)
            {
                var snowColumn = this.EntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey&& it.UnderType == UtilConstants.LongType);
                if (snowColumn != null)
                {

                    if (Convert.ToInt64(snowColumn.PropertyInfo.GetValue(result)) == 0)
                    {
                        var id = await this.ExecuteReturnSnowflakeIdAsync();
                        snowColumn.PropertyInfo.SetValue(result, id);
                    }
                    else 
                    {
                        await this.ExecuteCommandAsync();
                    }
                    return true;
                }
                else
                {
                    return await this.ExecuteCommandAsync() > 0;
                }
            }
            var idValue =await ExecuteReturnBigIdentityAsync();
            Check.Exception(identityKeys.Count > 1, "ExecuteCommandIdentityIntoEntity does not support multiple identity keys");
            var identityKey = identityKeys.First();
            object setValue = 0;
            if (idValue > int.MaxValue)
                setValue = idValue;
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(uint)))
            {
                setValue = Convert.ToUInt32(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(ulong)))
            {
                setValue = Convert.ToUInt64(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(ushort)))
            {
                setValue = Convert.ToUInt16(idValue);
            }
            else if (this.EntityInfo.Columns.Any(it => it.IsIdentity && it.PropertyInfo.PropertyType == typeof(short)))
            {
                setValue = Convert.ToInt16(idValue);
            }
            else
                setValue = Convert.ToInt32(idValue);
            this.Context.EntityMaintenance.GetProperty<T>(identityKey).SetValue(result, setValue, null);
            return idValue > 0;
        }
        public Task<long> ExecuteReturnBigIdentityAsync(CancellationToken token) 
        {
            this.Context.Ado.CancellationToken= token;
            return ExecuteReturnBigIdentityAsync();
        }
        public virtual async Task<long> ExecuteReturnBigIdentityAsync()
        {
            if (this.InsertObjs.Count() == 1 && this.InsertObjs.First() == null)
            {
                return 0;
            }
            string sql = _ExecuteReturnBigIdentity();
            long result = 0;
            if (InsertBuilder.IsOleDb)
            {
                var isAuto = false;
                if (this.Context.Ado.IsAnyTran() == false && this.Context.CurrentConnectionConfig.IsAutoCloseConnection)
                {
                    isAuto = this.Context.CurrentConnectionConfig.IsAutoCloseConnection;
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = false;
                }
                result = Ado.GetInt(sql.Split(';').First(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                result = Ado.GetInt(sql.Split(';').Last(), InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray());
                if (this.Context.Ado.IsAnyTran() == false && isAuto)
                {
                    this.Ado.Close();
                    this.Context.CurrentConnectionConfig.IsAutoCloseConnection = isAuto;
                }
            }
            else
            {
                result = Convert.ToInt64(await Ado.GetScalarAsync(sql, InsertBuilder.Parameters == null ? null : InsertBuilder.Parameters.ToArray()));
            }
            After(sql, result);
            return result;
        }

        #endregion

        #region Setting
        public InsertablePage<T> PageSize(int pageSize) 
        {
            InsertablePage<T> result = new InsertablePage<T>();
            result.PageSize = pageSize;
            result.Context = this.Context;
            result.DataList = this.InsertObjs;
            result.TableName = this.InsertBuilder.AsName;
            result.IsEnableDiffLogEvent = this.IsEnableDiffLogEvent;
            result.DiffModel = this.diffModel;
            result.IsOffIdentity = this.InsertBuilder.IsOffIdentity;
            if(this.InsertBuilder.DbColumnInfoList.Any())
              result.InsertColumns = this.InsertBuilder.DbColumnInfoList.GroupBy(it => it.TableId).First().Select(it=>it.DbColumnName).ToList();
            return result;
        }
        public IParameterInsertable<T> UseParameter()
        {
            var result = new ParameterInsertable<T>();
            result.Context= this.Context;
            result.Inserable = this;
            return result;
        }
        public IInsertable<T> AsType(Type tableNameType) 
        {
            return AS(this.Context.EntityMaintenance.GetEntityInfo(tableNameType).DbTableName);
        }
        public IInsertable<T> AS(string tableName)
        {
            this.InsertBuilder.AsName = tableName;
            return this; ;
        }
        public IInsertable<T> IgnoreColumns(Expression<Func<T, object>> columns)
        {
            if (columns == null) return this;
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !ignoreColumns.Any(ig => ig.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }
        public IInsertable<T> IgnoreColumns(params string[] columns)
        {
            if (columns == null)
                columns = new string[] { };
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !columns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => !columns.Any(ig => ig.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> IgnoreColumnsNull(bool isIgnoreNull = true) 
        {
            if (isIgnoreNull) 
            {
                Check.Exception(this.InsertObjs.Count() > 1 , ErrorMessage.GetThrowMessage("ignoreNullColumn NoSupport batch insert, use .PageSize(1).IgnoreColumnsNull().ExecuteCommand()", "ignoreNullColumn 不支持批量操作,你可以用PageSzie(1).IgnoreColumnsNull().ExecuteCommand()"));
                this.InsertBuilder.IsNoInsertNull = true;
            }
            return this;
        }

        public IInsertable<T> MySqlIgnore() 
        {
            this.InsertBuilder.MySqlIgnore = true; 
            return this;
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object>> columns)
        {
            if (columns == null) return this;
            var ignoreColumns = InsertBuilder.GetExpressionValue(columns, ResolveExpressType.ArraySingle).GetResultArray().Select(it => this.SqlBuilder.GetNoTranslationColumnName(it)).ToList();
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => ignoreColumns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase)) || ignoreColumns.Any(ig => ig.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> InsertColumns(string[] columns)
        {
            if (columns == null) return this;
            this.InsertBuilder.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.Where(it => columns.Any(ig => ig.Equals(it.PropertyName, StringComparison.CurrentCultureIgnoreCase))|| columns.Any(ig => ig.Equals(it.DbColumnName, StringComparison.CurrentCultureIgnoreCase))).ToList();
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            if (this.Context.CurrentConnectionConfig.DbType == DbType.SqlServer)
                this.InsertBuilder.TableWithString = lockString;
            return this;
        }
        public IInsertable<T> OffIdentity(bool isSetOn) 
        {
            if (isSetOn)
            {
                return this.OffIdentity();
            }
            else
            {
                return this;
            }
        }
        public IInsertable<T> OffIdentity() 
        {
            this.IsOffIdentity = true;
            this.InsertBuilder.IsOffIdentity = true;
            return this;
        }
        public IInsertable<T> IgnoreColumns(bool ignoreNullColumn, bool isOffIdentity = false) {
            Check.Exception(this.InsertObjs.Count() > 1&& ignoreNullColumn, ErrorMessage.GetThrowMessage("ignoreNullColumn NoSupport batch insert, use .PageSize(1).IgnoreColumnsNull().ExecuteCommand()", "ignoreNullColumn 不支持批量操作, 你可以使用 .PageSize(1).IgnoreColumnsNull().ExecuteCommand()"));
            this.IsOffIdentity = isOffIdentity;
            this.InsertBuilder.IsOffIdentity = isOffIdentity;
            if (this.InsertBuilder.LambdaExpressions == null)
                this.InsertBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.Context.CurrentConnectionConfig);
            this.InsertBuilder.IsNoInsertNull = ignoreNullColumn;
            return this;
        }

        public IInsertable<T> RemoveDataCache()
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCache(cacheService, this.Context.EntityMaintenance.GetTableName<T>());
            };
            return this;
        }
        public IInsertable<T> RemoveDataCache(string likeString)
        {
            this.RemoveCacheFunc = () =>
            {
                var cacheService = this.Context.CurrentConnectionConfig.ConfigureExternalServices.DataInfoCacheService;
                CacheSchemeMain.RemoveCacheByLike(cacheService, likeString);
            };
            return this;
        }
        public MySqlBlukCopy<T> UseMySql()
        {
            return new MySqlBlukCopy<T>(this.Context, this.SqlBuilder, InsertObjs);
        }
        public SqlServerBlukCopy UseSqlServer()
        {
            PreToSql();
            var currentType = this.Context.CurrentConnectionConfig.DbType;
            Check.Exception(currentType != DbType.SqlServer, "UseSqlServer no support " + currentType);
            SqlServerBlukCopy result = new SqlServerBlukCopy();
            result.DbColumnInfoList =this.InsertBuilder.DbColumnInfoList.GroupBy(it => it.TableId).ToList();
            result.InsertBuilder = this.InsertBuilder;
            result.Builder = this.SqlBuilder;
            result.Context = this.Context;
            result.Inserts=this.InsertObjs;
            return result;
        }
        public OracleBlukCopy UseOracle()

        {

            PreToSql();

            var currentType = this.Context.CurrentConnectionConfig.DbType;

            Check.Exception(currentType != DbType.Oracle, "UseSqlServer no support " + currentType);

            OracleBlukCopy result = new OracleBlukCopy();

            result.DbColumnInfoList = this.InsertBuilder.DbColumnInfoList.GroupBy(it => it.TableId).ToList();

            result.InsertBuilder = this.InsertBuilder;

            result.Builder = this.SqlBuilder;

            result.Context = this.Context;

            result.Inserts = this.InsertObjs;
            InsertBuilder.IsBlukCopy = true;

            return result;

        }


        public IInsertable<T> EnableDiffLogEventIF(bool isDiffLogEvent, object diffLogBizData) 
        {
            if (isDiffLogEvent) 
            {
                return EnableDiffLogEvent(diffLogBizData);
            }
            return this;
        }
        public IInsertable<T> EnableDiffLogEvent(object businessData = null)
        {
            //Check.Exception(this.InsertObjs.HasValue() && this.InsertObjs.Count() > 1, "DiffLog does not support batch operations");
            diffModel = new DiffLogModel();
            this.IsEnableDiffLogEvent = true;
            diffModel.BusinessData = businessData;
            diffModel.DiffType = DiffType.insert;
            return this;
        }

        public ISubInsertable<T> AddSubList(Expression<Func<T, object>> items)
        {
            Check.Exception(GetPrimaryKeys().Count == 0, typeof(T).Name + " need Primary key");
            Check.Exception(GetPrimaryKeys().Count > 1, typeof(T).Name + "Multiple primary keys are not supported");
            //Check.Exception(this.InsertObjs.Count() > 1, "SubInserable No Support Insertable(List<T>)");
            //Check.Exception(items.ToString().Contains(".First().")==false, items.ToString()+ " not supported ");
            if (this.InsertObjs == null || this.InsertObjs.Count() == 0)
            {
                return new SubInsertable<T>();
            }
            SubInsertable<T> result = new SubInsertable<T>();
            result.InsertObjects = this.InsertObjs;
            result.Context = this.Context;
            result.SubList = new List<SubInsertTreeExpression>();
            result.SubList.Add(new SubInsertTreeExpression() { Expression= items });
            result.InsertBuilder = this.InsertBuilder;
            result.Pk = GetPrimaryKeys().First();
            result.Entity = this.EntityInfo;
            return result;
        }
        public ISubInsertable<T> AddSubList(Expression<Func<T, SubInsertTree>> tree)
        {
            Check.Exception(GetPrimaryKeys().Count == 0, typeof(T).Name + " need Primary key");
            Check.Exception(GetPrimaryKeys().Count > 1, typeof(T).Name + "Multiple primary keys are not supported");
            //Check.Exception(this.InsertObjs.Count() > 1, "SubInserable No Support Insertable(List<T>)");
            //Check.Exception(items.ToString().Contains(".First().")==false, items.ToString()+ " not supported ");
            if (this.InsertObjs == null || this.InsertObjs.Count() == 0)
            {
                return new SubInsertable<T>();
            }
            SubInsertable<T> result = new SubInsertable<T>();
            result.InsertObjects = this.InsertObjs;
            result.Context = this.Context;
            result.SubList = new List<SubInsertTreeExpression>();
            result.InsertBuilder = this.InsertBuilder;
            result.Pk = GetPrimaryKeys().First();
            result.Entity = this.EntityInfo;
            result.AddSubList(tree);
            return result;
        }
        public SplitInsertable<T> SplitTable(SplitType splitType)
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.EntityInfo
            };
            helper.CheckPrimaryKey();
            SplitInsertable<T> result = new SplitInsertable<T>();
            result.Context = this.Context;
            result.EntityInfo = this.EntityInfo;
            result.Helper = helper;
            result.SplitType = splitType;
            result.TableNames = new List<KeyValuePair<string, object>>();
            foreach (var item in this.InsertObjs)
            {
                var splitFieldValue = helper.GetValue(splitType, item);
                var tableName=helper.GetTableName(splitType, splitFieldValue);
                result.TableNames.Add(new KeyValuePair<string, object>(tableName,item));
            }
            result.Inserable = this;
            return result;
        }

        public SplitInsertable<T> SplitTable()
        {
            UtilMethods.StartCustomSplitTable(this.Context, typeof(T));
            var splitTableAttribute = typeof(T).GetCustomAttribute<SplitTableAttribute>();
            if (splitTableAttribute != null)
            {
                return SplitTable((splitTableAttribute as SplitTableAttribute).SplitType);
            }
            else 
            {
                Check.Exception(true,$" {typeof(T).Name} need SplitTableAttribute");
                return null;
            }
        }

        #endregion

    }
}
