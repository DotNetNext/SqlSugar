using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            db.CodeFirst.InitTables<CodeFirstEnum>();
            db.DbMaintenance.TruncateTable<CodeFirstEnum>();
            db.Storageable(new CodeFirstEnum() { dbType = DbType.Access, Name = "a" }).ExecuteCommand();
            db.CodeFirst.InitTables<CodeFirstArray>();
            db.Insertable(new List<CodeFirstArray>() { 
                new CodeFirstArray() {   floats = new float[] { 1 } },
                   new CodeFirstArray() {   floats = new float[] { 1 } }

            }).ExecuteCommand();
            var list5=db.Queryable<CodeFirstArray>().ToList();
            db.CodeFirst.InitTables<CodeFirstArraryBigInt>();
            db.Updateable<CodeFirstArraryBigInt>()
                .SetColumns(it => it.longs == new long[] { 1, 2 })
                .Where(it=>it.id==1)
                .ExecuteCommand();
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s);
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                PgSqlIsAutoToLowerCodeFirst = false,
                PgSqlIsAutoToLower=false
            };
            db.CodeFirst.InitTables<CodeFirstNoUpper>();
            db.Insertable(new CodeFirstNoUpper() { Id = Guid.NewGuid() + "", Name = "a" }).ExecuteCommand();
            var list6= db.Queryable<CodeFirstNoUpper>().Where(it => it.Id != null).ToList();
            db.Updateable(list6).ExecuteCommand();
            db.Deleteable(list6).ExecuteCommand();
            db.Updateable(list6.First()).ExecuteCommand();
            db.Deleteable<CodeFirstNoUpper>().Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper>().SetColumns(it => it.Name == "a").Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper>().SetColumns(it => new CodeFirstNoUpper()
            {
                Name = "a"
            }).Where(it => it.Id != null).ExecuteCommand();
            db.Insertable(new CodeFirstNoUpper() { Name = "a",Id="1" }).ExecuteReturnIdentity();
            db.Insertable(new CodeFirstNoUpper() { Name = "a" ,Id="2"}).AddQueue();
            db.SaveQueues();
            db.CodeFirst.InitTables<CodeFirstChartest>();
            db.Queryable<CodeFirstChartest>().Where(it => it.Test == 's').ToList();
            db.CodeFirst.InitTables<CodeFloatddfa1a1>();
            db.Insertable(new CodeFloatddfa1a1() { xx = (float)11.1 }).ExecuteCommand();
            var list7 = db.Queryable<CodeFloatddfa1a1>().ToList();
            db.CodeFirst.InitTables<CodeFloatddfa1a2>();
            db.Insertable(new CodeFloatddfa1a2() { xx = DateTimeOffset.Now }).ExecuteCommand();
            db.Insertable(new List<CodeFloatddfa1a2> { new CodeFloatddfa1a2() { xx = DateTimeOffset.Now }, new CodeFloatddfa1a2() { xx = DateTimeOffset.Now } }).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFloatddfa1a2
    {
        public DateTimeOffset xx { get; set; }
    }
    public class CodeFloatddfa1a1
    {
        public float xx { get; set; }
    }
    public class CodeFirstChartest 
    {
        [SugarColumn(ColumnDataType ="varchar(1)")]
        public char Test { get; set; }
    }
    [SugarTable(null,"备注表")]
    public class CodeFirstNoUpper
    {
        [SugarColumn(IsPrimaryKey = true, ColumnDescription ="备注列")]
        public string Id { get; set; }
        public string Name { get; set; }
    }
    public class CodeFirstArraryBigInt 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int id { get; set; }
        [SugarColumn(IsArray =true,ColumnDataType ="int8 []")]
        public long[] longs { get; set; }
    }
    public class CodeFirstArray 
    {
        [SugarColumn(IsArray =true,ColumnDataType = "real[]")]
        public float[] floats { get; set; }
    }
    public class CodeFirstEnum 
    {
        [SugarColumn(IsPrimaryKey =true)]
        public DbType dbType { get; set; }
        public string Name { get; set; }
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
