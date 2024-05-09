using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitSplitadfa
    {
        public static void Init()
        {

            var db = NewUnitTest.Db;

            db.CopyNew().Insertable(new ClassA() { Sold = 1, Id = 1 })
              .SplitTable()
              .ExecuteCommand();

            db.CopyNew().Deleteable(new ClassA() { Sold = 1, Id = 1 })
           .SplitTable()
           .ExecuteCommand();

            var table = db.SplitHelper<ClassA>().GetTableName(1);

            //db.CurrentConnectionConfig.ConfigureExternalServices
            //    .SplitTableService = null;

            db.Insertable(new ClassB
            {
                Time = DateTime.Now
            })
             .SplitTable()
            .ExecuteReturnSnowflakeId();

            db.Deleteable(new ClassA() { Sold = 1, Id = 1 })
                  .SplitTable()
                  .ExecuteCommand();
            db.Insertable(new ClassA() { Sold = 1, Id = 1 })
                .SplitTable()
                .ExecuteCommand();
            db.CopyNew().Deleteable(new ClassA() { Sold = 1, Id = 1 })
                 .SplitTable()
                 .ExecuteCommand();

        }
    }
    [SplitTable(SplitType._Custom01, typeof(MySplitTableService))]
    [SugarIndex("idx_ClassA_Sold", nameof(Sold), OrderByType.Asc)]
    internal class ClassA
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SplitField]
        public int Sold { get; set; }
    }

    [SplitTable(SplitType.Day)]
    [SugarTable("ClassB_{year}{month}{day}")]
    [SugarIndex("idx_ClassB_Time", nameof(Time), OrderByType.Desc)]
    internal class ClassB
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Id { get; set; }

        [SplitField]
        public DateTime Time { get; set; }
    }

    public class MySplitTableService : ISplitTableService
    {
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> result = new();

            foreach (var item in tableInfos)
            {
                if (item.Name.StartsWith($"{EntityInfo.DbTableName}_"))
                {
                    SplitTableInfo data = new()
                    {
                        TableName = item.Name
                    };

                    result.Add(data);
                }
            }

            return result.OrderBy(it => it.TableName).ToList();
        }

        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.First(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() is not null);
            var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
            return value ?? -1;
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo)
        {
            return $"{EntityInfo.DbTableName}_-1";
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo, SplitType type)
        {
            return $"{EntityInfo.DbTableName}_-1";
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            return $"{entityInfo.DbTableName}_{fieldValue}";
        }
    }
}
