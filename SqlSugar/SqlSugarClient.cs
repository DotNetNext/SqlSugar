using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Reflection;
using System.Data;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

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
        /// <summary>
        /// 初始化 SqlSugarClient 类的新实例（主从模式）
        /// </summary>
        /// <param name="masterConnectionString">主:写入事务等操作</param>
        /// <param name="slaveConnectionString">从:无事务读取</param>
        public SqlSugarClient(string masterConnectionString, params string[] slaveConnectionString)
            : base(masterConnectionString, slaveConnectionString)
        {
            ConnectionString = masterConnectionString;
            IsNoLock = false;
        }
        #endregion


        #region private variables
        internal List<KeyValue> _mappingTableList = null;
        internal List<KeyValue> _mappingColumns
        {
            get
            {
                string cacheKey = "SqlSugarClient.InitAttributes";
                var cm = CacheManager<List<KeyValue>>.GetInstance();
                return cm[cacheKey];

            }
        }
        private Dictionary<string, Func<KeyValueObj>> _filterRows = null;
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
                string cacheKey = "SqlSugarClient.InitAttributes";
                var cm = CacheManager<List<KeyValue>>.GetInstance();
                var mappingInfo = ReflectionSugarMapping.GetMappingInfo<T>();
                if (_mappingTableList == null)
                {
                    _mappingTableList = new List<KeyValue>();
                }
                if (!_mappingTableList.Contains(mappingInfo.TableMaping))
                {
                    _mappingTableList.Add(mappingInfo.TableMaping);
                    if (mappingInfo.ColumnsMapping.IsValuable())
                    {
                        foreach (var item in mappingInfo.ColumnsMapping)
                        {
                            if (_mappingColumns == null || !_mappingColumns.Any(it => it.Key == item.Key))
                            {
                                List<KeyValue> cmList = cm.ContainsKey(cacheKey) ? cm[cacheKey] : new List<KeyValue>();
                                cmList.Add(item);
                                cm.Add(cacheKey, cmList, cm.Day);
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
        private void AddFilter<T>(Queryable<T> queryable, string key) where T : new()
        {
            if (_filterRows.ContainsKey(key))
            {
                var filterInfo = _filterRows[key];
                var filterValue = filterInfo();
                string whereStr = string.Format(" AND {0} ", filterValue.Key);
                queryable.WhereValue.Add(whereStr);
                if (filterValue.Value != null)
                    queryable.Params.AddRange(SqlSugarTool.GetParameters(filterValue.Value));
            }
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
        /// 设置批量操作的数量
        /// </summary>
        public int BulkNum = 100;

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
        /// 添加禁止更新列
        /// </summary>
        /// <param name="columns"></param>
        public void AddDisableUpdateColumns(params string[] columns) {

            this.DisableUpdateColumns = this.DisableUpdateColumns.ArrayAdd(columns);
        }

        /// <summary>
        /// 设置禁止插入的列
        /// </summary>
        public string[] DisableInsertColumns { get; set; }

        /// <summary>
        /// 添加禁止插入列
        /// </summary>
        /// <param name="columns"></param>
        public void AddDisableInsertColumns(params string[] columns)
        {
            this.DisableInsertColumns = this.DisableInsertColumns.ArrayAdd(columns);
        }

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
        /// <param name="filterRows">参数Dictionary string 为过滤器的名称 , Dictionary Func&lt;KeyValueObj&gt; 为过滤函数 (KeyValueObj 中的 Key为Sql条件,Value为Sql参数)</param>
        public void SetFilterItems(Dictionary<string, Func<KeyValueObj>> filterRows)
        {
            _filterRows = filterRows;
        }

        /// <summary>
        /// 设置过滤器（用户权限过滤）
        /// </summary>
        /// <param name="filterColumns">参数Dictionary string 为过滤器的名称 , Dictionary List&lt;string&gt;为允许查询的列的集合</param>
        public void SetFilterItems(Dictionary<string, List<string>> filterColumns)
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
        /// 添加实体类与表名的映射， Key为实体类 Value为表名
        /// </summary>
        /// <param name="mappingTable"></param>
        public void AddMappingTable(KeyValue mappingTable)
        {
            Check.ArgumentNullException(mappingTable, "AddMappingTables.mappingTable不能为null。");
            if (_mappingTableList == null)
                _mappingTableList = new List<KeyValue>();
            Check.Exception(_mappingTableList.Any(it => it.Key == mappingTable.Key), "mappingTable的Key已经存在。");
            _mappingTableList.Add(mappingTable);
        }
        /// <summary>
        /// 设置实体字段与数据库字段的映射，Key为实体字段 Value为表字段名称 （注意：不区分表，设置后所有表通用）
        /// </summary>
        /// <param name="mappingColumns"></param>
        public void SetMappingColumns(List<KeyValue> mappingColumns)
        {

            if (mappingColumns.IsValuable())
            {
                string cacheKey = "SqlSugarClient.InitAttributes";
                var cm = CacheManager<List<KeyValue>>.GetInstance();
                cm.RemoveAll();
                cm.Add(cacheKey, mappingColumns, cm.Day);
            }
        }
        /// <summary>
        /// 添加实体字段与数据库字段的映射，Key为实体字段 Value为表字段名称 （注意：不区分表，设置后所有表通用）
        /// </summary>
        /// <param name="mappingColumn"></param>
        public void AddMappingColumn(KeyValue mappingColumn)
        {
            Check.ArgumentNullException(mappingColumn, "AddMappingTables.mappingColumns不能为null。");
            Check.Exception(_mappingColumns.Any(it => it.Key == mappingColumn.Key), "mappingColumns的Key已经存在。");
            _mappingColumns.Add(mappingColumn);
            string cacheKey = "SqlSugarClient.InitAttributes";
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            cm.Add(cacheKey, _mappingColumns, cm.Day);
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

        /// <summary>
        /// 在同一个会话中可以存储一些临时数据
        /// </summary>
        public object TempData = null;
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
                if (_filterRows.IsValuable())
                {
                    var keys = CurrentFilterKey.Split(',');
                    foreach (var key in keys)
                    {
                        if (_filterRows.ContainsKey(key))
                        {
                            var filterInfo = _filterRows[key];
                            var filterVlue = filterInfo();
                            string whereStr = string.Format(" AND {0} ", filterVlue.Key);
                            sqlable.Where.Add(whereStr);
                            if (filterVlue.Value != null)
                                sqlable.Params.AddRange(SqlSugarTool.GetParameters(filterVlue.Value));
                        }
                    }
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
        public Queryable<T> Queryable<T>() where T : class,new()
        {
            InitAttributes<T>();
            var queryable = new Queryable<T>() { DB = this, TableName = typeof(T).Name };
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
                if (_filterRows.IsValuable())
                {
                    string keys = CurrentFilterKey;
                    foreach (var key in keys.Split(','))
                    {
                        AddFilter<T>(queryable, key);
                    }
                }
                if (_filterColumns.IsValuable())
                {
                    var keys = CurrentFilterKey.Split(',');
                    foreach (var key in keys)
                    {
                        if (_filterColumns.ContainsKey(key))
                        {
                            var columns = _filterColumns[key];
                            Check.Exception(queryable.SelectValue.IsValuable(), "对不起列过滤只能设一个，行过滤可以设多个。");
                            queryable.SelectValue = string.Join(",", columns);
                        }
                    }
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
        public Queryable<T> Queryable<T>(string tableName) where T : class,new()
        {
            InitAttributes<T>();
            var queryable = new Queryable<T>() { DB = this, TableName = tableName };
            //全局过滤器
            if (CurrentFilterKey.IsValuable())
            {
                if (_filterRows.IsValuable())
                {
                    string keys = CurrentFilterKey;
                    foreach (var key in keys.Split(','))
                    {
                        AddFilter<T>(queryable, key);
                    }
                }
                if (_filterColumns.IsValuable())
                {
                    var keys = CurrentFilterKey.Split(',');
                    foreach (var key in keys)
                    {
                        if (_filterColumns.ContainsKey(key))
                        {
                            var columns = _filterColumns[key];
                            Check.Exception(queryable.SelectValue.IsValuable(), "对不起列过滤只能设一个，行过滤可以设多个。");
                            queryable.SelectValue = string.Join(",", columns);
                        }
                    }
                }
            }
            return queryable;
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
                if (_filterRows.IsValuable())
                {
                    var keys = CurrentFilterKey.Split(',');
                    foreach (var key in keys)
                    {
                        if (_filterRows.ContainsKey(key))
                        {
                            var filterInfo = _filterRows[key];
                            var filterValue = filterInfo();
                            sql += string.Format(" AND {0} ", filterValue.Key);
                            if (filterValue.Value != null)
                            {
                                pars.AddRange(SqlSugarTool.GetParameters(filterValue.Value));
                            }
                        }
                    }
                }
            }
            var type = typeof(T);
            if (base.CommandType == CommandType.Text)
            {
                sql = string.Format(PubModel.SqlSugarClientConst.SqlQuerySqlTemplate, type.Name, sql);
            }
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


                sbInsertSql.Append("insert into " + typeName.GetTranslationSqlName() + " (");

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
                        sbInsertSql.Append(propName.GetTranslationSqlName() + ",");
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
                if (!isIdentity || identities.Any(it => it.Value.ToLower() != propName.ToLower()))
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
                        if (this.DisableInsertColumns.Any(it => it.ToLower() == propName.ToLower()))
                        {
                            continue;
                        }
                    }
                    if (!cacheSqlManager.ContainsKey(cacheSqlKey))
                        sbInsertSql.Append(propName.GetSqlParameterName() + ",");
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
                        val =Convert.ToInt64(val);
                    }

                    var par = new SqlParameter(SqlSugarTool.ParSymbol + propName, val);
                    SqlSugarTool.SetParSize(par);
                    if (par.SqlDbType == SqlDbType.Udt)
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    if (val == DBNull.Value) {//防止文件类型报错
                        SqlSugarTool.SetSqlDbType(prop,par);
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
                var lastInsertRowId = base.GetScalar(sql, pars.ToArray());
                return lastInsertRowId;
            }
            catch (Exception ex)
            {
                throw new SqlSugarException(ex.Message, sql, entity);
            }

        }

        /// <summary>
        /// 大数据插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns>全部插入成功返回true</returns>
        public bool SqlBulkCopy<T>(List<T> entities) where T : class
        {
            int actionNum = BulkNum;
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
            InitAttributes<T>();
            if (entities == null) { return false; };

            Type type = typeof(T);

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

            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            var isIdentity = identityNames != null && identityNames.Count > 0;
            var columnNames =props.Select(it=>it.Name).ToList();
            if (DisableInsertColumns.IsValuable())
            {//去除禁止插入列
                columnNames.RemoveAll(it=>DisableInsertColumns.Any(dc=>dc.ToLower()==it.ToLower()));
            }
            //启用别名列
            if (this.IsEnableAttributeMapping = true && _mappingColumns.IsValuable()) {
                //将别名列转换成数据列
                columnNames = columnNames.Select(it =>
                {
                    var cmInfo=_mappingColumns.Where(mc => mc.Key == it).ToList();
                    return cmInfo.IsValuable()?cmInfo.Single().Value:it;
                }).ToList();
            }
            if (this.IsIgnoreErrorColumns)
            {//去除非数据库列
               var tableColumns=SqlSugarTool.GetColumnsByTableName(this, typeName);
               columnNames = columnNames.Where(it => tableColumns.Any(tc => tc.ToLower() == it.ToLower())).ToList();
            }
            if (isIdentity)
            {
                columnNames = columnNames.Where(c => !identityNames.Any(it => it.Value == c)).ToList();//去掉自添列
            
            }
            Check.Exception(columnNames == null || columnNames.Count == 0, "没有可插入的列，请查看实体和插入配置。");

            StringBuilder sbSql = new StringBuilder("INSERT INTO ");
            sbSql.AppendLine(typeName.GetTranslationSqlName());
            sbSql.AppendFormat("({0})", string.Join(",", columnNames.Select(it => it.GetTranslationSqlName())));


            foreach (var entity in entities)
            {

                sbSql.AppendLine("SELECT ");
                foreach (var name in columnNames)
                {
                    var className = name;
                      //启用别名列
                    if (this.IsEnableAttributeMapping = true && _mappingColumns.IsValuable()) {
                        var mappInfo = _mappingColumns.Where(mc => mc.Value.ToLower() == name.ToLower()).ToList();
                        if (mappInfo.IsValuable()) {
                            className = mappInfo.Single().Key;
                        }
                    }
                    var isLastName = name == columnNames.Last();
                    var prop = props.Single(it => it.Name == className);
                    var objValue = prop.GetValue(entity, null);
                    bool isNullable = false;
                    var underType = SqlSugarTool.GetUnderType(prop, ref isNullable);
                    if (objValue == null)
                    {
                        objValue = "NULL";
                    }
                    else if (underType == SqlSugarTool.DateType)
                    {
                        objValue = "'" + objValue.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss") + "'";
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

        #region InsertOrUpdate
        /// <summary>
        /// 主键有值则更新，无值则插入，不支持复合主键。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="operationObj">操作的实体对象</param>
        /// <returns>更新返回bool,插入如果有自增列返回自增列的值否则也返回bool</returns>
        public object InsertOrUpdate<T>(T operationObj) where T : class
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            string pkClassName = GetMappingColumnClassName(pkName);
            Check.Exception(pkName == null, string.Format("InsertOrUpdate操作失败，因为表{0}中不存在主键。", typeName));
            var prop= type.GetProperties().Single(it => it.Name.ToLower() == pkClassName.ToLower());
            var value= prop.GetValue(operationObj,null);
            var isAdd = value == null || value.ToString() == "" || value.ToString() == "0" || value.ToString() == Guid.Empty.ToString();
            if (isAdd) {
                return Insert(operationObj); 
            } else { 
                return Update(operationObj); 
            }
        }
        #endregion

        #region update
        /// <summary>
        /// 根据表达式条件将实体对象更新到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="setValues">set后面的部分字符串 例如( id=@id,num=num+1 )</param>
        /// <param name="expression">表达式条件</param>
        /// <param name="whereObj">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns></returns>
        public bool Update<T>(string setValues, Expression<Func<T, bool>> expression, object whereObj=null) where T:class
        {
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            Check.ArgumentNullException(setValues.IsNullOrEmpty(), "Update.setValues不为能空。");
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression, this);
            string sql =string.Format( "UPDATE {0} SET {1} WHERE 1=1 {2}",typeName.GetTranslationSqlName(),setValues,re.SqlWhere);
            var pars = SqlSugarTool.GetParameters(whereObj).ToList();
            pars.AddRange(re.Paras);
            var reval= base.ExecuteCommand(sql, pars.ToArray())>0;
            sql = null;
            return reval;
        }

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
                sbSql = new StringBuilder(string.Format(" UPDATE {0} SET ", typeName.GetTranslationSqlName()));
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
                    sbSql.Append(string.Format(" {0}={1},", name.GetTranslationSqlName(), name.GetSqlParameterName()));
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
                var updateRowCount = base.ExecuteCommand(sbSql.ToString(), parsList.ToArray());
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
            StringBuilder sbSql = new StringBuilder();
            Type type = typeof(T);
            var isClassUpdate=whereIn.Length==0;
            PropertyInfo[] props = null;
            //属性缓存
            string cachePropertiesKey = "db." + type.FullName + ".GetProperties";
            var cachePropertiesManager = CacheManager<PropertyInfo[]>.GetInstance();
            if (isClassUpdate) {
                if (cachePropertiesManager.ContainsKey(cachePropertiesKey))
                {
                    props = cachePropertiesManager[cachePropertiesKey];
                }
                else
                {
                    props = type.GetProperties();
                    cachePropertiesManager.Add(cachePropertiesKey, props, cachePropertiesManager.Day);
                }

            }
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var pars = SqlSugarTool.GetParameters(rowObj, props);
            string pkName = SqlSugarTool.GetPrimaryKeyByTableName(this, typeName);
            string pkClassPropName = pkClassPropName = GetMappingColumnClassName(pkName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            string cacheKey = typeName + this.IsIgnoreErrorColumns;

            if (!isClassUpdate)
            {
                cacheKey += string.Join("", pars.Select(it => it.ParameterName));
            }
            if (_mappingColumns.IsValuable()) {
                cacheKey +=string.Join("", _mappingColumns.Select(it => it.Key)); ;
            }
            var cm=CacheManager<string>.GetInstance();
            if (cm.ContainsKey(cacheKey))
            {
                sbSql.Append(cm[cacheKey]);
            }
            else
            {
                sbSql.Append(string.Format(" UPDATE {0} SET ", typeName.GetTranslationSqlName()));
                foreach (var r in pars)
                {
                    string name = GetMappingColumnDbName(r.ParameterName.GetSqlParameterNameNoParSymbol());
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
                            continue;
                    }
                    sbSql.Append(string.Format(" {0}={1}  ,", name.GetTranslationSqlName(), name.GetSqlParameterName()));
                }
                sbSql.Remove(sbSql.Length - 1, 1);
                cm.Add(cacheKey, sbSql.ToString(), cm.Day);
            }
            if (isClassUpdate)
            {
                sbSql.AppendFormat("WHERE {0} = @{1}",pkName.GetTranslationSqlName(),pkName);
            }
            else
            {
                sbSql.AppendFormat("WHERE {0} IN ({1})",  pkName.GetTranslationSqlName(), whereIn.ToJoinSqlInVal());
            }
            if (pars != null)
            {
                pars = pars.Select(par =>
                {
                    string name = SqlSugarTool.ParSymbol + GetMappingColumnDbName(par.ParameterName.TrimStart(SqlSugarTool.ParSymbol));
                    if (par.Value != null && par.Value.GetType().IsClass && par.Value.GetType() != SqlSugarTool.StringType)
                    {
                        par.Value = DBNull.Value;
                    }
                    if (par.SqlDbType == SqlDbType.Udt || par.ParameterName.ToLower().Contains("hierarchyid"))
                    {
                        par.UdtTypeName = "HIERARCHYID";
                    }
                    par.ParameterName = name;
                    return par;

                }).ToArray();
            }
            try
            {
                var updateRowCount = base.ExecuteCommand(sbSql.ToString(), pars);
                sbSql = null;
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
            int actionNum = BulkNum;
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
            InitAttributes<T>();
            if (entities == null) { return false; };

            Type type = typeof(T);

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

            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var pkNames = SqlSugarTool.GetPrimaryKeyByTableNames(this, typeName);
            var identityNames = SqlSugarTool.GetIdentitiesKeyByTableName(this, typeName);
            var isIdentity = identityNames != null && identityNames.Count > 0;
            var columnNames = props.Select(it => it.Name).ToList();
            if (DisableUpdateColumns.IsValuable())
            {//去除禁止插入列
                columnNames.RemoveAll(it => DisableUpdateColumns.Any(dc => dc.ToLower()==it.ToLower()));
            }
            //启用别名列
            if (this.IsEnableAttributeMapping = true && _mappingColumns.IsValuable())
            {
                //将别名列转换成数据列
                columnNames = columnNames.Select(it =>
                {
                    var cmInfo = _mappingColumns.Where(mc => mc.Key == it).ToList();
                    return cmInfo.IsValuable() ? cmInfo.Single().Value : it;
                }).ToList();
            }
            if (this.IsIgnoreErrorColumns)
            {//去除非数据库列
                var tableColumns = SqlSugarTool.GetColumnsByTableName(this, typeName);
                columnNames = columnNames.Where(it => tableColumns.Any(tc => tc.ToLower() == it.ToLower())).ToList();
            }
            var columnNamesWidthIdentity = columnNames;
            if (isIdentity)
            {
                columnNames = columnNames.Where(c => !identityNames.Any(it => it.Value == c)).ToList();//去掉自添列

            }
            Check.Exception(columnNames == null || columnNames.Count == 0, "没有可插入的列，请查看实体和插入配置。");

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat(@"UPDATE S SET {{0}} FROM {0} S INNER JOIN 
            (
              {{1}}

            ) T ",typeName.GetTranslationSqlName());
            string sqlWhere = null;
            foreach (var item in pkNames)
            {
                var isFirst = pkNames.IndexOf(item)==0;
                sqlWhere += (isFirst ? (" ON " + string.Format("T.{0}=S.{0}", item.GetTranslationSqlName())) : string.Format(" AND T.{0}=S.{0}", item.GetTranslationSqlName()));
            }
            sbSql.Append(sqlWhere);
            StringBuilder sbSqlInnerFromTables = new StringBuilder();
            StringBuilder sbSqlInnerUpdateColumns = new StringBuilder();
            sbSqlInnerUpdateColumns.Append(string.Join(",", columnNames.Select(it => "S." + it.GetTranslationSqlName() + "=" + "T." + it.GetTranslationSqlName())));
            foreach (var entity in entities)
            {

                sbSqlInnerFromTables.AppendLine("SELECT ");
                foreach (var name in columnNamesWidthIdentity)
                {
                    var className = name;
                    //启用别名列
                    if (this.IsEnableAttributeMapping = true && _mappingColumns.IsValuable())
                    {
                        var mappInfo = _mappingColumns.Where(mc => mc.Value.ToLower() == name.ToLower()).ToList();
                        if (mappInfo.IsValuable())
                        {
                            className = mappInfo.Single().Key;
                        }
                    }
                    var isLastName = name == columnNames.Last();
                    var prop = props.Single(it => it.Name == className);
                    var objValue = prop.GetValue(entity, null);
                    bool isNullable = false;
                    var underType = SqlSugarTool.GetUnderType(prop, ref isNullable);
                    if (objValue == null)
                    {
                        objValue = "NULL";
                    }
                    else if (underType == SqlSugarTool.DateType)
                    {
                        objValue = "'" + objValue.ObjToDate().ToString("yyyy-MM-dd HH:mm:ss")  + "'";
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

                    sbSqlInnerFromTables.Append(objValue + (isLastName ? (" AS "+name) :(" AS "+name+",")));
                }
                var isLastEntity = entities.Last() == entity;
                if (!isLastEntity)
                {
                    sbSqlInnerFromTables.AppendLine(" UNION ALL ");
                }
            }

            string sql = string.Format(sbSql.ToString(), sbSqlInnerUpdateColumns.ToString(), sbSqlInnerFromTables);
            var reval = base.ExecuteCommand(sql);
            sbSqlInnerFromTables = null;
            sbSqlInnerUpdateColumns=null;
            sbSql = null;
            sql = null;
            return reval > 0;
        }
        #endregion


        #region delete
        /// <summary>
        /// 根据实体删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteObj"></param>
        /// <returns></returns>
        public bool Delete<T>(T deleteObj) where T:class
        {
            InitAttributes<T>();
            var isDynamic = typeof(T).IsAnonymousType();
            if (isDynamic)
            {
                throw new SqlSugarException("Delete(T)不支持匿名类型。");
            }
            Type type = typeof(T);
            if (deleteObj == null) { throw new ArgumentNullException("SqlSugarClient.Delete.deleteObj"); }
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var pkNames = SqlSugarTool.GetPrimaryKeyByTableNames(this, typeName);
            Check.ArgumentNullException(pkNames == null || pkNames.Count==0, typeName + "没有找到主键。");
            string whereString = "";
            var pars=new List<SqlParameter>();
            foreach (var pkName in pkNames)
            {
              string pkClassPropName = pkClassPropName = GetMappingColumnClassName(pkName);
              var pkValue=type.GetProperty(pkClassPropName).GetValue(deleteObj,null);
              if (pkValue.GetType().IsEnum) {
                  pkValue = pkValue.ObjToInt();
              }
              Check.Exception(pkValue == DBNull.Value, typeName + "主键的值不能为DBNull.Value。");
              whereString += string.Format(" AND {0}={1} ",pkName.GetTranslationSqlName(),pkName.GetSqlParameterName());
              SqlParameter par= new SqlParameter(pkName.GetSqlParameterName(), pkValue);
              pars.Add(par);
              SqlSugarTool.SetParSize(par);
            }
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 {1}", typeName.GetTranslationSqlName(),whereString);
            bool isSuccess = base.ExecuteCommand(sql, pars.ToArray()) > 0;
            return isSuccess;
        }

        /// <summary>
        /// 根据实体集合删除（对性能没有要求可以使用,否则请使用其它重载方法）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deleteObjList"></param>
        /// <returns>全部删除成功返回true</returns>
        public bool Delete<T>(List<T> deleteObjList) where T:class
        {
            if (deleteObjList == null || deleteObjList.Count == 0) return false;
            var reval=true;
            foreach (var item in deleteObjList)
            {
                if (Delete(item)==false) {
                    reval = false;
                    break;
                }
            }
            return reval;
        }

        /// <summary>
        /// 根据表达式删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression">表达式条件</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T>(Expression<Func<T, bool>> expression) where T:class
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            ResolveExpress re = new ResolveExpress();
            re.ResolveExpression(re, expression, this);
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 {1}", typeName.GetTranslationSqlName(), re.SqlWhere);
            bool isSuccess = base.ExecuteCommand(sql, re.Paras.ToArray()) > 0;
            return isSuccess;
        }


        /// <summary>
        /// 根据Where字符串删除
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sqlWhereString">不包含Where的字符串</param>
        /// <param name="whereObj">匿名参数(例如:new{id=1,name="张三"})</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T>(string sqlWhereString, object whereObj = null) where T:class
        {
            InitAttributes<T>();
            Type type = typeof(T);
            string typeName = type.Name;
            typeName = GetTableNameByClassType(typeName);
            var pars = SqlSugarTool.GetParameters(whereObj).ToList();
            if (sqlWhereString.IsValuable()) {
                sqlWhereString = Regex.Replace(sqlWhereString,@"^\s*(and|where)\s*","",RegexOptions.IgnoreCase);
            }
            string sql = string.Format("DELETE FROM {0} WHERE 1=1 AND {1}", typeName.GetTranslationSqlName(), sqlWhereString);
            bool isSuccess = base.ExecuteCommand(sql, pars.ToArray()) > 0;
            return isSuccess;
        }

        /// <summary>
        /// 根据主键集合批量删除数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <typeparam name="FiledType">主键类型</typeparam>
        /// <param name="whereIn">主键集合</param>
        /// <returns>删除成功返回true</returns>
        public bool Delete<T, FiledType>(params FiledType[] whereIn) where T:class
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
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", typeName.GetTranslationSqlName(), SqlSugarTool.GetPrimaryKeyByTableName(this, typeName).GetTranslationSqlName(), whereIn.ToJoinSqlInVal());
                int deleteRowCount = base.ExecuteCommand(sql);
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
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, List<FiledType> whereIn) where T:class
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
        public bool Delete<T, FiledType>(Expression<Func<T, object>> expression, params FiledType[] whereIn) where T:class
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
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN ({2})", typeName.GetTranslationSqlName(), fieldName.GetTranslationSqlName(), whereIn.ToJoinSqlInVal());
                int deleteRowCount = base.ExecuteCommand(sql);
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
        public bool FalseDelete<T, FiledType>(string field, params FiledType[] whereIn) where T:class
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
                string sql = string.Format("UPDATE  {0} SET {3}=1 WHERE {1} IN ({2})", typeName.GetTranslationSqlName(), SqlSugarTool.GetPrimaryKeyByTableName(this, typeName), whereIn.ToJoinSqlInVal(), field);
                int deleteRowCount = base.ExecuteCommand(sql);
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
        public bool FalseDelete<T>(string field, Expression<Func<T, bool>> expression) where T:class
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
            string sql = string.Format("UPDATE  {0} SET {1}=1 WHERE  1=1 {2}", typeName.GetTranslationSqlName(), field, re.SqlWhere);
            int deleteRowCount = base.ExecuteCommand(sql, re.Paras.ToArray());
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
