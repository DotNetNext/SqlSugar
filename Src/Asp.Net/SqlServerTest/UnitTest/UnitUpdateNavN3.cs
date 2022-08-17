using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace OrmTest 
{
    public class UnitUpdateNavN3
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;

            db.CodeFirst.InitTables(typeof(ClassA), typeof(ClassB), typeof(ClassC));
            db.DbMaintenance.TruncateTable<ClassA,ClassB,ClassC>();


            //初始化数据 1对多-1对多 共三层
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

            //导航插入三表数据
            db.InsertNav(a).Include(t => t.B).ThenInclude(t => t.C).ExecuteCommand();

            //修改数据 修改b表id 修改c表对应b表id
            b.BId = "bb";
            c.BId = "bb";
            c.CId = "cc";
            

            //导航更新三表数据
            db.UpdateNav(a)
            .Include(t => t.B, new UpdateNavOptions()
            {
                OneToManyDeleteAll = true//B下面存在一对多将其删除掉
            })
            .ThenInclude(t => t.C).ExecuteCommand();

            //正常情况C数据库中应该只有1条 如果是多条则说明三级表数据存在垃圾数据
            var count = db.Queryable<ClassC>().ToList();

            if (count.Count() > 1) { throw new Exception("unit error"); }

            //清空表中数据
            db.DbMaintenance.DropTable("ClassA");
            db.DbMaintenance.DropTable("ClassB");
            db.DbMaintenance.DropTable("ClassC"); 
            Console.WriteLine($"第三级表中总数据条数：{count}");
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
        }
    }
}
