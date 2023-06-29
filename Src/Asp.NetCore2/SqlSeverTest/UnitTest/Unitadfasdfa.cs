using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class Unitadfasdfa
    {
        public static void Init()
        {
            var db = new SqlSugarScope(new List<SqlSugar.ConnectionConfig>()
            {
                new ConnectionConfig()
                {
                    ConfigId = "B",
                    DbLinkName = "HGT",//默认: dblinkName.[表名]；dblinkName以_结尾: dblinkName表名；dblinkName以@开头： [表名]@dblinkName
                    ConnectionString = "Data Source=172.18.15.141/eee;User ID=aaa;Password=***",
                    DbType = DbType.Oracle,
                    IsAutoCloseConnection = true
                },
                new ConnectionConfig()
                {
                    ConfigId = "A",
                    DbLinkName = "HGT",//默认: dblinkName.[表名]；dblinkName以_结尾: dblinkName表名；dblinkName以@开头： [表名]@dblinkName
                    ConnectionString = "Data Source=172.18.15.141/eee;User ID=aaa;Password=***",
                    DbType = DbType.Oracle,
                    IsAutoCloseConnection = true,
                    ConfigureExternalServices = new ConfigureExternalServices
                    {
                        EntityNameService = (type, entity) => // 处理表
                        {
                            if (true && !entity.DbTableName.Contains('_') && entity.DbTableName?.EndsWith("OutPut") == false)
                                entity.DbTableName = UtilMethods.ToUnderLine(entity.DbTableName); // 驼峰转下划线
 
                            if(type.Name == nameof(KingJsonTest))
                            {

                            }
 
                            // 获取 CustomAttribute 特性
                            var customAttribute = Attribute.GetCustomAttribute(type.Assembly, typeof(SugarTable));
                        },
                        EntityService = (type, column) => // 处理列
                        {
                            if (true && !column.IsIgnore && !column.DbColumnName.Contains('_') && column.DbTableName?.EndsWith("OutPut") == false)
                                column.DbColumnName = UtilMethods.ToUnderLine(column.DbColumnName); // 驼峰转下划线
                        },
                    }
                },

            });

            new List<String>() { "A", "B" }.ForEach(configId =>
            {
                //每次Sql执行前事件            
                db.GetConnectionScope(configId).Aop.OnLogExecuting = (sql, pars) =>
                {



                    Console.WriteLine("【" + DateTime.Now + "——执行SQL】\r\n" + UtilMethods.GetSqlString(db.CurrentConnectionConfig.DbType, sql, pars) + "\r\n");
                };
                //出错打印日志
                db.GetConnectionScope(configId).Aop.OnError = (e) =>
                {
                    Console.WriteLine($"执行SQL出错：{e.Message}");
                };
            });



            //if (!db.DbMaintenance.IsAnyTable(true ? UtilMethods.ToUnderLine(nameof(KingJsonTest)) : nameof(KingJsonTest), false))
            //{
            //    db.CodeFirst.InitTables<KingJsonTest>();
            //}




            var sql = db.QueryableWithAttr<KingJsonTest>().LeftJoin<KingTree>((t, j) => t.Id == j.Id).ToSqlString();
            if (!sql.Contains("\"KINGTREE\""))
            {
                throw new Exception("unit error");
            }
            if (!sql.Contains("\"KING_JSON_TEST\""))
            {
                throw new Exception("unit error");
            }
            var sq2 = db.QueryableWithAttr<KingJsonTest>()
                   .LeftJoin<KingTree>((t, j) => t.Id == j.Id)
                   .Where((t, j) => t.CreateTime == null || j.ParentId == 1)
                   .Select((t, j) => new KingOutPut() { }, true)
                   .ToSqlString();
            var sql3 = "SELECT \"T\".\"CREATE_TIME\" AS \"CREATETIME\" ,\"J\".\"PARENTID\" AS \"PARENTID\" FROM \"KING_JSON_TEST\" \"T\" Left JOIN HGT.\"KINGTREE\" \"J\" ON ( \"T\".\"ID\" = \"J\".\"ID\" )   WHERE (( \"T\".\"CREATE_TIME\" IS NULL ) OR ( \"J\".\"PARENTID\" = 1 ))";
            if (sq2 != sql3)
            {
                throw new Exception("unit error");
            }

        }
        public class KingOutPut
        {
            public DateTime CreateTime { get; set; }
            public int ParentId { get; set; }

        }
        //Json字段实体
        [TenantAttribute("A")]//对应ConfigId
        [SugarTable(null, "Json测试表King")]
        public class KingJsonTest
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(ColumnDataType = StaticConfig.CodeFirst_BigString, IsJson = true)]
            public JObject ExtJson { get; set; }
            [SqlSugar.SugarColumn]
            public DateTime CreateTime { get; set; }
        }

        //实体
        [TenantAttribute("B")]
        [SugarTable(null, "Tree测试表King")]
        public class KingTree
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; } //关联字段 (如果非主键的话，也要设成主键才会有效，可以新建个类)
            public string Name { get; set; }
            public int ParentId { get; set; }//父级字段
            [SqlSugar.SugarColumn(IsIgnore = true)]
            public List<KingTree> Child { get; set; }
        }
    }
}
