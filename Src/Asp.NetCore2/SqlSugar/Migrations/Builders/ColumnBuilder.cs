namespace SqlSugar.Migrations
{
    public class ColumnBuilder
    {
        private readonly DbColumnInfo _column;

        public ColumnBuilder(string name)
        {
            _column = new DbColumnInfo
            {
                DbColumnName = name,
                IsNullable = true,
                IsPrimarykey = false,
                IsIdentity = false
            };
        }

        public ColumnBuilder AsInt() { _column.DataType = "int"; return this; }
        public ColumnBuilder AsLong() { _column.DataType = "bigint"; return this; }
        public ColumnBuilder AsString(int length = 255) { _column.DataType = "varchar"; _column.Length = length; return this; }
        public ColumnBuilder AsText() { _column.DataType = "text"; return this; }
        public ColumnBuilder AsDecimal(int precision = 18, int scale = 2) { _column.DataType = "decimal"; _column.DecimalDigits = precision; _column.Scale = scale; return this; }
        public ColumnBuilder AsBool() { _column.DataType = "bit"; return this; }
        public ColumnBuilder AsDateTime() { _column.DataType = "datetime"; return this; }
        public ColumnBuilder AsDate() { _column.DataType = "date"; return this; }
        public ColumnBuilder AsGuid() { _column.DataType = "uniqueidentifier"; return this; }
        public ColumnBuilder PrimaryKey() { _column.IsPrimarykey = true; _column.IsNullable = false; return this; }
        public ColumnBuilder AutoIncrement() { _column.IsIdentity = true; return this; }
        public ColumnBuilder NotNull() { _column.IsNullable = false; return this; }
        public ColumnBuilder Nullable() { _column.IsNullable = true; return this; }
        public ColumnBuilder Default(string value) { _column.DefaultValue = value; return this; }

        public DbColumnInfo Build() => _column;
    }
}
