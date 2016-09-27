using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NewTest.Dao;
using Models;
using System.Data.SqlClient;

namespace NewTest.Demos
{
    //更新
    public class Update : IDemos
    {

        public void Init()
        {
            Console.WriteLine("启动Ado.Update");
            using (var db = SugarDao.GetInstance())
            {

                //指定列更新
                db.Update<School>(new { name = "蓝翔14" }, it => it.id == 14); //只更新name列
                db.Update<School, int>(new { name = "蓝翔11 23 12", areaId = 2 }, 11, 23, 12);
                db.Update<School, string>(new { name = "蓝翔2" }, new string[] { "11", "21" });
                db.Update<School>(new { name = "蓝翔2" }, it => it.id == 100);
                var array=new int[]{1,2,3};
                db.Update<School>(new { name = "蓝翔2" }, it => array.Contains(it.id));// id in 1,2,3


                //支持字典更新，适合动态权限
                var dic = new Dictionary<string, string>();
                dic.Add("name", "第十三条");
                dic.Add("areaId", "1");
                db.Update<School, int>(dic, 13);


                //整个实体更新
                db.Update(new School { id = 16, name = "蓝翔16", AreaId = 1 });
                db.Update<School>(new School { id = 12, name = "蓝翔12", AreaId = 2 }, it => it.id == 18);
                db.Update<School>(new School() { id = 11, name = "青鸟11" });

                //设置不更新列
                db.DisableUpdateColumns = new string[] { "CreateTime" };//设置CreateTime不更新

                TestUpdateColumns updObj = new TestUpdateColumns()
                {
                    VGUID = Guid.Parse("542b5a27-6984-47c7-a8ee-359e483c8470"),
                    Name = "xx",
                    Name2 = "xx2",
                    IdentityField = 0,
                    CreateTime = null
                };

                //CreateTime将不会被更新
                db.Update(updObj);
                //以前实现这种更新需要用指定列的方式实现，现在就简单多了。



                //批量更新   数据量小时建议使用
                var updateResult = db.UpdateRange(GetUpdateList());

                //批量更新  数据量大时建议使用
                var updateResult2 = db.SqlBulkReplace(GetUpdateList2());

            }
        }



        private static List<Student> GetUpdateList()
        {
            List<Student> list = new List<Student>()
                {
                     new Student()
                {
                    id=1001,
                     name="1张1001"+new Random().Next(1,int.MaxValue)
                },
                 new Student()
                {
                    id=1002,
                    name="1张1002"+new Random().Next(1,int.MaxValue)
                }
                };
            return list;
        }
        private static List<Student> GetUpdateList2()
        {
            List<Student> list = new List<Student>()
                {
                     new Student()
                {
                    id=1010,
                    name="小妹"+new Random().Next(1,int.MaxValue),
                    isOk=false,
                    sch_id=2,
                    sex="gril"
                },
                 new Student()
                {
                    id=1011,
                    name="小子"+new Random().Next(1,int.MaxValue),
                    isOk=true,
                    sch_id=3,
                    sex="boy"
                }
                };
            return list;
        }
    }
}
