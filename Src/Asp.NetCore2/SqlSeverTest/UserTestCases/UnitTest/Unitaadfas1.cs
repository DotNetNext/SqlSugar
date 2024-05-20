using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSeverTest.UserTestCases.Models;

namespace OrmTest
{
    internal class Unitaadfas1
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(typeof(Test1), typeof(Test2), typeof(Test3));
            db.DbMaintenance.TruncateTable(typeof(Test1), typeof(Test2), typeof(Test3));
            db.InsertNav(new Test1()
            {
                GuId = 1,
                TopId = 1,
                Test2 = new Test2()
                {
                    TopId = 2,
                    GuId = 22,
                    Test3 = new Test3()
                    {
                        TopId = 33,
                        GuId = 34
                    }
                }
            }).Include(x=>x.Test2).ThenInclude(x=>x.Test3).ExecuteCommand();
            var list=db.Queryable<Test1>().Includes(x => x.Test2.Test3).ToList();
            var list2 = db.Queryable<Test1>().Includes(x => x.Test2.Test3)
                .Select(it=>new Test1dto
                {
                    Test3 = it.Test2.Test3,
                    Guid=it.GuId
                }).ToList();
            if (db.Utilities.SerializeObject(list2.First().Test3) != db.Utilities.SerializeObject(list.First().Test2.Test3)) 
            {
                throw new Exception("unit error");
            }
        }

        [SugarTable("UnitTest1111")]
        public partial class Test1
        {
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            [Column("GuId")]
            [Key]
            public long GuId { get; set; }
            public long TopId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(TopId))]
            public Test2 Test2 { get; set; }

        }
        [SugarTable("UnitTest21111")]
        public partial class Test2
        {
            /// <summary>
            ///  
            ///</summary>
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            [Column("GuId")]
            [Key]
            public long GuId { get; set; }
            public long TopId { get; set; }
            [Navigate(NavigateType.OneToOne, nameof(TopId))]
            public Test3 Test3 { get; set; }

        }
        [SugarTable("UnitTest31111")]
        public partial class Test3
        {
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            [Column("GuId")]
            [Key]
            public long GuId { get; set; }
            public long TopId { get; set; }

        }
    }
}
