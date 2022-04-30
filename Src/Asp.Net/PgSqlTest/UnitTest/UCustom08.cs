
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UCustom08
    {
        [SugarTable("Unit00UpdateTest")]

        public class UpdateTest

        {

            [SugarColumn(IsNullable = false, ColumnDescription = "主键", IsPrimaryKey = true)]

            public Guid Id { get; set; }



            [SugarColumn(IsNullable = false, ColumnDescription = "名称", ColumnDataType = "varchar(100)")]

            public string TN { get; set; }



            [SugarColumn(IsNullable = true, ColumnDescription = "测试")]

            public long? T1 { get; set; }

        }



        public static void Init()

        {

            var db = NewUnitTest.Db;

            db.Aop.OnError = (exp) =>//SQL报错

            {

                string sql = exp.Sql;

                    //exp.sql 这样可以拿到错误SQL

                };

            db.DbMaintenance.CreateDatabase();

            db.CodeFirst.InitTables(typeof(UpdateTest));

            var id = Guid.NewGuid();

            db.Insertable(new UpdateTest { Id = id, TN = "asdasd" }).ExecuteCommand();

            var data = db.Queryable<UpdateTest>().Where(x => x.Id == id).First();

            db.Updateable<UpdateTest>()
                .SetColumns(x => new UpdateTest { T1 = (int)data.T1   }).Where(x => x.Id == id).ExecuteCommand();

            db.Updateable<UpdateTest>()
               .SetColumns(x => new UpdateTest { T1 =  data.T1 }).Where(x => x.Id == id).ExecuteCommand();


            db.Updateable<UpdateTest>()
             .SetColumns(x => new UpdateTest { T1 = 1 }).Where(x => x.Id == id).ExecuteCommand();


            Console.ReadKey();

        }
    }
}

