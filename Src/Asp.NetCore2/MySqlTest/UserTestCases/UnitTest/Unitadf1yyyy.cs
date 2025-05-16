using SqlSugar;
using System.Collections.Generic;
using System.Linq;
using System;

namespace OrmTest
{
    internal class Unitadfa12
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            string tableName = "m_test";
            //建表 
            db.CodeFirst.As<Test001>(tableName).InitTables<Test001>();

            List<Test001> longList = new List<Test001>();
            for (int i = 0; i < 2; i++)
            {
                longList.Add(new Test001() { CreateTime = DateTime.Now });
            }

            //插入测试数据
            var indexList = db.Insertable(longList).AS(tableName).ExecuteReturnPkList<long>();
            if(db.DbMaintenance.IsAnyTable("UnitDefaultValueadsfa", false))
              db.DbMaintenance.DropTable<UnitDefaultValueadsfa>();
            db.CodeFirst.InitTables<UnitDefaultValueadsfa>();
            db.Insertable(new UnitDefaultValueadsfa() { name = "a" }).ExecuteCommand();
        }
    }

    public class UnitDefaultValueadsfa
    {
        [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int Id { get; set; }
        public string name { get; set; } 
        [SugarColumn(DefaultValue ="",IsOnlyIgnoreInsert =true)]
        public string a2 { get; set; }
    }

    //建类
    public class Test001
    {

        [SugarColumn(ColumnName = "id", ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = true)]
        public long Id
        {
            get; set;
        }

        [SugarColumn(ColumnName = "createTime", ColumnDescription = "创建时间")]
        public DateTime CreateTime
        {
            get; set;
        }
    }
}
