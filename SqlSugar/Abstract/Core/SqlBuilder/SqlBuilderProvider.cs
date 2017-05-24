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
        public SqlBuilderProvider()
        {
        }

        public CommandType CommandType { get; set; }
        public virtual string SqlParameterKeyWord { get { return "@"; } }
        public abstract string GetTranslationTableName(string name);
        public abstract string GetTranslationColumnName(string entityName, string propertyName);
        public abstract string GetTranslationColumnName(string propertyName);
        public abstract string GetNoTranslationColumnName(string name);

        public string AppendWhereOrAnd(bool isWhere, string sqlString)
        {
            return isWhere ? (" WHERE " + sqlString ):( " AND " + sqlString);
        }
        public string AppendHaving(string sqlString)
        {
            return  " HAVING " + sqlString;
        }


        public DeleteBuilder DeleteBuilder
        {
            get; set;
        }

        public InsertBuilder InsertBuilder
        {
            get; set;
        }

        public QueryBuilder QueryBuilder
        {
            get;set;
        }

        public SqlQueryBuilder SqlQueryBuilder
        {
            get
            {
                base._SqlQueryBuilder = PubMethod.IsNullReturnNew(base._SqlQueryBuilder);
                return base._SqlQueryBuilder;
            }
            set { base._SqlQueryBuilder = value; }
        }

        public UpdateBuilder UpdateBuilder
        {
            get; set;
        }

        public SqlSugarClient Context
        {
            get;set;
        }
    }
}
