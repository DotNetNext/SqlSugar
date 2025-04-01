using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SqlSugar;
namespace OrmTest
{
    public class Unitadfasdysss
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            //建表 
            db.CodeFirst.InitTables<UnitTest22a001>();
            //清空表



            //插入测试数据
            var result = db.Insertable(new UnitTest22a001() { id = 1, ctime = DateTime.Parse("2025-03-25") })
                .SplitTable().ExecuteCommand();//用例代码

            var startTime = DateTime.Parse("2025-03-03");
            var endTime = DateTime.Parse("2025-03-25");

            var dd = db.Queryable<UnitTest22a001>().Where(c => c.id == 1).SplitTable(startTime, endTime).ToList();
            /** 
             * dd 返回结果是 no table 
             * 查看源码后 发现 起始时间是日期不是1号，导致筛选不出分表
             * 如果非常明确 SplitType._Custom01 针对按月分表的话，可以考虑 论坛中描述的
             
             
             */ 
            Console.WriteLine(result);
            Console.WriteLine("用例跑完"); 
        }

        //建类
        [SplitTable(SplitType.Month, typeof(yyyyMMService))]
        [SugarTable("Unitxxx_{yyyyMM}")]
        public class UnitTest22a001
        {
            public int id { get; set; }

            [SplitField]
            public DateTime ctime { get; set; }
        }


    }
    public class yyyyMMService : ISplitTableService
    {
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in tableInfos)
            {
                var tableName = EntityInfo.DbTableName.Replace("_{yyyyMM}", "");
                if (EntityInfo.DbTableName.Contains("_{yyyyMM}") && item.Name.Contains(tableName))// MySpliteTest_202204  这种格式的表
                {
                    SplitTableInfo tableInfo = new SplitTableInfo();
                    tableInfo.TableName = item.Name;
                    var value = Regex.Match(item.Name, @"\d{6}$").Value;
                    if (value != null)
                    {
                        value = value.Insert(4, "-");
                        tableInfo.Date = Convert.ToDateTime(value + "-01");
                        //tableInfo.String = null;  Time table, it doesn't work
                        //tableInfo.Long = null;  Time table, it doesn't work
                        result.Add(tableInfo);
                    }
                }
            }
            return result;
        }

        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            if (splitColumn == null)
            {
                return db.GetDate();
            }
            else
            {
                var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
                return value;
            }
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo)
        {
            return EntityInfo.DbTableName.Replace("{yyyyMM}", DateTime.Now.ToString("yyyyMM"));
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo, SplitType type)
        {
            return EntityInfo.DbTableName.Replace("{yyyyMM}", DateTime.Now.ToString("yyyyMM"));
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            return entityInfo.DbTableName.Replace("{yyyyMM}", Convert.ToDateTime(fieldValue).ToString("yyyyMM"));
        }
    }
}