using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：SQL糖 ORM 核心类
    /// ** 创始时间：2015-7-13
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4649904.html
    /// </summary>
    public class SqlSugarClient : SqlHelper, SqlSugar.IClient
    {

        public SqlSugarClient(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
            IsNoLock = false;
        }
        private List<KeyValue> _mappingTableList = null;
        private Dictionary<string, Func<KeyValueObj>> _filterFuns = null;
        private List<PubModel.SerialNumber> _serialNumber = null;

        internal string GetTableNameByClassType(string typeName)
        {
            if (_mappingTableList.IsValuable())
            {
                //Key为实体类 Value为表名
                if (_mappingTableList.Any(it => it.Key == typeName))
                {
                    typeName = _mappingTableList.First(it => it.Key == typeName).Value;
                }
            }
            return typeName;
        }
        internal string GetClassTypeByTableName(string tableName)
        {
            if (_mappingTableList.IsValuable())
            {
                //Key为实体类 Value为表名
                if (_mappingTableList.Any(it => it.Value == tableName))
                {
                    tableName = _mappingTableList.First(it => it.Value == tableName).Key;
                }
            }
            return tableName;
        }

        /// <summary>
        /// 设置实体类与表名的映射， Key为实体类 Value为表名
        /// </summary>
        /// <param name="mappingTables"></param>
        public void SetMappingTables(List<KeyValue> mappingTables)
        {
            if (mappingTables.IsValuable())
            {
                _mappingTableList = mappingTables;
            }
        }
        /// <summary>
        /// 设置流水号 （说明：Dictionary<流水号名称, Func<{表名，字段名},返回的流水号>>）
        /// </summary>
        /// <param name="serNum"></param>
        public void SetSerialNumber(List<PubModel.SerialNumber> serNum)
        {
            if (serNum.IsValuable())
            {
                _serialNumber = serNum;
            }
        }

        public string ConnectionString { get; set; }

        /// <summary>
        /// 查询是否允许脏读，（默认为:true）
        /// </summary>
        public bool IsNoLock { get; set; }

        /// <summary>
        /// 忽略非数据库列，如果非特殊需求不建议启用
        /// </summary>
        public bool IsIgnoreErrorColumns = false;

        /// <summary>
        /// 设置禁止更新的列
        /// </summary>
        public string[] DisableUpdateColumns { get; set; }
        /// <summary>
        /// 设置序列化实体转成JSON的日期格式
        /// </summary>
        public string SerializerDateFormat = null;
        /// <summary>
        /// 设置分页类型
        /// </summary>
        public PageModel PageModel = PageModel.RowNumber;
        /// <summary>
        /// 设置多语言配置
        /// </summary>
        public PubModel.Language Language = null;

        /// <summary>
        /// 设置过滤器（用户权限过滤）
        /// Func《过滤器的名字,过滤的条件SQL，过滤的参数对象，返回条件加参数对象》
        /// </summary>
        /// <param name="filters"></param>
        public void SetFilterFilterParas(Dictionary<string, Func<KeyValueObj>> filters)
        {
            _filterFuns = filters;
        }




        /// <summary>
        /// 数据过滤器键
        /// </summary>
        public string CurrentFilterKey = null;

        /// <summary>
        /// 创建多表查询对象
        /// </summary>
        public Sqlable Sqlable()
        {
            var sqlable = new Sqlable() { DB = this };
            //全局过滤器
            if (CurrentFilterKey.IsValuable())
            {
                if (_filterFuns.IsValuable() && _filterFuns.ContainsKey(CurrentFilterKey))
                {
                    var filterInfo = _filterFuns[CurrentFilterKey];
                    var filterVlue = filterInfo();
                    string whereStr = string.Format(" AND {0} ", filterVlue.Key);
                    sqlable.Where.Add(whereStr);
                    if (filterVlue.Value != null)
                        sqlable.Params.AddRange(SqlSugarTool.GetParameters(filterVlue.Value));
                    return sqlable;
                }
            }
            return sqlable;
        }

        /// <summary>
        /// 创建单表查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>() where T : new()
        {
            var queryable = new Queryable<T>() { DB = this };
            //别名表
            if (_mappingTableList.IsValuable())
            {
                string name = typeof(T).Name;
                if (_mappingTableList.Any(it => it.Key == name))
                {
                    queryable.TableName = _mappingTableList.First(it => it.Key == name).Value;
                }
            }
            //全局过滤器
            if (CurrentFilterKey.IsValuable())
            {
                if (_filterFuns.IsValuable() && _filterFuns.ContainsKey(CurrentFilterKey))
                {
                    var filterInfo = _filterFuns[CurrentFilterKey];
                    var filterValue = filterInfo();
                    string whereStr = string.Format(" AND {0} ", filterValue.Key);
                    queryable.Where.Add(whereStr);
                    if (filterValue.Value != null)
                        queryable.Params.AddRange(SqlSugarTool.GetParameters(filterValue.Value));
                    return queryable;
                }
            }
            return queryable;

        }
        /// <summary>
        /// 创建单表查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>(string tableName) where T : new()
        {
            return new Queryable<T>() { DB = this, TableName = tableName };
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            var pars = SqlSugarTool.GetParameters(whereObj).ToList();
            return SqlQuery<T>(sql, pars);
        }
        /// <summary>
        /// 根据SQL语句将结果集映射到dynamic
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public dynamic SqlQueryDynamic(string sql, object whereObj = null)
        {
            return JsonConverter.ConvertJson(SqlQueryJson(sql, whereObj));
        }
        /// <summary>
        /// 根据SQL语句将结果集映射到json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="whereObj"></param>
        /// <returns></returns>
        public string SqlQueryJson(string sql, object whereObj = null)
        {
            return JsonConverter.DataTableToJson(GetDataTable(sql, whereObj), SerializerDateFormat);
        }


        /// <summary>
        /// 根据SQL语句将结果集映射到List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, SqlParameter[] pars)
        {
            return SqlQuery<T>(sql, pars.ToList());
        }
        /// <summary>
        /// 根据SQL语句将结果集映射到List《T》
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="reader"></param>
        /// <param name="pars"></param>
        /// <returns></returns>
        public List<T> SqlQuery<T>(string sql, List<SqlParameter> pars)
        {
            SqlDataReader reader = null;
            //全局过滤器
            if (CurrentFilterKey.IsValuable())
            {
                if (_filterFuns.IsValuable() && _filterFuns.ContainsKey(CurrentFilterKey))
                {
                    var filterInfo = _filterFuns[CurrentFilterKey];
                    var filterValue = filterInfo();
                    sql += string.Format(" AND {0} ", filterValue.Key);
                    if (filterValue.Value != null)
                    {
                        pars.AddRange(SqlSugarTool.GetParameters(filterValue.Value));
                    }
                }
            }
            var type = typeof(T);
            sql = string.Format(@"--{0}
{1}", type.Name, sql);
            reader = GetReader(sql, pars.ToArray());
            string fields = sql;
            if (sql.Length > 101)
            {
                fields = sql.Substring(0, 100);
            }
            var reval = SqlSugarTool.DataReaderToList<T>(type, reader, fields);
            fields = null;
            sql = null;
            return reval;
        }

        /// <summary>
        /// 批量插入
        /// 使用说明:sqlSugar.Insert(List《entity》);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public List<object> InsertRange<T>(List<T> entities, bool isIdentity = true) where T : class
        {
            List<object> reval = new List<object>();
            foreach (var it in entities)
            {
                reval.Add(Insert<T>(it, isIdentity));
            }
            return reval;
        }

        /// <summary>
        /// 插入
        /// 使用说明:sqlSugar.Insert(entity);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">该属性已经作废可以不填，主键是否为自增长,true可以不填,false必填</param>
        /// <returns></returns>
        public object Insert<T>(T entity, bool isIdentity = true) where T : class
        {

            Type type = entity.GetType();
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);

            StringBuilder sbInsertSql = new StringBuilder();
            List<SqlParameter> pars = new List<SqlParameter>();
            var identities = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            isIdentity = identities != null && identities.Count > 0;
            //sql语句缓存
            string cacheSqlKey = "db.Insert." + typeName;
            var cacheSqlManager = CacheManager<StringBuilder>.GetInstance();

            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();

            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            var isContainCacheSqlKey = cacheSqlManager.ContainsKey(cacheSqlKey);
            if (isContainCacheSqlKey)
            {
                sbInsertSql = cacheSqlManager[cacheSqlKey];
            }
            else
            {



                //2.获得实体的属性集合 


                //实例化一个StringBuilder做字符串的拼接 


                sbInsertSql.Append("insert into [" + typeName + "] (");

                //3.遍历实体的属性集合 
                foreach (PropertyInfo prop in props)
                {
                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == prop.Name.ToLower()))
                        {
                            continue;
                        }
                    }
                    //EntityState,@EntityKey
                    if (!isIdentity || identities.Any(it => it.Value.ToLower() != prop.Name.ToLower()))
                    {
                        //4.将属性的名字加入到字符串中 
                        sbInsertSql.Append("[" + prop.Name + "],");
                    }
                }
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbInsertSql.Append(" ) values(");

            }

            //5.再次遍历，形成参数列表"(@xx,@xx@xx)"的形式 
            foreach (PropertyInfo prop in props)
            {
                //EntityState,@EntityKey
                if (!isIdentity || identities.Any(it => it.Value.ToLower() != prop.Name.ToLower()))
                {
                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == prop.Name.ToLower()))
                        {
                            continue;
                        }
                    }
                    if (!cacheSqlManager.ContainsKey(cacheSqlKey))
                        sbInsertSql.Append("@" + prop.Name + ",");
                    object val = prop.GetValue(entity, null);
                    if (val == null)
                        val = DBNull.Value;
                    if (_serialNumber.IsValuable())
                    {
                        Func<PubModel.SerialNumber, bool> serEexp = it => it.TableName.ToLower() == typeName.ToLower() && it.FieldName.ToLower() == prop.Name.ToLower();
                        var isAnyNum = _serialNumber.Any(serEexp);
                        if (isAnyNum && (val == DBNull.Value || val.IsNullOrEmpty()))
                        {
                            if (_serialNumber.First(serEexp).GetNumFunc != null) {
                                val = _serialNumber.First(serEexp).GetNumFunc();
                            }
                            if (_serialNumber.First(serEexp).GetNumFuncWithDb != null)
                            {
                                val = _serialNumber.First(serEexp).GetNumFuncWithDb(this);
                            }
                        }
                    }

                    if (prop.PropertyType.IsEnum)
                    {
                        val = (int)(val);
                    }

                    var par = new SqlParameter("@" + prop.Name, val);
                    SqlSugarTool.SetParSize(par);
                    if (par.SqlDbType == SqlDbType.Udt)
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    pars.Add(par);
                }
            }
            if (!isContainCacheSqlKey)
            {
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                if (isIdentity == false)
                {
                    sbInsertSql.Append(");select 'true';");
                }
                else
                {
                    sbInsertSql.Append(");select SCOPE_IDENTITY();");
                }
                cacheSqlManager.Add(cacheSqlKey, sbInsertSql, cacheSqlManager.Day);
            }
            var sql = sbInsertSql.ToString();
            try
            {
                var lastInsertRowId = GetScalar(sql, pars.ToArray());
                return lastInsertRowId;
            }
            catch (Exception ex)
            {
                var cacheManager = CacheManager<string>.GetInstance();
                cacheManager.RemoveAll(it => it.Contains("KeyBy"));
                throw new Exception("sql:" + sql + "\n" + ex.Message);
            }

        }

        /// <summary>
        /// 大数据插入(结构体必须和数据库一致)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool SqlBulkCopy<T>(List<T> entities) where T : class
        {
            int actionNum = 100;
            var reval = true;
            while (entities.Count > 0)
            {
                var insertRes = SqlBulkCopy<T>(entities.Take(actionNum));
                if (reval && insertRes)
                {
                    reval = true;
                }
                else
                {
                    reval = false;
                }
                if (actionNum > entities.Count)
                {
                    actionNum = entities.Count;
                }
                entities.RemoveRange(0, actionNum);
            }
            return reval;
        }

        private bool SqlBulkCopy<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null) { return false; };

            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            var isIdentity = identityNames != null && identityNames.Count > 0;
            var columnNames = SqlSugarTool.GetColumnsByTableName(this, typeName);
            if (isIdentity)
            {
                columnNames = columnNames.Where(c => !identityNames.Any(it => it.Value == c)).ToList();//去掉自添列
            }
            StringBuilder sbSql = new StringBuilder("INSERT INTO ");
            sbSql.AppendLine(typeName);
            sbSql.AppendFormat("({0})", string.Join(",", columnNames.Select(it => "[" + it + "]")));


            int i = 0;
            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            foreach (var entity in entities)
            {
                sbSql.AppendLine("SELECT ");
                foreach (var name in columnNames)
                {
                    var isLastName = name == columnNames.Last();
                    var prop = props.Single(it => it.Name == name);
                    var objValue = prop.GetValue(entity, null);
                    bool isNullable = false;
                    var underType = SqlSugarTool.GetUnderType(prop, ref isNullable);
                    if (objValue == null)
                    {
                        objValue = "NULL";
                    }
                    else if (underType == SqlSugarTool.DateType)
                    {
                        objValue = "'" + objValue.ToString() + "'";
                    }
                    else if (underType == SqlSugarTool.BoolType)
                    {
                        objValue = Convert.ToBoolean(objValue) ? 1 : 0;
                    }
                    else if (underType == SqlSugarTool.StringType)
                    {
                        //string参数需要处理注入 (因为SqlParameter参数上限为2100所以无法使用参数化)
                        objValue = "'" + objValue.ToString().ToSqlFilter() + "'";
                    }
                    else
                    {
                        objValue = "'" + objValue.ToString() + "'";
                    }

                    sbSql.Append(objValue + (isLastName ? "" : ","));
                }
                var isLastEntity = entities.Last() == entity;
                if (!isLastEntity)
                {
                    sbSql.AppendLine(" UNION ALL ");
                }
            }
            var reval = base.ExecuteCommand(sbSql.ToString());
            sbSql = null;
            return reval > 0;
        }

        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="expression">it.id=100</param>
        /// <returns></returns>
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SqlSugarClient.Update.expression"); }


            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var rows = SqlSugarTool.GetParameters(rowObj);
            var isDynamic = rowObj.GetType() != type;
            var isClass = !isDynamic;

            //sql语句缓存
            string cacheSqlKey = "db.update." + typeName + rows.Length;
            var cacheSqlManager = CacheManager<StringBuilder>.GetInstance();



            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);


            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);


            StringBuilder sbSql = new StringBuilder();
            if (cacheSqlManager.ContainsKey(cacheSqlKey) && isClass)
            {
                sbSql = cacheSqlManager[cacheSqlKey];
            }
            else
            {
                sbSql = new StringBuilder(string.Format(" UPDATE [{0}] SET ", typeName));
                foreach (var r in rows)
                {
                    var name = r.ParameterName.TrimStart('@');
                    var isPk = pkName != null && pkName.ToLower() == name.ToLower();
                    var isIdentity = identityNames.Any(it => it.Value.ToLower() == name.ToLower());
                    var isDisableUpdateColumns = DisableUpdateColumns != null && DisableUpdateColumns.Any(it => it.ToLower() == name.ToLower());

                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == name.ToLower()))
                        {
                            continue;
                        }
                    }

                    if (isPk || isIdentity || isDisableUpdateColumns)
                    {
                        if (isClass)
                        {
                            continue;
                        }
                    }
                    sbSql.Append(string.Format(" [{0}] =@{0}  ,", name));
                }
                sbSql.Remove(sbSql.Length - 1, 1);
                sbSql.Append(" WHERE  1=1  ");
                sbSql.Append(re.SqlWhere);
                cacheSqlManager.Add(cacheSqlKey, sbSql, cacheSqlManager.Day);
            }

            List<SqlParameter> parsList = new List<SqlParameter>();
            parsList.AddRange(re.Paras);
            var pars = rows;
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    if (par.SqlDbType == SqlDbType.Udt)
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    SqlSugarTool.SetParSize(par);
                    parsList.Add(par);
                }
            }
            try
            {
                var updateRowCount = ExecuteCommand(sbSql.ToString(), parsList.ToArray());
                return updateRowCount > 0;
            }
            catch (Exception ex)
            {
                var cacheManager = CacheManager<string>.GetInstance();
                cacheManager.RemoveAll(it => it.Contains("KeyBy"));
                throw new Exception("sql:" + sbSql.ToString() + "\n" + ex.Message);
            }
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="rowObj">实体必须包含主键</param>
        /// <returns></returns>
        public bool Update<T>(T rowObj) where T : class
        {
            var reval = Update<T, object>(rowObj);
            return reval;
        }

        /// <summary>
        /// 更新
        /// 注意：rowObj为T类型将更新该实体的非主键所有列，如果rowObj类型为匿名类将更新指定列
        /// 使用说明:sqlSugar.Update《T》(rowObj,whereObj);
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObj">new T(){name="张三",sex="男"}或者new {name="张三",sex="男"}</param>
        /// <param name="whereIn">new int[]{1,2,3}</param>
        /// <returns></returns>
        public bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class
        {
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }

            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            StringBuilder sbSql = new StringBuilder(string.Format(" UPDATE [{0}] SET ", typeName));
            Dictionary<string, object> rows = SqlSugarTool.GetObjectToDictionary(rowObj);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            foreach (var r in rows)
            {
                var isPk = pkName != null && pkName.ToLower() == r.Key.ToLower();
                var isIdentity = identityNames.Any(it => it.Value.ToLower() == r.Key.ToLower());
                var isDisableUpdateColumns = DisableUpdateColumns != null && DisableUpdateColumns.Any(it => it.ToLower() == r.Key.ToLower());

                if (this.IsIgnoreErrorColumns)
                {
                    if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == r.Key.ToLower()))
                    {
                        continue;
                    }
                }

                if (isPk || isIdentity || isDisableUpdateColumns)
                {
                    if (rowObj.GetType() == type)
                    {
                        continue;
                    }
                }
                sbSql.Append(string.Format(" [{0}] =@{0}  ,", r.Key));
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            if (whereIn.Count() == 0)
            {
                var value = type.GetProperties().Cast<PropertyInfo>().Single(it => it.Name == pkName).GetValue(rowObj, null);
                sbSql.AppendFormat("WHERE {1} IN ('{2}')", typeName, pkName, value);
            }
            else
            {
                sbSql.AppendFormat("WHERE {1} IN ({2})", typeName, pkName, whereIn.ToJoinSqlInVal());
            }
            List<SqlParameter> parsList = new List<SqlParameter>();
            var pars = rows.Select(c => new SqlParameter("@" + c.Key, c.Value));
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    var isDisableUpdateColumns = DisableUpdateColumns != null && DisableUpdateColumns.Any(it => it.ToLower() == par.ParameterName.TrimStart('@').ToLower());
                    if (par.SqlDbType == SqlDbType.Udt || par.ParameterName.ToLower().Contains("hierarchyid"))
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    if (!isDisableUpdateColumns)
                    {
                        parsList.Add(par);
                    }
                }
            }
            try
            {
                var updateRowCount = ExecuteCommand(sbSql.ToString(), parsList.ToArray());
                return updateRowCount > 0;
            }
            catch (Exception ex)
            {
                var cacheManager = CacheManager<string>.GetInstance();
                cacheManager.RemoveAll(it => it.Contains("KeyBy"));
                throw new Exception("sql:" + sbSql.ToString() + "\n" + ex.Message);
            }
        }

        /// <summary>
        /// 删除,根据表达示
        /// 使用说明:
        /// Delete《T》(it=>it.id=100) 或者Delete《T》(3)
        /// </summary>
        /// <param name="expression">筛选表达示</param>
        public bool Delete<T>(Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            string sql = string.Format("DELETE FROM [{0}] WHERE 1=1 {1}", typeName, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Paras.ToArray()) > 0;
            return isSuccess;
        }

        /// <summary>
        /// 批量删除
        /// 注意：whereIn 主键集合  
        /// 使用说明:Delete《T》(new int[]{1,2,3}) 或者  Delete《T》(3)
        /// </summary>
        /// <param name="whereIn"> delete ids </param>
        public bool Delete<T, FiledType>(params FiledType[] whereIn)
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM [{0}] WHERE {1} IN ({2})", typeName, SqlSugarTool.GetPrimaryKeyByTableName(this, typeName), whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType">whereIn里面元素的类型</typeparam>
        /// <param name="expression">in 的字段名称</param>
        /// <param name="whereIn">需要删除条件值的数组集合</param>
        /// <returns></returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, List<FiledType> whereIn)
        {
            if (whereIn == null) return false;
            return Delete<T, FiledType>(expression, whereIn.ToArray());
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType">whereIn里面元素的类型</typeparam>
        /// <param name="expression">in 的字段名称</param>
        /// <param name="whereIn">需要删除条件值的数组集合</param>
        /// <returns></returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, params FiledType[] whereIn)
        {
            ResolveExpress re = new ResolveExpress();
            var fieldName = re.GetExpressionRightField(expression);
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("DELETE FROM [{0}] WHERE {1} IN ({2})", typeName, fieldName, whereIn.ToJoinSqlInVal());
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 批量假删除
        /// 使用说明::
        /// FalseDelete《T》("is_del",new int[]{1,2,3})或者Delete《T》("is_del",3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="whereIn">delete ids</param>
        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn)
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = SqlSugarTool.GetGetPropertiesByCache(type, cachePropertiesKey, cachePropertiesManager);
            bool isSuccess = false;
            if (whereIn != null && whereIn.Length > 0)
            {
                string sql = string.Format("UPDATE  [{0}] SET {3}=1 WHERE {1} IN ({2})", typeName, SqlSugarTool.GetPrimaryKeyByTableName(this, typeName), whereIn.ToJoinSqlInVal(), field);
                int deleteRowCount = ExecuteCommand(sql);
                isSuccess = deleteRowCount > 0;
            }
            return isSuccess;
        }

        /// <summary>
        /// 假删除，根据表达示
        /// 使用说明::
        /// FalseDelete《T》(new int[]{1,2,3})或者Delete《T》(3)
        /// </summary>
        /// <param name="field">更新删除状态字段</param>
        /// <param name="expression">筛选表达示</param>
        public bool FalseDelete<T>(string field, Expression<Func<T, bool>> expression)
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + typeName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            PropertyInfo[] props = null;
            if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
            {
                props = cachePropertiesManager[cachePropertiesKey];
            }
            else
            {
                props = type.GetProperties();
                cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
            }
            bool isSuccess = false;
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression);
            string sql = string.Format("UPDATE  [{0}] SET {1}=1 WHERE  1=1 {2}", typeName, field, re.SqlWhere);
            int deleteRowCount = ExecuteCommand(sql, re.Paras.ToArray());
            isSuccess = deleteRowCount > 0;
            return isSuccess;
        }

        /// <summary>
        /// 生成实体的对象
        /// </summary>
        public ClassGenerating ClassGenerating = new ClassGenerating();

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache()
        {
            CacheManager<int>.GetInstance().RemoveAll(c => true);
        }

    }

}
