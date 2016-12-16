using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;
using SqlSugar;

namespace NewTest.Demos
{
    //过滤器用法
    //使用场合(例如：假删除查询，这个时候就可以设置一个过滤器,不需要每次都 .Where(it=>it.IsDelete==true))  
    public class Filter : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Filter.Init");
            using (SqlSugarClient db = SugarDaoFilter.GetInstance())//开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role,role2"; //支持多个过滤器以逗号隔开

                //queryable
                var list = db.Queryable<Student>().ToList(); //通过全局过滤器对需要权限验证的数据进行过滤
                //相当于db.Queryable<Student>().Where("id=@id",new{id=1})


                //sqlable
                var list2 = db.Sqlable().From<Student>("s").SelectToList<Student>("*");
                //同上

                //sqlQuery
                var list3 = db.SqlQuery<Student>("select * from Student WHERE 1=1");
                //同上
            }
        }
        /// <summary>
        /// 扩展SqlSugarClient
        /// </summary>
        public class SugarDaoFilter
        {
            //禁止实例化
            private SugarDaoFilter()
            {

            }
            /// <summary>
            /// 页面所需要的过滤函数
            /// </summary>
            private static Dictionary<string, Func<KeyValueObj>> _filterParas = new Dictionary<string, Func<KeyValueObj>>()
        {
          { "role",()=>{
                    return new KeyValueObj(){ Key=" id=@id" , Value=new{ id=1}};
               }
          },
          { "role2",()=>{ 
                    return new KeyValueObj(){ Key=" id>0"};
              }
          },
        };
            public static SqlSugarClient GetInstance()
            {
                string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);
                db.SetFilterItems(_filterParas);

                db.IsEnableLogEvent = true;//启用日志事件
                db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
                return db;
            }
        }
    }
}
