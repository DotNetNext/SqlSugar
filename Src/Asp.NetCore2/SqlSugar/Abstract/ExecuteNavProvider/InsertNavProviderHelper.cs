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
            return pvValue == null || pvValue == UtilMethods.GetDefaultValue(pvValue.GetType());
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
                this._Context.Insertable(item).ExecuteCommandIdentityIntoEntity();
            }
        }

        private EntityColumnInfo GetPkColumnByNav(EntityInfo entity,EntityColumnInfo nav)
        {
            var pkColumn = entity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            if (nav.Navigat.Name2.HasValue())
            {
                pkColumn = _ParentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
            }
            return pkColumn;
        }
        private EntityColumnInfo GetFKColumnByNav(EntityInfo entity, EntityColumnInfo nav)
        {
            var fkColumn = entity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            return fkColumn;
        }
        private void InsertDatas<TChild>(List<TChild> children, EntityColumnInfo pkColumn) where TChild : class, new()
        {
            children = children.Distinct().ToList();
            var x = this._Context.Storageable(children).WhereColumns(new string[] { pkColumn.PropertyName }).ToStorage();
            var insertData = children = x.InsertList.Select(it => it.Item).ToList();
            if (pkColumn.IsIdentity)
            {
                InsertIdentity(insertData);
            }
            else
            {
                this._Context.Insertable(insertData).ExecuteCommand();
            }
            this._ParentList = children.Cast<object>().ToList();
        }
    }
}
