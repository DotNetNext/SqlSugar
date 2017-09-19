using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial interface ISqlBuilder
    {
        SqlSugarClient Context { get; set; }
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

        string GetTranslationTableName(string name);
        string GetTranslationColumnName(string entityName, string propertyName);
        string GetTranslationColumnName(string propertyName);
        string GetNoTranslationColumnName(string name);
        string GetPackTable(string sql,string shortName);
        string GetDefaultShortName();
        string GetUnionAllSql(List<string> sqlList);
        void RepairReplicationParameters(ref string appendSql, SugarParameter[] parameters, int addIndex);
    }
}
