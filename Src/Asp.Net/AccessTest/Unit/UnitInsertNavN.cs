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
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,//Master Connection
                DbType = DbType.Access,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true 
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType,sql,pars));//输出sql,查看执行sql 性能无影响
            };

            //如果不存在创建数据库存在不会重复创建
            //db.DbMaintenance.CreateDatabase();
            //根据实体名创建对应表
            db.CodeFirst.InitTables(typeof(ClassA), typeof(ClassB));
 
            //清空表中数据
            db.Deleteable<ClassA>().ExecuteCommand();
            db.Deleteable<ClassB>().ExecuteCommand(); 

            //初始化数据 共四层 都是一对多的关系表
            ClassA a = new ClassA();
            a.AId =1;
            a.UpdateTime= DateTime.Now;
         
            ClassB b = new ClassB();
            b.BId = 2;
            b.AId = a.AId;
            a.B = new List<ClassB>() { b };


          //导航插入四表数据
          db.InsertNav(a)
                .Include(t => t.B) 
                .ExecuteCommand();

            var list= db.Queryable<ClassA>().ToList();
            var list2=db.Queryable<ClassA>().Includes(z => z.B).ToList();
            if (list2.Count == 0) 
            {
                throw new Exception("unit error");
            }
            Console.WriteLine("----------程序结束----------");

        }

        /// <summary>
        /// 一级类
        /// </summary>
        [SugarTable("tb_a1")]
        public class ClassA
        {
            [SugarColumn(IsPrimaryKey = true,IsIdentity =true)]
            public int AId { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(ClassB.AId))]
            public List<ClassB> B { get; set; }
            [SugarColumn(IsNullable = true)]
            public DateTime? UpdateTime { get; set; }
        }
        /// <summary>
        /// 二级类
        /// </summary>
        [SugarTable("tb_b2")]
        public class ClassB
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int BId { get; set; }
            public int AId { get; set; }
          
        }
       
    }
}