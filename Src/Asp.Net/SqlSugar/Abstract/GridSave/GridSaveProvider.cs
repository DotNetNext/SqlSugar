using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class GridSaveProvider<T> where T : class, new()
    {
        internal SqlSugarProvider Context { get;  set; }
        internal List<T> OldList { get;  set; }
        internal List<T> SaveList { get;  set; } 
        public bool ExecuteCommand()
        {
            var deleteList = GetDeleteList();
            this.Context.Deleteable(deleteList).PageSize(1000).ExecuteCommand();
            this.Context.Storageable(SaveList).PageSize(1000).ExecuteCommand();
            return true;
        }
        public async Task<bool> ExecuteCommandAsync()
        {
            var deleteList= GetDeleteList();
            await this.Context.Deleteable(deleteList).PageSize(1000).ExecuteCommandAsync();
            await this.Context.Storageable(SaveList).PageSize(1000).ExecuteCommandAsync();
            return true;
        }  
         
        public List<T> GetDeleteList()
        {
            string[] primaryKeys = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.PropertyName).ToArray();
            var saveListDictionary = this.SaveList.ToDictionary(item => CreateCompositeKey(primaryKeys, item));
            var deleteList = this.OldList.Where(oldItem =>
            {
                var compositeKey = CreateCompositeKey(primaryKeys, oldItem);
                return !saveListDictionary.ContainsKey(compositeKey);
            }).ToList();
            return deleteList;
        }

        private string CreateCompositeKey(string[] propertyNames, object obj)
        {
            var keyValues = propertyNames.Select(propertyName => GetPropertyValue(obj, propertyName)?.ToString() ?? "");
            return string.Join("|", keyValues);
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var property = obj.GetType().GetProperty(propertyName);
            return property != null ? property.GetValue(obj) : null;
        }
    }
}
