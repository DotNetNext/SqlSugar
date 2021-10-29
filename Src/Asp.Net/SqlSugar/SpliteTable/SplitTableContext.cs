using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    internal class SplitTableContext
    {
        public SqlSugarProvider Context { get; set; }
        public EntityInfo EntityInfo { get; set; }
        public ISplitTableService Service { get; set; }
        public SplitTableContext() 
        {
            if (this.Context.CurrentConnectionConfig.ConfigureExternalServices != null&&this.Context.CurrentConnectionConfig.ConfigureExternalServices.SplitTableService!=null)
            {
                Service = this.Context.CurrentConnectionConfig.ConfigureExternalServices.SplitTableService;
            }
            else
            {
                Service = new DateSplitTableService();
            }
        }
        public List<SplitTableInfo> GetTables()
        {
            var oldIsEnableLogEvent = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            var tableInfos = this.Context.DbMaintenance.GetTableInfoList(false);
            List<SplitTableInfo> result = Service.GetAllTables(this.Context,EntityInfo,tableInfos);
            this.Context.Ado.IsEnableLogEvent = oldIsEnableLogEvent;
            return result;
        }

        internal string GetDefaultTableName()
        {
           return Service.GetTableName(this.Context,EntityInfo);
        }
        internal string GetTableName(SplitType splitType) 
        {
            return Service.GetTableName(this.Context,EntityInfo, splitType);
        }
        internal string GetTableName(SplitType splitType, object fieldValue)
        {
            return Service.GetTableName(this.Context,EntityInfo, splitType, fieldValue);
        }
        internal object GetValue(SplitType splitType, object entityValue)
        {
            return Service.GetFieldValue(this.Context,EntityInfo, splitType, entityValue);
        }
        public void CheckPrimaryKey()
        {
            Check.Exception(EntityInfo.Columns.Any(it => it.IsIdentity == true), ErrorMessage.GetThrowMessage("Split table can't IsIdentity=true", "分表禁止使用自增列"));
        }

   
    }
}
