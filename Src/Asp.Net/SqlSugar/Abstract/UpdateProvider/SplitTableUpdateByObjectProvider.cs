using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitTableUpdateByObjectProvider<T> where T : class, new()
    {
        public SqlSugarProvider Context;
        public UpdateableProvider<T> updateobj;
        public T[] UpdateObjects { get; set; }

        public IEnumerable<SplitTableInfo> Tables { get; set; }

        public int ExecuteCommand()
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += this.Context.Updateable(addList).AS(item.Key).ExecuteCommand();
            }
            return result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(UpdateObjects, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result +=await this.Context.Updateable(addList).AS(item.Key).ExecuteCommandAsync();
            }
            return result;
        }

        private void GroupDataList(T[] datas, out List<GroupModel> groupModels, out int result)
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
            result = 0;
        }
        internal class GroupModel
        {
            public string GroupName { get; set; }
            public T Item { get; set; }
        }
    }
}
