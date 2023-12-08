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
            if (_Options?.OneToManyInsertOrUpdate == true)
            {
                InsertOrUpdate<TChild>(name,nav);
            }
            else
            {
                DeleteInsert<TChild>(name, nav);
            }
        }
        private void InsertOrUpdate<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
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
                if (_Options?.OneToManyNoDeleteNull == true && childs == null)
                {
                    ids.Remove(parentValue);
                }
            } 
            if (NotAny(name))
            {
                DeleteMany(thisEntity, ids, thisFkColumn.DbColumnName);
                if (this._Options?.OneToManyEnableLogicDelete == true)
                {
                    var locgicColumn = thisEntity.Columns.FirstOrDefault(it => it.PropertyName.EqualCase("IsDeleted") || it.PropertyName.EqualCase("IsDelete"));
                    Check.ExceptionEasy(
                         locgicColumn == null,
                         thisEntity.EntityName + "Logical deletion requires the entity to have the IsDeleted property",
                         thisEntity.EntityName + "假删除需要实体有IsDeleted属性");
                    List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
                    conditionalModels.Add(new ConditionalModel()
                    {
                        FieldName = thisFkColumn.DbColumnName,
                        FieldValue = string.Join(",", ids.Distinct()),
                        ConditionalType = ConditionalType.In,
                        CSharpTypeName = thisFkColumn?.PropertyInfo?.PropertyType?.Name
                    });
                    var sqlObj = _Context.Queryable<object>().SqlBuilder.ConditionalModelToSql(conditionalModels);
                    this._Context.Updateable<object>()
                      .AS(thisEntity.DbTableName)
                      .Where(sqlObj.Key, sqlObj.Value)
                      .SetColumns(locgicColumn.DbColumnName, true)
                      .ExecuteCommand();
                }
                else
                {
                    var list=this._Context.Queryable<TChild>()
                        .AS(thisEntity.DbTableName)
                        .In(thisFkColumn.DbColumnName, ids.Distinct().ToList()) 
                        .ToList();
                    List<TChild> result = GetNoExistsId(list, children, thisPkColumn.PropertyName);
                    if (result.Any())
                    {
                        this._Context.Deleteable(result).ExecuteCommand();
                    }
                }
                _NavigateType = NavigateType.OneToMany;
                InsertDatas(children, thisPkColumn);
            }
            else
            {
                this._ParentList = children.Cast<object>().ToList();
            }
            _NavigateType = null;
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }
        private void DeleteInsert<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
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
                if (_Options?.OneToManyNoDeleteNull == true && childs == null)
                {
                    ids.Remove(parentValue);
                }
            }
            if (NotAny(name))
            {
                DeleteMany(thisEntity, ids, thisFkColumn.DbColumnName);
                if (this._Options?.OneToManyEnableLogicDelete == true)
                {
                    var locgicColumn = thisEntity.Columns.FirstOrDefault(it => it.PropertyName.EqualCase("IsDeleted") || it.PropertyName.EqualCase("IsDelete"));
                    Check.ExceptionEasy(
                         locgicColumn == null,
                         thisEntity.EntityName + "Logical deletion requires the entity to have the IsDeleted property",
                         thisEntity.EntityName + "假删除需要实体有IsDeleted属性");
                    List<IConditionalModel> conditionalModels = new List<IConditionalModel>();
                    conditionalModels.Add(new ConditionalModel()
                    {
                        FieldName = thisFkColumn.DbColumnName,
                        FieldValue = string.Join(",", ids.Distinct()),
                        ConditionalType = ConditionalType.In,
                        CSharpTypeName = thisFkColumn?.PropertyInfo?.PropertyType?.Name
                    });
                    var sqlObj = _Context.Queryable<object>().SqlBuilder.ConditionalModelToSql(conditionalModels);
                    this._Context.Updateable<object>()
                      .AS(thisEntity.DbTableName)
                      .Where(sqlObj.Key, sqlObj.Value)
                      .SetColumns(locgicColumn.DbColumnName, true)
                      .ExecuteCommand();
                }
                else
                {
                    if (this._Context?.CurrentConnectionConfig?.MoreSettings?.IsAutoDeleteQueryFilter == true)
                    {
                        this._Context.Deleteable<object>()
                           .AS(thisEntity.DbTableName)
                           .EnableQueryFilter(thisEntity.Type)
                           .In(thisFkColumn.DbColumnName, ids.Distinct().ToList()).ExecuteCommand();
                    }
                    else
                    {
                        this._Context.Deleteable<object>()
                            .AS(thisEntity.DbTableName) 
                            .In(thisFkColumn.DbColumnName, ids.Distinct().ToList()).ExecuteCommand();
                    }
                }
                _NavigateType = NavigateType.OneToMany;
                InsertDatas(children, thisPkColumn);
            }
            else
            {
                this._ParentList = children.Cast<object>().ToList();
            }
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

        public List<TChild> GetNoExistsId<TChild>(List<TChild> old, List<TChild> newList, string pkName)
        {
            List<TChild> result = new List<TChild>();

            // 将newList中的主键属性转换为字符串集合
            var newIds = newList.Select(item => GetPropertyValueAsString(item, pkName)).ToList();

            // 获取在old中但不在newList中的主键属性值
            result = old.Where(item => !newIds.Contains(GetPropertyValueAsString(item, pkName))) 
                        .ToList();

            return result;
        }

        // 获取对象的属性值
        private string GetPropertyValueAsString<TChild>(TChild item, string propertyName)
        {
            var property = item.GetType().GetProperty(propertyName);
            if (property != null)
            {
                return property.GetValue(item, null)+"";
            }
            else
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type {item.GetType().Name}");
            }
        }
    }
}
