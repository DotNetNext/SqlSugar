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
            var type = typeof(T);
            InitTables(type);
        }

        public void InitTables(Type type)
        {
            //var oldMapping = this.Context.Utilities.TranslateCopy(this.Context.MappingTables);
            SplitTableContext helper = new SplitTableContext(Context)
            {
                EntityInfo = this.Context.EntityMaintenance.GetEntityInfo(type)
            };
            helper.CheckPrimaryKey();
            var tables = helper.GetTables();
            //var oldMapingTables = this.Context.MappingTables;
            if (tables.Count > 0)
            {
                foreach (var item in tables)
                {
                    this.Context.MappingTables.Add(helper.EntityInfo.EntityName, item.TableName);
                    this.Context.CodeFirst.InitTables(type);
                }
            }
            else
            {
                this.Context.MappingTables.Add(helper.EntityInfo.EntityName, helper.GetDefaultTableName());
                this.Context.CodeFirst.InitTables(type);
            }
            this.Context.MappingTables.Add(helper.EntityInfo.EntityName, helper.EntityInfo.DbTableName);
        }

        public void InitTables(Type [] types) 
        {
            foreach (var type in types)
            {
                InitTables(type);
            }
        }
    }
}
