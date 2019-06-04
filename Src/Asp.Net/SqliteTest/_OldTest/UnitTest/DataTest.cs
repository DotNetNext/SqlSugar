using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.UnitTest
{
    public class DataTest : UnitTestBase
    {
        private DataTest() { }
        public DataTest(int eachCount)
        {
            this.Count = eachCount;
        }

        public void Init()
        {
            var guid = Guid.Parse("d8268a7e-a2d1-4329-9990-9bd801292415");
            var db = GetInstance();
            db.DbMaintenance.TruncateTable("DataTestInfo");
            var insertObject = new DataTestInfo()
            {
                Datetime1 = DateTime.Now,
                Datetime2 = DateTime.Now,
                Decimal1 = 1,
                Decimal2 = 2,
                Float1 = 3,
                Float2 = 4,
                Guid1 = guid,
                Guid2 = guid,
                Image1 = new byte[] { 1, 2 },
                Image2 = new byte[] { 2, 3 },
                Int2 = 6,
                Money1 = Convert.ToDecimal(7.1),
                Money2 = 8,
                Varbinary1 = new byte[] { 4, 5 },
                Varbinary2 = null,
                String = "string",
                Long1=100
            };
  
            var id = db.Insertable<DataTestInfo>(insertObject).ExecuteReturnIdentity();
            var x = db.Queryable<DataTestInfo2>().Select(it => it.PK).ToList();
            var x2 = db.Queryable<DataTestInfo>().Select(it => it.Money1).ToList();
            var data = db.Queryable<DataTestInfo>().InSingle(id);
            if (
                insertObject.Datetime1.ToString("yyyy-MM-dd") != data.Datetime1.ToString("yyyy-MM-dd") ||
                insertObject.Decimal1 != data.Decimal1 ||
                insertObject.Float1 != data.Float1 ||
                insertObject.Float2 != data.Float2 ||
                insertObject.Int2 != data.Int2 ||
                insertObject.Money1 != data.Money1 ||
                insertObject.Long1!=data.Long1||
                insertObject.Guid2!=data.Guid2||
               string.Join(",", insertObject.Varbinary1) != string.Join(",", data.Varbinary1) ||
                insertObject.String != data.String)
            {
                throw new Exception("DataTest Error");
            }
            data.Float1= data.Float1+1;
            db.Updateable(data).ExecuteCommand();
            data = db.Queryable<DataTestInfo>().InSingle(id);
            if (
                insertObject.Datetime1.ToString("yyyy-MM-dd") != data.Datetime1.ToString("yyyy-MM-dd") ||
                insertObject.Decimal1 != data.Decimal1 ||
                (insertObject.Float1+1) != data.Float1 ||
                insertObject.Float2 != data.Float2 ||
                insertObject.Int2 != data.Int2 ||
                insertObject.Money1 != data.Money1 ||
            string.Join(",", insertObject.Varbinary1) != string.Join(",", data.Varbinary1) ||
                insertObject.String != data.String)
            {
                throw new Exception("DataTest Error");
            }
        }
        public SqlSugarClient GetInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = Config.ConnectionString, DbType = DbType.Sqlite, IsAutoCloseConnection = true });
            return db;
        }
    }
}
