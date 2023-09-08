using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        private void DeleteOneToOne<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            var parentEntity = _ParentEntity;
            var parentList = _ParentList.Cast<T>().ToList();
            var parentColumn = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            var parentPkColumn = parentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            EntityColumnInfo thisPkColumn = GetPkColumnByNav(thisEntity, nav);
            Check.Exception(thisPkColumn == null, $" Navigate {parentEntity.EntityName} : {name} is error ", $"导航实体 {parentEntity.EntityName} 属性 {name} 配置错误");
 

            if (!_IsDeletedParant)
                SetContext(() => this._Context.Deleteable(parentList)
                .EnableDiffLogEventIF(_RootOptions?.IsDiffLogEvent == true, _RootOptions?.DiffLogBizData)
                .ExecuteCommand());

            Check.ExceptionEasy(parentColumn == null, "The one-to-one navigation configuration is incorrect", "一对一导航配置错误");
            var ids = _ParentList.Select(it => parentColumn.PropertyInfo.GetValue(it)).ToList();
            List<TChild> childList = this._Context.Queryable<TChild>().In(thisPkColumn.DbColumnName, ids).ToList();

            this._ParentList = childList.Cast<object>().ToList();
            this._ParentPkColumn = thisPkColumn;
            this._IsDeletedParant = true;

            SetContext(() => this._Context.Deleteable(childList).ExecuteCommand());
        }

    }
}
