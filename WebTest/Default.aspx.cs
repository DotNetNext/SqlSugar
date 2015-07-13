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
            string connStr = @"Server=(LocalDB)\v11.0; Integrated Security=true ;AttachDbFileName=" + Server.MapPath("~/App_Data/SqlSugar.mdf");
            using (SqlSugarClient db = new SqlSugarClient(connStr))
            {
                db.BeginTran();
                db.Sqlable.IsNoLock = true;


                try
                {
                   
                    var dt = db.SqlQuery<School>("select * from School");
                   

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

    public class School
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class  Student
    {

    }
  
}
