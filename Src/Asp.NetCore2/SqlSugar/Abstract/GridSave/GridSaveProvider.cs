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
        internal bool IsIncluesFirstAll { get; set; } 
        internal string[] IgnoreColumnsSaveInclues { get; set; }
        public bool ExecuteCommand()
        {
            var deleteList = GetDeleteList();
            this.Context.Deleteable(deleteList).PageSize(1000).ExecuteCommand();
            if (IsIncludesSave())
            {
                this.Context.Utilities.PageEach(SaveList, 1000, pageList =>
                {
                    var options = new UpdateNavRootOptions() { IsInsertRoot = true };
                    this.Context.UpdateNav(pageList, options)
                    .IncludesAllFirstLayer(IgnoreColumnsSaveInclues).ExecuteCommand();
                });
            }
            else
            {
                this.Context.Storageable(SaveList).PageSize(1000).ExecuteCommand();
            }
            return true;
        }

        public async Task<bool> ExecuteCommandAsync()
        {
            var deleteList= GetDeleteList();
            await this.Context.Deleteable(deleteList).PageSize(1000).ExecuteCommandAsync();
            if (IsIncludesSave()) 
            {
                await this.Context.Utilities.PageEachAsync(SaveList, 1000, async pageList =>
                {
                    var options = new UpdateNavRootOptions() { IsInsertRoot = true };
                    await this.Context.UpdateNav(pageList, options)
                    .IncludesAllFirstLayer(IgnoreColumnsSaveInclues).ExecuteCommandAsync();
                });
            }
            else
            {
                await this.Context.Storageable(SaveList).PageSize(1000).ExecuteCommandAsync();
            }
            return true;
        }  
         
        public List<T> GetDeleteList()
        {
            //下面代码ToDictionary会有重复错请修改
            string[] primaryKeys = this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Where(it => it.IsPrimarykey).Select(it => it.PropertyName).ToArray();
            var saveListDictionary = this.SaveList.Select(item =>
            new { Key = CreateCompositeKey(primaryKeys, item), Value = item });
            var deleteList = this.OldList.Where(oldItem =>
            {
                var compositeKey = CreateCompositeKey(primaryKeys, oldItem);
                return !saveListDictionary.Any(it=>it.Key==compositeKey);
            }).ToList();
            return deleteList;
        }

        public GridSaveProvider<T> IncludesAllFirstLayer(params string [] ignoreColumns)
        { 
            this.IsIncluesFirstAll = true;
            IgnoreColumnsSaveInclues = ignoreColumns;
            return this;
        }
         

        private bool IsIncludesSave()
        {
            return IsIncluesFirstAll && this.Context.EntityMaintenance.GetEntityInfo<T>().Columns.Any(it=>it.Navigat!=null);
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
