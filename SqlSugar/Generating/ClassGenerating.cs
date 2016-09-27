using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace SqlSugar
{
    /// <summary>
    /// ** 描述：实体生成类
    /// ** 创始时间：2015-4-17
    /// ** 修改时间：-
    /// ** 作者：sunkaixuan
    /// ** qq：610262374 欢迎交流,共同提高 ,命名语法等写的不好的地方欢迎大家的给出宝贵建议
    /// ** 使用说明：http://www.cnblogs.com/sunkaixuan/p/4482152.html
    /// </summary>
    public class ClassGenerating
    {

        /// <summary>
        /// 根据匿名类获取实体类的字符串
        /// </summary>
        /// <param name="entity">匿名对象</param>
        /// <param name="className">生成的类名</param>
        /// <returns></returns>
        public string DynamicToClass(object entity, string className)
        {
            StringBuilder propertiesValue = new StringBuilder();
            var propertiesObj = entity.GetType().GetProperties();
            string replaceGuid = Guid.NewGuid().ToString();
            string nullable = string.Empty;
            var classTemplate = ClassTemplate.Template;
            string _ns = "";
            string _foreach = "";
            string _className = className;
            foreach (var r in propertiesObj)
            {

                var type = r.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    type = type.GetGenericArguments()[0];
                    nullable = "?";
                }
                if (!type.Namespace.Contains("System.Collections.Generic"))
                {
                    propertiesValue.AppendLine();
                    string typeName = ChangeType(type);
                    propertiesValue.AppendFormat(ClassTemplate.ItemTemplate, typeName, r.Name, "{get;set;}", nullable);
                    propertiesValue.AppendLine();
                }
            }
            _foreach = propertiesValue.ToString();
            classTemplate = ClassTemplate.Replace(classTemplate, _ns, _foreach, _className);
            return classTemplate;
        }


        /// <summary>
        /// 根据DataTable获取实体类的字符串
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string DataTableToClass(DataTable dt, string className, string nameSpace = null, List<PubModel.DataTableMap> dataTableMapList = null)
        {
            StringBuilder reval = new StringBuilder();
            StringBuilder propertiesValue = new StringBuilder();
            string replaceGuid = Guid.NewGuid().ToString();

            var template = ClassTemplate.Template;
            string _ns = nameSpace;
            string _foreach = "";
            string _className = className;
            List<string> _primaryKeyName = new List<string>();
            foreach (DataColumn r in dt.Columns)
            {
                propertiesValue.AppendLine();
                string typeName = ChangeType(r.DataType);
                bool isAny = false;
                PubModel.DataTableMap columnInfo = new PubModel.DataTableMap();
                if (dataTableMapList.IsValuable())
                {
                    isAny = dataTableMapList.Any(it => it.COLUMN_NAME.ToString() == r.ColumnName);
                    if (isAny)
                    {
                        columnInfo = dataTableMapList.First(it => it.COLUMN_NAME.ToString() == r.ColumnName);
                        if (columnInfo.IS_PRIMARYKEY.ToString() == "1")
                        {
                            _primaryKeyName.Add(r.ColumnName);
                        }
                        propertiesValue.AppendFormat(@"     /// <summary>
     /// Desc:{0} 
     /// Default:{1} 
     /// Nullable:{2} 
     /// </summary>
",
   columnInfo.COLUMN_DESCRIPTION.IsValuable() ? columnInfo.COLUMN_DESCRIPTION.ToString() : "-", //{0}
   columnInfo.COLUMN_DEFAULT.IsValuable() ? columnInfo.COLUMN_DEFAULT.ToString() : "-", //{1}
   Convert.ToBoolean(columnInfo.IS_NULLABLE));//{2}
                    }
                }
                propertiesValue.AppendFormat(
                    ClassTemplate.ItemTemplate,
                    isAny ? ChangeNullable(typeName, Convert.ToBoolean(columnInfo.IS_NULLABLE)) : typeName,
                    r.ColumnName, "{get;set;}",
                    "");
                propertiesValue.AppendLine();
            }
            _foreach = propertiesValue.ToString();

            template = ClassTemplate.Replace(template, _ns, _foreach, _className, _primaryKeyName);
            return template;
        }


        /// <summary>
        /// 根据SQL语句获取实体类的字符串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string SqlToClass(SqlSugarClient db, string sql, string className)
        {
            var dt = db.GetDataTable(sql);
            var reval = DataTableToClass(dt, className);
            return reval;

        }
        /// <summary>
        /// 根据表名获取实体类的字符串
        /// </summary>
        /// <param name="db"></param>
        /// <param name="sql"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public string TableNameToClass(SqlSugarClient db, string tableName)
        {
            var dt = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
            var tableColumns = GetTableColumns(db, tableName);
            var reval = DataTableToClass(dt, tableName, null, tableColumns);
            return reval;
        }



        /// <summary>
        /// 创建SQL实体文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileDirectory"></param>
        /// <param name="nameSpace">命名空间（默认：null）</param>
        /// <param name="tableOrView">是生成视图文件还是表文件,null生成表和视图，true生成表，false生成视图(默认为：null)</param>
        public void CreateClassFiles(SqlSugarClient db, string fileDirectory, string nameSpace = null, bool? tableOrView = null, Action<string> callBack = null)
        {
            string sql = GetCreateClassSql(tableOrView);
            var tables = db.GetDataTable(sql);
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    var currentTable = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
                    if (callBack != null)
                    {
                        var tableColumns = GetTableColumns(db, tableName);
                        var classCode = DataTableToClass(currentTable, tableName, nameSpace, tableColumns);
                        string className = db.GetClassTypeByTableName(tableName);
                        classCode = classCode.Replace("class " + tableName, "class " + className);
                        FileSugar.CreateFile(fileDirectory.TrimEnd('\\') + "\\" + className + ".cs", classCode, Encoding.UTF8);
                        callBack(className);
                    }
                    else
                    {
                        var tableColumns = GetTableColumns(db, tableName);
                        string className = db.GetClassTypeByTableName(tableName);
                        var classCode = DataTableToClass(currentTable, className, nameSpace, tableColumns);
                        FileSugar.CreateFile(fileDirectory.TrimEnd('\\') + "\\" + className + ".cs", classCode, Encoding.UTF8);
                    }
                }
            }
        }



        /// <summary>
        /// 创建SQL实体文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileDirectory"></param>
        /// <param name="nameSpace">命名空间（默认：null）</param>
        /// <param name="tableOrView">是生成视图文件还是表文件,null生成表和视图，true生成表，false生成视图(默认为：null)</param>
        public void CreateClassFilesInterface(SqlSugarClient db, bool? tableOrView, Action<DataTable, string, string> callBack)
        {
            string sql = GetCreateClassSql(tableOrView);
            var tables = db.GetDataTable(sql);
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    var currentTable = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
                    string className = db.GetClassTypeByTableName(tableName);
                    callBack(tables, className, tableName);
                }
            }
        }
        /// <summary>
        ///tableOrView  null=u,v , true=u , false=v
        /// </summary>
        /// <param name="tableOrView"></param>
        /// <returns></returns>
        private static string GetCreateClassSql(bool? tableOrView)
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


        /// <summary>
        ///  创建SQL实体文件,指定表名
        /// </summary>
        public void CreateClassFilesByTableNames(SqlSugarClient db, string fileDirectory, string nameSpace, params string[] tableNames)
        {
            string sql = GetCreateClassSql(null);
            var tables = db.GetDataTable(sql);
            if (!FileSugar.IsExistDirectory(fileDirectory))
            {
                FileSugar.CreateDirectory(fileDirectory);
            }
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString().ToLower();
                    if (tableNames.Any(it => it.ToLower() == tableName))
                    {
                        var currentTable = db.GetDataTable(string.Format("select top 1 * from {0}", tableName));
                        var tableColumns = GetTableColumns(db, tableName);
                        string className = db.GetClassTypeByTableName(tableName);
                        var classCode = DataTableToClass(currentTable, className, nameSpace, tableColumns);
                        FileSugar.CreateFile(fileDirectory.TrimEnd('\\') + "\\" + className + ".cs", classCode, Encoding.UTF8);
                    }
                }
            }
        }

        /// <summary>
        /// 获取所有数据库表名
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetTableNames(SqlSugarClient db)
        {
            string sql = GetCreateClassSql(null);
            var tableNameList = db.SqlQuery<string>(sql).ToList();
            for (int i = 0; i < tableNameList.Count; i++)
            {
                var tableName = tableNameList[i];
                tableNameList[i] = db.GetClassTypeByTableName(tableName);
            }
            return tableNameList;
        }

        /// <summary>
        /// 匹配类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public string ChangeType(Type type)
        {
            string typeName = type.Name;
            switch (typeName)
            {
                case "Int32": typeName = "int"; break;
                case "String": typeName = "string"; break;
            }
            return typeName;
        }

        public string ChangeNullable(string typeName, bool isNull)
        {
            if (isNull)
            {
                switch (typeName.ToLower())
                {
                    case "int": typeName = "int?"; break;
                    case "double": typeName = "Double?"; break;
                    case "byte": typeName = "Byte?"; break;
                    case "boolean": typeName = "Boolean?"; break;
                    case "datetime": typeName = "DateTime?"; break;
                    case "decimal": typeName = "decimal?"; break;
                    case "guid": typeName = "Guid?"; break;

                }
            }
            return typeName;
        }

        public string DbTypeToFieldType(string dbtype)
        {
            if (string.IsNullOrEmpty(dbtype)) return dbtype;
            dbtype = dbtype.ToLower();
            string csharpType = "object";
            switch (dbtype)
            {
                case "bigint": csharpType = "long"; break;
                case "binary": csharpType = "byte[]"; break;
                case "bit": csharpType = "bool"; break;
                case "char": csharpType = "string"; break;
                case "date": csharpType = "DateTime"; break;
                case "datetime": csharpType = "DateTime"; break;
                case "datetime2": csharpType = "DateTime"; break;
                case "datetimeoffset": csharpType = "DateTimeOffset"; break;
                case "decimal": csharpType = "decimal"; break;
                case "float": csharpType = "double"; break;
                case "image": csharpType = "byte[]"; break;
                case "int": csharpType = "int"; break;
                case "money": csharpType = "decimal"; break;
                case "nchar": csharpType = "string"; break;
                case "ntext": csharpType = "string"; break;
                case "numeric": csharpType = "decimal"; break;
                case "nvarchar": csharpType = "string"; break;
                case "real": csharpType = "Single"; break;
                case "smalldatetime": csharpType = "DateTime"; break;
                case "smallint": csharpType = "short"; break;
                case "smallmoney": csharpType = "decimal"; break;
                case "sql_variant": csharpType = "object"; break;
                case "sysname": csharpType = "object"; break;
                case "text": csharpType = "string"; break;
                case "time": csharpType = "TimeSpan"; break;
                case "timestamp": csharpType = "byte[]"; break;
                case "tinyint": csharpType = "byte"; break;
                case "uniqueidentifier": csharpType = "Guid"; break;
                case "varbinary": csharpType = "byte[]"; break;
                case "varchar": csharpType = "string"; break;
                case "xml": csharpType = "string"; break;
                default: csharpType = "object"; break;
            }
            return csharpType;
        }
        // 获取表结构信息
        public List<PubModel.DataTableMap> GetTableColumns(SqlSugarClient db, string tableName)
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

            return db.SqlQuery<PubModel.DataTableMap>(sql);
        }
    }

}
