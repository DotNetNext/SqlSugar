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
                DbType = DbType.Oracle,
                ConnectionString = Config.ConnectionString3,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true
            });
            db.CodeFirst.InitTables(typeof(CodeFirstTable1));//Create CodeFirstTable1 
            db.Insertable(new CodeFirstTable1() { Name = "a", Text="a" }).ExecuteCommand();
            var list = db.Queryable<CodeFirstTable1>().ToList();
            db.CodeFirst.InitTables<PictureData>();
            db.CodeFirst.InitTables<PictureData>();
            db.CodeFirst.InitTables<EnumTypeClass>();
            db.Insertable(new EnumTypeClass() { enumType= EnumType.x1, SerialNo= Guid.NewGuid() + "" }).ExecuteCommand();
            var list2=db.Queryable<EnumTypeClass>().Select(x => new EnumTypeClass()
            {
                enumType =  x.enumType ,
                SerialNo = x.SerialNo
            }).ToList();
            db.Aop.OnLogExecuting = (s, p) => Console.WriteLine(s);
            db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings()
            {
                IsAutoToUpper = false
            };
            db.CodeFirst.InitTables<CodeFirstNoUpper>();
            db.Insertable(new CodeFirstNoUpper() { Id = Guid.NewGuid() + "", Name = "a" }).ExecuteCommand();
            var list3 = db.Queryable<CodeFirstNoUpper>().Where(it => it.Id != null).ToList();
            db.Updateable(list3).ExecuteCommand();
            db.Deleteable(list3).ExecuteCommand();
            db.Updateable(list3.First()).ExecuteCommand();
            db.Deleteable<CodeFirstNoUpper>().Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper>().SetColumns(it => it.Name == "a").Where(it => it.Id != null).ExecuteCommand();
            db.Updateable<CodeFirstNoUpper>().SetColumns(it => new CodeFirstNoUpper()
            {
                Name = "a"
            }).Where(it => it.Id != null).ExecuteCommand();
            Console.WriteLine("#### CodeFirst end ####");
        }
    }
    public class CodeFirstNoUpper
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class EnumTypeClass 
    {
        [SugarColumn(IsPrimaryKey = true,Length =50)]
        public string SerialNo { get; set; }
        [SugarColumn(ColumnDataType ="number(22,0)")]
        public EnumType enumType { get; set; }
    }
    public enum EnumType { 
       x1=-1,
       x2=2
    }

    [SugarTable("PictureData")]
    public class PictureData
    {
        [SugarColumn(IsPrimaryKey = true)]
        public string SampleNo { get; set; }

        [SugarColumn(IsPrimaryKey = true)]
        public int SerialNo { get; set; }
        public byte[] Data { get; set; }
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
