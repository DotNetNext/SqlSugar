using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest.UnitTest
{
    public class Update : UnitTestBase
    {
        private Update() { }
        public Update(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();
            var updateObj = new Student() { Id = 1, Name = "jack",SchoolId=0, CreateTime = Convert.ToDateTime("2017-05-21 09:56:12.610") };
            var updateObjs = new List<Student>() { updateObj,new Student() { Id=2,Name="sun",SchoolId=0 } }.ToArray();
            db.IgnoreColumns.Add("TestId", "Student");
            //db.MappingColumns.Add("id","dbid", "Student");

            var t1 = db.Updateable(updateObj).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@ID",1),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Name", "jack")
            }, t1.Key, t1.Value,"Update t1 error");

            //update reutrn Command Count
            var t2 = db.Updateable(updateObj).ExecuteCommand();

            db.IgnoreColumns = null;
            //Only  update  Name 
            var t3 = db.Updateable(updateObj).UpdateColumns(it => new { it.Name }).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [Name]=@Name  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@ID",1),
                           new SugarParameter("@Name", "jack")
            }, t3.Key, t3.Value, "Update t3 error");

            //Ignore  Name and TestId
            var t4 = db.Updateable(updateObj).IgnoreColumns(it => new { it.Name, it.TestId }).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [SchoolId]=@SchoolId,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@CreateTime",Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@SchoolId", 0),
                           new SugarParameter("@ID",1),
            }, t4.Key, t4.Value, "Update t4 error");

            //Ignore  Name and TestId
            var t5 = db.Updateable(updateObj).IgnoreColumns(it => it == "Name" || it == "TestId").With(SqlWith.UpdLock).ToSql();
            base.Check(@"UPDATE [STudent] WITH(UPDLOCK)  SET
           [SchoolId]=@SchoolId,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@CreateTime",Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@SchoolId", 0),
                           new SugarParameter("@ID",1),
            }, t5.Key, t5.Value, "Update t5 error");


            //Use Lock
            var t6 = db.Updateable(updateObj).With(SqlWith.UpdLock).ToSql();
            base.Check(@"UPDATE [STudent] WITH(UPDLOCK)  SET
           [SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@ID",1),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Name", "jack")
            }, t6.Key, t6.Value, "Update t6 error");


//            //update List<T>
//            var t7 = db.Updateable(updateObjs).With(SqlWith.UpdLock).ToSql();
//            base.Check(@"UPDATE S SET S.[SchoolId]=T.[SchoolId],S.[Name]=T.[Name],S.[CreateTime]=T.[CreateTime] FROM [STudent] S WITH(UPDLOCK)   INNER JOIN             (
              
// SELECT N'1' AS ID,N'0' AS SchoolId,N'jack' AS Name,'2017-05-21 09:56:12.610' AS CreateTime		
//UNION ALL 
// SELECT N'2' AS ID,N'0' AS SchoolId,N'sun' AS Name,NULL AS CreateTime


//            ) T ON S.[Id]=T.[Id]
//                ; ", null, t7.Key, null,"Update t7 error");

            //Re Set Value
            var t8 = db.Updateable(updateObj)
                .ReSetValue(it=>it.Name==(it.Name+1)).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [SchoolId]=@SchoolId, [Name] =( [Name] + @Const0 ),[CreateTime]=@CreateTime  WHERE [Id]=@Id",
            new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@ID",1),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Const0", 1)
            }, t8.Key, t8.Value, "Update t8 error"
           );

            //Where By Expression
            var t9 = db.Updateable(updateObj)
           .Where(it => it.Id==1).ToSql();
           base.Check(@"UPDATE [STudent]  SET
           [SchoolId]=@SchoolId,[Name]=@Name,[CreateTime]=@CreateTime  WHERE ( [ID] = @Id0 )",
          new List<SugarParameter>() {
                           new SugarParameter("@SchoolId",0),
                           new SugarParameter("@ID",1),
                           new SugarParameter("@Id0",1),
                           new SugarParameter("@CreateTime", Convert.ToDateTime("2017-05-21 09:56:12.610")),
                           new SugarParameter("@Name", "jack") },t9.Key,t9.Value,"Upate t9 error"
           );
            updateObj.SchoolId = 18;
            string name = "x";
            var t10 = db.Updateable<Student>().UpdateColumns(it => new Student() { Name =name, SchoolId=updateObj.SchoolId }).Where(it=>it.Id==11).ToSql();
            base.Check(@"UPDATE [STudent]  SET
            [SchoolId] = @Const0 , [Name] = @const3   WHERE ( [ID] = @Id1 )", new List<SugarParameter>() {
                           new SugarParameter("@const3","x"),
                           new SugarParameter("@Const0",18),
                           new SugarParameter("@Id1",11)},
                               t10.Key,
                               t10.Value,
                               "Update 10 error"
            );
            var t11 = db.Updateable<DataTestInfo>().UpdateColumns(it => new DataTestInfo() {  Datetime1=DateTime.MaxValue }).Where(it => it.Int1 == 11).ToSql();
            base.Check(@"UPDATE [DataTestInfo]  SET
            [Datetime1] = @constant0   WHERE ( [Int1] = @Int11 )", new List<SugarParameter>() {
                           new SugarParameter("@Int11",11),
                           new SugarParameter("@constant0",DateTime.MaxValue) },
                               t11.Key,
                               t11.Value,
                               "Update 11 error" 
            );

            var t12 = db.Updateable<DataTestInfo>().UpdateColumns(it => new DataTestInfo() {   Int2 = it.Int2+1 }).Where(it => it.Int1 == 11).ToSql();
            base.Check(@"UPDATE [DataTestInfo]  SET
            [Int2] = ( [Int2] + @Const0 )   WHERE ( [Int1] = @Int11 )", new List<SugarParameter>() {
                           new SugarParameter("@Int11",11),
                           new SugarParameter("@Const0",1) },
                               t12.Key,
                               t12.Value,
                               "Update 12 error"
            );



            var t13 = db.Updateable<Student>(new { Name = "a", id=1 }).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [Name]=@Name  WHERE [Id]=@Id", new List<SugarParameter>() {
                           new SugarParameter("@Name","a"),
                           new SugarParameter("@ID",1)
            }, t13.Key, t13.Value, "Insert t13 error");

            var t14 = db.Updateable<Student>(new Dictionary<string, object>() { { "id", 0 }, { "name", "2" } }).ToSql();
            base.Check(@"UPDATE [STudent]  SET
           [Name]=@Name  WHERE [Id]=@Id", new List<SugarParameter>() {
                               new SugarParameter("@Name", "2"),
                           new SugarParameter("@ID", 0)
            }, t14.Key, t14.Value, "Insert t14 error");
        }

    }
}
