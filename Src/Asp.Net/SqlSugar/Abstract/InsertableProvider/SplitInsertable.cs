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
            var result = 0;
            var groups = TableNames.GroupBy(it => it.Key).ToList();
            var parent = ((InsertableProvider<T>)Inserable);
            var names = parent.InsertBuilder.DbColumnInfoList.GroupBy(it => it.DbColumnName).Select(i => i.Key).ToList();
            foreach (var item in groups)
            {
                var list = item.Select(it => it.Value as T).ToList();
                var groupInserable = (InsertableProvider<T>)this.Context.Insertable<T>(list);
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

        private void CreateTable()
        {
            var isLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            foreach (var item in TableNames)
            {
                if (!this.Context.DbMaintenance.IsAnyTable(item.Key, false)) 
                {
                    this.Context.MappingTables.Add(EntityInfo.EntityName, item.Key);
                    this.Context.CodeFirst.InitTables<T>();
                }
            }
            this.Context.Ado.IsEnableLogEvent = isLog;
            this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
        }
    }
}
