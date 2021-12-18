using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitFastest<T>where T:class,new()
    {
        public FastestProvider<T> FastestProvider { get;  set; }
        public SqlSugarProvider Context { get { return this.FastestProvider.context; } }
        public EntityInfo EntityInfo { get { return this.Context.EntityMaintenance.GetEntityInfo<T>(); } }
        public int BulkCopy(List<T> datas)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                CreateTable(item.Key);
                var addList = item.Select(it => it.Item).ToList();
                result += FastestProvider.AS(item.Key).BulkCopy(addList);
                this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
            }
            return result;
        }
        public async Task<int> BulkCopyAsync(List<T> datas)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                CreateTable(item.Key);
                var addList = item.Select(it => it.Item).ToList();
                result +=await FastestProvider.AS(item.Key).BulkCopyAsync(addList);
                this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
            }
            return result;
        }


        public int BulkUpdate(List<T> datas)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                CreateTable(item.Key);
                var addList = item.Select(it => it.Item).ToList();
                result += FastestProvider.AS(item.Key).BulkUpdate(addList);
                this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
            }
            return result;
        }
        public async Task<int> BulkUpdateAsync(List<T> datas)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                CreateTable(item.Key);
                var addList = item.Select(it => it.Item).ToList();
                result += await FastestProvider.AS(item.Key).BulkUpdateAsync(addList);
                this.Context.MappingTables.Add(EntityInfo.EntityName, EntityInfo.DbTableName);
            }
            return result;
        }


        public int BulkUpdate(List<T> datas,string [] wherColumns,string [] updateColumns)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += FastestProvider.AS(item.Key).BulkUpdate(addList,wherColumns,updateColumns); ;
            }
            return result;
        }
        public async Task<int> BulkUpdateAsync(List<T> datas, string[] wherColumns, string[] updateColumns)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += await FastestProvider.AS(item.Key).BulkUpdateAsync(addList, wherColumns, updateColumns); ;
            }
            return result;
        }
        private void CreateTable(string tableName)
        {
            var isLog = this.Context.Ado.IsEnableLogEvent;
            this.Context.Ado.IsEnableLogEvent = false;
            if (!this.Context.DbMaintenance.IsAnyTable(tableName, false))
            {
                this.Context.MappingTables.Add(EntityInfo.EntityName, tableName);
                this.Context.CodeFirst.InitTables<T>();
            }
            this.Context.Ado.IsEnableLogEvent = isLog;
        }

        private void GroupDataList(List<T> datas, out List<GroupModel> groupModels, out int result)
        {
            var attribute = typeof(T).GetCustomAttribute<SplitTableAttribute>() as SplitTableAttribute;
            Check.Exception(attribute == null, $"{typeof(T).Name} need SplitTableAttribute");
            groupModels = new List<GroupModel>();
            var db = FastestProvider.context;
            foreach (var item in datas)
            {
                var value = db.SplitHelper<T>().GetValue(attribute.SplitType, item);
                var tableName = db.SplitHelper<T>().GetTableName(attribute.SplitType,value);
                groupModels.Add(new GroupModel() { GroupName = tableName, Item = item });
            }
            result = 0;
        }
        internal class GroupModel 
        {
            public string GroupName { get; set; }
            public T Item { get; set; }
        }
    }
    
}
