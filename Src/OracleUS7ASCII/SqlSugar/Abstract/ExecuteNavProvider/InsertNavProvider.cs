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
        public InsertNavRootOptions _RootOptions { get; set; }
        public List<Root> _Roots { get;  set; }
        public List<object> _ParentList { get; set; }
        public List<object> _RootList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public EntityColumnInfo _ParentPkColumn { get; set; }
        public SqlSugarProvider _Context { get;   set; }
        public NavigateType? _NavigateType { get; set; } 
        public bool IsFirst { get; set; }
        public InsertNavOptions _navOptions { get; set; }
        public bool IsNav { get; internal set; }
        internal NavContext NavContext { get; set; }

        public InsertNavProvider<Root, Root> AsNav()
        {
            return new InsertNavProvider<Root, Root> { 
             _Context = _Context,  
             _ParentEntity = null,
              _ParentList=null,
               _Roots= _Roots ,
               _ParentPkColumn=this._Context.EntityMaintenance.GetEntityInfo<Root>().Columns.First(it=>it.IsPrimarykey)
            };
        }

        public InsertNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression,InsertNavOptions options) where TChild : class, new()
        {
            _navOptions = options;
            return _ThenInclude(expression);
        }
        public InsertNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression, InsertNavOptions options) where TChild : class, new()
        {
            _navOptions = options;
            return _ThenInclude(expression);
        }

        public InsertNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            return _ThenInclude(expression);
        }
        public InsertNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            return _ThenInclude(expression);
        }



        private InsertNavProvider<Root, TChild> _ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            var name = ExpressionTool.GetMemberName(expression);
            var isRoot = false;
            if (this._ParentEntity == null)
            {
                this._ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<Root>();
                this.IsFirst = true;
                isRoot = true;
            }
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                InitParentList();
                InsertOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                _NavigateType = NavigateType.OneToMany;
                InitParentList();
                InsertOneToMany<TChild>(name, nav);
            }
            else
            {
                InitParentList();
                InsertManyToMany<TChild>(name, nav);
            }
            AddContextInfo(name,isRoot);
            return GetResult<TChild>();
        }

        private InsertNavProvider<Root, TChild> _ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            var name = ExpressionTool.GetMemberName(expression);
            var isRoot = false;
            if (this._ParentEntity == null)
            {
                this._ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<Root>();
                IsFirst = true;
                isRoot = true;
            }
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (nav.Navigat.NavigatType == NavigateType.OneToOne || nav.Navigat.NavigatType == NavigateType.ManyToOne)
            {
                InitParentList();
                InsertOneToOne<TChild>(name, nav);
            }
            else if (nav.Navigat.NavigatType == NavigateType.OneToMany)
            {
                _NavigateType = NavigateType.OneToMany;
                InitParentList();
                InsertOneToMany<TChild>(name, nav);
            }
            else
            {
                InitParentList();
                InsertManyToMany<TChild>(name, nav);
            }
            AddContextInfo(name,isRoot);
            return GetResult<TChild>();
        }

        private void AddContextInfo(string name,bool isRoot)
        {
            if (IsNav || isRoot)
            {
                if (this.NavContext != null && this.NavContext.Items != null)
                {
                    this.NavContext.Items.Add(new NavContextItem() { Level = 0, RootName = name });
                }
            }
        }
        private bool NotAny(string name)
        {
            if (IsFirst) return true;
            if (this.NavContext == null) return true;
            return this.NavContext?.Items?.Any(it => it.RootName == name) == false;
        }
    }
}
