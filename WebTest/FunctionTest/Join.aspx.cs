using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using Models;
namespace WebTest.Dao
{
    public partial class Join : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                var list = db.Sqlable().Form("student", "s").Join("school", "l", "s.sch_id", "l.id", JoinType.INNER).SelectToList<Student>("*");
                GetStudent(0, null);
                var count = GetCount();
            }
        }
        public int GetCount()
        {
            using (var db = SugarDao.GetInstance())
            {
                var sable = db.Sqlable().Form("student", "s").Join("school", "l", "s.sch_id", "l.id", JoinType.INNER);
                return sable.Count();
            }
        }
        public List<Student> GetStudent(int id, string name)
        {

            using (var db = SugarDao.GetInstance())
            {
                var sable = db.Sqlable().Form("student", "s").Join("school", "l", "s.sch_id", "l.id", JoinType.INNER);
                if (!string.IsNullOrEmpty(name))
                {
                    sable = sable.Where("s.name=s.@name");
                }
                if (!string.IsNullOrEmpty(name))
                {
                    sable = sable.Where("s.id=@id");
                }
                return sable.SelectToList<Student>("s.*", new { id = id, name = name });
            }
        }
    }
}