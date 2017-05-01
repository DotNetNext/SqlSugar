using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace SqlSugar
{
    public class InsertBuilder : IDMLBuilder
    {
        public InsertBuilder() {
            this.sql = new StringBuilder();
            this.DbColumnInfoList = new List<DbColumnInfo>();
        }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public  List<SugarParameter> Parameters { get; set; }
        public string EntityName { get; set; }
        public string TableWithString { get; set; }
        public List<DbColumnInfo> DbColumnInfoList { get; set; }
        public bool IsInsertNull { get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2}) ;SELECT SCOPE_IDENTITY();";
            }
        }

        public virtual void Clear()
        {

        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(EntityName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                return result;
            }
        }

        public virtual string ToSqlString()
        {
            string columnsString =string.Join("," ,this.DbColumnInfoList.Select(it => Builder.GetTranslationColumnName(it.ColumnName)));
            string columnParametersString = string.Join(",", this.DbColumnInfoList.Select(it =>Builder.SqlParameterKeyWord+it.ColumnName));
            return string.Format(SqlTemplate,GetTableNameString,columnsString, columnParametersString);
        }
    }
}
