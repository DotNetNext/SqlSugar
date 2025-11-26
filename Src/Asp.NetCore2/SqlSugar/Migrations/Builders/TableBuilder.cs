using System.Collections.Generic;

namespace SqlSugar.Migrations
{
    public class TableBuilder
    {
        private readonly SqlSugarClient _client;
        private readonly string _tableName;
        private readonly bool _isAlter;
        private readonly List<DbColumnInfo> _columns = new List<DbColumnInfo>();

        public TableBuilder(SqlSugarClient client, string tableName, bool isAlter = false)
        {
            _client = client;
            _tableName = tableName;
            _isAlter = isAlter;
        }

        private ColumnBuilder AddColumnBuilder(string name)
        {
            var builder = new ColumnBuilder(name);
            _columns.Add(builder.Build());
            return builder;
        }

        public ColumnBuilder Int(string name) => AddColumnBuilder(name).AsInt();
        public ColumnBuilder Long(string name) => AddColumnBuilder(name).AsLong();
        public ColumnBuilder String(string name, int length = 255) => AddColumnBuilder(name).AsString(length);
        public ColumnBuilder Text(string name) => AddColumnBuilder(name).AsText();
        public ColumnBuilder Decimal(string name, int precision = 18, int scale = 2) => AddColumnBuilder(name).AsDecimal(precision, scale);
        public ColumnBuilder Bool(string name) => AddColumnBuilder(name).AsBool();
        public ColumnBuilder DateTime(string name) => AddColumnBuilder(name).AsDateTime();
        public ColumnBuilder Date(string name) => AddColumnBuilder(name).AsDate();
        public ColumnBuilder Guid(string name) => AddColumnBuilder(name).AsGuid();

        public void Execute()
        {
            if (_isAlter)
            {
                foreach (var column in _columns)
                {
                    if (!_client.DbMaintenance.IsAnyColumn(_tableName, column.DbColumnName, false))
                    {
                        _client.DbMaintenance.AddColumn(_tableName, column);
                    }
                }
            }
            else
            {
                _client.DbMaintenance.CreateTable(_tableName, _columns, true);
            }
        }
    }
}
