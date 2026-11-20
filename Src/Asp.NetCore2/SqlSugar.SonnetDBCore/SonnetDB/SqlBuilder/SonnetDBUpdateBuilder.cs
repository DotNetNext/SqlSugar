using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SqlSugar.SonnetDB
{
    public class SonnetDBUpdateBuilder : PostgreSQLUpdateBuilder
    {
        public override string ReSetValueBySqlExpListType { get; set; } = "SonnetDB";

        protected override string TomultipleSqlString(List<IGrouping<int, DbColumnInfo>> groupList)
        {
            Check.Exception(PrimaryKeys == null || PrimaryKeys.Count == 0, "Update List<T> need Primary key");
            var rowValues = groupList.Select((row, rowIndex) => row.Select((column, columnIndex) => new { column, columnIndex }).ToDictionary(
                value => value.column.DbColumnName,
                value => GetBatchColumn(value.column, rowIndex, value.columnIndex),
                StringComparer.OrdinalIgnoreCase)).ToList();
            var predicates = groupList.Select((row, rowIndex) => string.Join(" AND ", PrimaryKeys.Select(key =>
            {
                var column = row.First(value => string.Equals(value.DbColumnName, key, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value.PropertyName, key, StringComparison.OrdinalIgnoreCase));
                var name = Builder.GetTranslationColumnName(column.DbColumnName);
                return column.Value == null ? name + " IS NULL" : name + "=" + rowValues[rowIndex][column.DbColumnName];
            }))).ToList();

            var columns = groupList.SelectMany(row => row).Where(column => !column.IsPrimarykey && (!column.IsIdentity || IsOffIdentity))
                .GroupBy(column => column.DbColumnName, StringComparer.OrdinalIgnoreCase);
            var assignments = columns.Select(columnGroup =>
            {
                var column = columnGroup.First();
                var name = Builder.GetTranslationColumnName(column.DbColumnName);
                var setValue = SetValues.FirstOrDefault(value => value.Key == name || value.Key == Builder.GetTranslationColumnName(column.PropertyName));
                if (setValue.Value != null) return setValue.Value;

                var cases = rowValues.Select((values, rowIndex) => values.TryGetValue(column.DbColumnName, out var value)
                    ? " WHEN " + predicates[rowIndex] + " THEN " + value : "");
                return name + "=CASE" + string.Concat(cases) + " ELSE " + name + " END";
            });

            var where = " WHERE (" + string.Join(" OR ", predicates.Select(predicate => "(" + predicate + ")")) + ")";
            if (PrimaryKeys.Count == 1)
            {
                var key = PrimaryKeys[0];
                var keyColumns = groupList.Select(row => row.First(column => string.Equals(column.DbColumnName, key, StringComparison.OrdinalIgnoreCase)
                    || string.Equals(column.PropertyName, key, StringComparison.OrdinalIgnoreCase))).ToList();
                var name = Builder.GetTranslationColumnName(keyColumns[0].DbColumnName);
                var values = keyColumns.Select((column, rowIndex) => column.Value == null ? "NULL" : rowValues[rowIndex][column.DbColumnName]);
                where = " WHERE (" + name + " IN (" + string.Join(",", values) + ")";
                if (keyColumns.Any(column => column.Value == null)) where += " OR " + name + " IS NULL";
                where += ")";
            }
            if (WhereValues.Count > 0) where += " AND (" + string.Join(" AND ", WhereValues) + ")";
            ActionMinDate();
            return string.Format(SqlTemplate, GetTableNameString, string.Join(",", assignments), where);
        }

        private string GetBatchColumn(DbColumnInfo column, int rowIndex, int columnIndex)
        {
            var name = Builder.SqlParameterKeyWord + "Update_" + rowIndex + "_" + columnIndex;
            var sql = base.GetDbColumn(column, name);
            var parameter = new SugarParameter(name, column.Value, column.PropertyType)
            {
                IsJson = column.IsJson,
                IsArray = column.IsArray
            };
            if (column.SqlParameterDbType is System.Data.DbType dbType) parameter.DbType = dbType;
            if (column.IsJson) Builder.ChangeJsonType(parameter);
            if (sql == name || ReSetValueBySqlExpList?.ContainsKey(column.PropertyName) == true) Parameters.Add(parameter);
            return sql;
        }
    }
}
