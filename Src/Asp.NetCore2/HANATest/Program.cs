
using SqlSugar;

var db = new SqlSugarClient(new ConnectionConfig()
{
    ConnectionString = "DRIVER={HANAQAS64};SERVERNODE=172.16.10.12:32015;UID=WLH_BPM_TASK;PWD=BPM4pass1;DATABASENAME=Q00",
    DbType = DbType.HANA,
    IsAutoCloseConnection = true,
});

db.Open(); 
db.Close();

var dt = db.Ado.GetDataTable("SELECT 1 as id");

Console.WriteLine("Hello, World!");
