using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    internal class UEnum2
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<Unitadfas3>();
            db.Insertable(new Unitadfas3() { DbType = SqlSugar.DbType.GBase }).ExecuteCommand();
            var first = db.Queryable<Unitadfas3>().First();
            db.Updateable(first).ExecuteCommand();
            var first2 = db.Queryable<Unitadfas3>().First();

        }
        public class Unitadfas3 
        {
            [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int id { get; set; }
            [SugarColumn(ColumnDataType ="varchar(20)",SqlParameterDbType =typeof(EnumToStringConvert))]
            public SqlSugar.DbType DbType { get; set; }
        }
    }
}
