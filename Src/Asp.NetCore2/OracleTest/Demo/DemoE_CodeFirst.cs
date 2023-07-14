using SqlSugar;
using SqlSugar.DbConvert;
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
                DbType = DbType.Oracle,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.CodeFirst.InitTables<T_DICT>();
            db.Updateable(new T_DICT() { LABEL = "a", TYPE = "a", TYPENAME = "a", VALUE = "a" })
                .ExecuteCommand();
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(UtilMethods.GetNativeSql(s,p));
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<CodeFirstTable22x2>();
            db.Updateable(new List<CodeFirstTable22x2>() { new CodeFirstTable22x2() { Name = "a" },new CodeFirstTable22x2() { Name = "a" } })
                .ExecuteCommand();
            db.Updateable(  new CodeFirstTable22x2() { Name = "a" }  )
            .ExecuteCommand();
            db.Insertable(new List<CodeFirstTable22x2>() { new CodeFirstTable22x2() { Name = "a" }, new CodeFirstTable22x2() { Name = "a" } })
              .ExecuteCommand();
            db.Insertable(new CodeFirstTable22x2() { Name = "a"   })
             .ExecuteCommand();
            var list2=db.Queryable<CodeFirstTable22x2>().ToList();
            db.CodeFirst.InitTables<CodeFirstUnitrew>();
            db.Insertable(new CodeFirstUnitrew() { Index = 1 }).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    /// <summary>
    /// 测试表
    /// </summary>
    [SugarTable("T_DICT")]
    public class T_DICT
    {
        /// <summary>
        /// Desc:字典类型
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string TYPE { get; set; } = "";
        /// <summary>
        /// Desc:字典名称
        /// Default:
        /// Nullable:True
        /// </summary>   
        public string? TYPENAME { get; set; }
        /// <summary>
        /// Desc:字典键值
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string VALUE { get; set; } = "";
        /// <summary>
        /// Desc:字典标签
        /// Default:
        /// Nullable:False
        /// </summary>           
        public string? LABEL { get; set; }
    }
    public class CodeFirstUnitrew 
    {
        public int Index { get; set; }
    }
    [SugarTable("CodeFirstTable22r2")]
    public class CodeFirstTable22x2
    {
        [SugarColumn(OracleSequenceName = "SEQ_ID", IsPrimaryKey = true)]
        public int Id { get; set; }
        [SugarColumn( SqlParameterDbType =typeof(Nvarchar2PropertyConvert) )]
        public string Name { get; set; } 
    }
    public class CodeFirstTable1
    {
        [SugarColumn(OracleSequenceName ="SEQ_ID", IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        [SugarColumn(IsNullable = true)]
        public DateTime CreateTime { get; set; }
    }
}
