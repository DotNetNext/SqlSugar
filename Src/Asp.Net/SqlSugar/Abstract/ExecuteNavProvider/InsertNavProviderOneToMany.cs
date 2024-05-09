using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar 
{
    public partial class InsertNavProvider<Root, T> where T : class, new() where Root : class, new()
    {

        private void InsertOneToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            List<TChild> children = new List<TChild>();
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentNavigateProperty = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == name);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            var thisPkColumn = GetPkColumnByNav2(thisEntity, nav);
            var thisFkColumn = GetFKColumnByNav(thisEntity, nav);
            EntityColumnInfo parentPkColumn = GetParentPkColumn();
            EntityColumnInfo parentNavColumn = GetParentPkNavColumn(nav);
            if (parentNavColumn != null)
            {
                parentPkColumn = parentNavColumn;
            }
            if (ParentIsPk(parentNavigateProperty))
            {
                parentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            }
            foreach (var item in parentList)
            {
                var parentValue = parentPkColumn.PropertyInfo.GetValue(item);
                var childs = parentNavigateProperty.PropertyInfo.GetValue(item) as List<TChild>;
                if (childs != null)
                {
                    foreach (var child in childs)
                    {
                        thisFkColumn.PropertyInfo.SetValue(child, parentValue, null);
                    }
                    children.AddRange(childs);
                }
            }
            var isTreeChild = GetIsTreeChild(parentEntity, thisEntity);
            Check.ExceptionEasy(thisPkColumn == null, $"{thisEntity.EntityName}need primary key", $"实体{thisEntity.EntityName}需要主键");
            if (NotAny(name) || isTreeChild)
            {
                InsertDatas(children, thisPkColumn);
            }
            else
            {
                this._ParentList = children.Cast<object>().ToList();
            }
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }

        private bool GetIsTreeChild(EntityInfo parentEntity , EntityInfo thisEntity)
        {
            return this.NavContext?.Items?.Any() == true && parentEntity.Type == thisEntity.Type;
        }

        private static bool ParentIsPk(EntityColumnInfo parentNavigateProperty)
        {
            return parentNavigateProperty != null && 
                   parentNavigateProperty.Navigat != null && 
                   parentNavigateProperty.Navigat.NavigatType == NavigateType.OneToMany && 
                   parentNavigateProperty.Navigat.Name2==null;
        }

        private EntityColumnInfo GetParentPkColumn()
        {
            EntityColumnInfo parentPkColumn = _ParentPkColumn;
            if (_ParentPkColumn == null)
            {
                parentPkColumn= _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            }
            return parentPkColumn;
        }
        private EntityColumnInfo GetParentPkNavColumn(EntityColumnInfo nav)
        {
            EntityColumnInfo result = null;
            if (nav.Navigat.Name2.HasValue())
            {
                result = _ParentPkColumn = this._ParentEntity.Columns.FirstOrDefault(it => it.PropertyName== nav.Navigat.Name2);
            }
            return result;
        }

        private void SetNewParent<TChild>(EntityInfo entityInfo,EntityColumnInfo entityColumnInfo) where TChild : class, new()
        {
            this._ParentEntity = entityInfo;
            this._ParentPkColumn = entityColumnInfo;
        }
    }
}
