using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UnitInsertNavN
    {
        public static void Init()
        {
            //导航更新问题
            //关联层级大于或等于四层时 存在脏数据

            Console.WriteLine("----------程序开始----------");

            //创建数据库对象
            SqlSugarClient db = NewUnitTest.Db;

            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType,sql,pars));//输出sql,查看执行sql 性能无影响
            };

            //如果不存在创建数据库存在不会重复创建
            db.DbMaintenance.CreateDatabase();
            //根据实体名创建对应表
            db.CodeFirst.InitTables(typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD));
            db.DbMaintenance.TruncateTable(typeof(ClassA), typeof(ClassB), typeof(ClassC), typeof(ClassD));
            //清空表中数据
            db.Deleteable<ClassA>().ExecuteCommand();
            db.Deleteable<ClassB>().ExecuteCommand();
            db.Deleteable<ClassC>().ExecuteCommand();
            db.Deleteable<ClassD>().ExecuteCommand();

            //初始化数据 共四层 都是一对多的关系表
            ClassA a = new ClassA();
            a.AId = "a";

            ClassB b = new ClassB();
            b.BId = "b";
            b.AId = a.AId;
            a.B = new List<ClassB>();
            a.B.Add(b);

            ClassC c = new ClassC();
            c.CId = "c";
            c.BId = b.BId;
            b.C = new List<ClassC>();
            b.C.Add(c);

            ClassD d = new ClassD();
            d.DId = "d";
            d.CId = c.CId;
            c.D = new List<ClassD>();
            c.D.Add(d);

            //导航插入四表数据
            db.InsertNav(a)
                .Include(t => t.B)
                .ThenInclude(t => t.C)
                .ThenInclude(t => t.D)
                .ExecuteCommand();

            //修改数据 修改b表id 修改c表对应b表id 修改d表对应c表id
            b.BId = "bb";
            c.BId = "bb";
            c.CId = "cc";
            d.CId = "cc";
            d.DId = "dd";

            //导航更新四表数据
            db.UpdateNav(a)
                .Include(t => t.B, new UpdateNavOptions { OneToManyDeleteAll = true }) //这里声明了删除脏数据 但只清理到了下一层 第四层还是存在脏数据
                .ThenInclude(t => t.C)
                .ThenInclude(t => t.D)
                .ExecuteCommand();

            //正常情况D表数据库中应该只有1条 如果是多条则说明四级表数据存在垃圾数据
            var c_count = db.Queryable<ClassC>().Count();
            var d_count = db.Queryable<ClassD>().Count();

            Console.WriteLine($"第三级表中总数据条数：{c_count}");
            Console.WriteLine($"第四级表中总数据条数：{d_count}");

            if (c_count > 1)
            {
                throw new Exception("unit error");
            }
            else
            {
                Console.WriteLine($"三级表中不存在脏数据！");
            }

            if (d_count > 1)
            {
                throw new Exception("unit error");
            }
            else
            {
                Console.WriteLine($"四级表中不存在脏数据！");
            }
            db.Deleteable<ClassB>().ExecuteCommand();
            a.AId = Guid.NewGuid() + "";
            a.B = new List<ClassB>() { new ClassB() {  BId="b11"}, new ClassB() { BId = "b22" } };
            db.InsertNav(a,new InsertNavRootOptions() { IgnoreColumns=new string[] { nameof(ClassA.UpdateTime) } })
    .Include(t => t.B)
    .ThenInclude(t => t.C)
    .ThenInclude(t => t.D)
    .ExecuteCommand();
            Console.WriteLine("---- Insert ----");
            a.AId = Guid.NewGuid() + "";
            db.UpdateNav(a,new UpdateNavRootOptions() { 
              IsInsertRoot=true
             })
             .Include(t => t.B)
             .ThenInclude(t => t.C)
             .ThenInclude(t => t.D)
             .ExecuteCommand();

            var list=db.Queryable<ClassA>().Where(z=>z.AId==a.AId).Includes(z => z.B, z => z.C, z => z.D).ToList();
            var list2 = db.Queryable<ClassB>().Count();
            if (list2 != list.First().B.Count()) 
            {
                throw new Exception("unit error");
            }
            Console.WriteLine("----------程序结束----------");

        }

        /// <summary>
        /// 一级类
        /// </summary>
        [SugarTable("tb_a")]
        public class ClassA
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string AId { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(ClassB.AId))]
            public List<ClassB> B { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime? UpdateTime { get; set; }
        }
        /// <summary>
        /// 二级类
        /// </summary>
        [SugarTable("tb_b")]
        public class ClassB
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string BId { get; set; }
            public string AId { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(ClassC.BId))]
            public List<ClassC> C { get; set; }
        }
        /// <summary>
        /// 三级类
        /// </summary>
        [SugarTable("tb_c")]
        public class ClassC
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string CId { get; set; }

            public string BId { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(ClassD.DId))]
            public List<ClassD> D { get; set; }
        }
        /// <summary>
        /// 四级类
        /// </summary>
        [SugarTable("tb_d")]
        public class ClassD
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string DId { get; set; }

            public string CId { get; set; }
        }
    }
}