﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebTest.Dao;
using SqlSugar;
using Models;
namespace WebTest.Demo
{
    /// <summary>
    /// 删除
    /// </summary>
    public partial class Delete : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var db = SugarDao.GetInstance())
            {
                //删除
                db.Delete<School, int>(10);
                db.Delete<School>(it => it.id > 100);
                //主键批量删除
                db.Delete<School, string>(new string[] { "100", "101", "102" });
                //非主键批量删除
                db.Delete<School, string>(it => it.name, new string[] { "" });
                db.Delete<School, int>(it => it.id, new int[] { 20, 22 });

                //假删除
                //db.FalseDelete<school>("is_del", 100);
                //等同于 update school set is_del=1 where id in(100)
                //db.FalseDelete<school>("is_del", it=>it.id==100);


            }
        }
    }
}