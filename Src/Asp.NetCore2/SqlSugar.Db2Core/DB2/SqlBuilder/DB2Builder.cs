using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar.DB2
{
    public class DB2Builder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft
        {
            get
            {
                return " ";
            }
        }
        public override string SqlTranslationRight
        {
            get
            {
                return " ";
            }
        }
        public override string SqlDateNow
        {
            get
            {
                return "current_date";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select current_date";
            }
        }

        public bool isAutoToLower
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.MoreSettings == null) return true;
                return this.Context.CurrentConnectionConfig.MoreSettings.PgSqlIsAutoToLower;
            }
        }
        public override string GetTranslationColumnName(string propertyName)
        {
            return GetTranslationLeftRigthSql(propertyName.ToUpper());
        }

        //public override string GetNoTranslationColumnName(string name)
        //{
        //    return name.TrimEnd(Convert.ToChar(SqlTranslationRight)).TrimStart(Convert.ToChar(SqlTranslationLeft)).ToLower();
        //}
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName.ToUpper() + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName.ToUpper() + SqlTranslationRight);
        }

        private string GetTranslationLeftRigthSql(string sql)
        {
            return SqlTranslationLeft + sql + SqlTranslationRight;
        }

        public override string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (name.Contains("."))
            {
                var tableNameArray = name.Split('.');
                for (int i = 0; i < tableNameArray.Length; i++)
                {
                    tableNameArray[i] = i + 1 >= tableNameArray.Length ? GetTranslationLeftRigthSql(tableNameArray[i].ToUpper()) : GetTranslationLeftRigthSql(tableNameArray[i]);
                }
                return string.Join(".", tableNameArray);
            }

            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name.ToUpper() : mappingInfo.DbTableName);
            return GetTranslationLeftRigthSql(name);
        }
        public override string GetUnionFomatSql(string sql)
        {
            return " ( " + sql + " )  ";
        }

        public override Type GetNullType(string tableName, string columnName)
        {
            if (tableName != null)
                tableName = tableName.Trim();
            var columnInfo = this.Context.DbMaintenance.GetColumnInfosByTableName(tableName).FirstOrDefault(z => z.DbColumnName?.ToLower() == columnName?.ToLower());
            if (columnInfo != null)
            {
                var cTypeName = this.Context.Ado.DbBind.GetCsharpTypeNameByDbTypeName(columnInfo.DataType);
                var value = SqlSugar.UtilMethods.GetTypeByTypeName(cTypeName);
                if (value != null)
                {
                    var key = "GetNullType_" + tableName + columnName;
                    return new ReflectionInoCacheService().GetOrCreate(key, () => value);
                }
            }
            return null;
        }
    }
}
