using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitCustom01
    {
        public static void Init()
        {
            var ssc = new SqlSugarClient(new ConnectionConfig()

            {
 
                ConnectionString = OrmTest.Config.ConnectionString,

                DbType = SqlSugar.DbType.MySql, //必填

                IsAutoCloseConnection = true

            });
            ssc.Aop.OnLogExecuting = (s, p) =>
            {
                Console.WriteLine(s);
            };
            ssc.CodeFirst.InitTables<Student>();
            var expMethods = new List<SqlFuncExternal>();

            expMethods.Add(new SqlFuncExternal()

            {

                UniqueMethodName = "SumSugar",

                MethodValue = (expInfo, dbType, expContext) =>

                {

                    if (dbType == DbType.SqlServer)

                        return string.Format("SUM({0})", expInfo.Args[0].MemberName);

                    else if (dbType == DbType.MySql)

                        return string.Format("SUM({0})", expInfo.Args[0].MemberName);

                    else

                        throw new Exception("未实现");

                }

            });

            ssc.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()

            {

                SqlFuncServices = expMethods //set ext method

            };

            try

            {
                ssc.Insertable(new Student() { Age = 1, Createtime = DateTime.Now, Grade = 1, Id = 1, Name = "a", Schoolid = 1 }).ExecuteCommand();
                ssc.Insertable(new Student() { Age = 1, Createtime = DateTime.Now, Grade = 1, Id = 1, Name = "a", Schoolid = 1 }).ExecuteCommand();
                var sss12 = ssc.Queryable<Student>().GroupBy(o => o.Name).Select(o => new { Age = SqlFunc.AggregateSum(o.Age)  }).ToList();

            }

            catch (Exception e)

            {



            }
            ssc.CodeFirst.InitTables<unittest1231sdaa>();

            ssc.CodeFirst.InitTables<unittest1231sdaa2>();
            var db = ssc;
            string p1 = "p1";
            db.Queryable<Order>().Where(x11 => x11.Name + "a" == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == x11.Name + "a").ToList();
            db.Queryable<Order>().Where(x11 => "a" + x11.Name + p1 == x11.Name).ToList();
            db.Queryable<Order>().Where(x11 => x11.Name == "a" + x11.Name + p1).ToList();
            db.Queryable<Order>().Where(x11 => SqlFunc.ToString("a" + p1 + x11.Name) == x11.Name).ToList();
            db.Updateable<Order>()
                .SetColumns(x => x.Name == x.Name + "a")
                .Where(z => z.Id == 1)
                .ExecuteCommand();
            db.Updateable<Order>()
              .SetColumns(x => new Order() { Name = x.Name + "a" })
              .Where(z => z.Id == 1)
              .ExecuteCommand();
        }
    }

    public class unittest1231sdaa
    {
        [SugarColumn(IndexGroupNameList = new string[] { "index3" }, ColumnDataType = "DATETIME(6)", DefaultValue = "CURRENT_TIMESTAMP(6)")]
        public DateTime update_time { get; set; }
    }
    public class unittest1231sdaa2
    {
        [SugarColumn(IndexGroupNameList = new string[] { "index3" }, ColumnDataType = "DATETIME", DefaultValue = "CURRENT_TIMESTAMP")]
        public DateTime update_time { get; set; }
    }
    [SugarTable("unitstudent1111")]
    public class Student
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "schoolid")]
        public int? Schoolid { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "name")]
        public string Name { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "createtime")]
        public DateTime? Createtime { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "age")]
        public int? Age { get; set; }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(ColumnName = "grade")]
        public int? Grade { get; set; }
    }


}
