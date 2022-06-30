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
        private void InsertOneToOne<TChild>(string name, EntityColumnInfo nav) where TChild : class, new()
        {
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentColumn = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            this._ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            EntityColumnInfo pkColumn = GetOneTwoOneChildPkColumn(nav);
            Check.Exception(pkColumn == null, $" Navigate {parentEntity.EntityName} : {name} is error ", $"导航实体 {parentEntity.EntityName} 属性 {name} 配置错误");
            List<TChild> childList = new List<TChild>();
            foreach (var parent in parentList)
            {
                var navPropertyValue = parentColumn.PropertyInfo.GetValue(parent);
                var childItem = (TChild)nav.PropertyInfo.GetValue(parent);
                if (childItem != null)
                {
                    if (IsDefaultValue(navPropertyValue))
                    {
                        var pkValue = pkColumn.PropertyInfo.GetValue(childItem);
                        if (IsDefaultValue(navPropertyValue))
                        {
                            navPropertyValue = pkValue;
                        }
                    }
                    pkColumn.PropertyInfo.SetValue(childItem, navPropertyValue);
                    childList.Add(childItem);
                }
            }
            var x = this._Context.Storageable(childList).WhereColumns(new string[] { pkColumn.PropertyName }).ToStorage();
            if (pkColumn.IsIdentity)
            {
                InsertIdentity(x.InsertList.Select(it => it.Item).ToList());
            }
            else
            {
                x.AsInsertable.ExecuteCommand();
            }
            this._ParentList = childList.Cast<object>().ToList();
        }

        private EntityColumnInfo GetOneTwoOneChildPkColumn(EntityColumnInfo nav)
        {
            var pkColumn = _ParentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            if (nav.Navigat.Name2.HasValue())
            {
                pkColumn = _ParentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name2);
            }

            return pkColumn;
        }

    }
}
