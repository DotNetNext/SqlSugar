using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitInsertable<T> 
    {
        public SqlSugarProvider Context;
        internal SplitTableContext Helper;
        public EntityInfo EntityInfo;
        public SplitType SplitType;
        internal IInsertable<T> Inserable { get;  set; }
        internal List<string> TableNames { get;  set; }

        public int ExecuteCommand()
        {
            CreateTable();
            if (TableNames.Count == 1)
            {
                return Inserable.AS(TableNames.First()).ExecuteCommand();
            }
            else 
            {
                return 0;
            }
        }

        private void CreateTable()
        {
            foreach (var item in TableNames)
            {
                if (!this.Context.DbMaintenance.IsAnyTable(item, false)) 
                {
                    this.Context.MappingTables.Add(EntityInfo.EntityName, item);
                    this.Context.CodeFirst.InitTables<T>();
                }
            }
            this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
        }
    }
}
