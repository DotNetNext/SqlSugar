using NET7Test;
using SqlSugar;

ServerTest();
SqliteTest();
MyTest();
 

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
    if(sqlugar.DbMaintenance.IsAnyTable("UnitDatez211afa2222",false))
       sqlugar.DbMaintenance.DropTable<UnitDatez211afa2222>();
    sqlugar.CodeFirst.InitTables<UnitDatez211afa2222>();
    sqlugar.Insertable(new UnitDatez211afa2222()).ExecuteCommand();
    sqlugar.Insertable(new UnitDatez211afa2222() { dateOnly=DateOnly.FromDateTime(DateTime.Now) }).ExecuteCommand();
    var list2=sqlugar.Queryable<UnitDatez211afa2222>().ToList();
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
    sqlugar.CodeFirst.InitTables<UnitDatez211afa>();
    sqlugar.Insertable(new UnitDatez211afa()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }).ExecuteCommand();
    var list0=sqlugar.Queryable<UnitDatez211afa>().ToList();
    sqlugar.DbMaintenance.DropTable<UnitDatez211afa>();
    sqlugar.Insertable(new UnitDate01231()
    {
        dateOnly = DateOnly.FromDateTime(DateTime.Now),
        timeOnly = TimeOnly.FromDateTime(DateTime.Now),
    }).ExecuteCommand();



    var list = sqlugar.Queryable<UnitDate01231>().OrderByDescending(it => it.dateOnly).ToList();

    var d1 = new UnitDate01231().dateOnly;
    var d2 = new UnitDate01231().timeOnly;

    ////测试demo2,成功
    string json = @" {    
                                ""user_name"": ""Jack5"",
                                ""pwd"": ""123456"",
                                ""create_user_id"": 1,
                                ""gmt_modified"": ""2023-02-01T04:40:04.700Z"",
                                ""deleted"": 0
                              }";


    sqlugar.CodeFirst.InitTables<Userinfo021>();
    //测试demo3, 5.1.3.47版本不成功， 5.0.9.6版本成功
    Dictionary<string, object> data3 = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(json);
    sqlugar.Insertable(data3).AS("Userinfo021").ExecuteReturnBigIdentity();
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
public class UnitDatez211afa
{
    public TimeOnly timeOnly { get; set; }
    public DateOnly dateOnly { get; set; }
}
public class UnitDatez211afa2222
{
    [SugarColumn(IsNullable =true)]
    public TimeOnly? timeOnly { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateOnly? dateOnly { get; set; }
}