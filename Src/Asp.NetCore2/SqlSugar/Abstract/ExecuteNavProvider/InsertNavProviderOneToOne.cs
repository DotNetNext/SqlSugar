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
            var parentPkColumn = parentEntity.Columns.FirstOrDefault(it => it.IsPrimarykey);
            var thisEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            EntityColumnInfo thisPkColumn = GetPkColumnByNav(thisEntity, nav);
            Check.ExceptionEasy(thisPkColumn == null, $" Navigate {parentEntity.EntityName} : {name} is error ", $"导航实体 {parentEntity.EntityName} 属性 {name} 配置错误");
            Check.ExceptionEasy(nav.Navigat.WhereSql.HasValue(), $" {name} Navigate(NavType,WhereSql)  no support insert ", $"导航一对一 {name} 配置了 Sql变量 不支持插入");
            List<TChild> childList = new List<TChild>();
            foreach (var parent in parentList)
            {
                var navPropertyValue = parentColumn.PropertyInfo.GetValue(parent);
                var childItem = (TChild)nav.PropertyInfo.GetValue(parent);
                if (childItem != null)
                {
                    if (IsDefaultValue(navPropertyValue))
                    {
                        var pkValue = thisPkColumn.PropertyInfo.GetValue(childItem);
                        if (IsDefaultValue(navPropertyValue))
                        {
                            navPropertyValue = pkValue;
                        }

                    }
                    if (!IsDefaultValue(navPropertyValue)&& parentColumn.IsPrimarykey==false) 
                    {
                        this._Context.Updateable<DbTableInfo>
                           ().AS(parentEntity.DbTableName)
                           .SetColumns(parentColumn.DbColumnName, navPropertyValue)
                           .Where(parentPkColumn.DbColumnName, "=", parentPkColumn.PropertyInfo.GetValue(parent)).ExecuteCommand();
                    }
                    if (IsDefaultValue(navPropertyValue)) 
                    {
                        InsertDatas<TChild>(new List<TChild>() { childItem }, thisPkColumn);
                        navPropertyValue = thisPkColumn.PropertyInfo.GetValue(childItem);
                        parentColumn.PropertyInfo.SetValue(parent,navPropertyValue);
                        this._Context.Updateable<DbTableInfo>
                            ().AS(parentEntity.DbTableName)
                            .SetColumns(parentColumn.DbColumnName, navPropertyValue)
                            .Where(parentPkColumn.DbColumnName,"=", parentPkColumn.PropertyInfo.GetValue(parent)).ExecuteCommand();
                    }
            
                    thisPkColumn.PropertyInfo.SetValue(childItem, navPropertyValue);
                    childList.Add(childItem);
                }
            }
            InsertDatas<TChild>(childList,thisPkColumn);
            this._ParentList = childList.Cast<object>().ToList();
            SetNewParent<TChild>(thisEntity, thisPkColumn);
        }
 
    }
}
