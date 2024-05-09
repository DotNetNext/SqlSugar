using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitUpdateColums
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<DemoUpdateColums>();
            db.Updateable(new DemoUpdateColums() { Id = 1 }).UpdateColumns(it => new
            {
                it.DBTYPE,
                it.Name
            }).ExecuteCommand();
        }

    }
    public class DemoUpdateColums 
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true,ColumnName ="MYID")]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "myDBTYPE")]
        public SqlSugar.DbType? DBTYPE { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "myname")]
        public String Name { get; set; }
    }
}
