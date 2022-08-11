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

        public UpdateNavOptions _Options { get; set; }
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
            return _ThenInclude(expression);
        }

        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            return _ThenInclude(expression);
        }

        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression,UpdateNavOptions options) where TChild : class, new()
        {
            _Options= options;  
            return _ThenInclude(expression);
        }

        public UpdateNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            _Options = options;
            return _ThenInclude(expression);
        }

        private UpdateNavProvider<Root, TChild> _ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            var isRoot = _RootList == null;
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            UpdateRoot(isRoot, nav);
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
        private UpdateNavProvider<Root, TChild> _ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            var isRoot = _RootList == null;
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            UpdateRoot(isRoot, nav);
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
        private void UpdateRoot(bool isRoot, EntityColumnInfo nav)
        {
            if (isRoot && nav.Navigat.NavigatType != NavigateType.ManyToMany)
            {
                this._Context.Updateable(_Roots).ExecuteCommand();
            }
            else
            {
                if (_Options != null && _Options.ManyToManyIsUpdateA)
                {
                    this._Context.Updateable(_Roots).ExecuteCommand();
                }
            }
        }

    }
}
