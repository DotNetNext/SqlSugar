using System;
using System.Data;
using System.Linq;
using System.Text;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBInsertBuilder : InsertBuilder
    {
        public override string SqlTemplate => "INSERT INTO {0} ({1}) VALUES ({2})" + (IsReturnIdentity ? " RETURNING $PrimaryKey" : "");
        public override Func<string, string, string> ConvertInsertReturnIdFunc { get; set; } =
            (name, sql) => sql.Trim().TrimEnd(';') + " RETURNING " + name;

        public override string ToSqlString()
        {
            if (IsNoInsertNull)
            {
                DbColumnInfoList = DbColumnInfoList.Where(column => column.Value != null).ToList();
            }

            var rows = DbColumnInfoList.GroupBy(column => column.TableId).ToList();
            var columns = string.Join(",", rows.First().Select(column => Builder.GetTranslationColumnName(column.DbColumnName)));
            if (rows.Count == 1)
            {
                var values = string.Join(",", rows[0].Select(column => base.GetDbColumn(column, Builder.SqlParameterKeyWord + column.DbColumnName)));
                ActionMinDate();
                return string.Format(SqlTemplate, GetTableNameString, columns, values);
            }

            var sql = new StringBuilder("INSERT INTO " + GetTableNameString + " (" + columns + ") VALUES ");
            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                if (rowIndex > 0) sql.Append(',');
                sql.Append('(');
                sql.Append(string.Join(",", rows[rowIndex].Select((column, columnIndex) => GetBatchColumn(column, rowIndex, columnIndex))));
                sql.Append(')');
            }
            if (IsReturnIdentity) sql.Append(" RETURNING $PrimaryKey");
            ActionMinDate();
            return sql.ToString();
        }

        private string GetBatchColumn(DbColumnInfo column, int rowIndex, int columnIndex)
        {
            var name = Builder.SqlParameterKeyWord + "Insert_" + rowIndex + "_" + columnIndex;
            var sql = base.GetDbColumn(column, name);
            if (sql == name)
            {
                var parameter = new SugarParameter(name, column.Value, column.PropertyType)
                {
                    IsJson = column.IsJson,
                    IsArray = column.IsArray
                };
                if (column.SqlParameterDbType is System.Data.DbType dbType) parameter.DbType = dbType;
                if (column.IsJson) Builder.ChangeJsonType(parameter);
                Parameters.Add(parameter);
            }
            return sql;
        }
    }
}
