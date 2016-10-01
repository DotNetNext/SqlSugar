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
    //通过属性的方法设置别名表和别名字段
    public class AttributesMapping : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动AttributesMapping.Init");
            using (var db = DBManager.GetInstance())
            {

                var list = db.Queryable<TestStudent>().Where(it=>it.classId==1).ToList();

            }
        }

        /// <summary>
        /// 属性只作为初始化映射，SetMappingTables和SetMappingColumns可以覆盖
        /// </summary>
        [SugarMapping(TableName = "Student")]
        public class TestStudent
        {

            [SugarMapping(ColumnName = "id")]
            public int classId { get; set; }


            public string name { get; set; }
        }
    }

    public class DBManager
    {

        public static SqlSugarClient GetInstance()
        {
            var db = new SqlSugarClient(SugarDao.ConnectionString);
            db.IsEnableAttributeMapping = true;//启用属性映射
            db.IsIgnoreErrorColumns = true;//忽略非数据库列
            return db;
        }
    }
}
