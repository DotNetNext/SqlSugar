using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    public class SharedConnection : DemoBase
    {
        public static void Init()
        {
            StudentDal studentDal = new StudentDal();
            SchoolDal schoolDal = new SchoolDal();

            try
            {
                studentDal.BeginTran();

                Console.WriteLine("school Count:"+ schoolDal.GetSchoolCount());//0

                studentDal.AddStudent(new Student() { Name = "StudentTest" });
                schoolDal.AddSchool(new School() { Name = "SchoolTest" });//1

                Console.WriteLine("school Count:" + schoolDal.GetSchoolCount());

                throw new Exception("error");
            }
            catch (Exception ex)
            {
                studentDal.RollbackTran();
                Console.WriteLine("school Count:" + schoolDal.GetSchoolCount());//0
            }
        }


    }
    public class StudentDal : BaseDao
    {
        public void AddStudent(Student sudent)
        {
            db.Insertable(sudent).ExecuteCommand();
        }
    }
    public class SchoolDal : BaseDao
    {
        public void AddSchool(School school)
        {
            db.Insertable(school).ExecuteCommand();
        }
        public int GetSchoolCount()
        {
            return db.Queryable<School>().Count();
        }
    }

    public class BaseDao
    {

        public SqlSugar.SqlSugarClient db { get { return GetInstance(); } }
        public void BeginTran()
        {
            db.Ado.BeginTran();
        }
        public void CommitTran()
        {
            db.Ado.CommitTran();
        }
        public void RollbackTran()
        {
            db.Ado.RollbackTran();
        }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(
                new ConnectionConfig() {
                    ConnectionString = Config.ConnectionString,
                    DbType = DbType.PostgreSQL,
                    IsAutoCloseConnection = false,
                    IsShardSameThread= true /*Shard Same Thread*/
                });
          
            return db;
        }
    }
}
