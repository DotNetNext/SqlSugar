using SqlSugar;

public static class QueryChecks
{
    public static void Run(SqlSugarClient db)
    {
        db.Ado.ExecuteCommand("CREATE TABLE query_records (\"Id\" INT, \"Group\" INT, \"Text\" STRING, \"Number\" INT, \"At\" DATETIME, \"Json\" JSON, PRIMARY KEY (\"Id\"))");
        db.Ado.ExecuteCommand("CREATE TABLE query_composite (\"A\" INT, \"B\" INT, \"Number\" INT, PRIMARY KEY (\"A\", \"B\"))");
        var start = new DateTime(2026, 10, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddDays(2);
        const string json = "{\"present\":null,\"text\":\"hello\",\"values\":[1,2]}";
        db.Insertable(Enumerable.Range(1, 4).Select(id => new Record
        {
            Id = id, Group = id % 2, Text = " abc" + id + " ", Number = id,
            At = start, Json = json
        }).ToList()).ExecuteCommand();

        var partition = db.Queryable<Record>().PartitionBy(row => row.Group).OrderBy(row => row.Id).Take(1).ToList();
        Ensure(partition.Select(row => row.Id).OrderBy(id => id).SequenceEqual(new[] { 1, 2 }), "PartitionBy must return the first row of each group.");
        Ensure(db.Queryable<Record>().PartitionBy(row => row.Group).OrderBy(row => row.Id).Take(1).ToPageList(2, 1).Single().Id == 2,
            "Paging partition results must preserve the requested order.");

        var text = db.Queryable<Record>().Where(row => row.Id == 1).Select(row => new
        {
            Position = SqlFunc.CharIndexNew(row.Text, "abc"), Missing = SqlFunc.CharIndexNew(row.Text, "missing"),
            Trimmed = row.Text.Trim(), Length = row.Text.Length, Part = row.Text.Substring(1, 3)
        }).Single();
        Ensure(text.Position == 2 && text.Missing == 0 && text.Trimmed == "abc1" && text.Length == 6 && text.Part == "abc",
            "String positions, missing matches and substring offsets must follow SqlSugar semantics.");

        var dates = db.Queryable<Record>().Where(row => row.Id == 1).Select(row => new
        {
            Added = SqlFunc.DateAdd(row.At, 1, DateType.Day), Difference = SqlFunc.DateDiff(DateType.Day, row.At, end)
        }).Single();
        Ensure(dates.Added == start.AddDays(1) && dates.Difference == 2,
            "Date arithmetic must preserve the time of day when returned as DateTime.");

        Ensure(db.Queryable<Record>().Where(row => SqlFunc.JsonContainsFieldName(row.Json, "present")).Count() == 4,
            "A JSON field with a null value still exists.");
        Ensure(db.Queryable<Record>().Where(row => SqlFunc.JsonContainsFieldName(row.Json, "missing")).Count() == 0,
            "A missing JSON field must not be reported as present.");
        Ensure(db.Queryable<Record>().Where(row => SqlFunc.JsonArrayAny(SqlFunc.JsonField(row.Json, "values"), 2)).Count() == 4
            && db.Queryable<Record>().Where(row => SqlFunc.JsonArrayAny(SqlFunc.JsonField(row.Json, "values"), 3)).Count() == 0,
            "JSON array membership must distinguish present and absent values.");
        Ensure(db.Queryable<Record>().Where(row => row.Id == 1).Select(row => SqlFunc.JsonArrayLength(SqlFunc.JsonField(row.Json, "values"))).Single() == 2,
            "Nested JSON arrays must retain their type and length.");
        Ensure(db.Queryable<Record>().Where(row => row.Id == 1).Select(row => SqlFunc.JsonParse(row.Json)).Single()?.ToString() == json,
            "JsonParse must retain the original JSON value.");

        db.Insertable(new[] { new Composite { A = 1, B = 1, Number = 1 }, new Composite { A = 1, B = 2, Number = 2 } }).ExecuteCommand();
        var compositeRows = new[] { new Composite { A = 1, B = 1, Number = 10 }, new Composite { A = 1, B = 2, Number = 20 } };
        Ensure(db.Updateable(compositeRows).ExecuteCommand() == 2
            && db.Queryable<Composite>().OrderBy(row => row.B).Select(row => row.Number).ToList().SequenceEqual(new[] { 10, 20 }),
            "Batch updates must match every part of a composite key.");

        var increments = new[] { new Record { Id = 1, Number = 2 }, new Record { Id = 2, Number = 3 } };
        Ensure(db.Updateable(increments).WhereColumns(row => row.Id).UpdateColumns(row => row.Number).PublicSetColumns(row => row.Number, "+").ExecuteCommand() == 2,
            "Batch arithmetic must update rows matched through WhereColumns.");
        Ensure(db.Queryable<Record>().InSingle(1).Number == 3 && db.Queryable<Record>().InSingle(2).Number == 5,
            "Batch arithmetic must use each row's stored value.");

        Ensure(db.Updateable<Record>().InnerJoin<Composite>((row, other) => row.Id == other.B)
            .SetColumns((row, other) => new Record { Number = other.Number }).Where((row, other) => row.Id == 1).ExecuteCommand() == 1
            && db.Queryable<Record>().InSingle(1).Number == 10 && db.Queryable<Record>().InSingle(2).Number == 5,
            "Join updates must read the joined source and preserve rows excluded by the filter.");

        var originalDiffLog = db.CurrentConnectionConfig.AopEvents?.OnDiffLogEvent;
        DiffLogModel? diffLog = null;
        db.Aop.OnDiffLogEvent = entry => diffLog = entry;
        try
        {
            db.Ado.ExecuteCommand("CREATE TABLE query_log_records (\"Id\" INT AUTO_INCREMENT, \"Text\" STRING, PRIMARY KEY (\"Id\"))");
            var inserted = db.Insertable(new Composite { A = 2, B = 1, Number = 30 })
                .EnableDiffLogEvent().ExecuteReturnIdentity();
            Ensure(inserted == 2 && !string.IsNullOrWhiteSpace(diffLog?.Sql) && diffLog?.Parameters?.Length > 0,
                "Identity return methods must include the executed SQL and parameters in the difference log.");
            for (var index = 0; index < 4; index++)
            {
                diffLog = null;
                var insert = db.Insertable(new LogRecord { Text = "diff-" + index }).EnableDiffLogEvent();
                var identity = index switch
                {
                    0 => insert.ExecuteReturnIdentity(),
                    1 => insert.ExecuteReturnIdentityAsync().GetAwaiter().GetResult(),
                    2 => insert.ExecuteReturnBigIdentity(),
                    _ => insert.ExecuteReturnBigIdentityAsync().GetAwaiter().GetResult()
                };
                Ensure(identity == index + 1 && !string.IsNullOrWhiteSpace(diffLog?.Sql) && diffLog?.Parameters?.Length > 0
                    && diffLog.AfterData?.Count == 1,
                    "Identity difference logs must query the inserted row with its native key type and retain SQL and parameters.");
            }
        }
        finally
        {
            db.Aop.OnDiffLogEvent = originalDiffLog;
        }
    }

    private static void Ensure(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    [SugarTable("query_records")]
    public sealed class Record
    {
        [SugarColumn(IsPrimaryKey = true)] public int Id { get; set; }
        public int Group { get; set; }
        public string Text { get; set; } = "";
        public int Number { get; set; }
        public DateTime At { get; set; }
        [SugarColumn(ColumnDataType = "JSON")] public string Json { get; set; } = "{}";
    }

    [SugarTable("query_composite")]
    public sealed class Composite
    {
        [SugarColumn(IsPrimaryKey = true)] public int A { get; set; }
        [SugarColumn(IsPrimaryKey = true)] public int B { get; set; }
        public int Number { get; set; }
    }

    [SugarTable("query_log_records")]
    public sealed class LogRecord
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)] public long Id { get; set; }
        public string Text { get; set; } = "";
    }
}
