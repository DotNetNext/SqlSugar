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
    /// <summary>
    ///如何避免初始化SqlSugarClient时，参数赋值引起的性能的浪费
    /// </summary>
    public class InitConfig : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动InitConfig.Init");
            using (SqlSugarClient db = SugarPocoDao.GetInstance())//开启数据库连接
            {

            }
        }

        /// <summary>
        /// SqlSugarClient初始化全配置类
        /// </summary>
        public class DaoInitConfig
        {
            //别名列
            public static List<KeyValue> columnMappingList= new List<KeyValue>() { 
                new KeyValue(){ Key="entityId", Value="tableId"},
                new KeyValue(){ Key="entityName", Value="tableName"}
            };

            //别名表 
            public static List<KeyValue> tableMappingList = null;


            //流水号
            public static List<PubModel.SerialNumber> serialNumber = new List<PubModel.SerialNumber>(){
              new PubModel.SerialNumber(){TableName="Student", FieldName="name", GetNumFunc=()=>{  return "stud-"+DateTime.Now.ToString("yyyy-MM-dd");}},
              new PubModel.SerialNumber(){TableName="School", FieldName="name",  GetNumFuncWithDb=db=>{ return "ch-"+DateTime.Now.ToString("syyyy-MM-dd"); }}
            };

            //自动排除非数据库列
            public static bool IsIgnoreErrorColumns=true;

        }


        /// <summary>
        /// 扩展SqlSugarClient
        /// </summary>
        public class SugarPocoDao
        {
            //禁止实例化
            private SugarPocoDao()
            {

            }

            public static SqlSugarClient GetInstance()
            {

                string connection = SugarDao.ConnectionString; //这里可以动态根据cookies或session实现多库切换
                var db = new SqlSugarClient(connection);

                /**这种写法只给db对象添加了4个指向地址（DaoInitConfig变量都为静态对象），并非指向内容，指向内容初始化后存储在内存当中，所以性能就不用说了 **/

                db.SetMappingTables(GetMappingTables(db));//设置别名表

                db.SetMappingColumns(DaoInitConfig.columnMappingList);//设置别名列

                db.SetSerialNumber(DaoInitConfig.serialNumber);//设置流水号

                db.IsIgnoreErrorColumns = DaoInitConfig.IsIgnoreErrorColumns;  //自动排除非数据库列


                return db;
            }

            /// <summary>
            /// 批量设置别名表
            /// </summary>
            /// <param name="db"></param>
            /// <returns></returns>
            private static List<KeyValue> GetMappingTables(SqlSugarClient db)
            {
                if (DaoInitConfig.tableMappingList == null)
                {
                    DaoInitConfig.tableMappingList = new List<KeyValue>();
                    db.ClassGenerating.ForeachTables(db, name =>//内置遍历表名和视图名函数
                    {
                        //给所有表名加dbo.
                        DaoInitConfig.tableMappingList.Add(new KeyValue() { Key = name, Value ="dbo."+name });

                        //动态获取sechma
                        // DaoInitConfig.tableMappingList.Add(new KeyValue() { Key = name, Value = db.ClassGenerating.GetTableNameWithSchema(db,name) });
                    });
                }
                return DaoInitConfig.tableMappingList;
            }
        }
    }
}
