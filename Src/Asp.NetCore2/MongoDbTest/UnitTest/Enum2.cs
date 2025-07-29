using SqlSugar;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    internal class Enum2
    {
        public static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<Student>();
            db.DbMaintenance.TruncateTable<Student>();
            db.Insertable(new Student() { type = DbType.Tidb }).ExecuteCommand();
            if (db.Queryable<Student>().First().type != DbType.Tidb) Cases.Init();
            if (db.Queryable<Student>().Where(s=>s.type==DbType.Tidb).First().type != DbType.Tidb) Cases.Init();
        }
        [SqlSugar.SugarTable("UnitStudentadsdujj3z1")]
        public class Student : MongoDbBase
        {
            //存储string枚举
            [SugarColumn(SqlParameterDbType =typeof(SqlSugar.DbConvert.EnumToStringConvert))]
            public DbType type { get; set; } 
        }
    }
}
