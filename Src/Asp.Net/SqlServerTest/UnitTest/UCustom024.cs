using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustom024
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var data=db.EntityMaintenance.GetEntityInfoNoCache(typeof(UnitNewP));
            db.CodeFirst.InitTables<UnitNewP>();
            db.Insertable(new UnitNewP() { Id = Guid.NewGuid() }).ExecuteCommand();
            var list=db.Queryable<UnitNewP>().Take(2).ToList();
            db.Updateable(list).ExecuteCommand();
            db.Deleteable(list).ExecuteCommand();
            var p = new TestModel2();
            db.Queryable<Order>().Where(z => z.Id != p.Id).ToList();
            var p2 = new Order();
            db.Queryable<Order>().Where(z => z.Id != p2.Id).ToList();
            db.CurrentConnectionConfig.ConfigureExternalServices = new SqlSugar.ConfigureExternalServices()
            {
                 SqlFuncServices=new List<SqlSugar.SqlFuncExternal>() 
                 {
                     new SqlSugar.SqlFuncExternal()
                     {
                       UniqueMethodName = "MyToString",
                        MethodValue = (expInfo, dbType, expContext) =>
                        {
                           return "1";
                        }
                     }
                 }
            };
           var sql2= db.Queryable<Order>()
                .Select(it => new
                {
                    name= MyToString()
                }).ToSql();

            if (sql2.Key != "SELECT  1 AS name  FROM [Order] ") 
            {
                throw new Exception("unit error");
            }
            var conditions = new List<IConditionalModel>() { new ConditionalModel { FieldName = "name", FieldValue = "x", ConditionalType = ConditionalType.Equal } };
            var conditions2 = new List<IConditionalModel>() { new ConditionalModel { FieldName = "name", FieldValue = "x2", ConditionalType = ConditionalType.Equal } };
            var list2=db.Queryable<Order>()
                .Where(conditions).MergeTable()
                .Where(conditions2).ToList();
        }

        public static object MyToString() { return null; } 
       
    }

    public class TestModel
    {
        public virtual long? Id { get; set; }
    }

    public class TestModel2 : TestModel
    {
        // 关键就是这个地方
        public new long Id { get; set; }
    }
    public class UnitNewP: UnitBaseP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public new Guid Id { get; set; }
        [SqlSugar.SugarColumn(IsNullable =true)]
        public string Name { get; set; }    
    }
    public class UnitBaseP 
    {
        public int Id { get; set; }
    }
}
