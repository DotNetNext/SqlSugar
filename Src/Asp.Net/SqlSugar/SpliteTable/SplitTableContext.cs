using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class SplitTableContext
    {
        internal SqlSugarProvider Context { get; set; }
        internal EntityInfo EntityInfo { get; set; }
        internal ISplitTableService Service { get; set; }
        private SplitTableContext() { }
        internal SplitTableContext(SqlSugarProvider context) 
        {
            this.Context = context;
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

        public string GetDefaultTableName()
        {
           return Service.GetTableName(this.Context,EntityInfo);
        }
        public string GetTableName(SplitType splitType) 
        {
            return Service.GetTableName(this.Context,EntityInfo, splitType);
        }
        public string GetTableName(SplitType splitType, object fieldValue)
        {
            return Service.GetTableName(this.Context,EntityInfo, splitType, fieldValue);
        }
        public string GetTableName(object fieldValue)
        {
            var attribute = EntityInfo.Type.GetCustomAttribute<SplitTableAttribute>() as SplitTableAttribute;
            Check.Exception(attribute == null, $" {EntityInfo.EntityName} need SplitTableAttribute");
            return Service.GetTableName(this.Context, EntityInfo, attribute.SplitType, fieldValue);
        }
        public object GetValue(SplitType splitType, object entityValue)
        {
            return Service.GetFieldValue(this.Context,EntityInfo, splitType, entityValue);
        }
        internal void CheckPrimaryKey()
        {
            Check.Exception(EntityInfo.Columns.Any(it => it.IsIdentity == true), ErrorMessage.GetThrowMessage("Split table can't IsIdentity=true", "分表禁止使用自增列"));
        }
    }
}
