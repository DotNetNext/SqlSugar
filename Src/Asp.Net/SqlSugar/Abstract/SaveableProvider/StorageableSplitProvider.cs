using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks; 

namespace SqlSugar 
{
    public class StorageableSplitProvider<T> where T:class,new()
    {
        public Storageable<T> SaveInfo { get; internal set; }
        public SqlSugarProvider Context { get; internal set; }
        public List<T> List { get; internal set; }
        public EntityInfo EntityInfo { get; internal set; }
        public int PageSize = 1000;
        public int ExecuteCommand()
        {
            if (List.Count > PageSize)
            {
                var result = 0;
                this.Context.Utilities.PageEach(List, PageSize, pageItem =>
                {
                    result+= _ExecuteCommand(pageItem);
                });
                return result;
            }
            else
            {
                var list = List;
                return _ExecuteCommand(list);
            }
            
        }
         

        public async Task<int> ExecuteCommandAsync()
        {
            if (List.Count > PageSize)
            {
                var result = 0;
                this.Context.Utilities.PageEach(List, PageSize, async pageItem =>
                {
                    result +=await _ExecuteCommandAsync(pageItem);
                });
                return result;
            }
            else
            {
                var list = List;
                return await _ExecuteCommandAsync(list);
            }
        }
        private async Task<int> _ExecuteCommandAsync(List<T> list)
        {
            int resultValue = 0;
            List<GroupModel> groupModels;
            int result;
            GroupDataList(list, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                resultValue +=await this.Context.Storageable(addList).ExecuteCommandAsync();
            }
            return result;
        }
        private int _ExecuteCommand(List<T> list)
        {
            int resultValue = 0;
            List<GroupModel> groupModels;
            int result;
            GroupDataList(list, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                resultValue += this.Context.Storageable(addList).ExecuteCommand();
            }
            return result;
        }

        private void GroupDataList(List<T> datas, out List<GroupModel> groupModels, out int result)
        {
            var attribute = typeof(T).GetCustomAttribute<SplitTableAttribute>() as SplitTableAttribute;
            Check.Exception(attribute == null, $"{typeof(T).Name} need SplitTableAttribute");
            groupModels = new List<GroupModel>();
            var db = this.Context;
            foreach (var item in datas)
            {
                var value = db.SplitHelper<T>().GetValue(attribute.SplitType, item);
                var tableName = db.SplitHelper<T>().GetTableName(attribute.SplitType, value);
                groupModels.Add(new GroupModel() { GroupName = tableName, Item = item });
            }
            var tablenames = groupModels.Select(it => it.GroupName).Distinct().ToList();
            CreateTable(tablenames); 
            result = 0;
        }
        private void CreateTable(List<string> tableNames)
        {
            var isLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            foreach (var item in tableNames)
            {
                if (!this.Context.DbMaintenance.IsAnyTable(item, false))
                {
                    if (item != null)
                    {
                        this.Context.MappingTables.Add(EntityInfo.EntityName, item);
                        this.Context.CodeFirst.InitTables<T>();
                    }
                }
            }
            this.Context.Ado.IsEnableLogEvent = isLog;
            this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
        }
        internal class GroupModel
        {
            public string GroupName { get; set; }
            public T Item { get; set; }
        }
    }
}
