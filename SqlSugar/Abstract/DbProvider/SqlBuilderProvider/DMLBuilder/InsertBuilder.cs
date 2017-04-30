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
        }
        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public ISqlBuilder Builder { get; set; }
        public StringBuilder sql { get; set; }
        public  List<SugarParameter> Parameters { get; set; }
        public string EntityName { get; set; }
        public string TableWithString { get; set; }
        public List<string> ColumNames{ get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                return @"INSERT INTO {0} 
           ({1})
     VALUES
           ({2})";
            }
        }

        public void Clear()
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

        public string ToSqlString()
        {
            string columnsString =string.Join("," ,this.ColumNames.Select(it => Builder.GetTranslationColumnName(it)));
            string columnParametersString = string.Join(",", this.ColumNames.Select(it =>Builder.SqlParameterKeyWord+it));
            return string.Format(this.sql.ToString(),columnsString, columnParametersString);
        }
    }
}
