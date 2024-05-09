using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class UnitCreateType
    {
        public static void Init()
        {

            Console.WriteLine("");
            Console.WriteLine("#### CreateType Start ####");


            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            }, db =>
            {
                //单例参数配置，所有上下文生效
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
 
                    //Console.WriteLine(sql);//输出sql
                };
            });
             
            List<Guid> ids = new List<Guid>();
            for (int i = 0; i < 100; i++)
            {
                var guids = db.Queryable<Order>().Select(it =>
                 SqlFunc.NewUid()
                ).Take(10).ToList(); ;
                ids.AddRange(guids);
            }
            var count= ids.Distinct().Count();
            if (count != ids.Count) 
            {
                throw new Exception("unit error");
            }
            db.DbMaintenance.CreateDatabase();
            var type = db.DynamicBuilder().CreateClass("UnitEntityA",
             new SugarTable()
             {
                 TableDescription = "表备注",
                 //DisabledUpdateAll=true 可以禁止更新只创建
             })


         .CreateProperty("name", Type.GetType("System.String"),
         new SugarColumn()
         {
             IsIgnore = false,
             IsPrimaryKey = false,
             IsIdentity = false,
             ColumnDescription = "",
             Length = 255,
             IsNullable = false,
             OldColumnName = null,
             ColumnDataType = "varchar",
             DecimalDigits = 0,
             OracleSequenceName = null,
             IsOnlyIgnoreInsert = false,
             IsOnlyIgnoreUpdate = false,
             IsJson = false,
             DefaultValue = "",
             IsArray = false,
             SqlParameterDbType = null,
             SqlParameterSize = null
         })
        .BuilderType();

            if (db.DbMaintenance.IsAnyTable("UnitEntityA", false))
            {
                db.DbMaintenance.DropTable("UnitEntityA");
            }
            db.CodeFirst.InitTables(type);


            var type2 = db.DynamicBuilder().CreateClass("UnitEntityA",
           new SugarTable()
           {
               TableDescription = "表备注",
               //DisabledUpdateAll=true 可以禁止更新只创建
           })
           .CreateProperty("Id", typeof(string), new SugarColumn() { IsPrimaryKey = true, ColumnDescription = "列备注" }).BuilderType();

            db.CodeFirst.InitTables(type2);

            var data=db.DbMaintenance.GetColumnInfosByTableName("UnitEntityA",false);

            if (data[0].IsPrimarykey == false) throw new Exception("unit error");

            Console.WriteLine("#### DemoE_CreateType end ####");
        }
    }
}
