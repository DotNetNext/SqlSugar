using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SqlSugar
{
    /// <summary>
    /// SqlSugarTool局部类存放具有拼接SQL的函数(方便工具移植到其它数据库版本)
    /// </summary>
    public partial class SqlSugarTool
    {
        /// <summary>
        /// 将参数sql转成 '('+sql+')'
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string PackagingSQL(string sql)
        {
            return string.Format("({0})", sql);
        }

        internal static StringBuilder GetQueryableSql<T>(Queryable<T> queryable)
        {
            string joinInfo = string.Join(" ", queryable.JoinTableValue);
            string withNoLock = queryable.DB.IsNoLock ? "WITH(NOLOCK)" : null;
            if (joinInfo.IsValuable())
            {
                withNoLock = null;
                if (queryable.DB.IsNoLock)
                {

                }
            }
            StringBuilder sbSql = new StringBuilder();
            string tableName = queryable.TableName.IsNullOrEmpty() ? queryable.TName : queryable.TableName;
            if (queryable.DB.Language.IsValuable() && queryable.DB.Language.Suffix.IsValuable())
            {
                var viewNameList = LanguageHelper.GetLanguageViewNameList(queryable.DB);
                var isLanView = viewNameList.IsValuable() && viewNameList.Any(it => it == tableName);
                if (!queryable.DB.Language.Suffix.StartsWith(LanguageHelper.PreSuffix))
                {
                    queryable.DB.Language.Suffix = LanguageHelper.PreSuffix + queryable.DB.Language.Suffix;
                }

                //将视图变更为多语言的视图
                if (isLanView)
                    tableName = typeof(T).Name + queryable.DB.Language.Suffix;
            }
            if (queryable.DB.PageModel == PageModel.RowNumber)
            {
                #region  rowNumber
                var order = queryable.OrderByValue.IsValuable() ? (",row_index=ROW_NUMBER() OVER(ORDER BY " + queryable.OrderByValue + " )") : null;

                sbSql.AppendFormat("SELECT " + queryable.SelectValue.GetSelectFiles() + " {1} FROM {0} {5} {2} WHERE 1=1 {3} {4} ", tableName.GetTranslationSqlName(), order, withNoLock, string.Join("", queryable.WhereValue), queryable.GroupByValue.GetGroupBy(), joinInfo);
                if (queryable.Skip == null && queryable.Take != null)
                {

                    sbSql.Insert(0, "SELECT * FROM ( ");
                    sbSql.Append(") t WHERE t.row_index<=" + queryable.Take);
                }
                else if (queryable.Skip != null && queryable.Take == null)
                {
                    sbSql.Insert(0, "SELECT * FROM ( ");
                    sbSql.Append(") t WHERE t.row_index>" + (queryable.Skip));
                }
                else if (queryable.Skip != null && queryable.Take != null)
                {

                    sbSql.Insert(0, "SELECT * FROM ( ");
                    sbSql.Append(") t WHERE t.row_index BETWEEN " + (queryable.Skip + 1) + " AND " + (queryable.Skip + queryable.Take));
                }
                #endregion
            }
            else
            {

                #region offset
                var order = queryable.OrderByValue.IsValuable() ? ("ORDER BY " + queryable.OrderByValue + " ") : null;
                sbSql.AppendFormat("SELECT " + queryable.SelectValue.GetSelectFiles() + " {1} FROM {0} {5} {2} WHERE 1=1 {3} {4} ", tableName.GetTranslationSqlName(), "", withNoLock, string.Join("", queryable.WhereValue), queryable.GroupByValue.GetGroupBy(), joinInfo);
                sbSql.Append(order);
                if (queryable.Skip != null || queryable.Take != null)
                {
                    sbSql.AppendFormat("OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY", Convert.ToInt32(queryable.Skip), Convert.ToInt32(queryable.Take));
                }
                #endregion
            }
            return sbSql;
        }

        internal static void GetSqlableSql(Sqlable sqlable, string fileds, string orderByFiled, int pageIndex, int pageSize, StringBuilder sbSql)
        {
            if (sqlable.DB.PageModel == PageModel.RowNumber)
            {
                sbSql.Insert(0, string.Format("SELECT {0},row_index=ROW_NUMBER() OVER(ORDER BY {1} )", fileds, orderByFiled));
                sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.OrderBy);
                sbSql.Append(sqlable.GroupBy);
                int skip = (pageIndex - 1) * pageSize + 1;
                int take = pageSize;
                sbSql.Insert(0, "SELECT * FROM ( ");
                sbSql.AppendFormat(") t WHERE  t.row_index BETWEEN {0}  AND {1}   ", skip, skip + take - 1);
            }
            else
            {
                sbSql.Insert(0, string.Format("SELECT {0}", fileds));
                sbSql.Append(" WHERE 1=1 ").Append(string.Join(" ", sqlable.Where));
                sbSql.Append(sqlable.GroupBy);
                sbSql.AppendFormat(" ORDER BY {0} ", orderByFiled);
                int skip = (pageIndex - 1) * pageSize;
                int take = pageSize;
                sbSql.AppendFormat("OFFSET {0} ROW FETCH NEXT {1} ROWS ONLY", skip, take);
            }
        }

        /// <summary>
        /// 获取 WITH(NOLOCK)
        /// </summary>
        /// <param name="isNoLock"></param>
        /// <returns></returns>
        public static string GetLockString(bool isNoLock)
        {
            return isNoLock ? " WITH(NOLOCK) " : "";
        }
        /// <summary>
        /// 根据表获取主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static string GetPrimaryKeyByTableName(SqlSugarClient db, string tableName)
        {
            var pkValues = GetPrimaryKeyByTableNames(db, tableName);
            if (pkValues.IsValuable())
            {
                return pkValues.First();
            }
            return null;
        }
        /// <summary>
        /// 根据表获取主键
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static List<string> GetPrimaryKeyByTableNames(SqlSugarClient db, string tableName)
        {
            if (DataBaseConfig.IsManualConfiguration) { //手动配置
                if (DataBaseConfig.PrimaryKeys == null) return null;
                return DataBaseConfig.PrimaryKeys.Where(it => it.Key.ToLower() == tableName.ToLower()).Select(it=>it.Value).ToList();
            }
            string key = "GetPrimaryKeyByTableName" + tableName;
            tableName = tableName.ToLower();
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            List<KeyValue> primaryInfo = null;

            //获取主键信息
            if (cm.ContainsKey(key))
                primaryInfo = cm[key];
            else
            {
                string sql = @"  				SELECT a.name as keyName ,d.name as tableName
  FROM   syscolumns a 
  inner  join sysobjects d on a.id=d.id       
  where  exists(SELECT 1 FROM sysobjects where xtype='PK' and  parent_obj=a.id and name in (  
  SELECT name  FROM sysindexes   WHERE indid in(  
  SELECT indid FROM sysindexkeys WHERE id = a.id AND colid=a.colid  
)))";
                var isLog = db.IsEnableLogEvent;
                db.IsEnableLogEvent = false;
                var dt = db.GetDataTable(sql);
                db.IsEnableLogEvent = isLog;
                primaryInfo = new List<KeyValue>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        primaryInfo.Add(new KeyValue() { Key = dr["tableName"].ToString().ToLower(), Value = dr["keyName"].ToString() });
                    }
                }
                cm.Add(key, primaryInfo, cm.Day);
            }

            //反回主键
            if (!primaryInfo.Any(it => it.Key == tableName))
            {
                return null;
            }
            return primaryInfo.Where(it => it.Key == tableName).Select(it => it.Value).ToList();

        }

        /// <summary>
        ///根据表名获取自添列 keyTableName Value columnName
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static List<KeyValue> GetIdentitiesKeyByTableName(SqlSugarClient db, string tableName)
        {
            if (DataBaseConfig.IsManualConfiguration)
            { //手动配置
                if (DataBaseConfig.IdentityKeys == null) return null;
                return DataBaseConfig.IdentityKeys.Where(it => it.Key.ToLower() == tableName.ToLower()).ToList();
            }
            string key = "GetIdentityKeyByTableName" + tableName;
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            List<KeyValue> identityInfo = null;
            if (cm.ContainsKey(key))
            {
                identityInfo = cm[key];
                return identityInfo;
            }
            else
            {
                string sql = string.Format(@"
                            declare @Table_name varchar(60)
                            set @Table_name = '{0}';


                            Select so.name tableName,                   --表名字
                                   sc.name keyName,             --自增字段名字
                                   ident_current(so.name) curr_value,    --自增字段当前值
                                   ident_incr(so.name) incr_value,       --自增字段增长值
                                   ident_seed(so.name) seed_value        --自增字段种子值
                              from sysobjects so 
                            Inner Join syscolumns sc
                                on so.id = sc.id

                                   and columnproperty(sc.id, sc.name, 'IsIdentity') = 1

                            Where upper(so.name) = upper(@Table_name)
         ", tableName);
                var isLog = db.IsEnableLogEvent;
                db.IsEnableLogEvent = false;
                var dt = db.GetDataTable(sql);
                db.IsEnableLogEvent = isLog;
                identityInfo = new List<KeyValue>();
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        identityInfo.Add(new KeyValue() { Key = dr["tableName"].ToString().ToLower(), Value = dr["keyName"].ToString() });
                    }
                }
                cm.Add(key, identityInfo, cm.Day);
                return identityInfo;
            }
        }

        /// <summary>
        /// 根据表名获取列名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        internal static List<string> GetColumnsByTableName(SqlSugarClient db, string tableName)
        {
            string key = "GetColumnNamesByTableName" + tableName;
            var cm = CacheManager<List<string>>.GetInstance();
            if (cm.ContainsKey(key))
            {
                return cm[key];
            }
            else
            {
                var isLog = db.IsEnableLogEvent;
                db.IsEnableLogEvent = false;
                string sql = " SELECT Name FROM SysColumns WHERE id=Object_Id('" + tableName + "')";
                var reval = db.SqlQuery<string>(sql);
                db.IsEnableLogEvent = isLog;
                cm.Add(key, reval, cm.Day);
                return reval;
            }
        }

        /// <summary>
        ///tableOrView  null=u,v , true=u , false=v
        /// </summary>
        /// <param name="tableOrView"></param>
        /// <returns></returns>
        internal static string GetCreateClassSql(bool? tableOrView)
        {
            string sql = null;
            if (tableOrView == null)
            {
                sql = "select name from sysobjects where xtype in ('U','V') ";
            }
            else if (tableOrView == true)
            {
                sql = "select name from sysobjects where xtype in ('U') ";
            }
            else
            {
                sql = "select name from sysobjects where xtype in ('V') ";
            }
            return sql;
        }

        internal static string GetTtableColumnsInfo(string tableName)
        {
            string sql = @"SELECT  Sysobjects.name AS TABLE_NAME ,
								syscolumns.Id  AS TABLE_ID,
								syscolumns.name AS COLUMN_NAME ,
								systypes.name AS DATA_TYPE ,
								syscolumns.length AS CHARACTER_MAXIMUM_LENGTH ,
								sys.extended_properties.[value] AS COLUMN_DESCRIPTION ,
								syscomments.text AS COLUMN_DEFAULT ,
								syscolumns.isnullable AS IS_NULLABLE,
                                (case when exists(SELECT 1 FROM sysobjects where xtype= 'PK' and name in ( 
                                SELECT name FROM sysindexes WHERE indid in( 
                                SELECT indid FROM sysindexkeys WHERE id = syscolumns.id AND colid=syscolumns.colid 
                                ))) then 1 else 0 end) as IS_PRIMARYKEY

								FROM    syscolumns
								INNER JOIN systypes ON syscolumns.xtype = systypes.xtype
								LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
								LEFT OUTER JOIN sys.extended_properties ON ( sys.extended_properties.minor_id = syscolumns.colid
																			 AND sys.extended_properties.major_id = syscolumns.id
																		   )
								LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id
								WHERE   syscolumns.id IN ( SELECT   id
												   FROM     SYSOBJECTS
												   WHERE    xtype in( 'U','V') )
								AND ( systypes.name <> 'sysname' ) AND Sysobjects.name='" + tableName + "'  AND systypes.name<>'geometry' AND systypes.name<>'geography'  ORDER BY syscolumns.colid";
            return sql;
        }

        internal static string GetSelectTopSql()
        {
            return "select top 1 * from {0}";
        }

        /// <summary>
        /// 将SqlType转成C#Type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        internal static string ChangeDBTypeToCSharpType(string typeName)
        {
            string reval = string.Empty;
            switch (typeName.ToLower())
            {
                case "int":
                    reval = "int";
                    break;
                case "text":
                    reval = "string";
                    break;
                case "bigint":
                    reval = "long";
                    break;
                case "binary":
                    reval = "object";
                    break;
                case "bit":
                    reval = "bool";
                    break;
                case "char":
                    reval = "string";
                    break;
                case "datetime":
                case "date":
                case "datetime2":
                    reval = "dateTime";
                    break;
                case "single":
                case "decimal":
                    reval = "decimal";
                    break;
                case "float":
                    reval = "double";
                    break;
                case "image":
                    reval = "byte[]";
                    break;
                case "money":
                    reval = "decimal";
                    break;
                case "nchar":
                    reval = "string";
                    break;
                case "ntext":
                    reval = "string";
                    break;
                case "numeric":
                    reval = "decimal";
                    break;
                case "nvarchar":
                    reval = "string";
                    break;
                case "real":
                    reval = "float";
                    break;
                case "smalldatetime":
                    reval = "dateTime";
                    break;
                case "smallint":
                    reval = "short";
                    break;
                case "smallmoney":
                    reval = "decimal";
                    break;
                case "timestamp":
                    reval = "dateTime";
                    break;
                case "tinyint":
                    reval = "byte";
                    break;
                case "uniqueidentifier":
                    reval = "guid";
                    break;
                case "varbinary":
                    reval = "byte[]";
                    break;
                case "varchar":
                    reval = "string";
                    break;
                case "Variant":
                    reval = "object";
                    break;
                default:
                    reval = "other";
                    break;
            }
            return reval;
        }

        /// <summary>
        /// par的符号
        /// </summary>
        public const char ParSymbol = '@';

        /// <summary>
        /// 获取转释后的表名和列名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetTranslationSqlName(string name)
        {
            Check.ArgumentNullException(name, "表名或者列名不能为空，检查所在表是否有主键。");
            var hasScheme = name.Contains(".");
            if (name.Contains("[")) return name;
            if (hasScheme)
            {
                var array = name.Split('.');
                if (array.Length == 2)
                {
                    return string.Format("[{0}].[{1}]", array.First(), array.Last());
                }
                else
                {
                    return string.Join(".", array.Select(it => "[" + it + "]"));
                }
            }
            else
            {
                return "[" + name + "]";
            }
        }
        /// <summary>
        /// 获取参数名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetSqlParameterName(string name)
        {
            return ParSymbol + name;
        }

        /// <summary>
        ///获取没有符号的参数名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static string GetSqlParameterNameNoParSymbol(string name)
        {
            return name.TrimStart(ParSymbol);
        }

        /// <summary>
        /// 获取Schema和表名的集合
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        internal static List<KeyValue> GetSchemaList(SqlSugarClient db)
        {
            var cm = CacheManager<List<KeyValue>>.GetInstance();
            string cacheKey = "SqlSugarTool.GetSchemaList";
            if (cm.ContainsKey(cacheKey)) return cm[cacheKey];
            else
            {
                var reval = db.SqlQuery<KeyValue>(@"select  s.name as [Key],t.name as [Value] from sys.sysobjects t , sys.schemas s where t.xtype in ('U','V') and t.uid = s.schema_id");
                cm.Add(cacheKey, reval, cm.Day);
                return reval;
            }
        }
    }
}
