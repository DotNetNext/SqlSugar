using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial interface ISqlBuilder
    {
        SqlSugarProvider Context { get; set; }
        CommandType CommandType { get; set; }
        String AppendWhereOrAnd(bool isWhere, string sqlString);
        string AppendHaving(string sqlString);

        SqlQueryBuilder SqlQueryBuilder { get; set; }
        QueryBuilder QueryBuilder { get; set; }
        InsertBuilder InsertBuilder { get; set; }
        DeleteBuilder DeleteBuilder { get; set; }
        UpdateBuilder UpdateBuilder { get; set; }

        string SqlParameterKeyWord { get; }
        string SqlFalse { get; }
        string SqlDateNow { get; }
        string FullSqlDateNow { get; }
        string SqlTranslationLeft { get; }
        string SqlTranslationRight { get; }
        string SqlSelectAll { get;  }

        string GetTranslationTableName(string name);
        string GetTranslationColumnName(string entityName, string propertyName);
        string GetTranslationColumnName(string propertyName);
        string GetNoTranslationColumnName(string name);
        string GetPackTable(string sql,string shortName);
        string GetDefaultShortName();

        string GetWhere(string fieldName, string conditionalType, int? parameterIndex = null);
        string GetUnionAllSql(List<string> sqlList);
        string GetUnionSql(List<string> sqlList);
        void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex);
        KeyValuePair<string, SugarParameter[]> ConditionalModelToSql(List<IConditionalModel> models, int beginIndex = 0);
    }
}
