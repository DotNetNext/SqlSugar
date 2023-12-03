using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks; 

namespace SqlSugar 
{
    public class StorageableSplitProvider<T> where T:class,new()
    {
        internal Storageable<T> SaveInfo { get;   set; }
        internal SqlSugarProvider Context { get;   set; }
        internal List<T> List { get;   set; }
        internal EntityInfo EntityInfo { get;   set; }
        internal Expression<Func<T, object>> whereExpression { get;   set; }

        internal int pageSize = 1000;
        internal Action<int>  ActionCallBack =null;
        public StorageableSplitProvider<T> PageSize(int size, Action<int> ActionCallBack = null) 
        {
            this.pageSize = size;
            return this;
        }
        public int ExecuteCommand()
        {
            if (List.Count > pageSize)
            {
                var result = 0;
                this.Context.Utilities.PageEach(List, pageSize, pageItem =>
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
        public int ExecuteSqlBulkCopy()
        {
            if (List.Count > pageSize)
            {
                var result = 0;
                this.Context.Utilities.PageEach(List, pageSize, pageItem =>
                {
                    result += _ExecuteSqlBulkCopy(pageItem);
                });
                return result;
            }
            else
            {
                var list = List;
                return _ExecuteSqlBulkCopy(list);
            }

        }


        public async Task<int> ExecuteCommandAsync()
        {
            if (List.Count > pageSize)
            {
                var result = 0;
                await this.Context.Utilities.PageEachAsync(List, pageSize, async pageItem =>
                {
                    result +=await _ExecuteCommandAsync(pageItem);
                    if (ActionCallBack != null) 
                    {
                        ActionCallBack(result);
                    }
                });
                return result;
            }
            else
            {
                var list = List;
                return await _ExecuteCommandAsync(list);
            }
        }
        public async Task<int> ExecuteSqlBulkCopyAsync()
        {
            if (List.Count > pageSize)
            {
                var result = 0;
                await this.Context.Utilities.PageEachAsync(List, pageSize, async pageItem =>
                {
                    result += await _ExecuteSqlBulkCopyAsync(pageItem);
                    if (ActionCallBack != null)
                    {
                        ActionCallBack(result);
                    }
                });
                return result;
            }
            else
            {
                var list = List;
                return await _ExecuteSqlBulkCopyAsync(list);
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
                resultValue +=await this.Context.Storageable(addList).As(item.Key).WhereColumns(whereExpression).ExecuteCommandAsync();
                if (ActionCallBack != null)
                {
                    ActionCallBack(resultValue);
                }
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
                resultValue += this.Context.Storageable(addList).As(item.Key).WhereColumns(whereExpression).ExecuteCommand();
            }
            return result;
        }

        private async Task<int> _ExecuteSqlBulkCopyAsync(List<T> list)
        {
            int resultValue = 0;
            List<GroupModel> groupModels;
            int result;
            GroupDataList(list, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                resultValue += await this.Context.Storageable(addList).As(item.Key).WhereColumns(whereExpression).ExecuteSqlBulkCopyAsync();
                if (ActionCallBack != null)
                {
                    ActionCallBack(resultValue);
                }
            }
            return result;
        }
        private int _ExecuteSqlBulkCopy(List<T> list)
        {
            int resultValue = 0;
            List<GroupModel> groupModels;
            int result;
            GroupDataList(list, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                resultValue += this.Context.Storageable(addList).As(item.Key).WhereColumns(whereExpression).ExecuteSqlBulkCopy();
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
