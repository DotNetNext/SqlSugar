using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        #region Properties
        public SqlSugarProvider Context { get; set; }
        public CommandType CommandType { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public SqlQueryBuilder SqlQueryBuilder
        {
            get
            {
                base._SqlQueryBuilder = UtilMethods.IsNullReturnNew(base._SqlQueryBuilder);
                return base._SqlQueryBuilder;
            }
            set { base._SqlQueryBuilder = value; }
        }
        #endregion

        #region abstract Methods
        public virtual void ChangeJsonType(SugarParameter paramter) 
        {

        }
        public virtual string GetTranslationTableName(string name)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (!name.Contains("<>f__AnonymousType") &&name.IsContainsIn("(", ")", SqlTranslationLeft)&&name!= "Dictionary`2")
            {
                var tableInfo = this.Context
                .MappingTables?
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                if (tableInfo != null) 
                {
                    return GetTranslationColumnName(tableInfo.DbTableName);
                }
                return name;
            }
            if (Context.MappingTables == null) 
            {
                return name;
            }
            var context = this.Context;
            var mappingInfo = context
                .MappingTables
                .FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            name = (mappingInfo == null ? name : mappingInfo.DbTableName);
            if (name.IsContainsIn("(", ")", SqlTranslationLeft))
            {
                return name;
            }
            if (name.Contains("."))
            {
                return string.Join(".", name.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else
            {
                return SqlTranslationLeft + name + SqlTranslationRight;
            }
        }
        public virtual string GetTranslationColumnName(string entityName, string propertyName)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            Check.ArgumentNullException(propertyName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            var context = this.Context;
            var mappingInfo = context
                 .MappingColumns
                 .FirstOrDefault(it =>
                 it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase) &&
                 it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            return (mappingInfo == null ? SqlTranslationLeft + propertyName + SqlTranslationRight : SqlTranslationLeft + mappingInfo.DbColumnName + SqlTranslationRight);
        }

        public virtual string GetTranslationColumnName(string propertyName)
        {
            if (propertyName.Contains(SqlTranslationLeft)) return propertyName;
            if (propertyName.Contains("."))
            {
                return string.Join(".", propertyName.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
            }
            else
                return SqlTranslationLeft + propertyName + SqlTranslationRight;
        }

        public virtual string GetNoTranslationColumnName(string name)
        {
            if (name.Contains("="))
            {
               name=name.Split('=').First();
            }
            if (!name.Contains(SqlTranslationLeft)) return name;
            if (!name.Contains(".")&& name.StartsWith(SqlTranslationLeft) && name.EndsWith(SqlTranslationRight)) 
            {
                var result= name.TrimStart(Convert.ToChar(SqlTranslationLeft)).TrimEnd(Convert.ToChar(SqlTranslationRight));
                return result;
            }
            return name == null ? string.Empty : Regex.Match(name, @".*" + "\\" + SqlTranslationLeft + "(.*?)" + "\\" + SqlTranslationRight + "").Groups[1].Value;
        }
        public virtual string GetPackTable(string sql, string shortName)
        {
            return UtilMethods.GetPackTable(sql, shortName);
        }
        public virtual string GetDefaultShortName()
        {
            return "t";
        }


        public string GetWhere(string fieldName,string conditionalType,int? parameterIndex=null)
        {
            return string.Format(" {0} {1} {2}{3} ",this.GetTranslationColumnName(fieldName),conditionalType,this.SqlParameterKeyWord,fieldName.Replace(".","_")+ parameterIndex);
        }
        public virtual string GetUnionAllSql(List<string> sqlList)
        {
            return string.Join(" UNION ALL \r\n", sqlList);
        }
        public virtual string GetUnionSql(List<string> sqlList)
        {
            return string.Join(" UNION \r\n", sqlList);
        }
        public virtual void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex)
        {
            UtilMethods.RepairReplicationParameters(ref appendSql, parameters, addIndex);
        }
        public virtual string GetUnionFomatSql(string sql)
        {
            return sql;
        }
        public virtual Type GetNullType(string tableName,string columnName) 
        {
            return null;
        }

        public virtual string RemoveParentheses(string sql)
        {
            return sql;
        }

        private static object GetFieldValue(ConditionalModel item)
        {
            if (item.FieldValueConvertFunc != null)
            {
                return item.FieldValueConvertFunc(item.FieldValue);
            }
            else if (item.CSharpTypeName.HasValue())
            {
                return  UtilMethods.ConvertDataByTypeName(item.CSharpTypeName,item.FieldValue);
            }
            else
            {
                return item.FieldValue;
            }
        }
        public virtual void FormatSaveQueueSql(StringBuilder sqlBuilder)
        {
        }
        public virtual string RemoveN(string sql) 
        {
            return sql;
        }
        #endregion

        #region Common SqlTemplate
        public string AppendWhereOrAnd(bool isWhere, string sqlString)
        {
            return isWhere ? (" WHERE " + sqlString) : (" AND " + sqlString);
        }
        public string AppendHaving(string sqlString)
        {
            return " HAVING " + sqlString;
        }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public abstract string SqlTranslationLeft { get; }
        public abstract string SqlTranslationRight { get; }
        public virtual string SqlFalse { get { return "1=2 "; } }
        public virtual string SqlDateNow { get { return "GETDATE()"; } }
        public virtual string FullSqlDateNow { get { return "SELECT GETDATE()"; } }
        public virtual string SqlSelectAll { get { return "*"; } }
        #endregion
    }
}
