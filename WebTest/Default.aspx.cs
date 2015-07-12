using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;

namespace WebTest
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string connStr = "server=.;uid=sa;password=sasa;database=nfd";
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.BeginTran();
                db.Sqlable.IsNoLock = true;


                try
                {
                    var sql = db.Sqlable.MappingTable<CutBill, CutBillShipment>("t1.c_id=t2.c_id").SelectToSql("t1.*");
                    var dt = db.GetDataTable(sql);
                    var id = db.Insert(new test() { name = "哈哈" + DateTime.Now });
                    var del = db.Delete<test>(21);
                    var update = db.Update<test>(new { name = "5555" + DateTime.Now }, new { id=1 });

                }
                catch (Exception)
                {

                    db.RollbackTran();
                }

            }
            ;
            //var xx = SqlTool.CreateMappingTable(20);
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
