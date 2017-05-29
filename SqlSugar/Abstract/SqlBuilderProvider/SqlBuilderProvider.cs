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
        public SqlSugarClient Context { get; set; }
        public CommandType CommandType { get; set; }
        public DeleteBuilder DeleteBuilder { get; set; }
        public InsertBuilder InsertBuilder { get; set; }
        public QueryBuilder QueryBuilder { get; set; }
        public UpdateBuilder UpdateBuilder { get; set; }
        public SqlQueryBuilder SqlQueryBuilder
        {
            get
            {
                base._SqlQueryBuilder = PubMethod.IsNullReturnNew(base._SqlQueryBuilder);
                return base._SqlQueryBuilder;
            }
            set { base._SqlQueryBuilder = value; }
        }
        #endregion

        #region abstract Methods
        public abstract string GetTranslationTableName(string name);
        public abstract string GetTranslationColumnName(string entityName, string propertyName);
        public abstract string GetTranslationColumnName(string propertyName);
        public abstract string GetNoTranslationColumnName(string name);
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
        public virtual string SqlFalse { get { return "1=2 "; } }
        #endregion
    }
}
