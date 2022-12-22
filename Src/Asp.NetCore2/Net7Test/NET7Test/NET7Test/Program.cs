using SqlSugar;

SqliteTest();
MyTest();
ServerTest();

Console.Read();
static void MyTest()
{
    var sqlugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = DbType.MySql,
        ConnectionString = "server=localhost;Database=SqlSugar4xTest;Uid=root;Pwd=123456;"
    },
    it =>
    {
        it.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s, p);
    });
    sqlugar.DbMaintenance.CreateDatabase();
    sqlugar.CodeFirst.InitTables<UnitDate01231>();
    sqlugar.Insertable(new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }).ExecuteCommand();

    sqlugar.Insertable(new List<UnitDate01231> { new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    },new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    } }).ExecuteCommand();

    var list = sqlugar.Queryable<UnitDate01231>().OrderByDescending(it => it.dateOnly).OrderByDescending(it=>it.timeOnly).ToList();

    var d1 = new UnitDate01231().dateOnly;
    var d2 = new UnitDate01231().timeOnly;
}
static void ServerTest()
{
    var sqlugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = DbType.SqlServer,
        ConnectionString = "SERVER=.;uid=sa;pwd=sasa;database=SqlSugar4Text4"
    },
    it =>
    {
        it.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s, p);
    });
    sqlugar.DbMaintenance.CreateDatabase();
    sqlugar.CodeFirst.InitTables<UnitDate01231>();
    sqlugar.Insertable(new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }).ExecuteCommand();



    var list = sqlugar.Queryable<UnitDate01231>().OrderByDescending(it => it.dateOnly).ToList();

    var d1 = new UnitDate01231().dateOnly;
    var d2 = new UnitDate01231().timeOnly;
}

static void SqliteTest()
{
    var sqlugar = new SqlSugarClient(new ConnectionConfig()
    {
        DbType = DbType.Sqlite,
        ConnectionString = "datasource=SqlSugar4Text4.db"
    },
    it =>
    {
        it.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s, p);
    });
    sqlugar.DbMaintenance.CreateDatabase();
    sqlugar.CodeFirst.InitTables<UnitDate01231>();
    sqlugar.Insertable(new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }).ExecuteCommand();



    var list = sqlugar.Queryable<UnitDate01231>().OrderByDescending(it => it.dateOnly).ToList();

    var d1 = new UnitDate01231().dateOnly;
    var d2 = new UnitDate01231().timeOnly;
}

public class UnitDate01231
{
    [SugarColumn(ColumnDataType = "time")]
    public TimeOnly timeOnly { get; set; }
    [SugarColumn(ColumnDataType = "datetime")]
    public DateOnly dateOnly { get; set; }
}
