using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using System.Data.SqlClient;
using SqlSugar;
namespace NewTest.Demos
{
    //别名列的功能
    public class MappingColumns : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动MappingColumns.Init");

            //全局设置
            using (var db = SugarFactory.GetInstance())
            {
                var list = db.Queryable<Student>().Where(it=>it.classId==1).ToList();
            }
        }

        public class Student
        {

            //id
            public int classId { get; set; }

            //name
            public string className { get; set; }

            //sch_id
            public int classSchoolId { get; set; }

            public int isOk { get; set; }
        }

        /// <summary>
        /// 全局配置别名列（不区分表）
        /// </summary>
        public class SugarConfigs
        {
            //key实体字段名 value表字段名 ，KEY唯一否则异常
            public static List<KeyValue> MpList = new List<KeyValue>(){
            new KeyValue(){ Key="classId", Value="id"},
            new KeyValue(){ Key="className", Value="name"},
            new KeyValue(){ Key="classSchoolId", Value="sch_id"}
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
                db.SetMappingColumns(SugarConfigs.MpList);//设置关联列 (引用地址赋值，每次赋值都只是存储一个内存地址)
                return db;
            }
        }
    }
}
