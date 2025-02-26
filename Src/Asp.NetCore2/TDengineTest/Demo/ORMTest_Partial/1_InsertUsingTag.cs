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

            db.Insertable(new List<SUsingTagModel>(){
                new SUsingTagModel()
                {
                        Boolean = true,
                        Tag1 = "a",
                        Ts = DateTime.Now

                 },
                 new SUsingTagModel()
                {
                        Boolean = false,
                        Tag1 = "a",
                        Ts = DateTime.Now

                 },
                 new SUsingTagModel()
                 {
                        Boolean = true,
                        Tag1 = "b",
                        Ts = DateTime.Now
                }})
           .SetTDengineChildTableName((stableName, it) => $"{stableName}_{it.Tag1}" /*设置子表名字*/ )
           .ExecuteCommand(); 
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