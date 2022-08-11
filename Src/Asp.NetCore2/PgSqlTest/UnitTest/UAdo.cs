using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Ado()
        {

            var task1 = Db.Ado.GetScalarAsync("select 1");
            task1.Wait();
            UValidate.Check(1, task1.Result, "ado");

            var task2 = Db.Ado.GetIntAsync("select 2");
            task2.Wait();
            UValidate.Check(2, task2.Result, "ado");


            var task3 = Db.Ado.GetLongAsync("select 3");
            task3.Wait();
            UValidate.Check(3, task3.Result, "ado");


            var task4 = Db.Ado.GetDataTableAsync("select 4 as id");
            task4.Wait();
            UValidate.Check(4, task4.Result.Rows[0]["id"], "ado");


            var task5 = Db.Ado.GetInt("select @id as id",new { id=5});
            UValidate.Check(5, task5, "ado");



            var task6 = Db.Ado.SqlQuery<dynamic>("select @id as id", new { id = 5 });
            UValidate.Check(5, task6[0].id, "ado");


            var task7 = Db.Ado.SqlQueryAsync<dynamic>("select @id as id", new { id = 7 });
            task7.Wait();
            UValidate.Check(7, task7.Result[0].id, "ado");


            var task8 = Db.Ado.SqlQueryAsync<dynamic>("select 8 as id");
            task8.Wait();
            UValidate.Check(8, task8.Result[0].id, "ado");

            var task9=Db.Ado.SqlQuery<Order, OrderItem>(@"select * from ""order"";select * from OrderDetail");

            var task10 = Db.Ado.SqlQueryAsync<Order, OrderItem>(@"select * from ""order"";select * from OrderDetail");
            task10.Wait();
        }
    }
}
