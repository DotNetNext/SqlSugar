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
    //设置分页模式
    public class SqlPageModel : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动SqlPageModel.Init");
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    db.PageModel = PageModel.Offset;  //启用Sql2012的方式进行分页(默认：RowNumber)

                    var list = db.Queryable<Student>().OrderBy("id").Skip(0).Take(2).ToList();
                    var list1 = db.Sqlable().From<Student>("t").SelectToPageList<Student>("*", "id", 1, 2);


                    List<School> dataPageList = db.Sqlable()
                    .From("school", "s")
                    .Join("student", "st", "st.id", "s.id", JoinType.Inner)
                    .Join("student", "st2", "st2.id", "st.id", JoinType.Left)
                    .Where("s.id>100 and s.id<100")
                    .SelectToPageList<School>("st.*", "s.id", 1, 10);

                    db.PageModel = PageModel.RowNumber;
                    var list2 = db.Queryable<Student>().OrderBy("id").Skip(0).Take(2).ToList();
                    var list22 = db.Sqlable().From<Student>("t").SelectToPageList<Student>("*", "id", 1, 2);
                }
                catch (Exception ex)
                {
                    
                    throw new Exception("该Demo要求SqlSever版本为2012及以上版本，错误信息:"+ex.Message);
                }
            }
        }
    }
}
