using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace OrmTest
{
    public class DemoE_CodeFirst
    {
        public static void Init()
        {
            Console.WriteLine("");
            Console.WriteLine("#### CodeFirst Start ####");
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = DbType.PostgreSQL,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables<CodeFirstunitea>();
            db.Insertable(new CodeFirstunitea()
            {
                id2 = null
            }).ExecuteCommand();
            db.Insertable(new CodeFirstunitea()
            {
                id2 = 1
            }).ExecuteCommand();
            db.Updateable<CodeFirstunitea>().SetColumns(it => new CodeFirstunitea()
            {
                id2 = 1
            }).Where(it=>it.id>0).ExecuteCommand();
            db.Updateable<CodeFirstunitea>().SetColumns(it => new CodeFirstunitea()
            {
                id2 = null
            }).Where(it => it.id > 0).ExecuteCommand();
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<CodeFirstChar2>();
            db.Insertable(new CodeFirstChar2() { CharTest = '1' }).ExecuteCommand();
            var list2=db.Queryable<CodeFirstChar2>().ToList();
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(UtilMethods.GetNativeSql(s, p));;
            db.CodeFirst.InitTables<CODEFIRSTSAF>();
            db.CodeFirst.InitTables<CodeFirstsaf>();
            db.Insertable(new CodeFirstsaf() { Json = "a" })
                .ExecuteCommand();
            var list3=db.Queryable<CodeFirstsaf>().ToList();
            db.CodeFirst.InitTables<CodeFirstadfafaa>();
            db.DbMaintenance.TruncateTable<CodeFirstadfafaa>();
            db.Insertable(new CodeFirstadfafaa() { Id = 1, Name = "a" }).ExecuteCommand();
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add("id", 1);
            result.Add("name", "jack");
            result.Add("price", null);
            db.Updateable(result).AS("CodeFirstadfafaa").WhereColumns("id").ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    [SugarTable("CodeFirstadfafaaxx")]
    public class CodeFirstadfafaa 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(IsNullable =true)]
        public int Price { get; set; }
    }
    public class CodeFirstunitea 
    {
        [SugarColumn(IsIdentity =true,IsPrimaryKey =true)]
        public uint id { get; set; }
        [SugarColumn(IsNullable =true)]
        public uint? id2 { get; set; }
    }
    public class CodeFirstsaf 
    {
        [SugarColumn(DefaultValue ="")]
        public string Json { get; set; }
        [SugarColumn(DefaultValue = "1")]
        public bool x { get; set; }
    }
    public class CODEFIRSTSAF
    {
        [SugarColumn(DefaultValue = "a")]
        public string Json { get; set; }
    }
    public class CodeFirstChar2 
    {
        [SqlSugar.SugarColumn(ColumnDataType ="varchar(1)")]
        public char CharTest { get; set; }
    }
    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "varchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
