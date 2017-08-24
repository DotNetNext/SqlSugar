using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MySqlCodeFirst : CodeFirstProvider
    {
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.IsValuable())
            {
                foreach (var item in entityInfo.Columns)
                {
                    DbColumnInfo dbColumnInfo = this.EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns);
        }
        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var result = new DbColumnInfo()
            {
                DataType = this.Context.Ado.DbBind.GetDbTypeName(PubMethod.GetUnderType(item.PropertyInfo).Name),
                TableId = entityInfo.Columns.IndexOf(item),
                DbColumnName = item.DbColumnName.IsValuable() ? item.DbColumnName : item.PropertyName,
                IsPrimarykey = item.IsPrimarykey,
                IsIdentity = item.IsIdentity,
                TableName = tableName,
                IsNullable = item.IsNullable,
                DefaultValue = item.DefaultValue,
                ColumnDescription = item.ColumnDescription,
                Length = item.Length
            };
            if (result.DataType.Equals("varchar", StringComparison.CurrentCultureIgnoreCase) && result.Length == 0)
            {
                result.Length = 1;
            }
            return result;
        }

        protected override void ConvertColumns(List<DbColumnInfo> dbColumns)
        {
            foreach (var item in dbColumns)
            {
                if (item.DataType == "DateTime")
                {
                    item.Length = 0;
                }
            }
        }

        protected override void ChangeKey(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            if (!item.IsPrimarykey)
                this.Context.DbMaintenance.DropConstraint(tableName,null);
            if (item.IsPrimarykey)
                this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }
    }
}
