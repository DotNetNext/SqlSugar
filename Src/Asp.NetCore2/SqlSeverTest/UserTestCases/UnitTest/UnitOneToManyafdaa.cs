using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrmTest
{
    internal class UnitOneToManyafdaa
    {
        public static void Init() 
        {
              Get().GetAwaiter().GetResult();
        }
       
        public static async Task Get()
        {
            var db = NewUnitTest.Db;
             

            //建表 
            db.CodeFirst.InitTables<Test001>();
            db.CodeFirst.InitTables<TestSubClass001>();
            db.CodeFirst.InitTables<TestSubClass002>();
            db.CodeFirst.InitTables<TestSubClass003>();
            db.CodeFirst.InitTables<Common>();

            #region 数据库插入

            var result = await db.InsertNav(new Test001()
            {
                Name = "A",
                Other = 5,
                TenantId = 1,
                Sub001 = new List<TestSubClass001>
                {
                    new TestSubClass001
                    {
                        Name = "张三",
                        TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 1,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass001
                    {
                        Name = "sadsa",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 1,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass001
                    {
                        Name = "123",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 1,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass001
                    {
                        Name = "kukhg",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 1,
                                TenantId = 1,
                            }
                        }
                    }
                },
                Sub002 = new List<TestSubClass002>
                {
                    new TestSubClass002
                    {
                        Name = "张三",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass002
                    {
                        Name = "sadsa",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass002
                    {
                        Name = "123",TenantId = 1,Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass002
                    {
                        Name = "kukhg",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass002
                    {
                        Name = "fswrqqw",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass002
                    {
                        Name = "kiukiyu",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 2,
                                TenantId = 1,
                            }
                        }
                    }
                },
                Sub003 = new List<TestSubClass003>
                {
                    new TestSubClass003
                    {
                        Name = "张三",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 3,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass003
                    {
                        Name = "sadsa",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 3,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass003
                    {
                        Name = "123",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 3,
                                TenantId = 1,
                            }
                        }
                    },new TestSubClass003
                    {
                        Name = "kukhg",TenantId = 1,
                        Commons = new List<Common>
                        {
                            new Common
                            {
                                Desctiption = "描述",
                                Type = 3,
                                TenantId = 1,
                            }
                        }
                    }
                }
            })
                .Include(i => i.Sub001)
                .Include(i => i.Sub002)
                .Include(i => i.Sub003)
                .ExecuteCommandAsync();

            #endregion

            db.QueryFilter.AddTableFilter<Test001>(x => x.TenantId == 1);
            db.QueryFilter.AddTableFilter<TestSubClass001>(x => x.TenantId == 1);
            db.QueryFilter.AddTableFilter<TestSubClass002>(x => x.TenantId == 1);
            db.QueryFilter.AddTableFilter<TestSubClass003>(x => x.TenantId == 1);
            db.QueryFilter.AddTableFilter<Common>(x => x.TenantId == 1);

            //用例代码
            try
            {
                var riskList = db.Queryable<Test001>()
               .Select(t => new
               {
                   Name = t.Name,
                   Other = t.Other,
                   Sub001Count = t.Sub001.Count(),
                   Sub002Count = t.Sub002.Count(),
                   Sub003Count = t.Sub003.Count(),
               })
               .ToList();

                var riskList2 = db.Queryable<Test001>()
                    .Where(it=>
                    it.Sub001.Any(s=>s.Commons.Any(t=>t.TenantId==1))||
                    it.Sub002.Any(s => s.Commons.Any(t => t.TenantId == 1))||
                    it.Sub003.Any(s => s.Commons.Any(t => t.TenantId == 1)))
            .Select(it => new
            {
                Name = it.Name,
                Other = it.Other,
                Sub001Count = it.Sub001.Count(),
                Sub002Count = it.Sub002.Count(),
                Sub003Count = it.Sub003.Count(),
            })
            .ToList();
            }
            catch (Exception e)
            {

                throw;
            } 
        }
        [SugarTable("unittestfadfa")]
        public class Test001 : ITenantIdFilter
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

            public string Name { get; set; }

            public int? Other { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(TestSubClass001.PId))]
            public List<TestSubClass001> Sub001 { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(TestSubClass002.PId))]
            public List<TestSubClass002> Sub002 { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(TestSubClass003.PId))]
            public List<TestSubClass003> Sub003 { get; set; }
            public long TenantId { get; set; }
        }

        public class TestSubClass001 : ITenantIdFilter
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

            public long PId { get; set; }

            public string Name { get; set; }
            public long TenantId { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(Common.SubId))]
            public List<Common> Commons { get; set; }
        }

        public class TestSubClass002 : ITenantIdFilter
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

            public long PId { get; set; }

            public string Name { get; set; }
            public long TenantId { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(Common.SubId))]
            public List<Common> Commons { get; set; }
        }

        public class TestSubClass003 : ITenantIdFilter
        {
            [SugarColumn(IsPrimaryKey = true)]
            public long Id { get; set; }

            public long PId { get; set; }

            public string Name { get; set; }
            public long TenantId { get; set; }

            [Navigate(NavigateType.OneToMany, nameof(Common.SubId))]
            public List<Common> Commons { get; set; }
        }

        public class Common : ITenantIdFilter
        {
            public long Id { get; set; }

            public string Desctiption { get; set; }

            public long SubId { get; set; }

            public int Type { get; set; }
            public long TenantId { get; set; }
        }

        public interface ITenantIdFilter
        {
            public long TenantId { get; set; }
        }
    }
}
