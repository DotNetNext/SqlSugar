using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitADFAydd
    {
        public static void Init() 
        {
            var Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = NewUnitTest.Db.CurrentConnectionConfig.ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    //实体表特性扩展
                    EntityNameService = (type, entity) =>
                    {
                        //实体禁止删除列(全局禁止)
                        entity.IsDisabledDelete = true;
                        //实体字段排序(全局设置)
                        entity.IsCreateTableFiledSort = true;

                        //获取所有实体特性
                        var attributes = type.GetCustomAttributes(true);
                        //如没有标记SugarTable，可以使用TableAttribute标记表名称
                        if (!attributes.Any(it => it is SugarTable))
                        {
                            //表名称
                            if (attributes.Any(it => it is TableAttribute))
                            {
                                var tableAttr = attributes.FirstOrDefault(it => it is TableAttribute) as TableAttribute;
                                entity.DbTableName = tableAttr?.Name;
                            }

                            //表描述
                            if (attributes.Any(it => it is DescriptionAttribute))
                            {
                                var descAttr = attributes.FirstOrDefault(it => it is DescriptionAttribute) as
                                    DescriptionAttribute;
                                entity.TableDescription = descAttr?.Description;
                            }
                        }
                    }
                }
            },
db =>
{
    db.Aop.DataExecuting = (obj, entityInfo) =>
    {
        if (entityInfo.EntityColumnInfo.IsPrimarykey)  //主键
        {
            if (!entityInfo.EntityColumnInfo.IsIdentity)  //非自增 Guid，String
            {
                //id
                var val = obj;
            }
        }
    };

    db.Aop.OnDiffLogEvent = diff =>
    {



    };
});

            Db.DbMaintenance.CreateDatabase();

            //生成表
            Db.CodeFirst
                .SetStringDefaultLength(200)
                .BackupTable()
                .InitTables(typeof(Person), typeof(Books));

            //配置自定义Guid生产生成规则
            StaticConfig.CustomGuidFunc = () =>
            {
                return Guid.NewGuid();
            };
            var methodName = "EnableDiffLogEvent"; //方法名称
            StaticConfig.CompleteInsertableFunc = it => { it.GetType().GetMethod(methodName)?.Invoke(it,new object[] { null}); };
            StaticConfig.CompleteUpdateableFunc = it => { it.GetType().GetMethod(methodName)?.Invoke(it, new object[] { null }); };
            StaticConfig.CompleteDeleteableFunc = it => { it.GetType().GetMethod(methodName)?.Invoke(it, new object[] { null }); };
            //常规添加
             Db.Insertable(new Person
            {
                Id = Guid.NewGuid(),
                Name = "123",
                Age = 20
            })
            .ExecuteCommand();

            //导航添加
            var p = new Person
            {
                Name = "123",
                Age = 20,
                BookList = new List<Books>() {
                    new(){
            Name="123" },
                     new(){
            Name="345"}
                } 
            };
             
            //导航更新
            var entity =   Db.InsertNav(p).Include(x => x.BookList).ExecuteReturnEntity();
            entity.BookList = new List<Books>() {
                new Books{ Name ="1111"},

                new Books{ Name ="2222"},
                };


            //此处有问题
              Db.UpdateNav(entity).Include(x => x.BookList).ExecuteCommand();

            StaticConfig.CompleteInsertableFunc = null;
            StaticConfig.CompleteUpdateableFunc = null;
            StaticConfig.CompleteDeleteableFunc = null;

        }
        /// <summary>
        /// 书籍
        /// </summary>
        [SugarTable("unitdsafa"+nameof(Books), "书籍")]
        public class Books
        {
            /// <summary>
            /// 主键编号
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键编号", CreateTableFieldSort = 0)]
            public Guid Id { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [SugarColumn(ColumnDescription = "名称", CreateTableFieldSort = 1)]
            public string? Name { get; set; }

            /// <summary>
            /// 学生编号
            /// </summary>
            [SugarColumn(ColumnDescription = "学生编号", CreateTableFieldSort = 10)]
            public Guid? StudenId { get; set; }

        }

        /// <summary>
        /// 人类
        /// </summary>
        [SugarTable("unitdsafa" + nameof(Person), "人类")]
        public class Person
        {
            /// <summary>
            /// 主键编号
            /// </summary>
            [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键编号", CreateTableFieldSort = 0)]
            public Guid Id { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            [SugarColumn(ColumnDescription = "名称", CreateTableFieldSort = 1)]
            public string? Name { get; set; }

            /// <summary>
            /// 年龄
            /// </summary>
            [SugarColumn(ColumnDescription = "年龄", CreateTableFieldSort = 10)]
            public int? Age { get; set; }


            /// <summary>
            /// 书
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(Books.StudenId))]
            public List<Books>? BookList { get; set; }


        }
    }
}
