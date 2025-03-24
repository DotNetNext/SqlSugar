using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unitasdfays
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(typeof(SqlSugarTestEntity));
            db.DbMaintenance.TruncateTable<SqlSugarTestEntity>();
            SqlSugarTestEntity entity = new SqlSugarTestEntity { Id = 1, BarCode = "1111", PatientName = "小明" };
            int affrow = db.Storageable(entity).WhereColumns(it => it.BarCode).SplitTable().ExecuteCommand();
            entity.PatientName = "小明1";
            entity.Id = 0;
            var x = db.Storageable(entity).As("SQLSUGARTEST").WhereColumns(it => it.BarCode).ExecuteCommand();
            var x2 = db.Updateable(new List<SqlSugarTestEntity>() { entity }).AS("SQLSUGARTEST").WhereColumns(it => it.BarCode).ExecuteCommand();
            affrow += db.Storageable(entity).WhereColumns(it => it.BarCode).SplitTable().ExecuteCommand();
            if (affrow != 2 || x != 1 || x2 != 1)
            {
                throw new Exception("unit error");
            }

        }
    }
    [SqlSugar.SugarTable("SQLSUGARTEST")]
    [SqlSugar.SplitTable(SplitType._Custom01, typeof(SplitTableService))]
    public class SqlSugarTestEntity
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public int Id { get; set; }
        public string BarCode { get; set; }
        public string PatientName { get; set; }

        /// <summary>
        /// 操作时间  分表字段
        /// </summary>
        [SplitField]
        public DateTime? ProTime { get; set; } = DateTime.Now;
    }
    /// <summary>
    /// 自定义分表集合
    /// </summary>
    public class SplitTableService : ISplitTableService
    {
        /// <summary>
        /// 获取所有匹配的表
        /// </summary>
        /// <param name="db"></param>
        /// <param name="EntityInfo"></param>
        /// <param name="tableInfos"></param>
        /// <returns></returns>
        public List<SplitTableInfo> GetAllTables(ISqlSugarClient db, EntityInfo EntityInfo, List<DbTableInfo> tableInfos)
        {
            List<SplitTableInfo> result = new List<SplitTableInfo>();
            foreach (var item in tableInfos)
            {
                if (item.Name.Contains(EntityInfo.DbTableName))
                //if (item.Name.Contains("_2021") || item.Name == EntityInfo.DbTableName ) //区分标识如果不用正则符复杂一些，防止找错表
                {
                    SplitTableInfo data = new SplitTableInfo()
                    {
                        TableName = item.Name //要用item.name不要写错了
                    };
                    result.Add(data);
                }
            }
            return result.OrderBy(it => it.TableName).ToList();//打断点看一下有没有查出所有分表
        }

        public object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue)
        {
            var splitColumn = entityInfo.Columns.FirstOrDefault(it => it.PropertyInfo.GetCustomAttribute<SplitFieldAttribute>() != null);
            var value = splitColumn.PropertyInfo.GetValue(entityValue, null);
            return value;
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo)
        {
            return entityInfo.DbTableName;
            //return entityInfo.DbTableName + "_FirstA";
        }

        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType type)
        {
            return entityInfo.DbTableName;//目前模式少不需要分类(自带的有 日、周、月、季、年等进行区分)
        }
        /// <summary>
        /// 根据分表字段进行，查找表名，如果当前年份
        /// </summary>
        /// <param name="db"></param>
        /// <param name="entityInfo"></param>
        /// <param name="splitType"></param>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue)
        {
            var dbLst = db.DbMaintenance.GetTableInfoList();
            var dt = (DateTime)fieldValue;
            if (dbLst.Any(x => x.Name == $"{entityInfo.DbTableName}_{dt.Year}"))
                return $"{entityInfo.DbTableName}_{dt.Year}";//根据值按首字母
            return entityInfo.DbTableName;

        }
    }
}
