using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.UnitTest
{
    public class SqlRemark : UnitTestBase
    {
        private SqlRemark() { }
        public SqlRemark(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var db = GetInstance();

            //column remak
            if (db.DbMaintenance.IsAnyColumnRemark("id", "student"))
            {
                db.DbMaintenance.DeleteColumnRemark("id", "student");
                base.Check(false.ToString(), null, db.DbMaintenance.IsAnyColumnRemark("id", "student").ToString(), null, "SqlRemark error");
            }
            else {
                db.DbMaintenance.AddColumnRemark("id", "student","hhXX");
                base.Check(true.ToString(), null, db.DbMaintenance.IsAnyColumnRemark("id", "student").ToString(), null, "SqlRemark error");
            }


            //table remak
            if (db.DbMaintenance.IsAnyTableRemark("student"))
            {
                db.DbMaintenance.DeleteTableRemark("student");
                base.Check(false.ToString(), null, db.DbMaintenance.IsAnyTableRemark("student").ToString(), null, "SqlRemark error");
            }
            else
            {
                db.DbMaintenance.AddTableRemark("student", "hhXX");
                base.Check(true.ToString(), null, db.DbMaintenance.IsAnyTableRemark("student").ToString(), null, "SqlRemark error");
            }
        }
    }
}