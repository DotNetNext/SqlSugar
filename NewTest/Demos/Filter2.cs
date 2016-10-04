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
    //行过滤加列过滤
    //权限管理的最佳设计
    public class Filter2 : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Filter2.Init");
            using (SqlSugarClient db = SugarDaoFilter.GetInstance())//开启数据库连接
            {
                //设置走哪个过滤器
                db.CurrentFilterKey = "role1";
                //queryable
                var list = db.Queryable<Student>().ToJson(); //where id=1 , 可以查看id和name


                //设置走哪个过滤器
                db.CurrentFilterKey = "role2";
                //queryable
                var list2 = db.Queryable<Student>().ToJson(); //where id=2 , 可以查看name

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
            /// 页面所需要的过滤行
            /// </summary>
            private static Dictionary<string, Func<KeyValueObj>> _filterParas = new Dictionary<string, Func<KeyValueObj>>()
            {
              { "role1",()=>{
                        return new KeyValueObj(){ Key=" id=@id" , Value=new{ id=1}};
                   }
              },
              { "role2",()=>{
                  return new KeyValueObj() { Key = " id=@id", Value = new { id = 2 } };
                  }
              },
            };
            /// <summary>
            /// 页面所需要的过滤列
            /// </summary>
            private static Dictionary<string, List<string>> _filterColumns = new Dictionary<string, List<string>>()
            {
              { "role1",new List<string>(){"id","name"}
              },
              { "role2",new List<string>(){"name"}
              },
            };
            public static SqlSugarClient GetInstance()
            {
                string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);

                //支持sqlable和queryable
                db.SetFilterFilterParas(_filterParas);

                //列过滤只支持queryable
                db.SetFilterFilterParas(_filterColumns);


                db.IsEnableLogEvent = true;//启用日志事件
                db.LogEventStarting = (sql, par) => { Console.WriteLine(sql + " " + par + "\r\n"); };
                return db;
            }
        }
    }
}
