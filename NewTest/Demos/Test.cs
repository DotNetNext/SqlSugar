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
    public class MyCost { public static Student2 p = new Student2() { id = 5, name = "张表", isOk = true };}
    //单元测试
    public class Test : IDemos
    {
        public Student2 Getp() { return new Student2() { id = 4, name = "张表", isOk = true }; }
        public string Getp2() { return new Student2() { id = 4, name = "张表", isOk = true }.name; }
        public Student2 _p = new Student2() { id = 3, name = "张表", isOk = true };
        public void Init()
        {
            Student2 p = new Student2() { id = 2, name = "张表", isOk = true };
            Console.WriteLine("启动Test.Init");
            using (var db = SugarDao.GetInstance())
            {
                //初始化数据
                InitData(db);

                //查询测试
                Select(p,db);

                //拉姆达测试 
                 Exp(p, db);
                
                //新容器转换测试
                SelectNew(p, db);

                //更新测试
                Update(p, db);

                //删除测试
                //Delete(p,db);

            }
        }

        /// <summary>
        /// 删除测试
        /// </summary>
        /// <param name="p"></param>
        /// <param name="db"></param>
        private void Delete(Student2 p, SqlSugarClient db)
        {
            db.Delete<Student>(it => it.id == 1);
            db.Delete<Student>(new Student() {  id=2});
            db.Delete<Student,int>(3,4);
            db.Delete<Student, int>(it=>it.id,5,6);
            var list = db.Queryable<Student>().ToList();
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="p"></param>
        /// <param name="db"></param>
        private void Update(Student2 p, SqlSugarClient db)
        {
            db.UpdateRange(StudentListUpd);
            var list = db.Queryable<Student>().ToList();
            db.SqlBulkReplace(StudentListUpd2);
            list = db.Queryable<Student>().ToList();
        }

        /// <summary>
        /// 查询测试
        /// </summary>
        /// <param name="p"></param>
        /// <param name="d"></param>
        private void Select(Student2 p, SqlSugarClient db)
        {

            //查询第一条
            var select1= db.Queryable<Student>().Single(it => it.id == p.id);
            var select2 = db.Queryable<Student>().Single(it => it.id == _p.id);


            //查询分页 id3-id5
            var select3 = db.Queryable<Student>().OrderBy(it=>it.id).Skip(2).Take(3).ToList();
            //查询分页 id4-id6
            var select4 = db.Queryable<Student>().OrderBy(it => it.id).ToPageList(2,3).ToList();

            //2表join
            var join1 = db.Queryable<Student>().JoinTable<School>((st, sc) => st.sch_id == sc.id)
                .Select<School,V_Student>((st, sc) => new V_Student() {SchoolName=sc.name,id=st.id, name=st.name}).ToList();

            //3表join
            var join2 = db.Queryable<Student>()
                .JoinTable<School>((st, sc) => st.sch_id == sc.id)
                .JoinTable<School,Area>((st,sc,a)=>sc.AreaId==a.id)
               .Select<School,Area, V_Student>((st, sc,a) => new V_Student() { SchoolName = sc.name, id = st.id, name = st.name,AreaName=a.name }).ToList();
        
        }


        #region 初始化数据
        private void InitData(SqlSugarClient db)
        {
            db.ExecuteCommand("truncate table Subject");
            db.ExecuteCommand("truncate table student");
            db.ExecuteCommand("truncate table school");
            db.ExecuteCommand("truncate table area");

            db.SqlBulkCopy(SchoolList);
            db.SqlBulkCopy(StudentList);
            db.SqlBulkCopy(SubjectList);
            db.InsertRange(AreaList);
        }


        public List<Area> AreaList
        {
            get
            {
                List<Area> areaList = new List<Area>()
                {
                    new Area(){ name="上海"},
                    new Area(){ name="北京"},
                    new Area(){ name="南通"},
                    new Area(){ name="日本"}
                };
                return areaList;
            }
        }

        public List<Subject> SubjectList
        {
            get
            {
                List<Subject> subList = new List<Subject>()
                {
                    new Subject(){ name="语文", studentId=1},
                    new Subject(){name="数学", studentId=2},
                    new Subject(){name=".NET", studentId=4},
                    new Subject(){name="JAVA", studentId=5},
                    new Subject(){name="IOS", studentId=6},
                    new Subject(){name="PHP", studentId=7}

                };
                return subList;
            }
        }

        /// <summary>
        ///插入学生表数据
        /// </summary>
        public List<Student> StudentList
        {
            get
            {
                List<Student> student = new List<Student>()
                {
                    new Student(){ isOk=true, name="小杰", sch_id=1, sex="boy"},
                     new Student(){ isOk=false, name="小明", sch_id=2, sex="grid"},
                      new Student(){ isOk=true, name="张三", sch_id=3, sex="boy"},
                       new Student(){ isOk=false, name="李四", sch_id=2, sex="grid"},
                        new Student(){ isOk=true, name="王五", sch_id=3, sex="boy"},
                         new Student(){ isOk=false, name="小姐", sch_id=1, sex="grid"},
                          new Student(){ isOk=true, name="小捷", sch_id=3, sex="grid"},
                           new Student(){ isOk=true, name="小J", sch_id=1, sex="grid"}
                };

                return student;
            }
        }

        /// <summary>
        /// 学校表数据
        /// </summary>
        public List<School> SchoolList
        {
            get
            {
                List<School> school = new List<School>()
                {
                    new School(){ AreaId=1,  name="北大青鸟"},
                    new School(){ AreaId=2,  name="IT清华"},
                    new School(){ AreaId=3,  name="全智"},
                    new School(){ AreaId=2,  name="黑马"},
                    new School(){ AreaId=3,  name="幼儿园"},
                    new School(){ AreaId=1,  name="蓝翔"}
                };
                return school;
            }
        } 
        #endregion

        #region 修改数据
        public List<Student> StudentListUpd
        {
            get
            {
                List<Student> student = new List<Student>()
                {
                    new Student(){  id=1,isOk=true, name="小杰1", sch_id=1, sex="1"},
                     new Student(){ id=2,isOk=true, name="小明1", sch_id=1, sex="1"},
                      new Student(){id=3, isOk=true, name="张三1", sch_id=1, sex="1"},
                       new Student(){id=4, isOk=true, name="李四1", sch_id=1, sex="1"},
                        new Student(){ id=5,isOk=true, name="王五1", sch_id=1, sex="1"},
                         new Student(){id=6, isOk=true, name="小姐1", sch_id=1, sex="1"},
                          new Student(){id=7, isOk=true, name="小捷1", sch_id=1, sex="1"},
                           new Student(){ id=8,isOk=false, name="小J1", sch_id=1, sex="1"}
                };

                return student;
            }
        }
        public List<Student> StudentListUpd2
        {
            get
            {
                List<Student> student = new List<Student>()
                {
                    new Student(){  id=1,isOk=false, name="x1", sch_id=1, sex="2"},
                     new Student(){ id=2,isOk=true, name="小明1", sch_id=1, sex="1"},
                      new Student(){id=3, isOk=true, name="张三1", sch_id=1, sex="1"},
                       new Student(){id=4, isOk=true, name="李四1", sch_id=1, sex="1"},
                        new Student(){ id=5,isOk=true, name="王五1", sch_id=1, sex="1"},
                         new Student(){id=6, isOk=true, name="小姐1", sch_id=1, sex="1"},
                          new Student(){id=7, isOk=true, name="小捷1", sch_id=1, sex="1"},
                           new Student(){ id=8,isOk=false, name="小J1", sch_id=1, sex="1"}
                };

                return student;
            }
        }


        #endregion

        /// <summary>
        /// 测试select new
        /// </summary>
        /// <param name="p"></param>
        /// <param name="db"></param>
        private void SelectNew(Student2 p, SqlSugarClient db)
        {

            //测试用例
            var queryable = db.Queryable<Student>().Where(c => c.id < 10)
                .Select<V_Student>(c => new V_Student { id = c.id, name = c.name, AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" });

            var list = queryable.ToList();


            //多表操作将Student转换成V_Student
            var queryable2 = db.Queryable<Student>()
             .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
             .Where<School>((s1, s2) => s2.id < 10)
             .Select<School, V_Student>((s1, s2) => new V_Student() { id = s1.id, name = s1.name, AreaName = "默认地区", SchoolName = s2.name, SubjectName = "NET" });//select new 目前只支持这种写法

            var list2 = queryable2.ToList();


            //select字符串 转换成V_Student
            var list3 = db.Queryable<Student>()
           .JoinTable<School>((s1, s2) => s1.sch_id == s2.id)
           .Where(s1 => s1.id <= 3)
           .Select<V_Student>("s1.*,s2.name SchoolName")
           .ToList();



            //新容器转换函数的支持 只支持ObjToXXX和Convert.ToXXX
            var f1 = db.Queryable<InsertTest>().Select<Student>(it => new Student()
            {
                name = it.d1.ObjToString(),
                id = it.int1.ObjToInt() // 支持ObjToXXX 所有函数

            }).ToList();

            var f2 = db.Queryable<InsertTest>().Select<Student>(it => new Student()
            {
                name = Convert.ToString(it.d1),//支持Convet.ToXX所有函数
                id = it.int1.ObjToInt(),
                sex = Convert.ToString(it.d1),

            }).ToList();

            var f3 = db.Queryable<InsertTest>()
                .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                .Select<InsertTest, Student>((i1, i2) => new Student()
                {
                    name = Convert.ToString(i1.d1), //多表查询例子
                    id = i1.int1.ObjToInt(),
                    sex = Convert.ToString(i2.d1),

                }).ToList();


            //Select 外部参数用法
            var f4 = db.Queryable<InsertTest>().Where("1=1", new { id = 100 }).Select<Student>(it => new Student()
            {
                id = "@id".ObjToInt(), //取的是 100 的值
                name = "张三",//内部参数可以直接写
                sex = it.txt,
                sch_id = it.id

            }).ToList();
            var f6 = db.Queryable<InsertTest>()
           .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
           .Where("1=1", new { id = 100, name = "张三", isOk = true }) //外部传参给@id
           .Select<InsertTest, Student>((i1, i2) => new Student()
           {
               name = "@name".ObjToString(), //多表查询例子
               id = "@id".ObjToInt(),
               sex = i2.txt,
               sch_id = 1,
               isOk = "@isOk".ObjToBool()

           }).ToList();


            try
            {

                //测试用例
                db.Queryable<Student>().Where(c => c.id < 10)
                  .Select<V_Student>(c => new V_Student { id = c.id, name = _p.name, AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" }).ToList();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<Student>().Where(c => c.id < 10)
                  .Select<V_Student>(c => new V_Student { id = c.id, name = p.name, AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" }).ToList();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<Student>().Where(c => c.id < 10)
                  .Select<V_Student>(c => new V_Student { id = c.id, name = Getp().name, AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" }).ToList();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<Student>().Where(c => c.id < 10)
                  .Select<V_Student>(c => new V_Student { id = c.id, name = Getp2(), AreaName = "默认地区", SchoolName = "默认学校", SubjectName = "NET" }).ToList();


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<InsertTest>()
                     .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                     .Select<InsertTest, Student>((i1, i2) => new Student()
                     {
                         name = Getp2(), //多表查询例子
                         id = i1.int1.ObjToInt(),
                         sex = Convert.ToString(i2.d1),

                     }).ToList();



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<InsertTest>()
                     .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                     .Select<InsertTest, Student>((i1, i2) => new Student()
                     {
                         name = Getp().name, //多表查询例子
                         id = i1.int1.ObjToInt(),
                         sex = Convert.ToString(i2.d1),

                     }).ToList();



            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<InsertTest>()
                     .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                     .Select<InsertTest, Student>((i1, i2) => new Student()
                     {
                         name = Getp().name, //多表查询例子
                         id = i1.int1.ObjToInt(),
                         sex = Convert.ToString(i2.d1),

                     }).ToList();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {

                //测试用例
                db.Queryable<InsertTest>()
                     .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                     .Select<InsertTest, Student>((i1, i2) => new Student()
                     {
                         name = Getp().name, //多表查询例子
                         id = i1.int1.ObjToInt() + 1,
                         sex = Convert.ToString(i2.d1),

                     }).ToList();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {

                //测试用例
                db.Queryable<InsertTest>()
                     .JoinTable<InsertTest>((i1, i2) => i1.id == i2.id)
                     .Select<InsertTest, Student>((i1, i2) => new Student()
                     {
                         name = p.name, //多表查询例子
                         sex = Convert.ToString(i2.d1),

                     }).ToList();

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 拉姆达测试
        /// </summary>
        /// <param name="p"></param>
        /// <param name="db"></param>
        private void Exp(Student2 p, SqlSugarClient db)
        {
            //解析拉姆达各种情况组合

            //基本
            var t1 = db.Queryable<Student>().Where(it => it.id == 1).ToList();
            var t2 = db.Queryable<Student>().Where(it => it.id == p.id).ToList();
            var t3 = db.Queryable<Student>().Where(it => it.id == _p.id).ToList();
            var t4 = db.Queryable<Student>().Where(it => it.id == Getp().id).ToList();
            var t5 = db.Queryable<Student>().Where(it => it.id == MyCost.p.id).ToList();


            //BOOL
            var t11 = db.Queryable<Student2>("Student").Where(it => it.isOk).ToList();
            var t21 = db.Queryable<Student2>("Student").Where(it => it.isOk == MyCost.p.isOk).ToList();
            var t31 = db.Queryable<Student2>("Student").Where(it => !it.isOk).ToList();
            var t41 = db.Queryable<Student2>("Student").Where(it => it.isOk == true).ToList();
            var t51 = db.Queryable<Student2>("Student").Where(it => it.isOk == false).ToList();
            var t61 = db.Queryable<Student2>("Student").Where(it => it.isOk == _p.isOk).ToList();
            var t71 = db.Queryable<Student2>("Student").Where(it => !it.isOk && !p.isOk == it.isOk).ToList();
            var t91 = db.Queryable<Student2>("Student").Where(it => _p.isOk == false).ToList();
            var t81 = db.Queryable<Student>().Where(it => it.isOk == false).ToList();
            var t111 = db.Queryable<Student2>("Student").Where(it => it.isOk && false).ToList();


            //length
            var c1 = db.Queryable<Student>().Where(c => c.name.Length > 4).ToList();
            var c2 = db.Queryable<Student>().Where(c => c.name.Length > _p.name.Length).ToList();
            var c3 = db.Queryable<Student>().Where(c => c.name.Length > "aa".Length).ToList();
            var c4 = db.Queryable<Student>().Where(c => c.name.Length > Getp().id).ToList();


            //Equals
            var a1 = db.Queryable<Student>().Where(c => c.name.Equals(null)).ToList();
            var x = new InsertTest() { };
            var x1 = db.Queryable<Student>().Where(c => c.name.Equals(x.int1)).ToList();
            var a2 = db.Queryable<Student>().Where(c => c.name.Equals(p.name)).ToList();
            var a4 = db.Queryable<Student>().Where(c => c.name.Equals(Getp().name)).ToList();



            //Contains
            var s = db.Queryable<Student>().Where(c => c.name.Contains(null)).ToList();
            var s0 = new InsertTest() { };
            var s1 = db.Queryable<Student>().Where(c => c.name.Contains(x.v1)).ToList();
            var s3 = db.Queryable<Student>().Where(c => c.name.Contains(p.name)).ToList();
            var s4 = db.Queryable<Student>().Where(c => c.name.Contains(Getp().name)).ToList();

            var s5 = db.Queryable<Student>().Where(c => c.name.StartsWith(Getp().name)).ToList();
            var s6 = db.Queryable<Student>().Where(c => c.name.EndsWith(Getp().name)).ToList();


            //异常处理测试,防止程序中出现未知错误
            try
            {
                var e6 = db.Queryable<Student>().Where(c => Getp().name.StartsWith(c.name)).ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {
                var e6 = db.Queryable<Student>().Where(c => Getp().name.Equals(c.name)).ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            try
            {
                var e6 = db.Queryable<Student>().Where(c => c.name.First() != null).ToList();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            //组合测试
            var z = db.Queryable<Student>().Where(c => (c.name.Equals(Getp().name) || c.name == p.name) && true && c.id > 1).ToList();
            var z23 = db.Queryable<Student>().Where(c => !string.IsNullOrEmpty(c.name) || (c.id == 1 || c.name.Contains(p.name))).ToList();
            var z2 = db.Queryable<Student>().Where(c => !string.IsNullOrEmpty(c.name) || !string.IsNullOrEmpty(c.name)).ToList();
        }
    }
}
