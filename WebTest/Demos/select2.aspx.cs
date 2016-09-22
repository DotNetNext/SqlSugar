using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SqlSugar;
using WebTest.Dao;
using Models;

namespace WebTest.Demos
{
    /// <summary>
    /// 高级查询例子
    /// </summary>
    public partial class Select2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (SqlSugarClient db = SugarDao.GetInstance())
            {

                var childQuery = db.Queryable<Area>().Where("id=@id").Select(it => new { id=it.id }).ToSql();//创建子查询SQL
                string childTableName=string.Format("({0})",childQuery.Key);//将SQL语句用()包成表


                var queryable = db.Queryable<Student>()
                 .JoinTable<Student, School>((s1, s2) => s1.sch_id == s2.id) 
                 .JoinTable(childTableName, "a1", "a1.id=s2.areaid", new { id = 1 }, JoinType.INNER)
                 .OrderBy(s1 => s1.id);
              

                //SELECT * FROM  ( 
                
                                     //SELECT newid = s1.id, studentName = s1.name, schoolName = s2.name, areaName = a1.name ,row_index=ROW_NUMBER() OVER(ORDER BY s1.id ASC ) FROM [Student]   s1 
                                     //LEFT JOIN School  s2 ON  ( s1.sch_id  = s2.id )    
                                     //INNER JOIN (SELECT *  FROM [Area]   WHERE 1=1  AND id=@id   ) a1 ON a1.id=s2.areaid    WHERE 1=1
                                //
                                //) t WHERE t.row_index BETWEEN 1 AND 200

                 var list= queryable.Select<Student, School, Area, classNew>((s1, s2, a1) => new classNew { newid = s1.id, studentName = s1.name, schoolName = s2.name, areaName = a1.name })
                     .ToPageList(0,200);

                 var count=queryable.Count();
            }
        }
    }
}