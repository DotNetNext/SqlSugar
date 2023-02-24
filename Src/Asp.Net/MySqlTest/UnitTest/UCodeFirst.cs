using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void CodeFirst()
        {
            if (Db.DbMaintenance.IsAnyTable("UnitCodeTest1", false))
                Db.DbMaintenance.DropTable("UnitCodeTest1");
            Db.CodeFirst.InitTables<UnitCodeTest1>();
            Db.CodeFirst.InitTables<UnitCodeTest2222>();
            Db.Insertable(new UnitCodeTest2222()
            {
                Id = 1,
                Id2 = 2,
                Id3 = 3,
                Id4 = 4
            }).ExecuteCommand();
            var list = Db.Queryable<UnitCodeTest2222>().ToList();
            Db.CodeFirst.InitTables<UnitCodeTest2a2c22>();
            Db.Insertable(new UnitCodeTest2a2c22()
            {
                a = 1,
                b = new byte[] { 1, 2, 3 }
            })
            .ExecuteCommand();
            var xx = Db.Queryable<UnitCodeTest2a2c22>().Select(it => new
            {
                id = it.a,
                b = it.b
            }).ToList();
            Db.CodeFirst.InitTables<UnitTest012213>();
            Db.CodeFirst.InitTables<UnitTest3131>();
            Db.CodeFirst.InitTables<UnitDateOfTime2>();
            Db.CodeFirst.InitTables<UnitDateOfTime222>();
            Db.Insertable(new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now }).ExecuteCommand();
            Db.Insertable(new List<UnitDateOfTime2> { new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now }, new UnitDateOfTime2() { DateTimeOffset1 = DateTimeOffset.Now } }).ExecuteCommand();
            var list2 = Db.Queryable<UnitDateOfTime2>().ToList();
            Db.Insertable(new UnitDateOfTime222() { DateTimeOffset1 = null }).ExecuteCommand();
            if (Db.DbMaintenance.IsAnyTable("Unit1afa113111"))
                Db.DbMaintenance.DropTable("Unit1afa113111");
            Db.CodeFirst.InitTables<Unit1afa113111>();
            var entity = Db.Insertable(new Unit1afa113111() { Name = "a" }).ExecuteReturnEntity();
            if (Db.DbMaintenance.IsAnyTable("UnitCodeFirst131", false))
                Db.DbMaintenance.DropTable("UnitCodeFirst131");
            Db.CodeFirst.InitTables<UnitCodeFirst131>();
            Db.Insertable(new UnitCodeFirst131() { Id = 1 }).ExecuteCommand();
            Db.CodeFirst.InitTables<UNITCODEFIRST131>();
            Db.CodeFirst.InitTables<UNITCOdEFIRST131>();
            Db.CodeFirst.InitTables<UnitTest0123>();
            if (Db.DbMaintenance.GetColumnInfosByTableName("UnitTest0123", false).First().DbColumnName != "Id")
            {
                throw new Exception("unit error");
            }
            Db.CodeFirst.InitTables<UnitUint>();
            Db.Insertable(new UnitUint { Id = 3833200526 }).ExecuteCommand();
            var list3 = Db.Queryable<UnitUint>().ToList();
            Db.CodeFirst.InitTables<UnitBooll1>();
            Db.Insertable(new UnitBooll1() { id = true }).ExecuteCommand();
            Db.Insertable(new UnitBooll1() { id = false }).ExecuteCommand();
            var list4 = Db.Queryable<UnitBooll1>().ToList();
            Db.DbMaintenance.DropTable<UnitBooll1>();
            if (!list4.Any(it => it.id) || !list4.Any(it => !it.id))
                throw new Exception("unit error");
            Db.CodeFirst.InitTables<UnitTestint>();
            var id = Db.Insertable(new UnitTestint()
            {
                xx = 1
            }).ExecuteReturnIdentity();
            var pass = true;
            Db.Updateable<UnitTestint>()
                .SetColumns(x => x.xx , (pass ? 1 : 0))
                .Where(z => z.id == id).ExecuteCommand();
        }

        public class UnitTestint
        {
            [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int id { get; set; }
            public int xx { get; set; }
        }

        public class UnitBooll1 
        {
            public bool id { get; set; }
        }
        public class UnitUint 
        {
            [SugarColumn(ColumnDataType = "int unsigned")]
            public uint Id { get; set; }
        }

        [SugarTable("UnitTest0123", IsCreateTableFiledSort= true)]
        public class UnitTest0123
        {
            public string Name { get; set; }
            [SugarColumn(CreateTableFieldSort =-1)]
            public int Id { get; set; }
        }

        public class UnitCodeFirst131
        {
            public int Id { get; set; }
        }
        [SugarTable("UnitCodeFirst131")]
        public class UNITCODEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "a")]
            public string Name { get; set; }
        }
        [SugarTable("UnitCodeFirst131")]
        public class UNITCOdEFIRST131
        {
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "a")]
            public string Name { get; set; }
            [SqlSugar.SugarColumn(DefaultValue = "0")]
            public int dt { get; set; }
        }

        public class Unit1afa113111 
        {
            [SugarColumn(ColumnName ="aid",IsPrimaryKey =true,IsIdentity =true,ColumnDataType ="int(11)unsigned")]
            public uint id { get; set; }
            public string Name { get; set; }
        }
        public class UnitDateOfTime2
        {
            [SqlSugar.SugarColumn(ColumnDataType ="datetime(3)")]
            public DateTimeOffset DateTimeOffset1 { get; set; }
        }
        public class UnitDateOfTime222
        {
            [SqlSugar.SugarColumn(ColumnDataType = "datetime(3)",IsNullable =true)]
            public DateTimeOffset? DateTimeOffset1 { get; set; }
        }
        public class UnitTest3131 
        {
            public sbyte Id { get; set; }
        }
        public class UnitTest012213
        {

            [SugarColumn(ColumnDataType = "image,longblob")]
            public byte[] x { get; set; }
        }
        public class UnitCodeTest1
        {
            [SqlSugar.SugarColumn(IndexGroupNameList = new string[] { "group1" })]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(DefaultValue="now()", IndexGroupNameList =new string[] {"group1" } )]
            public DateTime? CreateDate { get; set; }
        }

        public class UnitCodeTest2a2c22
        {
            public int a { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType ="blob")]
            public byte[] b { get; set; }
        }

        public class UnitCodeTest2222
        {
            public uint Id { get; set; }
            public ulong Id2 { get; set; }
            public ushort Id3 { get; set; }
            public uint? Id4 { get; set; }
        }
    }
}
