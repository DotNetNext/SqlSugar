using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlSugar.ClickHouse
{
    public class ClickHouseCodeFirst : CodeFirstProvider
    {
        protected override int DefultLength
        {
            get { return 0; }
            set { value = 0; }
        }
        protected override void ExistLogicEnd(List<EntityColumnInfo> dbColumns)
        {
            foreach (EntityColumnInfo column in dbColumns) 
            {
                if (column.DefaultValue != null) 
                {
                    this.Context.DbMaintenance.AddDefaultValue(column.DbTableName,column.DbColumnName,column.DefaultValue.ToSqlValue());
                }
            }
        }
        public override void NoExistLogic(EntityInfo entityInfo)
        {
            var tableName = GetTableName(entityInfo);
            //Check.Exception(entityInfo.Columns.Where(it => it.IsPrimarykey).Count() > 1, "Use Code First ,The primary key must not exceed 1");
            List<DbColumnInfo> columns = new List<DbColumnInfo>();
            if (entityInfo.Columns.HasValue())
            {
                foreach (var item in entityInfo.Columns.Where(it=>it.IsIgnore==false))
                {
                    DbColumnInfo dbColumnInfo = this.EntityColumnToDbColumn(entityInfo, tableName, item);
                    columns.Add(dbColumnInfo);
                }
                if (entityInfo.IsCreateTableFiledSort)
                {
                    columns = columns.OrderBy(c => c.CreateTableFieldSort).ToList();
                }
            }
            columns = columns.OrderBy(it => it.IsPrimarykey ? 0 : 1).ToList();
            if (entityInfo.Type.GetCustomAttribute<CKTable>() != null) 
            {
                tableName = (tableName + UtilConstants.ReplaceKey + entityInfo.Type.GetCustomAttribute<CKTable>().engineValue);
            }
            this.Context.DbMaintenance.CreateTable(tableName, columns,true);
        }
        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var propertyType = UtilMethods.GetUnderType(item.PropertyInfo);
            var result = new DbColumnInfo()
            {
                TableId = entityInfo.Columns.IndexOf(item),
                DbColumnName = item.DbColumnName.HasValue() ? item.DbColumnName : item.PropertyName,
                IsPrimarykey = item.IsPrimarykey,
                IsIdentity = item.IsIdentity,
                TableName = tableName,
                IsNullable = item.IsNullable,
                DefaultValue = item.DefaultValue,
                ColumnDescription = item.ColumnDescription,
                Length = item.Length
            };
            if (propertyType == UtilConstants.DecType) 
            {
                result.Scale = item.DecimalDigits;
                result.DecimalDigits = item.DecimalDigits;
            }
            GetDbType(item, propertyType, result);
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
            //this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            //if (!item.IsPrimarykey)
            //    this.Context.DbMaintenance.DropConstraint(tableName,null);
            //if (item.IsPrimarykey)
            //    this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }

    }
}
