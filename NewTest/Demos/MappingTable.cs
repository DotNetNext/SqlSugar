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
    //别名表的功能
    public class MappingTable : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动MappingTable.Init");

            //单个设置
            using (var db = SugarDao.GetInstance())
            {
                var list = db.Queryable<V_Student>("Student").ToList();//查询的是 select * from student 而我的实体名称为V_Student
            }


            //全局设置
            using (var db = SugarFactory.GetInstance())
            {
                var list = db.Queryable<V_Student>().ToList();//查询的是 select * from student 而我的实体名称为V_Student
            }
        }


        /// <summary>
        /// 全局配置类
        /// </summary>
        public class SugarConfigs
        {
            //key类名 value表名
            public static List<KeyValue> MpList = new List<KeyValue>(){
            new KeyValue(){ Key="FormAttr", Value="Flow_FormAttr"},
            new KeyValue(){ Key="Student3", Value="Student"},
            new KeyValue(){ Key="V_Student", Value="Student"}
            };
        }

        /// <summary>
        /// SqlSugar实例工厂
        /// </summary>
        public class SugarFactory
        {

            //禁止实例化
            private SugarFactory()
            {

            }
            public static SqlSugarClient GetInstance()
            {
                string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);
                db.SetMappingTables(SugarConfigs.MpList);//设置关联表 (引用地址赋值，每次赋值都只是存储一个内存地址)
                return db;
            }
        }
    }
}
