using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitadfasdysdfa
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(new Type[] { typeof(TestSQLVariant) });
            db.DbMaintenance.TruncateTable<TestSQLVariant>();
            TestSQLVariant testSQLVariant = new TestSQLVariant();
            testSQLVariant.Value = "-0.01~0.05";
            testSQLVariant.Id = 1;
            TestSQLVariant testSQLVariant1 = new TestSQLVariant();
            testSQLVariant1.Value = "false";
            testSQLVariant1.Id = 2;
            TestSQLVariant testSQLVariant2 = new TestSQLVariant();
            testSQLVariant2.Value = true;

            db.Insertable(new List<TestSQLVariant>() { testSQLVariant, testSQLVariant1, testSQLVariant2 }).ExecuteCommand();
            var list = db.Queryable<TestSQLVariant>().ToList();

        }

        public class TestSQLVariant
        {
            [SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            [SugarColumn(ColumnDataType = "sql_variant", IsNullable = true, SqlParameterDbType = typeof(SqlVariantConvert))]
            public object Value { get; set; }

        }

        //用例1：
        public class SqlVariantConvert : ISugarDataConverter
        {
            public SugarParameter ParameterConverter<T>(object value, int i)
            { 
                var name = "@myp" + i; 
                 
                return new SugarParameter(name, value) { CustomDbType=SqlDbType.Variant };
            }

            public T QueryConverter<T>(IDataRecord dr, int i)
            { 
                var str = dr.GetValue(i) + "";

                return (T)(object)str;
            }
        }
    }
}
