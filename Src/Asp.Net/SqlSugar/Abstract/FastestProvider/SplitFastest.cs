using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitFastest<T>where T:class,new()
    {
        public FastestProvider<T> FastestProvider { get;  set; }

        public int BulkCopy(List<T> datas)
        {
            List<GroupModel> groupModels;
            int result;
            GroupDataList(datas, out groupModels, out result);
            foreach (var item in groupModels.GroupBy(it => it.GroupName))
            {
                var addList = item.Select(it => it.Item).ToList();
                result += FastestProvider.BulkCopy(addList); ;
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
                var addList = item.Select(it => it.Item).ToList();
                result +=await FastestProvider.BulkCopyAsync(addList); ;
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
                var addList = item.Select(it => it.Item).ToList();
                result += FastestProvider.BulkUpdate(addList); ;
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
                var addList = item.Select(it => it.Item).ToList();
                result += await FastestProvider.BulkUpdateAsync(addList); ;
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
                result += FastestProvider.BulkUpdate(addList,wherColumns,updateColumns); ;
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
                result += await FastestProvider.BulkUpdateAsync(addList, wherColumns, updateColumns); ;
            }
            return result;
        }


        private void GroupDataList(List<T> datas, out List<GroupModel> groupModels, out int result)
        {
            groupModels = new List<GroupModel>();
            var db = FastestProvider.context;
            foreach (var item in datas)
            {
                var tableName = db.SplitHelper<T>().GetTableName(item);
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
