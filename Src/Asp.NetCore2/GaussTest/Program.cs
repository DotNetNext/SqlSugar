using SqlSugar;
using System.Data;


SqlSugar.InstanceFactory.CustomNamespace = "SqlSugar.GaussDB";
SqlSugar.InstanceFactory.CustomDbName = "GaussDB";
SqlSugar.InstanceFactory.CustomDllName = "SqlSugar.GaussDBCore";
//创建DB
var db = new SqlSugarClient(new ConnectionConfig()
{
    ConnectionString = "PORT=5432;DATABASE=SqlSugar5Demo;HOST=localhost;PASSWORD=postgres;USER ID=postgres",
    DbType = SqlSugar.DbType.Custom,
    IsAutoCloseConnection = true,
    MoreSettings = new ConnMoreSettings()
    {
        DatabaseModel = SqlSugar.DbType.OpenGauss
    }
}, db =>
{


    db.Aop.OnLogExecuting = (x, y) =>
    {
        Console.WriteLine(x);
    };

});

db.Open();
db.Close();

var dt = db.Ado.GetDataTable("SELECT * from tb_user limit 10");

dt.AsEnumerable().ToList().ForEach(r => 
{
    Console.WriteLine(r[0].ToString());
});

Console.WriteLine("Hello, World!");