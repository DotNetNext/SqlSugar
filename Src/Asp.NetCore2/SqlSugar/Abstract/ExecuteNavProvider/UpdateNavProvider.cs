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

        public List<Root> _Roots { get; set; }
        public List<object> _ParentList { get; set; }
        public List<object> _RootList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public EntityColumnInfo _ParentPkColumn { get; set; }
        public SqlSugarProvider _Context { get; set; }


        public UpdateNavProvider<Root, Root> AsNav()
        {
            return new UpdateNavProvider<Root, Root>
            {
                _Context = _Context,
                _ParentEntity = null,
                _ParentList = null,
                _Roots = _Roots,
                _ParentPkColumn = this._Context.EntityMaintenance.GetEntityInfo<Root>().Columns.First(it => it.IsPrimarykey)
            };
        }
        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                UpdateOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                UpdateOneToMany<TChild>(name, nav);
            }
            else
            {
                UpdateManyToMany<TChild>(name, nav);
            }
            return GetResult<TChild>();
        }
        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                UpdateOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                UpdateOneToMany<TChild>(name, nav);
            }
            else
            {
                UpdateManyToMany<TChild>(name, nav);
            }
            return GetResult<TChild>();
        }
    }
}
