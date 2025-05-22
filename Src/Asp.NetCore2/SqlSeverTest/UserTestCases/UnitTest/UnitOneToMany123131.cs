using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{

    public class UnitOneToMany123131
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            // db.DbMaintenance.DropTable< UnitNavLv1, UnitNavLv2, UnitNavLv3_1>();
            try
            {
                db.CodeFirst.InitTables<Person, UnitNavLv1, UnitNavLv2, UnitNavLv3_1, UnitNavLv3_2>();
            }
            catch (Exception)
            {

                throw;
            }
            db.DbMaintenance.TruncateTable<Person, UnitNavLv1, UnitNavLv2, UnitNavLv3_1, UnitNavLv3_2>();

            var person = new Person
            {
                Name = "John Doe"
            };

            var lv1Data = new UnitNavLv1
            {
                Name = "Level 1",
                lv2s = new List<UnitNavLv2>
                {
                    new UnitNavLv2
                    {
                        Name = "Level 2-1",
                        person = person,
                        lv3s_1 = new List<UnitNavLv3_1>
                        {
                            new UnitNavLv3_1 { Name = "Level 3-1" },
                            new UnitNavLv3_1 { Name = "Level 3-2" }
                        },
                        lv3s_2 = new List<UnitNavLv3_2>
                        {
                            new UnitNavLv3_2 { Name = "Level 3-3" }
                        }
                    },
                    new UnitNavLv2
                    {
                        Name = "Level 2-2",
                        lv3s_1 = new List<UnitNavLv3_1>
                        {
                            new UnitNavLv3_1 { Name = "Level 3-4" }
                        }
                    }
                }
            };
            db.InsertNav(lv1Data)
                .Include(x => x.lv2s)
                .ThenInclude(x => x.lv3s_1)
                .Include(x => x.lv2s, new InsertNavOptions() { OneToManyIfExistsNoInsert = true })
                .ThenInclude(x => x.lv3s_2)
                .Include(x => x.lv2s, new InsertNavOptions() { OneToManyIfExistsNoInsert = true })
                .ThenInclude(x => x.person)
                .ExecuteCommand();
            // int addressId = db.InsertNav(lv1).Insertable(address).ExecuteReturnIdentity();

            // // 创建 UnitPerson011 对象并插入记录
            // var person = new UnitPerson011
            // {
            //     Name = "John Doe",
            //     AddressId = addressId
            // };
            // int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitNavLv1>()
                // 导航2和3层，有多个3层，所以后面要多写下一次Include
                .Includes(lv1 => lv1.lv2s.ToList(lv2 => new UnitNavLv2
                {
                    // 模拟查指定字段
                    Name = lv2.Name,
                    personName1=lv2.person.Name

                }), lv2 => lv2.lv3s_1.ToList(lv3 => new UnitNavLv3_1
                {
                    // 模拟查指定字段
                    Name = lv3.Name
                }))
                // 问题1：只为了给2层导航另外一个3层，这里1和2层指定字段不需要多写一次ToList吧??
                .Includes(lv1 => lv1.lv2s, lv2 => lv2.lv3s_2.ToList(lv3 => new UnitNavLv3_2
                {
                    // 模拟查指定字段
                    Name = lv3.Name
                }))
                .Select(x1 => new
                {
                    x1.Id,
                    x1.Name,
                    otherName = x1.lv2s
                })
                .ToList();
            if (list.First().otherName[0].lv3s_1.Count() == 0) 
            {
                throw new Exception("unit error");
            }
        }
    }





    public class UnitNavLv1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string? Name { get; set; }


        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitNavLv2.lv1Id))]
        public List<UnitNavLv2>? lv2s { get; set; }
    }

    public class UnitNavLv2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public string? Name { get; set; }

        public int lv1Id { get; set; }

        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitNavLv3_1.lv2Id))]
        public List<UnitNavLv3_1>? lv3s_1 { get; set; }

        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToMany, nameof(UnitNavLv3_2.lv2Id))]
        public List<UnitNavLv3_2>? lv3s_2 { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true, DefaultValue = "0")]
        public int? personId { get; set; }

        [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(personId))]
        public Person? person { get; set; }

        // 问题2：只查1个字段 这个怎么用？没有找到用例
        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string? personName { get => person?.Name; }

        [SqlSugar.SugarColumn(IsIgnore = true)]
        public string? personName1 { get; set; }
    }

    public class UnitNavLv3_1
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int lv2Id { get; set; }

        public string Name { get; set; }

    }

    public class UnitNavLv3_2
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        public int lv2Id { get; set; }

        public string Name { get; set; }

    }

    [SugarTable("unitPerson1231s")]
    public class Person
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string? Name { get; set; }

    }
}
