using System;

namespace SqlSugar.Migrations
{
    public static class SchemaBuilderExtensions
    {
        public static void CreateTableIfNotExists(this SchemaBuilder schema, SqlSugarClient client, 
            string tableName, Action<TableBuilder> buildAction)
        {
            if (!client.DbMaintenance.IsAnyTable(tableName, false))
            {
                schema.CreateTable(tableName, buildAction);
            }
        }

        public static void DropTableIfExists(this SchemaBuilder schema, SqlSugarClient client, string tableName)
        {
            if (client.DbMaintenance.IsAnyTable(tableName, false))
            {
                schema.DropTable(tableName);
            }
        }

        public static void AddColumnIfNotExists(this SchemaBuilder schema, SqlSugarClient client,
            string tableName, string columnName, Action<ColumnBuilder> columnAction)
        {
            if (!client.DbMaintenance.IsAnyColumn(tableName, columnName, false))
            {
                schema.AddColumn(tableName, columnName, columnAction);
            }
        }

        public static void DropColumnIfExists(this SchemaBuilder schema, SqlSugarClient client,
            string tableName, string columnName)
        {
            if (client.DbMaintenance.IsAnyColumn(tableName, columnName, false))
            {
                schema.DropColumn(tableName, columnName);
            }
        }
    }
}
