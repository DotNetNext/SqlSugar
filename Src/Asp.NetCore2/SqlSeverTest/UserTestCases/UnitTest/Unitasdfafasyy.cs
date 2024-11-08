using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unita3affafa 
    {

       public static void Init() 
        {

            var db = new SqlSugarClient(new ConnectionConfig()

            {

                ConnectionString =Config.ConnectionString,

                DbType = DbType.SqlServer,

                IsAutoCloseConnection = true,

                InitKeyType = InitKeyType.Attribute

            }, db => {



                //过滤器写在这儿就行了



                #region 添加查询过滤器

                db.QueryFilter.AddTableFilter<IDeletedFilter>(x => x.IsDeleted == 0);

                db.QueryFilter.AddTableFilter<IOrgFilter>(x => x.OrgId == 10); 

                #endregion

            });



            #region SQL执行前

            db.Aop.OnLogExecuting = (sql, pars) =>//SQL执行前

            {

                var sqlString = UtilMethods.GetSqlString(DbType.MySql, sql, pars);

                Console.WriteLine(sqlString);

            };

            #endregion



            //#region 添加查询过滤器

            //db.QueryFilter.AddTableFilter<IDeletedFilter>(x => x.IsDeleted == 0);

            //db.QueryFilter.AddTableFilter<IOrgFilter>(x => x.OrgId == 10);

            //#endregion


            db.DbMaintenance.DropTable<TestSplit001>();
            //用例代码

            var testSplit001 = new TestSplit001()

            {

                Id = DateTime.Now.Ticks,

                Name = "insert-" + DateTime.Now.Ticks,

                CreateTime = DateTime.Now,

                IsDeleted = 0,

                OrgId = 10,

            };



            {

                Console.WriteLine("-----------------------------------插入数据-------------------------------------");

                var result = db.Insertable(testSplit001).SplitTable().ExecuteCommand();//用例代码

            }



            //testSplit001.IsDeleted = 1;

            //testSplit001.Name = "delete";



            //{

            //    Console.WriteLine("-----------------------------------同步更新数据(只更新IsDeleted)-------------------------------------");

            //    //同步没问题--只更新IsDeleted字段

            //    var result2 = db.Updateable(testSplit001).UpdateColumns(it => it.IsDeleted).SplitTable().ExecuteCommand();//用例代码

            //}



            //{

            //    Console.WriteLine("-----------------------------------异步更新数据(只更新IsDeleted)-------------------------------------");

            //    //异步有问题--IsDeleted、Field1都更新了

            //    db.Updateable(testSplit001).UpdateColumns(it => it.IsDeleted).SplitTable().ExecuteCommandAsync();

            //}



            {

                Console.WriteLine("-----------------------------------查询-------------------------------------");
                var list = db.Queryable<TestSplit001>().SplitTable().ToList();
                var sql = db.Queryable<TestSplit001>().SplitTable().ToSqlString();
                if (sql.Split("AND").Length > 3) 
                {
                    throw new Exception("unit error");
                }
                if (!sql.Contains("AND"))
                {
                    throw new Exception("unit error");
                }
            }
             

        }

    }



    /// <summary>

    /// 

    /// </summary>

    [SplitTable(SplitType.Month)]//按月分表 （自带分表支持 年、季、月、周、日）

    [SugarTable("test001_{year}{month}{day}", IsCreateTableFiledSort = true)]//3个变量必须要有，这么设计为了兼容开始按年，后面改成按月、按日

    [SugarIndex("index_createtime_", nameof(CreateTime), OrderByType.Asc)]

    public partial class TestSplit001 : IDeletedFilter, IOrgFilter

    {

        /// <summary>

        /// 

        /// </summary>

        public TestSplit001()

        {



        }



        /// <summary>

        /// 主键

        /// </summary>

        [SugarColumn(IsPrimaryKey = true)]

        public long Id { get; set; }



        /// <summary>

        /// 

        /// </summary>

        public string Name { get; set; }



        /// <summary>

        /// 创建时间

        /// </summary>

        [SplitField]

        public DateTime CreateTime { get; set; }



        /// <summary>

        /// 

        /// </summary>

        public byte IsDeleted { get; set; }



        /// <summary>

        /// 

        /// </summary>

        public long OrgId { get; set; }

    }



    /// <summary>

    /// 接口过滤器(删除标志)

    /// </summary>

    public interface IDeletedFilter

    {

        /// <summary>

        /// 删除标志

        /// </summary>

        public byte IsDeleted { get; set; }

    }



    /// <summary>

    /// 接口过滤器(机构标志)

    /// </summary>

    public interface IOrgFilter

    {

        /// <summary>

        /// 机构标志

        /// </summary>

        public long OrgId { get; set; }

    }


}
