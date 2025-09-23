using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.Data;
using System.Xml.Serialization;

namespace OrmTest
{
    internal class Unitasdfadsy22
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            //建库
            db.DbMaintenance.CreateDatabase();

            //建表
            db.CodeFirst.InitTables<Test001>();
            db.CodeFirst.InitTables<Test002>();
            //清空表
            db.DbMaintenance.TruncateTable<Test001>();
            db.DbMaintenance.TruncateTable<Test002>();
            //添加过滤器
            db.QueryFilter.AddTableFilter<IDeleted>(it => it.IsDelete == false);

            //插入测试数据
            var result = db.Insertable(new Test001() { Name = "01" }).ExecuteReturnEntity();//用例代码
            db.Insertable(new Test002() { Test001Id = result.Id, Name = "01_01" }).ExecuteCommand();//用例代码

            var data = db.Queryable<Test001>().Select(x => new TestView
            {
                Test002Count = SqlFunc.Subqueryable<Test002>().EnableTableFilter().Where(w => w.Test001Id == x.Id).Count()
            }, true);
            var list = data.ToList();
        }

        public class TestView : Test001
        {
            public int Test002Count { get; set; }
        }

        [SugarTable("unitTest001122")]
        public class Test001 : IDeleted
        {
            [SugarColumn(ColumnName = "id", IsNullable = false, ColumnDescription = "主键", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "名称", ColumnDataType = "varchar(32)")]
            public string Name { get; set; }

            [SugarColumn(ColumnName = "is_delete", IsNullable = false, ColumnDescription = "是否删除")]
            public bool IsDelete { get; set; } = false;
        }
        [SugarTable("unitTest222222")]
        public class Test002 : IDeleted
        {
            [SugarColumn(ColumnName = "id", IsNullable = false, ColumnDescription = "主键", IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            [SugarColumn(ColumnName = "Test001_id", IsNullable = false)]
            public int Test001Id { get; set; }

            [SugarColumn(ColumnName = "name", IsNullable = false, ColumnDescription = "名称", ColumnDataType = "varchar(32)")]
            public string Name { get; set; }

            [SugarColumn(ColumnName = "is_delete", IsNullable = false, ColumnDescription = "是否删除")]
            public bool IsDelete { get; set; } = false;
        }

        public interface IDeleted
        {
            /// <summary>
            /// 是否删除
            /// </summary>
            public bool IsDelete { get; set; }
        }
    }
}
