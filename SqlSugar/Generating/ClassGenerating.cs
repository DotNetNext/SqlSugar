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
        /// <param name="dt"></param>
        /// <param name="className"></param>
        /// <param name="nameSpace"></param>
        /// <param name="dataTableMapList"></param>
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
                        propertiesValue.AppendFormat(ClassTemplate.ClassFieldSummaryTemplate,
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
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public string TableNameToClass(SqlSugarClient db, string tableName)
        {
            var dt = db.GetDataTable(string.Format(SqlSugarTool.GetSelectTopSql(), tableName.GetTranslationSqlName()));
            var tableColumns = GetTableColumns(db, tableName);
            var reval = DataTableToClass(dt, tableName, null, tableColumns);
            return reval;
        }



        /// <summary>
        /// 创建实体文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="fileDirectory"></param>
        /// <param name="nameSpace">命名空间（默认：system）</param>
        /// <param name="tableOrView">是生成视图文件还是表文件,null生成表和视图，true生成表，false生成视图(默认为：null)</param>
        /// <param name="callBack">生成文件后的处理，参数string为实体名</param>
        /// <param name="preAction">生成文件前的处理，参数string为表名</param>
        public void CreateClassFiles(SqlSugarClient db, string fileDirectory, string nameSpace = null, bool? tableOrView = null, Action<string> callBack = null, Action<string> preAction = null)
        {
            var isLog = db.IsEnableLogEvent;
            db.IsEnableLogEvent = false;
            string sql = SqlSugarTool.GetCreateClassSql(tableOrView);
            var tables = db.GetDataTable(sql);
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    if (preAction != null)
                    {
                        preAction(tableName);
                    }
                    var currentTable = db.GetDataTable(string.Format(SqlSugarTool.GetSelectTopSql(), GetTableNameWithSchema(db,tableName).GetTranslationSqlName()));
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
            db.IsEnableLogEvent = isLog;
        }

        /// <summary>
        /// 创建SQL实体文件
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableOrView">是生成视图文件还是表文件,null生成表和视图，true生成表，false生成视图(默认为：null)</param>
        /// <param name="callBack">回调函数</param>
        public void CreateClassFilesInterface(SqlSugarClient db, bool? tableOrView, Action<DataTable, string, string> callBack)
        {
            var isLog = db.IsEnableLogEvent;
            db.IsEnableLogEvent = false;
            string sql = SqlSugarTool.GetCreateClassSql(tableOrView);
            var tables = db.GetDataTable(sql);
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    var currentTable = db.GetDataTable(string.Format(SqlSugarTool.GetSelectTopSql(),GetTableNameWithSchema(db,tableName).GetTranslationSqlName()));
                    string className = db.GetClassTypeByTableName(tableName);
                    callBack(tables, className, tableName);
                }
            }
            db.IsEnableLogEvent = isLog;
        }


        /// <summary>
        ///  创建SQL实体文件,指定表名
        /// </summary>
        public void CreateClassFilesByTableNames(SqlSugarClient db, string fileDirectory, string nameSpace, params string[] tableNames)
        {
            var isLog = db.IsEnableLogEvent;
            db.IsEnableLogEvent = false;
            string sql = SqlSugarTool.GetCreateClassSql(null);
            var tables = db.GetDataTable(sql);
            if (!FileSugar.IsExistDirectory(fileDirectory))
            {
                FileSugar.CreateDirectory(fileDirectory);
            }
            if (tables != null && tables.Rows.Count > 0)
            {
                foreach (DataRow dr in tables.Rows)
                {
                    string tableName = dr["name"].ToString();
                    if (tableNames.Any(it => it.ToLower() == tableName.ToLower()))
                    {
                        var currentTable = db.GetDataTable(string.Format(SqlSugarTool.GetSelectTopSql(), GetTableNameWithSchema(db,tableName).GetTranslationSqlName()));
                        var tableColumns = GetTableColumns(db, tableName);
                        string className = db.GetClassTypeByTableName(tableName);
                        var classCode = DataTableToClass(currentTable, className, nameSpace, tableColumns);
                        FileSugar.CreateFile(fileDirectory.TrimEnd('\\') + "\\" + className + ".cs", classCode, Encoding.UTF8);
                    }
                }
            }
            db.IsEnableLogEvent = isLog;
        }

        /// <summary>
        /// 获取所有数据库表名
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public List<string> GetTableNames(SqlSugarClient db)
        {
            var isLog = db.IsEnableLogEvent;
            db.IsEnableLogEvent = false;
            string sql = SqlSugarTool.GetCreateClassSql(null);
            var tableNameList = db.SqlQuery<string>(sql).ToList();
            for (int i = 0; i < tableNameList.Count; i++)
            {
                var tableName = tableNameList[i];
                tableNameList[i] = db.GetClassTypeByTableName(tableName);
            }
            db.IsEnableLogEvent = isLog;
            return tableNameList;
        }

        /// <summary>
        /// 匹配类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string ChangeType(Type type)
        {
            string typeName = type.Name;
            switch (typeName)
            {
                case "Int32": typeName = "int"; break;
                case "String": typeName = "string"; break;
            }
            return typeName;
        }

        private string ChangeNullable(string typeName, bool isNull)
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

        /// <summary>
        /// 获取表结构信息
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<PubModel.DataTableMap> GetTableColumns(SqlSugarClient db, string tableName)
        {
            string sql = SqlSugarTool.GetTtableColumnsInfo(tableName);
            return db.SqlQuery<PubModel.DataTableMap>(sql);
        }

        /// <summary>
        ///遍历表名和视图名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="action">string为表名</param>
        public void ForeachTables(SqlSugarClient db, Action<string> action)
        {
            Check.ArgumentNullException(action,"ForeachTables.action不能为null。");
            string cacgeKey = "ClassGenerating.ForeachTables";
            var cm = CacheManager<List<string>>.GetInstance();
            List<string> tables = null;
            if (cm.ContainsKey(cacgeKey))
            {
                tables = cm[cacgeKey];
            }
            else
            {
                tables = GetTableNames(db);
            }
            if (tables.IsValuable())
            {
                foreach (var item in tables)
                {
                    action(item);
                }
            }

        }

        /// <summary>
        /// 获取带schema的表名
        /// </summary>
        /// <param name="db"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string GetTableNameWithSchema(SqlSugarClient db,string tableName) {
          
            var list=SqlSugarTool.GetSchemaList(db).Where(it => it.Value == tableName).ToList();
            if (list.Any()) {
                Check.Exception(list.Count != 1,tableName+"不能同时存在两个Schema,请重命名表名。");
                return string.Format("{0}.{1}",list.Single().Key,list.Single().Value);
            }
            return tableName;
        }

    }
}
