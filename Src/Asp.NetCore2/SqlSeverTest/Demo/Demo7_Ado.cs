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
                DbType = DbType.SqlServer,
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
            var dt = db.Ado.GetDataTable("select * from [order] where  @id>0 or name=@name", new List<SugarParameter>(){
              new SugarParameter("@id",1),
              new SugarParameter("@name","2")
            });

            //sql  
            var dt2 = db.Ado.GetDataTable("select * from [order] where @id>0  or name=@name", new { id = 1, name = "2" });


            //create sp
            db.Ado.ExecuteCommand(@"
                    if object_id('up_user') is not null
                    drop proc up_user;");
            db.Ado.ExecuteCommand(@"     
                    create proc up_user
                    @id int,
                    @name varchar(10) ='' output
                    as
               
                    begin
                       set @name='abc'
                       select @id as id
                    end
                    ");
            //get output
            var dt3 = db.Ado.UseStoredProcedure().GetDataTable("up_user", new { name = "张三", id = 0 });
            var IdP = new SugarParameter("@id", 1);
            var NameP = new SugarParameter("@name", null, true);//isOutput=true
            var dt4 = db.Ado.UseStoredProcedure().GetDataTable("up_user", IdP, NameP);
            var outputValue = NameP.Value;


            //There are many methods to under db.ado
            var list= db.Ado.SqlQuery<Order>("select * from [order] ");
            var intValue=db.Ado.SqlQuerySingle<int>("select 1");
            db.Ado.ExecuteCommand("delete [order] where id>1000");
            //db.Ado.xxx
            Console.WriteLine("#### Ado End ####");
        }
    }
}
