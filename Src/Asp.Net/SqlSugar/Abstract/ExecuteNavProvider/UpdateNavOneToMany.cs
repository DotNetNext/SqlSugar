using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class UpdateNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        public NavigateType? _NavigateType { get; set; }
        private void UpdateOneToMany<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
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
            var ids = new List<object>();
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
                ids.Add(parentValue);
            }
            DeleteMany(thisEntity, ids, thisFkColumn.DbColumnName);
            this._Context.Deleteable<object>()
                .AS(thisEntity.DbTableName)
                .In(thisFkColumn.DbColumnName, ids.Distinct().ToList()).ExecuteCommand();
            _NavigateType = NavigateType.OneToMany;
            InsertDatas(children, thisPkColumn);
            _NavigateType = null;
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }
        private static bool ParentIsPk(EntityColumnInfo parentNavigateProperty)
        {
            return parentNavigateProperty != null &&
                   parentNavigateProperty.Navigat != null &&
                   parentNavigateProperty.Navigat.NavigatType == NavigateType.OneToMany &&
                   parentNavigateProperty.Navigat.Name2 == null;
        }
        private void DeleteMany(EntityInfo thisEntity, List<object> ids,string fkName)
        {
            if (_Options == null||_Options.OneToManyDeleteAll==false) 
            {
                return;
            }
            var oneToManys = thisEntity.Columns.Where(it => it.Navigat != null && it.Navigat.NavigatType == NavigateType.OneToMany).ToList();
            foreach (var oneToMany in oneToManys)
            {
                var fkFieldName = oneToMany.Navigat.Name2 ?? thisEntity.Columns.FirstOrDefault(it => it.IsPrimarykey).PropertyName;
                var fkDbColumnName = thisEntity.Columns.FirstOrDefault(it => it.PropertyName == fkFieldName).DbColumnName;
                var fks = this._Context.Queryable<object>()
                .AS(thisEntity.DbTableName)
                .In(fkName, ids.Distinct().ToList()).Select(fkDbColumnName).ToDataTable().Rows.Cast<System.Data.DataRow>().Select(x => x[0]).ToArray();

                var type = oneToMany.PropertyInfo.PropertyType.GenericTypeArguments[0];
                var entity = this._Context.EntityMaintenance.GetEntityInfo(type);
                var id = oneToMany.Navigat.Name;
                var column = entity.Columns.FirstOrDefault(it => it.PropertyName == id).DbColumnName;

                DeleteChild(fks, entity, column);

                this._Context.Deleteable<object>()
                                                .AS(entity.DbTableName)
                                                .In(column, fks.Distinct().ToList()).ExecuteCommand();
            }
        }

        private void DeleteChild(object[] fks, EntityInfo entity, string column)
        {
            var childs = entity.Columns.Where(it => it.Navigat != null && it.Navigat?.NavigatType == NavigateType.OneToMany).ToList();
            if (childs.Any())
            {
                var pkColumn = entity.Columns.First(it => it.IsPrimarykey);
                var pkIds = this._Context.Queryable<object>()
                                         .AS(entity.DbTableName)
                                         .In(column, fks.Distinct().ToList())
                                         .Select(pkColumn.DbColumnName).ToDataTable().Rows
                                         .Cast<System.Data.DataRow>().Select(it => it[0]).ToList();
                DeleteChildChild(pkIds, childs);
            }
        }

        int childIndex = 0;
        private void DeleteChildChild(List<object> ids, List<EntityColumnInfo> childs)
        {
            childIndex++;
            if (childIndex > 4)
            {
                Check.ExceptionEasy("Removing too many levels", "安全机制限制删除脏数据层级不能超过7层");
            }
            foreach (var columnInfo in childs)
            {
                var navigat = columnInfo.Navigat;
                var type = columnInfo.PropertyInfo.PropertyType.GenericTypeArguments[0];
                var thisEntity = this._Context.EntityMaintenance.GetEntityInfo(type);
                var fkColumn = thisEntity.Columns.FirstOrDefault(it => navigat.Name.EqualCase(it.PropertyName));
                var thisPkColumn = thisEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
                var childs2 = thisEntity.Columns.Where(it => it.Navigat != null && it.Navigat?.NavigatType == NavigateType.OneToMany).ToList(); ;
                if (childs2.Any())
                {
                    var pkIds = _Context.Queryable<object>().AS(thisEntity.DbTableName)
                                         .In(fkColumn.DbColumnName, ids)
                                         .Select(thisPkColumn.DbColumnName).ToDataTable().Rows
                                        .Cast<System.Data.DataRow>().Select(it => it[0]).ToList();

                    DeleteChildChild(pkIds, childs2);
                }
                _Context.Deleteable<object>().AS(thisEntity.DbTableName).In(fkColumn.DbColumnName, ids).ExecuteCommand();
            }
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

        private void SetNewParent<TChild>(EntityInfo entityInfo, EntityColumnInfo entityColumnInfo) where TChild : class, new()
        {
            this._ParentEntity = entityInfo;
            this._ParentPkColumn = entityColumnInfo;
        }
    }
}
