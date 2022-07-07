using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public partial class DeleteNavProvider<Root, T> where T : class, new() where Root : class, new()
    {
        public List<Root> _Roots { get; set; }
        public List<object> _ParentList { get; set; }
        public List<object> _RootList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public EntityColumnInfo _ParentPkColumn { get; set; }
        public SqlSugarProvider _Context { get; set; }

        public DeleteNavTask<Root, TChild> ThenInclude< TChild>(Expression<Func<Root, TChild>> expression)
            where TChild : class, new()
        {
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                DeleteOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                DeleteOneToMany<TChild>(name, nav);
            }
            else
            {
                DeleteManyToMany<TChild>(name, nav);
            }
            return null;
        }

        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<Root, List<TChild>>> expression)
         where TChild : class, new()
        {
            throw new NotImplementedException();
        }
    }
}
