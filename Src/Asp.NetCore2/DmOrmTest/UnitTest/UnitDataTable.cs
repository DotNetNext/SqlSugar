using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    internal class UnitDataTable
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadffa>();//建表
            db.DbMaintenance.TruncateTable<Unitadffa>();//清空
            db.Insertable(new Unitadffa() { a = "a", b = "b", c = "c" }).ExecuteCommand();
            db.Insertable(new Unitadffa() { a = "a1", b = "b1", c = "c" }).ExecuteCommand();
            db.Insertable(new Unitadffa() { a = "a2", b = "b2", c = "c" }).ExecuteCommand();
            db.Insertable(new Unitadffa() { a = "a3", b = "b3", c = "c" }).ExecuteCommand();
            db.Insertable(new Unitadffa() { a = "a4", b = "b4", c = "c" }).ExecuteCommand();

            DataTable dt = new DataTable();
            dt.Columns.Add("a", typeof(string));
            dt.Columns.Add("b", typeof(string));
            dt.Columns.Add("c", typeof(string));

            dt.Rows.Add("a", "b", "c");
            dt.Rows.Add("a1", "b1", "c");
            dt.Rows.Add("a2", "b2", "c");
            dt.Rows.Add("a3", "b3", "c");
            dt.Rows.Add("a4", "b4", "c");
            dt.Rows.Add("a5", "b5", "c");
            dt.Rows.Add("a6", "b6", "c");
            dt.TableName = "Unitadffa";
            var x= db.Storageable(dt)
                     .SplitUpdate(it => it.Any()) 
                     .SplitInsert(it => true)
                     .WhereColumns(new string[] { "a", "b" }).ToStorage();

            x.AsUpdateable.ExecuteCommand();
            x.AsInsertable.ExecuteCommand();

        }
    }

    public class Unitadffa
    {
        public string a { get; set; }
        public string b { get; set; }
        public string c { get; set; }
    }
}
