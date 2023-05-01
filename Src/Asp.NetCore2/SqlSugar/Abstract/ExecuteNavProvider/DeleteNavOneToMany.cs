using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {

        private void DeleteOneToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            var parentEntity = _ParentEntity;
            var prentList = _ParentList.Cast<T>().ToList();
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = GetPkColumnByNav(thisEntity, nav);
            var thisFkColumn = GetFKColumnByNav(thisEntity, nav);
            EntityColumnInfo parentPkColumn = GetParentPkColumn();
            EntityColumnInfo parentNavColumn = GetParentPkNavColumn(nav);
            if (parentNavColumn != null)
            {
                parentPkColumn = parentNavColumn;
            }

            if (!_IsDeletedParant)
                SetContext(() => this._Context.Deleteable(prentList)
                .EnableDiffLogEventIF(_RootOptions?.IsDiffLogEvent==true,_RootOptions?.DiffLogBizData)
                .ExecuteCommand());

            var ids = _ParentList.Select(it => parentPkColumn.PropertyInfo.GetValue(it)).ToList();
            var childList = GetChildList<TChild>().In(thisFkColumn.DbColumnName, ids).ToList();

            this._ParentList = childList.Cast<object>().ToList();
            this._ParentPkColumn = thisPkColumn;
            this._IsDeletedParant = true;

            SetContext(() => this._Context.Deleteable(childList).ExecuteCommand());
        }

        private ISugarQueryable<TChild> GetChildList<TChild>() where TChild : class, new()
        {
            var queryable = this._Context.Queryable<TChild>();
            if (_WhereList.HasValue())
            {
                foreach (var item in _WhereList)
                {
                    queryable.Where(item);
                }
                queryable.AddParameters(_Parameters);
            }
            return queryable;
        }

        private void SetContext(Action action)
        {
            var key = "_DeleteNavTask";
            if (this._Context.TempItems == null)
            {
                this._Context.TempItems = new Dictionary<string, object>();
            }
            if (!this._Context.TempItems.ContainsKey(key))
            {
                this._Context.TempItems.Add(key, null);
            }
            var oldTask = this._Context.TempItems[key];
            var newTask = new List<Action>();
            if (oldTask != null)
            {
                newTask = (List<Action>)oldTask;
            }
            newTask.Add(action);
            this._Context.TempItems[key] = newTask;
        }

        private EntityColumnInfo GetParentPkColumn()
        {
            EntityColumnInfo parentPkColumn = _ParentPkColumn;
            if (_ParentPkColumn == null)
            {
                parentPkColumn = _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            }
            return parentPkColumn;
        }
        private EntityColumnInfo GetParentPkNavColumn(EntityColumnInfo nav)
        {
            EntityColumnInfo result = null;
            if (nav.Navigat.Name2.HasValue())
            {
                result = _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
            }
            return result;
        }

        private EntityColumnInfo GetPkColumnByNav(EntityInfo entity, EntityColumnInfo nav)
        {
            var pkColumn = entity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            if (nav.Navigat.Name2.HasValue())
            {
                pkColumn = entity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
            }
            return pkColumn;
        }
        private EntityColumnInfo GetFKColumnByNav(EntityInfo entity, EntityColumnInfo nav)
        {
            var fkColumn = entity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            return fkColumn;
        }
    }
}
