using SqlSugar;
using SqlSugarTest;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitBizDelete
    {
        public static void Init()
        {
            var _db = NewUnitTest.Db;
            _db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings
            {

                IsAutoDeleteQueryFilter = true,//启用删除查询过滤器  
                IsAutoUpdateQueryFilter = true//启用更新查询过滤器 （表达式更新，如果是实体方式更新建议先查询在更新）
            };
            _db.CodeFirst.InitTables(typeof(Special));//创建表
            var entityType = typeof(Special);

            _db.QueryFilter.AddTableFilter<Special>(it => it.IsDeleted == false);
            //新增数据
            var special = new Special();
            special.Id = SnowFlakeSingle.Instance.NextId();
            special.Name = "测试的哦";
            _db.Insertable(special).ExecuteCommand();

            //假删除数据
            _db.Deleteable<Special>().In(special.Id)
                      .IsLogic()
                      .ExecuteCommand("IsDeleted", true, "DeletedTime", "DeletedUserName", "admin");

        }
    }
}
