using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class InsertNavProvider<Root,T> where T : class,new() where Root:class,new()
    {

        private static bool IsDefaultValue(object pvValue)
        {
            return pvValue == null || pvValue.Equals(UtilMethods.GetDefaultValue(pvValue.GetType()));
        }
        private void InitParentList()
        {
            if (_RootList == null)
            {
               _RootList= _ParentList = GetRootList(_Roots).Cast<object>().ToList();
            }
            else if (_ParentList == null) 
            {
                _ParentList = _RootList;
                var pkColumn = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.FirstOrDefault(it => it.IsPrimarykey);
                this._ParentPkColumn = pkColumn;
            }
            IsFirst = false;
        }

        private InsertNavProvider<Root, TChild> GetResult<TChild>() where TChild : class, new()
        {
            return new InsertNavProvider<Root, TChild>()
            {
                _Context = this._Context,
                _ParentEntity = this._ParentEntity,
                _ParentList = this._ParentList,
                _Roots = this._Roots,
                _ParentPkColumn=this._ParentPkColumn,
                _RootList=this._RootList
            };
        }

        private List<Type> GetRootList<Type>(List<Type> datas) where Type : class, new()
        {
            List<Type> result = new List<Type>();
            this._Context.InitMappingInfo<Type>();
            var entity = this._Context.EntityMaintenance.GetEntityInfo<Type>();
            var pkColumn = entity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            InsertDatas(datas, pkColumn);
            this._ParentEntity = entity;
            result = datas;
            return result;
        }

        private void InsertIdentity<Type>(List<Type> datas) where Type : class, new()
        {
            foreach (var item in datas)
            {
                if (IsFirst&&_RootOptions!=null)
                {
                    this._Context.Insertable(item)
                        .IgnoreColumns(_RootOptions.IgnoreColumns)
                        .InsertColumns(_RootOptions.InsertColumns)
                        .EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent, _RootOptions.DiffLogBizData)
                        .ExecuteCommandIdentityIntoEntity();
                }
                else
                {
                    this._Context.Insertable(item).ExecuteCommandIdentityIntoEntity();
                }
            }
        }

        private EntityColumnInfo GetPkColumnByNav(EntityInfo entity,EntityColumnInfo nav)
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
        private void InsertDatas<TChild>(List<TChild> children, EntityColumnInfo pkColumn, EntityColumnInfo NavColumn=null) where TChild : class, new()
        {
            children = children.Distinct().ToList();
            if (pkColumn == null) 
            {
                Check.ExceptionEasy($"{typeof(TChild).Name} need primary key ", $"{typeof(TChild).Name}需要主键");
            }
            var x = this._Context.Storageable(children).WhereColumns(new string[] { pkColumn.PropertyName }).GetStorageableResult();
            var insertData = children = x.InsertList.Select(it => it.Item).ToList();
            var IsNoExistsNoInsert = _navOptions != null && _navOptions.OneToManyIfExistsNoInsert == true;
            if (_NavigateType == NavigateType.OneToMany && IsFirst == false && IsNoExistsNoInsert == false)
            {
                var updateData = x.UpdateList.Select(it => it.Item).ToList();
                ClearPk(updateData, pkColumn);
                insertData.AddRange(updateData);
            }
            else if (_NavigateType == NavigateType.OneToMany && IsNoExistsNoInsert == true) 
            {
                children = new List<TChild>();
                children.AddRange(x.InsertList.Select(it => it.Item).ToList());
                var updateData = x.UpdateList.Select(it => it.Item).ToList();
                children.AddRange(updateData);
            }
            Check.ExceptionEasy(pkColumn==null&&NavColumn==null,$"The entity is invalid",$"实体错误无法使用导航");
            InitData(pkColumn, insertData);
            this._ParentList = children.Cast<object>().ToList();
        }

        private  void ClearPk<TChild>(List<TChild> updateData, EntityColumnInfo pkColumn) where TChild : class, new()
        {
            foreach (var child in updateData)
            {
                var defaultValue =UtilMethods.DefaultForType(pkColumn.PropertyInfo.PropertyType);
                pkColumn.PropertyInfo.SetValue(child, defaultValue);
            }
        }

        private void InitData<TChild>(EntityColumnInfo pkColumn, List<TChild> insertData) where TChild : class, new()
        {
            if (pkColumn.IsIdentity || pkColumn.OracleSequenceName.HasValue())
            {
                InsertIdentity(insertData);
            }
            else if (pkColumn.UnderType == UtilConstants.LongType)
            {
                SetValue(pkColumn, insertData, ()=>SnowFlakeSingle.Instance.NextId());
            }
            else if (pkColumn.UnderType == UtilConstants.GuidType)
            {
                SetValue(pkColumn, insertData, () => Guid.NewGuid());
            }
            else if (pkColumn.UnderType == UtilConstants.StringType)
            {
                SetValue(pkColumn, insertData, () => Guid.NewGuid().ToString());
            }
            else
            {
                SetError(pkColumn, insertData);
            }
        }

        private void SetValue<TChild>(EntityColumnInfo pkColumn, List<TChild> insertData,Func<object> value) where TChild : class, new()
        {
            foreach (var child in insertData)
            {
                if (IsDefaultValue(pkColumn.PropertyInfo.GetValue(child)))
                {
                    pkColumn.PropertyInfo.SetValue(child, value());
                }
            }
            if (IsFirst && _RootOptions != null)
            {
                this._Context.Insertable(insertData)
                    .IgnoreColumns(_RootOptions.IgnoreColumns)
                    .InsertColumns(_RootOptions.InsertColumns)
                    .EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent, _RootOptions.DiffLogBizData)
                    .ExecuteCommand();
            }
            else
            {
                this._Context.Insertable(insertData).ExecuteCommand();
            }
        }
        private void SetError<TChild>(EntityColumnInfo pkColumn, List<TChild> insertData) where TChild : class, new()
        {
            foreach (var child in insertData)
            {
                if (IsDefaultValue(pkColumn.PropertyInfo.GetValue(child)))
                {
                    var name = pkColumn.EntityName + " " + pkColumn.DbColumnName;
                    Check.ExceptionEasy($"The field {name} is not an autoassignment type and requires an assignment", $"字段{name}不是可自动赋值类型需要赋值（并且不能是已存在值） , 可赋值类型有 自增、long、Guid、string");
                }
            }
            if (IsFirst && _RootOptions != null)
            {
                this._Context.Insertable(insertData)
                    .IgnoreColumns(_RootOptions.IgnoreColumns)
                    .InsertColumns(_RootOptions.InsertColumns)
                    .EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent, _RootOptions.DiffLogBizData)
                    .ExecuteCommand();
            }
            else
            {
                this._Context.Insertable(insertData).ExecuteCommand();
            }
        }
    }
}
