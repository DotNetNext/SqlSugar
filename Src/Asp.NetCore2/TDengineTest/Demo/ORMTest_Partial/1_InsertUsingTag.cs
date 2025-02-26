using SqlSugar;
using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void InsertUsingTag(SqlSugarClient db) 
        { 
            db.CodeFirst.InitTables<SUsingTagModel>();
        }
        [STableAttribute(STableName = "SUsingTagModel", Tag1 = nameof(Tag1))]
        public class SUsingTagModel
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public DateTime Ts { get; set; }
            public bool Boolean { get; set; }
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public string Tag1 { get; set; }
        }
    }
}