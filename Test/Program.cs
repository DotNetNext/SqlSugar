using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string connStr = "server=.;uid=sa;password=sasa;database=nfd";
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.BeginTran();
                db.Sqlable.IsNoLock = true;
                var sql = db.Sqlable.MappingTable<CutBill, CutBillShipment>("t1.c_id=t2.c_id").SelectToSql("t1.*");
                db.Sqlable.IsNoLock = false;
                var sql2 = db.Sqlable.MappingTable<CutBill, CutBillShipment>("t1.c_id=t2.c_id").SelectToSql("t1.*");
                db.Sqlable.IsNoLock = true;
                var sql3 = db.Sqlable.MappingTable<CutBill, CutBillShipment>("t1.c_id=t2.c_id").SelectToSql("t1.*");

                try
                {
                    var dt = db.GetDataTable(sql);
                    for (int i = 0; i < 10; i++)
                    {
                        var id = db.Insert(new test() { name = "哈哈" });
                        Console.WriteLine(id);
                    }


                }
                catch (Exception)
                {

                    db.RollbackTran();
                }

            }
            ;
            var xx = SqlTool.CreateMappingTable(20);
            Console.Read();
        }

    }

    public class test
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class V_Student
    {

    }
    public class CutBill
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class CutBillShipment
    {

    }
}
