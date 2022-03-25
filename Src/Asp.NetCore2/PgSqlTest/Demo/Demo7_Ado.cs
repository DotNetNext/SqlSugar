using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public  class Demo7_Ado
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Ado Start ####");

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.PostgreSQL,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
            //sql
            var dt = db.Ado.GetDataTable("select * from \"order\" where  @id>0 or name=@name", new List<SugarParameter>(){
              new SugarParameter("@id",1),
              new SugarParameter("@name","2")
            });

            //sql  
            var dt2 = db.Ado.GetDataTable("select * from \"order\" where @id>0  or name=@name", new { id = 1, name = "2" });

            //Stored Procedure
            //var dt3 = db.Ado.UseStoredProcedure().GetDataTable("sp_school", new { name = "张三", age = 0 }); 
            //var nameP = new SugarParameter("@name", "张三");
            //var ageP = new SugarParameter("@age", null, true);//isOutput=true
            //var dt4 = db.Ado.UseStoredProcedure().GetDataTable("sp_school", nameP, ageP);



            //There are many methods to under db.ado
            var list= db.Ado.SqlQuery<Order>("select * from \"order\" ");
            var intValue=db.Ado.SqlQuerySingle<int>("select 1");
            db.Ado.ExecuteCommand("delete  from \"order\" where id>1000");
            //db.Ado.xxx
            Console.WriteLine("#### Ado End ####");
        }
    }
}
