using SqlSugar;
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
                DbType = DbType.PostgreSQL,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.DbMaintenance.CreateDatabase(); 
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<CodeFirstArrary>();
            db.Insertable(new CodeFirstArrary()
            {
                 RoleIds=new long[] { 1,2}
            }).ExecuteCommand();
            var list2=db.Queryable<CodeFirstArrary>().Select(it => 
            it.RoleIds  ).First();
            var list3= db.Queryable<CodeFirstArrary>().Select(it =>
            it.RoleIds).FirstAsync().GetAwaiter().GetResult();
            db.CodeFirst.InitTables<CodeFirstByte>();
            db.Insertable(new CodeFirstByte() { array = new byte[] { 1, 2, 4, 5 } }).ExecuteCommand();
            var list4=db.Queryable<CodeFirstByte>().ToList();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFirstByte 
    {
        public byte[] array { get; set; }
    }
    public class CodeFirstArrary 
    {
        [SugarColumn(ColumnDescription = "绑定的角色ids", ColumnDataType = "int8[]", IsArray = true)]
        public long[] RoleIds { get; set; }
    }
    [SugarIndex("{table}idindex",nameof(Id),OrderByType.Asc)]
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
