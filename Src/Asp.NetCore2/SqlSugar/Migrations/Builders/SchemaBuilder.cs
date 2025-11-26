using System;

namespace SqlSugar.Migrations
{
    public class SchemaBuilder
    {
        private readonly SqlSugarClient _client;

        public SchemaBuilder(SqlSugarClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public void CreateTable(string tableName, Action<TableBuilder> buildAction)
        {
            var builder = new TableBuilder(_client, tableName);
            buildAction(builder);
            builder.Execute();
        }

        public void DropTable(string tableName)
        {
            _client.DbMaintenance.DropTable(tableName);
        }

        public void RenameTable(string oldName, string newName)
        {
            _client.DbMaintenance.RenameTable(oldName, newName);
        }

        public void AlterTable(string tableName, Action<TableBuilder> alterAction)
        {
            var builder = new TableBuilder(_client, tableName, isAlter: true);
            alterAction(builder);
            builder.Execute();
        }

        public void AddColumn(string tableName, string columnName, Action<ColumnBuilder> columnAction)
        {
            var column = new ColumnBuilder(columnName);
            columnAction(column);
            _client.DbMaintenance.AddColumn(tableName, column.Build());
        }

        public void DropColumn(string tableName, string columnName)
        {
            _client.DbMaintenance.DropColumn(tableName, columnName);
        }

        public void RenameColumn(string tableName, string oldName, string newName)
        {
            _client.DbMaintenance.RenameColumn(tableName, oldName, newName);
        }

        public void CreateIndex(string tableName, string indexName, params string[] columns)
        {
            _client.DbMaintenance.CreateIndex(tableName, columns, indexName, false);
        }

        public void CreateUniqueIndex(string tableName, string indexName, params string[] columns)
        {
            _client.DbMaintenance.CreateIndex(tableName, columns, indexName, true);
        }

        public void DropIndex(string tableName, string indexName)
        {
            _client.DbMaintenance.DropIndex(indexName, tableName);
        }

        public void Sql(string sql)
        {
            _client.Ado.ExecuteCommand(sql);
        }

        public void Sql(string sql, object parameters)
        {
            _client.Ado.ExecuteCommand(sql, parameters);
        }
    }
}
