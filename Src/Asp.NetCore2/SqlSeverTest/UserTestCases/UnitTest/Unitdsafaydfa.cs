using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Data;
using System.Numerics;
using System.Reflection;
namespace OrmTest
{
    public class Unitadfafassys
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            DateRangeTest(db);

            //建表 
            db.CodeFirst.InitTables<A>();
            db.CodeFirst.InitTables<AB>();
            db.CodeFirst.InitTables<B>();
            //清空表
            db.DbMaintenance.TruncateTable<A>();
            db.DbMaintenance.TruncateTable<AB>();
            db.DbMaintenance.TruncateTable<B>();


            //插入测试数据
            var result = db.Insertable(new A() { Id = 1, Aid = 2 }).ExecuteCommand();//用例代码
            var result2 = db.Insertable(new AB() { Bid = 2, Aid = 2 }).ExecuteCommand();//用例代码
            var result3 = db.Insertable(new B() { Id = 3, Bid = 2, MustNum = 2, Color = "3" }).ExecuteCommand();//用例代码
            var result4 = db.Insertable(new B() { Id = 4, Bid = 2, MustNum = 3, Color = "4" }).ExecuteCommand();//用例代码
                                                                                                                //正常示例
            var faPlans =
            db.Queryable<A>()
               .Includes(p => p.ABEntries)
               .Where(p => p.Id == 1)
               .ToList();
            var faPlans2 = db.Queryable<A>()
                  .Includes(p => p.ABEntries)
                  .Where(p => p.Id == 1)
                  .Select(p => new
                  {
                      p.Id,
                      p.Aid,
                      entitys = p.ABEntries
                  }).ToList();
            var faPlans3 =
                db.Queryable<A>()
                .Includes(p => p.ABEntries.Select(it => new B { Color = it.Color, MustNum = it.MustNum }).ToList())
                .Where(p => p.Id == 1)
                .Select(p => new { p.Id, p.Aid, entitys = p.ABEntries.Select(p => new { p.Color, p.MustNum }) })
                .ToList();

            if (faPlans3.First().entitys.Count() != 2) throw new Exception("unit error");
            if (faPlans2.First().entitys.Count() != 2) throw new Exception("unit error");
            if (faPlans.First().ABEntries.Count() != 2) throw new Exception("unit error");
        }

        private static void DateRangeTest(SqlSugarClient db)
        {
            db.CodeFirst.InitTables<UserInfo001>();
            var userInfo1 = db.Queryable<UserInfo001>()
             .Where(new List<IConditionalModel>()
          {
                            new ConditionalModel()
                            {
                                 ConditionalType=ConditionalType.RangeDate,
                                 FieldValue="2025-08,2025-08",
                                  FieldName="RegistrationDate"
                            }

          }).ToList();
            var userInfo2 = db.Queryable<UserInfo001>()
              .Where(new List<IConditionalModel>()
                {
                    new ConditionalModel()
                    {
                         ConditionalType=ConditionalType.RangeDate,
                         FieldValue="2025-08-27 09,2025-08-27 09",
                          FieldName="RegistrationDate"
                    }

                }).ToList();
            var userInfo3 = db.Queryable<UserInfo001>()
                .Where(new List<IConditionalModel>()
             {
                        new ConditionalModel()
                        {
                             ConditionalType=ConditionalType.RangeDate,
                             FieldValue="2025-08-27 09:01,2025-08-27 09:01",
                              FieldName="RegistrationDate"
                        }

             }).ToList();
            var userInfo4 = db.Queryable<UserInfo001>()
                .Where(new List<IConditionalModel>()
             {
                                new ConditionalModel()
                                {
                                     ConditionalType=ConditionalType.RangeDate,
                                     FieldValue="2025-08-27 09:01:01,2025-08-27 09:01:01",
                                      FieldName="RegistrationDate"
                                }

             }).ToList();
            var userInfo5 = db.Queryable<UserInfo001>()
                .Where(new List<IConditionalModel>()
             {
                                            new ConditionalModel()
                                            {
                                                 ConditionalType=ConditionalType.RangeDate,
                                                 FieldValue="2025,2025",
                                                  FieldName="RegistrationDate"
                                            }

             }).ToList();
        }

        [SugarTable("Unit0000A")]
        public class A
        {
            /// <summary>
            /// 雪花Id
            /// </summary>
            [SugarColumn(ColumnName = "Id", ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = false)]
            public virtual long Id { get; set; }
            /// <summary>
            /// 生产订单明细Id
            /// </summary>
            [SugarColumn(ColumnDescription = "生产订单明细Id")]
            [Required]
            public long Aid { get; set; }

            [Navigate(typeof(AB), nameof(AB.Aid), nameof(AB.Bid), nameof(A.Aid), nameof(B.Bid))]
            public List<B> ABEntries { get; set; }
        }
        [SugarTable("Unit0000AB")]
        public class AB
        {
            /// <summary>
            /// 雪花Id
            /// </summary>
            [SugarColumn(ColumnName = "Bid", ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = false)]
            public virtual long Bid { get; set; }

            /// <summary>
            /// 生产订单分录Id
            /// </summary>
            [SugarColumn(ColumnDescription = "生产订单分录Id")]
            public long Aid { get; set; }

            /// <summary>
            /// 用料明细
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(B.Bid))]
            public List<B> ABEntries { get; set; }
        }
        [SugarTable("Unit0000B")]
        public class B
        {
            /// <summary>
            /// 雪花Id
            /// </summary>
            [SugarColumn(ColumnName = "Id", ColumnDescription = "主键Id", IsPrimaryKey = true, IsIdentity = false)]
            public virtual long Id { get; set; }
            /// <summary>
            /// 用料清单Id
            /// </summary>
            [SugarColumn(ColumnDescription = "用料清单Id")]
            public long Bid { get; set; }

            /// <summary>
            /// 物料
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(Bid))]
            public AB AB { get; set; }

            /// <summary>
            /// 应发数量
            /// </summary>
            [SugarColumn(ColumnDescription = "应发数量")]
            public decimal MustNum { get; set; }

            /// <summary>
            /// 颜色
            /// </summary>
            [SugarColumn(ColumnDescription = "颜色", Length = 50)]
            [MaxLength(50)]
            public string Color { get; set; }
        }
        [SugarTable("UnitDoubleColorPrintOutPutEntry")]
        public class DoubleColorPrintOutPutEntry
        {
            /// <summary>
            /// 颜色
            /// </summary>
            public string Color { get; set; }
            /// <summary>
            /// 数量
            /// </summary>
            public decimal MustNum { get; set; }
        }
        public class UserInfo001
        {
            /// <summary>
            /// User ID (Primary Key)
            /// 用户ID（主键）
            /// </summary>
            [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
            public int UserId { get; set; }

            /// <summary>
            /// User name
            /// 用户名
            /// </summary>
            [SugarColumn(Length = 50, IsNullable = false)]
            public string UserName { get; set; }

            /// <summary>
            /// User email
            /// 用户邮箱
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public string Email { get; set; }


            /// <summary>
            /// Product price
            /// 产品价格
            /// </summary> 
            public decimal Price { get; set; }

            /// <summary>
            /// User context
            /// 用户内容
            /// </summary>
            [SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsNullable = true)]
            public string Context { get; set; }

            /// <summary>
            /// User registration date
            /// 用户注册日期
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public DateTime? RegistrationDate { get; set; }
        }

    }
}
