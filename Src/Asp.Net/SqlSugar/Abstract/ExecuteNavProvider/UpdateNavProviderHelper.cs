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

        private static bool IsDefaultValue(object pvValue)
        {
            return pvValue == null || pvValue.Equals(UtilMethods.GetDefaultValue(pvValue.GetType()));
        }
        private void InitParentList()
        {
            if (_RootList == null)
            {
                _RootList = _ParentList = _Roots.Cast<object>().ToList();
                _ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<Root>();
            }
            else if (_ParentList == null)
            {
                _ParentList = _RootList;
                var pkColumn = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.FirstOrDefault(it => it.IsPrimarykey);
                this._ParentPkColumn = pkColumn;
            }
        }

        private UpdateNavProvider<Root, TChild> GetResult<TChild>() where TChild : class, new()
        {
            return new UpdateNavProvider<Root, TChild>()
            {
                _Context = this._Context,
                _ParentEntity = this._ParentEntity,
                _ParentList = this._ParentList,
                _Roots = this._Roots,
                _ParentPkColumn = this._ParentPkColumn,
                _RootList = this._RootList
            };
        }
 

        private void InsertIdentity<Type>(List<Type> datas) where Type : class, new()
        {
            foreach (var item in datas)
            {
                this._Context.Insertable(item).ExecuteCommandIdentityIntoEntity();
            }
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
        private EntityColumnInfo GetPkColumnByNav2(EntityInfo entity, EntityColumnInfo nav)
        {
            var pkColumn = entity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            return pkColumn;
        }
        private EntityColumnInfo GetFKColumnByNav(EntityInfo entity, EntityColumnInfo nav)
        {
            var fkColumn = entity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            return fkColumn;
        }
        private void InsertDatas<TChild>(List<TChild> children, EntityColumnInfo pkColumn, EntityColumnInfo NavColumn = null) where TChild : class, new()
        {
            children = children.Distinct().ToList();
            var x = this._Context.Storageable(children).WhereColumns(new string[] { pkColumn.PropertyName }).ToStorage();
            var insertData  = x.InsertList.Select(it => it.Item).ToList();
            var updateData  = x.UpdateList.Select(it => it.Item).ToList();
            Check.ExceptionEasy(pkColumn == null && NavColumn == null, $"The entity is invalid", $"实体错误无法使用导航");
            if (_Options != null && _Options.CurrentFunc != null)
            {
                var updateable = x.AsUpdateable;
                var exp = _Options.CurrentFunc as Expression<Action<IUpdateable<TChild>>>;
                Check.ExceptionEasy(exp == null, "UpdateOptions.CurrentFunc is error", "UpdateOptions.CurrentFunc参数设置错误");
                var com = exp.Compile();
                com(updateable);
                if (IsDeleted)
                {
                    updateable.PageSize(1).EnableQueryFilter().ExecuteCommand();
                }
                else
                {
                    updateable.ExecuteCommand();
                }
            }
            else if (pkColumn.IsPrimarykey == false) 
            {
               var pk= this._Context.EntityMaintenance.GetEntityInfo<TChild>().Columns.Where(it => it.IsPrimarykey);
                List<string> ignoreColumns = new List<string>();
                if (_Options?.IgnoreColumns != null) 
                {
                    ignoreColumns.AddRange(_Options.IgnoreColumns);
                }
                if (pk.Any()) 
                {
                    ignoreColumns.AddRange(pk.Select(it=>it.PropertyName));
                }
                if (IsDeleted)
                {
                    x.AsUpdateable.IgnoreColumns(ignoreColumns.ToArray()).PageSize(1).EnableQueryFilter().ExecuteCommand();
                }
                else
                {
                    x.AsUpdateable.IgnoreColumns(ignoreColumns.ToArray()).ExecuteCommand();
                }
            }
            else
            {
                var ignoreColumns = _Options?.IgnoreColumns;
                if (IsDeleted)
                {
                    x.AsUpdateable.IgnoreColumns(ignoreColumns?.ToArray()).PageSize(1).EnableQueryFilter().ExecuteCommand();
                }
                else
                {
                    x.AsUpdateable.IgnoreColumns(ignoreColumns?.ToArray()).ExecuteCommand();
                }
            }
            InitData(pkColumn, insertData);
            if (_NavigateType == NavigateType.OneToMany)
            {
                this._ParentList = children.Cast<object>().ToList();
            }
            else
            {
                this._ParentList = insertData.Union(updateData).Cast<object>().ToList();
            }
        }

        private void InitData<TChild>(EntityColumnInfo pkColumn, List<TChild> UpdateData) where TChild : class, new()
        {
            if (pkColumn.IsIdentity || pkColumn.OracleSequenceName.HasValue())
            {
                InsertIdentity(UpdateData);
            }
            else if (pkColumn.UnderType == UtilConstants.LongType)
            {
                SetValue(pkColumn, UpdateData, () => SnowFlakeSingle.Instance.NextId());
            }
            else if (pkColumn.UnderType == UtilConstants.GuidType)
            {
                SetValue(pkColumn, UpdateData, () => Guid.NewGuid());
            }
            else if (pkColumn.UnderType == UtilConstants.StringType)
            {
                SetValue(pkColumn, UpdateData, () => Guid.NewGuid().ToString());
            }
            else
            {
                SetError(pkColumn, UpdateData);
            }
        }

        private void SetValue<TChild>(EntityColumnInfo pkColumn, List<TChild> UpdateData, Func<object> value) where TChild : class, new()
        {
            foreach (var child in UpdateData)
            {
                if (IsDefaultValue(pkColumn.PropertyInfo.GetValue(child)))
                {
                    pkColumn.PropertyInfo.SetValue(child, value());
                }
            }
            this._Context.Insertable(UpdateData).ExecuteCommand();
        }
        private void SetError<TChild>(EntityColumnInfo pkColumn, List<TChild> UpdateData) where TChild : class, new()
        {
            foreach (var child in UpdateData)
            {
                if (IsDefaultValue(pkColumn.PropertyInfo.GetValue(child)))
                {
                    var name = pkColumn.EntityName + " " + pkColumn.DbColumnName;
                    Check.ExceptionEasy($"The field {name} is not an autoassignment type and requires an assignment", $"字段{name}不是可自动赋值类型，需要赋值 , 可赋值类型有 自增、long、Guid、string");
                }
            }
            this._Context.Insertable(UpdateData).ExecuteCommand();
        }
    }
}
