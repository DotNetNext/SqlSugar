using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar.GBase
{
    public class GBaseCodeFirst:CodeFirstProvider
    {
        public virtual bool IsNoTran { get; set; } = true;
        public override void ExistLogic(EntityInfo entityInfo) 
        {
            this.Context.Ado.ExecuteCommand("select '不支持修改表' from dual ");
        }

        protected override DbColumnInfo EntityColumnToDbColumn(EntityInfo entityInfo, string tableName, EntityColumnInfo item)
        {
            var result= base.EntityColumnToDbColumn(entityInfo, tableName, item);
            if (item.UnderType == UtilConstants.GuidType) 
            {
                item.Length = 36;
            }
            return result;
        }
        protected override string GetTableName(EntityInfo entityInfo)
        {
            var table= this.Context.EntityMaintenance.GetTableName(entityInfo.EntityName);
            var tableArray = table.Split('.');
            var noFormat = table.Split(']').Length==1;
            if (tableArray.Length > 1 && noFormat)
            {
                var dbMain = new GBaseDbMaintenance() { Context = this.Context };
                return dbMain.SqlBuilder.GetTranslationTableName(table);
            }
            else
            {
                return table;
            }
        }
      }
}
