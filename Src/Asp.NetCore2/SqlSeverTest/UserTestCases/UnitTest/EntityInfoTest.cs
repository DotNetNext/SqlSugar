using SqlSeverTest.UserTestCases;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;

namespace OrmTest
{
    internal class EntityInfoTest
    {
        public static void Init()
        {
            for (int i = 0; i < 100; i++)
            {
                var db = new SqlSugarClient(new List<ConnectionConfig>()
            {
             //这儿声名所有上下文都生效
             new ConnectionConfig(){ConfigId="0",
             ConfigureExternalServices=new ConfigureExternalServices(){
              EntityService=(x,y)=>{ y.IsPrimarykey=true; }
             },
                 DbType=DbType.SqlServer,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true},
             new ConnectionConfig(){ConfigId="1",
               ConfigureExternalServices=new ConfigureExternalServices(){
              EntityService=(x,y)=>{ y.IsIdentity=true; }
             },DbType=DbType.SqlServer,ConnectionString=Config.ConnectionString,IsAutoCloseConnection=true  }
            });

                if (db.GetConnection("0").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != false
                    || db.GetConnection("0").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != true)
                {
                    throw new Exception("unit error");
                }

                if (db.GetConnection("1").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != true
               || db.GetConnection("1").EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != false)
                {
                    throw new Exception("unit error");
                }

                if (db.EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsIdentity != false
                   || db.EntityMaintenance.GetEntityInfo<classTest>().Columns.First().IsPrimarykey != true)
                {
                    throw new Exception("unit error");
                }
                db = NewUnitTest.Db;

                db.CodeFirst.InitTables<Unitadfafa11>();
                db.Insertable(new Unitadfafa11() { dob = 1.11 }).ExecuteCommand();
                db.Queryable<Unitadfafa11>()
                    .Select(IT => new Unitadfafa1DTO()
                    {
                         dob=(IT.dob??0).ToString("0.0")
                    }).ToList();

                var sql = db.Queryable<Order>()
               .Select(it => new {
                   //num=SqlFunc.Subqueryable<OrderItem>().Where(s=>s.OrderId==it.Id).Sum(s=>s.Price)??0,
                   //num2 =SqlFunc.IsNull( SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId == it.Id).Sum(s => s.Price),0) ,
                   num3 = SqlFunc.Subqueryable<OrderItem>().Where(s => s.OrderId == it.Id).Sum(s => s.Price) + it.Id
               }).ToSqlString();
                if (sql != "SELECT  ((SELECT SUM([Price]) FROM [OrderDetail] [s]  WHERE ( [OrderId] = [it].[Id] )) + [it].[Id] ) AS [num3]  FROM [Order] [it] ") 
                {
                    throw new Exception("unit error");
                }
                db.Queryable<Order>().OrderBy(it => SqlFunc.Desc(it.Id)).ToList();
            }
        }
    }
    public class Unitadfafa1DTO
    {
        [SugarColumn(ColumnDataType = "double")]
        public string dob { get; set; }
    }
    public class Unitadfafa11
    {
        [SugarColumn(ColumnDataType = "float", IsNullable =true)]
        public double? dob { get; set; }
    }
    public class classTest 
    {
        public string Id { get; set; }
    }
}
