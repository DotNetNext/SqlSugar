using System;
using System.Linq;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft => "\"";
        public override string SqlTranslationRight => "\"";
        public override string SqlDateNow => "CURRENT_UTC_DATETIME()";
        public override string FullSqlDateNow => "SELECT CURRENT_UTC_DATETIME()";

        public override Type GetNullType(string tableName, string columnName)
        {
            var columnInfo = Context.DbMaintenance.GetColumnInfosByTableName(tableName?.Trim())
                .FirstOrDefault(column => string.Equals(column.DbColumnName, columnName?.Trim().Trim('"'), StringComparison.OrdinalIgnoreCase));
            if (columnInfo == null) return null!;
            var typeName = Context.Ado.DbBind.GetCsharpTypeNameByDbTypeName(columnInfo.DataType);
            return UtilMethods.GetTypeByTypeName(typeName);
        }
    }
}
