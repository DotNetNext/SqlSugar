﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
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
                DbType = DbType.SqlServer,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.Aop.OnLogExecuted = (s, p) =>
            {
                Console.WriteLine(s);
            };
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.DbMaintenance.TruncateTable<CodeFirstTable1>();
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a",CreateTime=DateTime.Now }).ExecuteCommand();
            db.Insertable(new CodeFirstTable1() { Name = "a", Text = "a", CreateTime = DateTime.Now.AddDays(1) }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<UnituLong>();
            db.Insertable(new UnituLong() { longx = 1 }).ExecuteCommand();
            var ulList=db.Queryable<UnituLong>().Where(x => x.longx > 0).ToList();
            db.CodeFirst.As<UnituLong>("UnituLong0011").InitTables<UnituLong>();
            Console.WriteLine("#### CodeFirst end ####");
            db.CodeFirst.InitTables<Unituadfasf1>();
            db.CodeFirst.InitTables<CodeFirstTable111>();
            db.Queryable<CodeFirstTable111>()
                .Select(it => new CodeFirstTable111()
                {
                    Name = it.CreateTime.HasValue ? it.CreateTime.Value.ToString("yyyy-MM-dd") : string.Empty
                }).ToList();
            db.CodeFirst.InitTables<CodeFirstimg>();
            if (db.DbMaintenance.IsAnyTable("CodeFirstMaxString1",false))
                db.DbMaintenance.DropTable<CodeFirstMaxString1>();
            db.CodeFirst.InitTables<CodeFirstMaxString1>();


            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (s, p) => 
                {
                    p.IfTable<UnitIFTable>()
                    .UpdateProperty(it=>it.id,it =>
                    {
                        it.IsIdentity = true;
                        it.IsPrimarykey= true;
                    })
                    .UpdateProperty(it => it.Name, it=>{
                        it.Length = 100;
                        it.IsNullable = true;
                        
                    })
                    .OneToOne(it=>it.UnitIFTableInfo,nameof(UnitIFTable.id));
                }
            };
            db.CodeFirst.InitTables<UnitIFTable>();
            db.CodeFirst.InitTables<Unittest1011, Unittest22221>();
            db.Insertable(new Unittest1011() { name = "a" }).ExecuteCommand();
            db.Insertable(new Unittest22221() { name = "a" }).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class Unittest22221
    {
        [SugarColumn(DefaultValue = " newsequentialid()")]
        public Guid id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }
    }
    public class Unittest1011
    {
        [SugarColumn(DefaultValue ="newid()")]
        public Guid id { get; set; }
        [SugarColumn(IsNullable =true)]
        public string name { get; set; }
    }
    public class UnitIFTable 
    {
        //[SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
        public int id { get; set; }
       // [SugarColumn(IsNullable =true)]
        public string Name { get; set; }

        public UnitIFTable UnitIFTableInfo { get; set; }
    }

    public class CodeFirstMaxString1
    {
        [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString)]
        public string img { get; set; }
    }
   
    public class CodeFirstimg 
    {
        [SugarColumn(Length =100)]
        public byte[] img { get; set; }
    }
    public class CodeFirstTable111
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime? CreateTime { get; set; }
    }
    [SugarIndex("IndexUnituadfasf1_longx{include:name,id}", nameof(longx), OrderByType.Asc)]
    public class Unituadfasf1
    {

        public ulong longx { get; set; }
        public int id { get; set; }
        public string name { get; set; }
    }

    public class UnituLong
    {

        public ulong longx{get;set;}
    }


    [SugarIndex("index_codetable1_name",nameof(CodeFirstTable1.Name),OrderByType.Asc)]
    [SugarIndex("index_codetable1_nameid", nameof(CodeFirstTable1.Name), OrderByType.Asc,nameof(CodeFirstTable1.Id),OrderByType.Desc)]
    [SugarIndex("unique_codetable1_CreateTime", nameof(CodeFirstTable1.CreateTime), OrderByType.Desc,true)]
    public class CodeFirstTable1
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [SugarColumn(ColumnDataType = "Nvarchar(255)")]//custom
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
