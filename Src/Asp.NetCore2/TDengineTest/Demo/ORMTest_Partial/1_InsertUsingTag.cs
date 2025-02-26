using SqlSugar;
using SqlSugar.TDengine;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void InsertUsingTag(SqlSugarClient db) 
        { 
            //创建超级表扔在程序初始话的地方
           db.CodeFirst.InitTables<SUsingTagModel>();
 
            //插入并根据Tag的值创建子表
            db.CopyNew().Insertable(new List<SUsingTagModel>(){
                new SUsingTagModel()
                {
                        Boolean = true,
                        Tag1 = "a",
                        Ts = DateTime.Now.AddMilliseconds(1)//没启用微秒纳秒时间不能一样

                 },
                 new SUsingTagModel()
                {
                        Boolean = false,
                        Tag1 = "a",
                        Ts = DateTime.Now.AddMilliseconds(2)

                 },
                 new SUsingTagModel()
                 {
                        Boolean = true,
                        Tag1 = "b",
                        Ts = DateTime.Now.AddMilliseconds(3)
                }})
           .SetTDengineChildTableName((stableName, it) => $"{stableName}_{it.Tag1}" /*设置子表名字*/ )
           .ExecuteCommand();

            var list1=db.Queryable<SUsingTagModel>().ToList();

            //查询子表A
            var tagA = db.Queryable<SUsingTagModel>().Where(it=>it.Tag1=="a").ToList();

            db.Deleteable<SUsingTagModel>().Where(it => it.Ts > Convert.ToDateTime("2020-1-1")).ExecuteCommand();

        }
        [STableAttribute(STableName = "SUsingTagModel", Tag1 = nameof(Tag1))]
        public class SUsingTagModel
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public DateTime Ts { get; set; }
            public bool Boolean { get; set; } 
            public string Tag1 { get; set; }
        }
    }
}