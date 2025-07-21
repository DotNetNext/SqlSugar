using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    internal class Enum
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { type = DbType.Tidb }).ExecuteCommand();
            if (db.Queryable<Student>().First().type != DbType.Tidb) Cases.Init();
        }
        [SqlSugar.SugarTable("UnitStudentads992ds3z1")]
        public class Student : MongoDbBase
        {
            public DbType type { get; set; }

        }
    }
}
