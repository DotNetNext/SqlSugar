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
    //流水号的功能
    public class SerialNumber : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动SerialNumber.Init");
            using (SqlSugarClient db = SugarFactory.GetInstance())//开启数据库连接
            {
                var dientityValue = db.Insert<Student>(new Student() {   });
                var name = db.Queryable<Student>().Single(it => it.id == dientityValue.ObjToInt()).name;
                Console.WriteLine(name);

                var dientityValue2 = db.Insert<School>(new School() { });
                var name2 = db.Queryable<School>().Single(it => it.id == dientityValue2.ObjToInt()).name;
                Console.WriteLine(name2); ;
            }
        }

        /// <summary>
        /// 全局配置类
        /// </summary>
        public class SugarConfigs
        {
            public static List<PubModel.SerialNumber> NumList = new List<PubModel.SerialNumber>(){
              new PubModel.SerialNumber(){TableName="Student", FieldName="name", GetNumFunc=()=>{ //GetNumFunc在没有事中使用
                  return "stud-"+DateTime.Now.ToString("yyyy-MM-dd");
              }},
                new PubModel.SerialNumber(){TableName="School", FieldName="name",  GetNumFuncWithDb=db=>{ //事务中请使用GetNumFuncWithDb保证同一个DB对象,不然会出现死锁
                  return "ch-"+DateTime.Now.ToString("syyyy-MM-dd");
              }}
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
                db.SetSerialNumber(SugarConfigs.NumList);//设置流水号
                return db;
            }
        }
    }
}
