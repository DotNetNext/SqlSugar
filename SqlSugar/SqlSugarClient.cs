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
    public class SqlSugarClient : SqlHelper
    {
        #region constructor
        /// <summary>
        /// 初始化 SqlSugarClient 类的新实例
        /// </summary>
        /// <param name="connectionString">数据库连接字符串</param>
        public SqlSugarClient(string connectionString)
            : base(connectionString)
        {
            ConnectionString = connectionString;
            IsNoLock = false;
        }
        #endregion


        #region private variables
        internal List<KeyValue> _mappingTableList = null;
        internal List<KeyValue> _mappingColumns = null;
        private Dictionary<string, Func<KeyValueObj>> _filterFuns = null;
        private Dictionary<string, List<string>> _filterColumns = null;
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
        internal void InitAttributes<T>()
        {
            if (IsEnableAttributeMapping)
            {
                var mappingInfo = ReflectionSugarMapping.GetMappingInfo<T>();
                if (_mappingTableList == null)
                {
                    _mappingTableList = new List<KeyValue>();
                }
                if (!_mappingTableList.Contains(mappingInfo.TableMaping))
                {
                    _mappingTableList.Add(mappingInfo.TableMaping);
                    if (_mappingColumns == null)
                    {
                        _mappingColumns = new List<KeyValue>();
                    }
                    foreach (var item in mappingInfo.ColumnsMapping)
                    {
                        if (!_mappingColumns.Any(it => it.Key == item.Key))
                        {
                            _mappingColumns.Add(item);
                        }
                        else if (_mappingColumns.Any(it => it.Key == item.Key && it.Value != item.Value))
                        {
                            string throwMessage = string.Format(PubModel.SqlSugarClientConst.AttrMappingError,
                                item.Key,
                                _mappingColumns.Single(it => it.Key == item.Key).Value,
                                item.Value);
                            throw new SqlSugarException(throwMessage);
                        }
                    }
                }
            }
        }
        private string GetMappingColumnDbName(string name)
        {
            if (this.IsEnableAttributeMapping && _mappingColumns.IsValuable())
            {
                if (_mappingColumns.Any(it => it.Key == name))
                {
                    name = this._mappingColumns.Single(it => it.Key == name).Value;
                }
            }
            return name;
        }
        private string GetMappingColumnClassName(string name)
        {
            if (this.IsEnableAttributeMapping && _mappingColumns.IsValuable())
            {
                if (_mappingColumns.Any(it => it.Value == name))
                {
                    name = _mappingColumns.Single(it => it.Value == name).Key;
                }
            }
            return name;
        }

        #endregion


        #region readonly info
        /// <summary>
        /// 当前连接字符串
        /// </summary>
        public string ConnectionString { get; internal set; }
        #endregion


        #region setting
        /// <summary>
        /// 是否启用属性映射
        /// </summary>
        public bool IsEnableAttributeMapping = false;

        /// <summary>
        /// 查询是否允许脏读（默认为:true）
        /// </summary>
        public bool IsNoLock { get; set; }

        /// <summary>
        /// 忽略非数据库列 （默认为:false）
        /// </summary>
        public bool IsIgnoreErrorColumns = false;

        /// <summary>
        /// 设置禁止更新的列
        /// </summary>
        public string[] DisableUpdateColumns { get; set; }

        /// <summary>
        /// 设置禁止插入的列
        /// </summary>
        public string[] DisableInsertColumns { get; set; }

        /// <summary>
        ///设置Queryable或者Sqlable转换成JSON字符串时的日期格式
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
        /// 当前滤器名称
        /// </summary>
        public string CurrentFilterKey = null;

        /// <summary>
        /// 设置过滤器（用户权限过滤）
        /// </summary>
        /// <param name="filters">参数Dictionary string 为过滤器的名称 , Dictionary Func&lt;KeyValueObj&gt; 为过滤函数 (KeyValueObj 中的 Key为Sql条件,Value为Sql参数)</param>
        public void SetFilterFilterParas(Dictionary<string, Func<KeyValueObj>> filters)
        {
            _filterFuns = filters;
        }

        /// <summary>
        /// 设置过滤器（用户权限过滤）
        /// </summary>
        /// <param name="filterColumns">参数Dictionary string 为过滤器的名称 , Dictionary List&lt;string&gt;为允许查询的列的集合</param>
        public void SetFilterFilterParas(Dictionary<string, List<string>> filterColumns)
        {
            if (filterColumns.Values == null || filterColumns.Values.Count == 0)
            {
                throw new Exception("过滤器的列名集合不能为空SetFilterFilterParas.filters");
            }
            _filterColumns = filterColumns;
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
        /// 设置流水号 
        /// </summary>
        /// <param name="serNum">设置流水号所需要的参数集合</param>
        public void SetSerialNumber(List<PubModel.SerialNumber> serNum)
        {
            if (serNum.IsValuable())
            {
                _serialNumber = serNum;
            }
        }
        #endregion


        #region sqlable
        /// <summary>
        /// 创建更接近Sql语句的查询对象
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
        #endregion


        #region queryable
        /// <summary>
        /// 创建拉姆达查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Queryable<T> Queryable<T>() where T : new()
        {
            InitAttributes<T>();
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
                    queryable.WhereValue.Add(whereStr);
                    if (filterValue.Value != null)
                        queryable.Params.AddRange(SqlSugarTool.GetParameters(filterValue.Value));
                }
                if (_filterColumns.IsValuable() && _filterColumns.ContainsKey(CurrentFilterKey))
                {
                    var columns = _filterColumns[CurrentFilterKey];
                    queryable.SelectValue = string.Join(",", columns);
                }
            }
            return queryable;

        }

        /// <summary>
        /// 创建拉姆达查询对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableName">T类型对应的真实表名</param>
        /// <returns></returns>
        public Queryable<T> Queryable<T>(string tableName) where T : new()
        {
            InitAttributes<T>();
            return new Queryable<T>() { DB = this, TableName = tableName };
        }
        #endregion


        #region sqlQuery
        /// <summary>
        /// 根据SQL语句将结果集映射到List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">实体对象</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="whereObj">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns>T的集合</returns>
        public List<T> SqlQuery<T>(string sql, object whereObj = null)
        {
            var pars = SqlSugarTool.GetParameters(whereObj).ToList();
            return SqlQuery<T>(sql, pars);
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到dynamic
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="whereObj">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns>动态集合</returns>
        public dynamic SqlQueryDynamic(string sql, object whereObj = null)
        {
            return JsonConverter.ConvertJson(SqlQueryJson(sql, whereObj));
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到json
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="whereObj">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns>JSON数据</returns>
        public string SqlQueryJson(string sql, object whereObj = null)
        {
            return JsonConverter.DataTableToJson(GetDataTable(sql, whereObj), SerializerDateFormat);
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="pars">SqlParameter的集合</param>
        /// <returns>T的集合</returns>
        public List<T> SqlQuery<T>(string sql, SqlParameter[] pars)
        {
            return SqlQuery<T>(sql, pars.ToList());
        }

        /// <summary>
        /// 根据SQL语句将结果集映射到List&lt;T&gt;
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sql">sql语句</param>
        /// <param name="pars">SqlParameter集合</param>
        /// <returns>T的集合</returns>
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
            sql = string.Format(PubModel.SqlSugarClientConst.SqlQuerySqlTemplate, type.Name, sql);
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
        #endregion


        #region insert
        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">插入对象的集合</param>
        /// <param name="isIdentity">过期参数无需填写</param>
        /// <returns>默认返回Identity的集合，如果没有Identity执行成功将返回bool的集合</returns>
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
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">插入对象</param>
        /// <param name="isIdentity">过期参数无需填写</param>
        /// <returns>默认返回Identity，如果没有Identity执行成功将返回true</returns>
        public object Insert<T>(T entity, bool isIdentity = true) where T : class
        {
            InitAttributes<T>();
            Type type = entity.GetType();
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);

            StringBuilder sbInsertSql = new StringBuilder();
            List<SqlParameter> pars = new List<SqlParameter>();
            var identities = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            isIdentity = identities != null && identities.Count > 0;
            //sql语句缓存
            string cacheSqlKey = "db.Insert." + type.FullName;
            if (this.DisableInsertColumns.IsValuable())
            {
                cacheSqlKey = cacheSqlKey + string.Join("", this.DisableInsertColumns);
            }
            var cacheSqlManager = CacheManager<StringBuilder>.GetInstance();

            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";

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
                    string propName = GetMappingColumnDbName(prop.Name);
                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == propName.ToLower()))
                        {
                            continue;
                        }
                    }
                    if (this.DisableInsertColumns.IsValuable())
                    {
                        if (this.DisableInsertColumns.Any(it => it.ToLower() == propName.ToLower()))
                        {
                            continue;
                        }
                    }

                    //EntityState,@EntityKey
                    if (!isIdentity || identities.Any(it => it.Value.ToLower() != propName.ToLower()))
                    {
                        //4.将属性的名字加入到字符串中 
                        sbInsertSql.Append("[" + propName + "],");
                    }
                }
                //**去掉最后一个逗号 
                sbInsertSql.Remove(sbInsertSql.Length - 1, 1);
                sbInsertSql.Append(" ) values(");

            }

            //5.再次遍历，形成参数列表"(@xx,@xx@xx)"的形式 
            foreach (PropertyInfo prop in props)
            {
                string propName = GetMappingColumnDbName(prop.Name);


                //EntityState,@EntityKey
                if (!isIdentity || identities.Any(it => it.Value.ToLower() != propName))
                {
                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == propName.ToLower()))
                        {
                            continue;
                        }
                    }
                    if (this.DisableInsertColumns.IsValuable())
                    {
                        if (this.DisableInsertColumns.Any(it => it.ToLower() == propName))
                        {
                            continue;
                        }
                    }
                    if (!cacheSqlManager.ContainsKey(cacheSqlKey))
                        sbInsertSql.Append(SqlSugarTool.ParSymbol + propName + ",");
                    object val = prop.GetValue(entity, null);
                    if (val == null)
                        val = DBNull.Value;
                    if (_serialNumber.IsValuable())
                    {
                        Func<PubModel.SerialNumber, bool> serEexp = it => it.TableName.ToLower() == typeName.ToLower() && it.FieldName.ToLower() == propName.ToLower();
                        var isAnyNum = _serialNumber.Any(serEexp);
                        if (isAnyNum && (val == DBNull.Value || val.IsNullOrEmpty()))
                        {
                            if (_serialNumber.First(serEexp).GetNumFunc != null)
                            {
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

                    var par = new SqlParameter(SqlSugarTool.ParSymbol + propName, val);
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
                if (this.IsEnableAttributeMapping && this._mappingColumns.IsValuable())
                {
                    foreach (var item in this._mappingColumns)
                    {
                        sql = sql.Replace("[" + item.Key + "]", "[" + item.Value + "]");
                        sql = sql.Replace(SqlSugarTool.ParSymbol + item.Key + ",", SqlSugarTool.ParSymbol + item.Value + ",");
                        sql = sql.Replace(SqlSugarTool.ParSymbol + item.Key + ")", SqlSugarTool.ParSymbol + item.Value + ")");
                    }
                }
                var lastInsertRowId = GetScalar(sql, pars.ToArray());
                return lastInsertRowId;
            }
            catch (Exception ex)
            {
                throw new SqlSugarException(ex.Message, sql, entity);
            }

        }

        /// <summary>
        /// 大数据插入(结构体必须和数据库一致,不支持属性映射和别名表)
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>全部插入成功返回true</returns>
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

            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
        #endregion


        #region update

        /// <summary>
        /// 根据表达式条件将实体对象更新到数据库
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="rowObj">rowObj为匿名对象时只更新指定列( 例如:new{ name='abc'}只更新name )，为T类型将更新整个实体(排除主键、自增列和禁止更新列)</param>
        /// <param name="expression">表达式条件</param>
        /// <returns>更新成功返回true</returns>
        public bool Update<T>(object rowObj, Expression<Func<T, bool>> expression) where T : class
        {
            InitAttributes<T>();
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }
            if (expression == null) { throw new ArgumentNullException("SqlSugarClient.Update.expression"); }


            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var rows = SqlSugarTool.GetParameters(rowObj);
            var isDynamic = rowObj.GetType() != type;
            var isClass = !isDynamic;

            //sql语句缓存
            string cacheSqlKey = "db.update." + type.FullName + rows.Length;
            var cacheSqlManager = CacheManager<StringBuilder>.GetInstance();



            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);


            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression, this);


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
                    var name = r.ParameterName.TrimStart(SqlSugarTool.ParSymbol);
                    name = GetMappingColumnDbName(name);
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
                    sbSql.Append(string.Format(" [{0}] =" + SqlSugarTool.ParSymbol + "{0}  ,", name));
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
                    par.ParameterName = SqlSugarTool.ParSymbol + GetMappingColumnDbName(par.ParameterName.TrimStart(SqlSugarTool.ParSymbol));
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
                throw new SqlSugarException(ex.Message, sbSql.ToString(), new { rowObj = rowObj, expression = expression + "" });
            }
        }

        /// <summary>
        /// 将实体对象更新到数据库
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="rowObj">rowObj必需包含主键并且不能为匿名对象</param>
        /// <returns>更新成功返回true</returns>
        public bool Update<T>(T rowObj) where T : class
        {
            InitAttributes<T>();
            var isDynamic = typeof(T).IsAnonymousType();
            if (isDynamic)
            {
                throw new SqlSugarException("Update(T)不支持匿名类型，请使用Update<T,Expression>方法。");
            }
            var reval = Update<T, object>(rowObj);
            return reval;
        }

        /// <summary>
        /// 批量插入(说明：一次更新超过10条以上建议使用SqlBulkReplace)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rowObjList">更新实体的集合，rowObj必需包含主键并且不能为匿名对象</param>
        /// <returns>执行成功将返回bool的集合</returns>
        public List<bool> UpdateRange<T>(List<T> rowObjList) where T : class
        {
            var reval = new List<bool>();
            if (rowObjList.IsValuable())
            {
                foreach (T item in rowObjList)
                {
                    reval.Add(Update<T>(item));
                }
            }
            return reval;
        }

        /// <summary>
        /// 将实体对象更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="FiledType"></typeparam>
        /// <param name="rowObj">rowObj为匿名对象时只更新指定列( 例如:new{ name='abc'}只更新name )，为T类型将更新整个实体(排除主键、自增列和禁止更新列)</param>
        /// <param name="whereIn">主键集合</param>
        /// <returns>更新成功返回true</returns>
        public bool Update<T, FiledType>(object rowObj, params FiledType[] whereIn) where T : class
        {
            InitAttributes<T>();
            if (rowObj == null) { throw new ArgumentNullException("SqlSugarClient.Update.rowObj"); }

            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            StringBuilder sbSql = new StringBuilder(string.Format(" UPDATE [{0}] SET ", typeName));
            Dictionary<string, object> rows = SqlSugarTool.GetObjectToDictionary(rowObj);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            string pkClassPropName =pkClassPropName = GetMappingColumnClassName(pkName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            foreach (var r in rows)
            {
                string name = GetMappingColumnDbName(r.Key);
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
                    if (rowObj.GetType() == type)
                    {
                        continue;
                    }
                }
                sbSql.Append(string.Format(" [{0}] =" + SqlSugarTool.ParSymbol + "{0}  ,", name));
            }
            sbSql.Remove(sbSql.Length - 1, 1);
            if (whereIn.Count() == 0)
            {
                var value = type.GetProperties().Cast<PropertyInfo>().Single(it => it.Name == (pkClassPropName == null ? pkName : pkClassPropName)).GetValue(rowObj, null);
                sbSql.AppendFormat("WHERE {1} IN ('{2}')", typeName, pkName, value);
            }
            else
            {
                sbSql.AppendFormat("WHERE {1} IN ({2})", typeName, pkName, whereIn.ToJoinSqlInVal());
            }
            List<SqlParameter> parsList = new List<SqlParameter>();
            var pars = rows.Select(c => new SqlParameter(SqlSugarTool.ParSymbol + c.Key, c.Value));
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    string name = SqlSugarTool.ParSymbol + GetMappingColumnDbName(par.ParameterName.TrimStart(SqlSugarTool.ParSymbol));
                    var isDisableUpdateColumns = DisableUpdateColumns != null && DisableUpdateColumns.Any(it => it.ToLower() == par.ParameterName.TrimStart(SqlSugarTool.ParSymbol).ToLower());
                    if (par.SqlDbType == SqlDbType.Udt || par.ParameterName.ToLower().Contains("hierarchyid"))
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    if (!isDisableUpdateColumns)
                    {
                        par.ParameterName = name;
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
                throw new SqlSugarException(ex.Message, sbSql.ToString(), new { rowObj = rowObj, whereIn = whereIn });
            }
        }

    

        /// <summary>
        /// 大数据更新 支持IsIgnoreErrorColumns和isDisableUpdateColumns
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>全部更新成功返回true</returns>
        public bool SqlBulkReplace<T>(List<T> entities) where T : class
        {
            InitAttributes<T>();
            int actionNum = 100;
            var reval = true;
            while (entities.Count > 0)
            {
                var insertRes = SqlBulkReplace<T>(entities.Take(actionNum));
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

        private bool SqlBulkReplace<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null) { return false; };
            StringBuilder sbSql = new StringBuilder("");
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            pkName = GetMappingColumnDbName(pkName);
            string pkClassPropName = GetMappingColumnClassName(pkName);
            Check.Exception(pkName.IsNullOrEmpty(), "没有找到主键。");
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            var isIdentity = identityNames != null && identityNames.Count > 0;
            var columnNames = SqlSugarTool.GetColumnsByTableName(this, typeName);
            if (isIdentity)
            {
                columnNames = columnNames.Where(c => !identityNames.Any(it => it.Value == c)).ToList();//去掉自添列
            }
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
                string pkValue = string.Empty;
                sbSql.Append(" UPDATE ");
                sbSql.Append(typeName);
                sbSql.Append(" SET ");
                pkValue = props.Single(it => it.Name.ToLower() ==pkClassPropName.ToLower()).GetValue(entity, null).ToString();
                foreach (var name in columnNames)
                {
                    var dbName = GetMappingColumnDbName(name);
                    var className = GetMappingColumnClassName(name);
                    var isPk = pkName != null && pkName.ToLower() == dbName.ToLower();
                    var isDisableUpdateColumns = DisableUpdateColumns != null && DisableUpdateColumns.Any(it => it.ToLower() == dbName.ToLower());
                    var isLastName = name == columnNames.Last();
                    var prop = props.FirstOrDefault(it => it.Name == className);
                    if (prop == null) continue;
                    var objValue = prop.GetValue(entity, null);
                    if (this.IsIgnoreErrorColumns)
                    {
                        if (!SqlSugarTool.GetColumnsByTableName(this, typeName).Any(it => it.ToLower() == dbName.ToLower()))
                        {
                            continue;
                        }
                    }
                    if (isPk || isDisableUpdateColumns)
                    {
                        continue;
                    }
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
                    sbSql.AppendFormat(" [{0}]={1}{2}  ", dbName, objValue, ",");
                }
                sbSql.Remove(sbSql.ToString().LastIndexOf(","), 1);
                sbSql.AppendFormat("WHERE [{0}]='{1}' ", pkName, pkValue.ToSuperSqlFilter());
            }
            var reval = base.ExecuteCommand(sbSql.ToString());
            sbSql = null;
            return reval > 0;
        }
        #endregion


        #region delete
        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T>(Expression<Func<T, bool>> expression)
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression, this);
            string sql = string.Format("DELETE FROM [{0}] WHERE 1=1 {1}", typeName, re.SqlWhere);
            bool isSuccess = ExecuteCommand(sql, re.Paras.ToArray()) > 0;
            return isSuccess;
        }

        /// <summary>
        /// 根据主键集合批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="FiledType">主键类型</typeparam>
        /// <param name="whereIn">主键集合</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T, FiledType>(params FiledType[] whereIn)
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
        /// 根据指定列集合批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="FiledType">指定列的类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <param name="whereIn">批定列值的集合</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, List<FiledType> whereIn)
        {
            InitAttributes<T>();
            if (whereIn == null) return false;
            return Delete<T, FiledType>(expression, whereIn.ToArray());
        }

        /// <summary>
        /// 根据指定列集合批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="FiledType">指定列的类型</typeparam>
        /// <param name="expression">表达式条件</param>
        /// <param name="whereIn">批定列值的集合</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, params FiledType[] whereIn)
        {
            InitAttributes<T>();
            ResolveExpress re = new ResolveExpress();
            var fieldName = re.GetExpressionRightField(expression, this);
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
        /// 状态删除 
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="FiledType">主键类型</typeparam>
        /// <param name="field">标识删除的字段</param>
        /// <param name="whereIn">主键集合</param>
        /// <returns>将field的值更新为1,则返回true表示状态删除成功</returns>
        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn)
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
        /// 状态删除
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="field">标识删除的字段</param>
        /// <param name="expression">表达式条件</param>
        /// <returns>将field的值更新为1,则返回true表示状态删除成功</returns>
        public bool FalseDelete<T>(string field, Expression<Func<T, bool>> expression)
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
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
            re.ResolveExpression(re, expression, this);
            string sql = string.Format("UPDATE  [{0}] SET {1}=1 WHERE  1=1 {2}", typeName, field, re.SqlWhere);
            int deleteRowCount = ExecuteCommand(sql, re.Paras.ToArray());
            isSuccess = deleteRowCount > 0;
            return isSuccess;
        }
        #endregion


        #region create class file

        /// <summary>
        /// 生成实体的对象
        /// </summary>
        public ClassGenerating ClassGenerating = new ClassGenerating();
        #endregion


        #region cache

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void RemoveAllCache<T>()
        {
            CacheManager<T>.GetInstance().RemoveAll(c => true);
        }

        #endregion

    }

}
