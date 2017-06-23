using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MySqlCodeFirst:CodeFirstProvider
    {
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = base.GetTableName(entityInfo);
            Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.IsValuable())
            {
                foreach (var item in entityInfo.Columns)
                {
                    DbColumnInfo dbColumnInfo = base.EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns);
        }
    }
}
