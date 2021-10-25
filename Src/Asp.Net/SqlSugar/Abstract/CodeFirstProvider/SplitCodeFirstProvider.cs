using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitCodeFirstProvider
    {
        public SqlSugarProvider Context;
        public void InitTables<T>()
        {
            //var oldMapping = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            SplitTableHelper helper = new SplitTableHelper()
            {
                Context = this.Context,
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo<T>()
            };
            helper.CheckPrimaryKey();
            var tables = helper.GetTables();
            //var oldMapingTables = this.Context.MappingTables;
            if (tables.Count >0)
            {
                foreach (var item in tables)
                {
                    this.Context.MappingTables.Add(helper.EntityInfo.EntityName, item.TableName);
                    this.Context.CodeFirst.InitTables(typeof(T));
                }
            }
            else
            {
                this.Context.MappingTables.Add(helper.EntityInfo.EntityName, helper.GetDefaultTableName());
                this.Context.CodeFirst.InitTables(typeof(T));
            }
            this.Context.MappingTables.Add(helper.EntityInfo.EntityName, helper.EntityInfo.DbTableName);
        }
    }
}
