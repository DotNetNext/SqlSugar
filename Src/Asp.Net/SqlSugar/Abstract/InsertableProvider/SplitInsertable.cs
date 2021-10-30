using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitInsertable<T>  where T:class ,new()
    {
        public SqlSugarProvider Context;
        internal SplitTableContext Helper;
        public EntityInfo EntityInfo;
        public SplitType SplitType;
        internal IInsertable<T> Inserable { get;  set; }
        internal List<KeyValuePair<string,object>> TableNames { get;  set; }

        public int ExecuteCommand()
        {
            CreateTable();
            if (TableNames.Count == 1)
            {
                return Inserable.AS(TableNames.First().Key).ExecuteCommand();
            }
            else 
            {
                var result = 0;
                var groups = TableNames.GroupBy(it => it.Key).ToList();
                var parent = ((InsertableProvider<T>)Inserable);
                var names= parent.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(i=>i.Key).ToList();
                foreach (var item in groups)
                {
                    var groupInserable =(InsertableProvider<T>) this.Context.Insertable<T>(item.ToList());
                    groupInserable.InsertBuilder.TableWithString = parent.InsertBuilder.TableWithString;
                    groupInserable.RemoveCacheFunc = parent.RemoveCacheFunc;
                    groupInserable.diffModel = parent.diffModel;
                    groupInserable.IsEnableDiffLogEvent = parent.IsEnableDiffLogEvent;
                    groupInserable.InsertBuilder.IsNoInsertNull = parent.InsertBuilder.IsNoInsertNull;
                    groupInserable.IsOffIdentity = parent.IsOffIdentity;
                    result += groupInserable.AS(item.Key).InsertColumns(names.ToArray()).ExecuteCommand();
                }
                return result;
            }
        }

        private void CreateTable()
        {
            foreach (var item in TableNames)
            {
                if (!this.Context.DbMaintenance.IsAnyTable(item.Key, false)) 
                {
                    this.Context.MappingTables.Add(EntityInfo.EntityName, item.Key);
                    this.Context.CodeFirst.InitTables<T>();
                }
            }
            this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
        }
    }
}
