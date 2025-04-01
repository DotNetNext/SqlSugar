using SqlSugar;
using System.Data;


//说明：GaussDB原生驱动访问数据库

//这行代码扔程序启动时
InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
            typeof(SqlSugar.GaussDBCore.GaussDBDataAdapter).Assembly };

//创建DB
var db = new SqlSugarClient(new ConnectionConfig()
{
    ConnectionString = "PORT=5432;DATABASE=SqlSugar5Demo;HOST=localhost;PASSWORD=postgres;USER ID=postgres",
    DbType = SqlSugar.DbType.GaussDBNative,
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