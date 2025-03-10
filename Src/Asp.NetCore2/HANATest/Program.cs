
using SqlSugar;

namespace GbaseTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //这行代码扔程序启动时
            InstanceFactory.CustomAssemblies = new System.Reflection.Assembly[] {
            typeof(SqlSugar.HANAConnector.HANAProvider).Assembly };

            //创建DB
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "DRIVER={HANAQAS64};SERVERNODE=172.16.10.12:32015;UID=XXX;PWD=XXX;DATABASENAME=Q00",
                DbType = DbType.HANA,
                IsAutoCloseConnection = true,
            }, db =>
            {


                db.Aop.OnLogExecuting = (x, y) =>
                {
                    Console.WriteLine(x);
                };

            }); 

            db.Open();
            db.Close();

            var dt = db.Ado.GetDataTable("SELECT * from DF_MM_POINFO where id=:id ", new { id = 1 });

            var list = db.Queryable<DF_MM_POINFO>().Where(IT => IT.ID > 0).ToList();

            int count = 0;
            var listPage = db.Queryable<DF_MM_POINFO>().Where(IT => IT.ID > 0).ToPageList(2, 2, ref count);

            Console.WriteLine("Hello, World!");
        }
        public class DF_MM_POINFO
        {
            public int ID { get; set; }
        }
    }
}
