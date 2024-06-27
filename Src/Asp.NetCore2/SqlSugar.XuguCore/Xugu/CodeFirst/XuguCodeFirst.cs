using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar.Xugu
{
    public class XuguCodeFirst : CodeFirstProvider
    {
        protected override int DefultLength { get; set; } = -1;
        public virtual bool IsNoTran { get; set; } = true;
        public override void ExistLogic(EntityInfo entityInfo)
        {
            base.ExistLogic(entityInfo);
            //this.Context.Ado.ExecuteCommand("select '不支持修改表' from dual ");
        }
        protected override void ConvertColumns(List<DbColumnInfo> dbColumns)
        {
            dbColumns.ForEach(d => { d.Length = 0;d.IsNullable = false; });
        }
        protected override bool IsNoSamgeType(EntityColumnInfo ec, DbColumnInfo dc)
        {
            var o = new DbColumnInfo();
            GetDbType(ec, UtilMethods.GetUnderType(ec.PropertyInfo), o);
            ec.DataType = o.DataType;
            //if (ec.UnderType == UtilConstants.GuidType) ec.Length = 32;
            if (ec.UnderType == UtilConstants.ByteArrayType) ec.DataType = "BINARY";
            if (ec.UnderType == UtilConstants.DateType) ec.DataType = "DATETIME";
            if (ec.UnderType == UtilConstants.DateTimeOffsetType) ec.DataType = "DATETIME WITH TIME ZONE";
            if (ec.UnderType == UtilConstants.FloatType) ec.DataType = "FLOAT";
            if (ec.UnderType == UtilConstants.DecType) ec.DataType = "DECIMAL";
            if (ec.UnderType == UtilConstants.StringType) ec.DataType = "VARCHAR";
            if (ec.UnderType == typeof(Version)) ec.DataType = "ROWVERSION";

            if (!string.IsNullOrWhiteSpace(ec.DataType)) ec.DataType = ec.DataType.ToUpper();
            ec.IsNullable = Nullable.GetUnderlyingType(ec.PropertyInfo.PropertyType) != null
                || !ec.PropertyInfo.PropertyType.IsValueType
                || !UtilConstants.NumericalTypes.Contains(ec.PropertyInfo.PropertyType);

            return base.IsNoSamgeType(ec, dc);
        }

        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var result = base.EntityColumnToDbColumn(entityInfo, tableName, item);
            //if (item.UnderType == UtilConstants.GuidType) result.Length = 32;
            if (item.UnderType == UtilConstants.ByteArrayType) result.DataType = "BINARY";
            if (item.UnderType == UtilConstants.DateType) result.DataType = "DATETIME";
            if (item.UnderType == UtilConstants.DateTimeOffsetType) result.DataType = "DATETIME WITH TIME ZONE";
            if (item.UnderType == UtilConstants.FloatType) result.DataType = "FLOAT";
            if (item.UnderType == UtilConstants.DecType) result.DataType = "DECIMAL";
            if (item.UnderType == UtilConstants.StringType) result.DataType = "VARCHAR";
            if (item.UnderType == typeof(Version)) result.DataType = "ROWVERSION";

            if (!string.IsNullOrWhiteSpace(item.DataType)) result.DataType = item.DataType.ToUpper();
            result.IsNullable = Nullable.GetUnderlyingType(item.PropertyInfo.PropertyType) != null
                || !item.PropertyInfo.PropertyType.IsValueType
                || !UtilConstants.NumericalTypes.Contains(item.PropertyInfo.PropertyType);
            return result;
        }
        protected override string GetTableName(EntityInfo entityInfo)
        {
            var table = this.Context.EntityMaintenance.GetTableName(entityInfo.EntityName);
            var tableArray = table.Split('.');
            var noFormat = table.Split(']').Length == 1;
            if (tableArray.Length > 1 && noFormat)
            {
                var dbMain = new XuguDbMaintenance() { Context = this.Context };
                return dbMain.SqlBuilder.GetTranslationTableName(table);
            }
            return table;
        }

        protected override void ExistLogicEnd(List<EntityColumnInfo> dbColumns)
        {
            foreach (EntityColumnInfo column in dbColumns)
            {
                if (column.DefaultValue != null)
                {
                    this.Context.DbMaintenance.AddDefaultValue(column.DbTableName, column.DbColumnName, column.DefaultValue.ToSqlValue());
                }
            }
        }

        protected override void ChangeKey(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            this.Context.DbMaintenance.UpdateColumn(tableName, EntityColumnToDbColumn(entityInfo, tableName, item));
            string keyName = this.Context.Ado.GetString(@$"SELECT I.INDEX_NAME FROM USER_INDEXES I
LEFT JOIN USER_TABLES T ON I.TABLE_ID=T.TABLE_ID
WHERE T.TABLE_NAME='{tableName}' AND FIND_IN_SET('""{item.DbColumnName}""',I.KEYS)>0 AND T.DB_ID=CURRENT_DB_ID
	AND T.USER_ID=CURRENT_USERID AND T.SCHEMA_ID=CURRENT_SCHEMAID");
            if (!item.IsPrimarykey && !string.IsNullOrWhiteSpace(keyName))
                this.Context.DbMaintenance.DropConstraint(tableName, keyName);
            if (item.IsPrimarykey)
                this.Context.DbMaintenance.AddPrimaryKey(tableName, item.DbColumnName);
        }
    }
}
