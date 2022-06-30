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
            var parentEntity = _ParentEntity;
            var parentList = _ParentList;
            var parentColumn = parentEntity.Columns.FirstOrDefault(it => it.PropertyName == nav.Navigat.Name);
            this._ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<TChild>();
            foreach (var item in parentList)
            {

            }
        }
    }
}
