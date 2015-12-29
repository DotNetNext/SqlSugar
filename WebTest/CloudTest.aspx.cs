using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;
using System.Threading;
namespace WebTest
{
    /// <summary>
    /// /*******************************************************************分布式计算 demo**************************************************************/
    /// </summary>
    public partial class CloudTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var id = Guid.Parse("4FC950EC-7C23-480F-9545-443EBDCA32E9");
            int pageCount = 0;

            using (CloudClient db = CloudDao.GetInstance())
            {
                db.PageMaxHandleNumber = 1500;
                using (CommittableTransaction trans = new CommittableTransaction())//启用分布式事务
                {


                    db.Tran = trans;

                    /*** 增、删、改 ***/

                    //根据配置的百分比，随机插入到某个节点库
                    var s = new student()
                    {
                        createTime = DateTime.Now,
                        id = Guid.NewGuid(),
                        name = Guid.NewGuid() + ""
                    };
                    //db.Insert<student>(s, false/*false表示不是自增列*/); //数据库设计主键为GUID为佳多库计算不会冲突



                    ////并发请求所有节点找出这条数据更新
                    //s.name = "改11";
                    //db.Update<student>(s, it => it.id == s.id);//根据表达示更新


                    //db.Update<student, Guid>(s, s.id);//根据主键更新

                    //db.Update<student, Guid>(s, new Guid[] { s.id });//根据主键数组更新



                    ////并发请求所有节点找出这条数据删除
                    //db.Delete<student>(it => it.id == s.id);


                    trans.Commit();
                }

                db.TranDispose();


                /*** 使用Taskable实现云计算 ***/


                //多线程请求所有数据库节点，获取汇总结果
                var taskDataTable = db.Taskable<DataTable>("SELECT max(NUM) FROM STUDENT").Tasks;
                foreach (var dr in taskDataTable)//遍历所有节点数据
                {
                    var dt = dr.Result.DataTable;//每个节点的数据 类型为DataTable
                    var connectionName = dr.Result.ConnectionString;
                }

                //多线程请求所有数据库节点，获取汇总结果
                var taskInt = db.Taskable<int>("SELECT max(NUM) FROM STUDENT").Tasks;
                foreach (var dr in taskInt)//遍历所有节点数据
                {
                    var dt = dr.Result.Value;//获取单个值
                    var connectionName = dr.Result.ConnectionString;
                }

                //多线程请求所有数据库节点，获取汇总结果
                var taskEntity = db.Taskable<student>("SELECT top 100 * FROM STUDENT").Tasks;
                foreach (var dr in taskEntity)//遍历所有节点数据
                {
                    var dt = dr.Result.Entities;//获取实体集合
                    var connectionName = dr.Result.ConnectionString;
                }

                // //Taskable实现分组查询
                // var groupList = db.Taskable<DataTable>("SELECT name,COUNT(*) AS [count],AVG(num) as num FROM STUDENT WHERE ID IS NOT NULL  GROUP BY NAME ")
                //     .Tasks
                //     .SelectMany(it => it.Result.DataTable.AsEnumerable())
                //     .Select(dr => new { num = Convert.ToInt32(dr["NUM"]), name = dr["NAME"].ToString(), count = Convert.ToInt32(dr["COUNT"]) })
                //     .GroupBy(dr => dr.name).Select(dt => new { name = dt.First().name, count = dt.Sum(dtItem => dtItem.count), num = dt.Sum(dtItem => dtItem.num) / dt.Sum(dtItem => dtItem.count) }).ToList();


                // //输出结果
                // foreach (var it in groupList)
                // {
                //     var num = it.num;
                //     var name = it.name;
                //     var count = it.count;
                // }


                // //简化
                // var groupList2 = db.Taskable<DataTable>("SELECT NAME,COUNT(*) AS [COUNT],AVG(NUM) as NUM FROM STUDENT WHERE ID IS NOT NULL  GROUP BY NAME ")
                //.MergeTable()//将结果集合并到一个集合
                //.Select(dr => new { num = Convert.ToInt32(dr["NUM"]), name = dr["NAME"].ToString(), count = Convert.ToInt32(dr["COUNT"]) })
                //.GroupBy(dr => dr.name).Select(dt => new { name = dt.First().name, count = dt.Sum(dtItem => dtItem.count), num = dt.Sum(dtItem => dtItem.num) / dt.Sum(dtItem => dtItem.count) }).ToList();

                // //再简化
                // List<V_Student> groupList3 = db.Taskable<V_Student>("SELECT name,COUNT(*) AS [count],AVG(num) as num FROM STUDENT WHERE ID IS NOT NULL  GROUP BY NAME ")
                //.MergeEntities()//将结果集合并到一个集合
                //.GroupBy(dr => dr.name).Select(dt => new V_Student { name = dt.First().name, count = dt.Sum(dtItem => dtItem.count), num = dt.Sum(dtItem => dtItem.num) / dt.Sum(dtItem => dtItem.count) }).ToList();



                //更多简化函数
                int maxValue = db.Taskable<int>("SELECT max(NUM) FROM STUDENT").Max();//求出所有节点的最大值
                int minValue = db.Taskable<int>("SELECT min(NUM) FROM STUDENT").Min();//求出所有节点的最小值
                var dataCount = db.Taskable<int>("SELECT count(1) FROM STUDENT").Count();//求出所有节点数据
                // var avg = db.TaskableWithCount<int>("SELECT avg(num)", " FROM STUDENT").Avg();
                var all = db.Taskable<student>("select * from student where Contains(name,@name)"/*使用contains需要建全文索引*/, new { name = "张三" }).ToList();//获取所有节点name为张三的数据,转为list

                var dnSingle= DateTime.Now;
                var data = db.Taskable<student>("select * from student where id=@id", new { id = id }).ToSingle();//从所有节点中查询一条记录
                var timeSingle = (DateTime.Now - dnSingle).TotalSeconds;

                var dn = DateTime.Now;
                //单列排序
                //  var list = db.TaskableWithPage<student>("id", "select * from student    "/*使用contains需要建全文索引*/, 2160, 25, ref pageCount, "num", OrderByType.desc, new { name = "\"*李*\"" });
                var time = (DateTime.Now - dn).TotalSeconds;

                //多组排序 order by num,time
                var dn2 = DateTime.Now;
                var listMultipleOrderBy = db.TaskableWithPage<student>("id", "select * from student   "/*使用contains需要建全文索引*/, 5000, 25, ref pageCount, new List<OrderByDictionary>() { 
                     new   OrderByDictionary(){ OrderByField="num", OrderByType=OrderByType.asc},
                     //       new   OrderByDictionary(){ OrderByField="createTime", OrderByType=OrderByType.desc},
                   //  new OrderByDictionary(){ OrderByField="createTime", OrderByType=OrderByType.asc}
                }, new { name = "\"*小*\"" });
                var time2 = (DateTime.Now - dn2).TotalSeconds;


            }
        }
    }
    /// <summary>
    /// 测试类
    /// </summary>
    public class student
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public DateTime createTime { get; set; }
        public int num { get; set; }
    }
    /// <summary>
    /// 获取数据库连接对象
    /// </summary>
    public class CloudDao
    {
        private CloudDao() { }
        public static CloudClient GetInstance()
        {
            return new CloudClient(new List<CloudConnectionConfig>() { 
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c1"].ToString(), Rate=1},
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c2"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c3"].ToString(), Rate=1},
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c4"].ToString(), Rate=1},
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c5"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c6"].ToString(), Rate=1},
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c7"].ToString(), Rate=1},
                 new CloudConnectionConfig(){  ConnectionString=ConfigurationManager.ConnectionStrings["c8"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c9"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c10"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c15"].ToString(), Rate=1},
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c16"].ToString(), Rate=1},              
                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["c17"].ToString(), Rate=1},
                new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["cy1"].ToString(), Rate=1},
                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["cy2"].ToString(), Rate=1},
                    new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["cy3"].ToString(), Rate=1},
                                      new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["zq1"].ToString(), Rate=1},
                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["zq2"].ToString(), Rate=1},
                    new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["zq3"].ToString(), Rate=1},
                            new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["qp1"].ToString(), Rate=1},
                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["qp2"].ToString(), Rate=1},
                    new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["qp3"].ToString(), Rate=1},
                                 new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wy1"].ToString(), Rate=1},
                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wy2"].ToString(), Rate=1},
                    new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wy3"].ToString(), Rate=1},
                                                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wwf1"].ToString(), Rate=1},
                   new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wwf2"].ToString(), Rate=1},
                    new CloudConnectionConfig(){ ConnectionString=ConfigurationManager.ConnectionStrings["wwf3"].ToString(), Rate=1},


            });
        }
    }

}