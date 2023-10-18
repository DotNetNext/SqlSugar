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
        internal UpdateNavRootOptions _RootOptions { get; set; }
        public List<Root> _Roots { get; set; }
        public List<object> _ParentList { get; set; }
        public List<object> _RootList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public EntityColumnInfo _ParentPkColumn { get; set; }
        public SqlSugarProvider _Context { get; set; }

        public UpdateNavOptions _Options { get; set; }
        public bool IsFirst { get; set; }
        public bool IsAsNav { get;  set; }
        internal NavContext NavContext { get; set; }

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
            IsFirst = isRoot && this._ParentList == null;
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            if (_RootOptions != null && _RootOptions.IsDisableUpdateRoot)
            {
                //Future
            }
            else
            {
              
                UpdateRoot(isRoot, nav);
            }
            IsFirst = false;
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
            AddContextInfo(name,isRoot);
            return GetResult<TChild>();
        }
        private UpdateNavProvider<Root, TChild> _ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            var isRoot = _RootList == null;
            IsFirst = isRoot && this._ParentList == null;
            InitParentList();
            var name = ExpressionTool.GetMemberName(expression);
            var nav = this._ParentEntity.Columns.FirstOrDefault(x => x.PropertyName == name);
            if (nav.Navigat == null)
            {
                Check.ExceptionEasy($"{name} no navigate attribute", $"{this._ParentEntity.EntityName}的属性{name}没有导航属性");
            }
            UpdateRoot(isRoot, nav);
            IsFirst = false;
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
            AddContextInfo(name, isRoot);
            return GetResult<TChild>();
        }
        private void UpdateRoot(bool isRoot, EntityColumnInfo nav)
        {
            if (isRoot && nav.Navigat.NavigatType != NavigateType.ManyToMany&&_RootOptions?.IsDisableUpdateRoot!=true)
            {
                UpdateRoot();
            }
            else if (isRoot &&_RootOptions?.IsInsertRoot==true&& nav.Navigat.NavigatType == NavigateType.ManyToMany)
            {
                UpdateRoot();
            }
            else
            {
                if (_Options != null && _Options.ManyToManyIsUpdateA)
                {
                    UpdateRoot();
                }
            }
        }

        private void UpdateRoot()
        {
            if (IsAsNav) 
            {
                return;
            }
            if (_Options != null && _Options.RootFunc != null)
            {
                var updateable = this._Context.Updateable(_Roots);
                var exp = _Options.RootFunc as Expression<Action<IUpdateable<Root>>>;
                Check.ExceptionEasy(exp == null, "UpdateOptions.RootFunc is error", "UpdateOptions.RootFunc");
                var com = exp.Compile();
                com(updateable);
                updateable.ExecuteCommand();
            }
            else if (IsFirst && _RootOptions != null)
            {
                var isInsert = _RootOptions.IsInsertRoot;
                if (isInsert)
                {
                    var newRoots = new List<Root>();
                    foreach (var item in _Roots)
                    {
                        var x = this._Context.Storageable(item).ToStorage();
                        if (x.InsertList.HasValue())
                        {
                            newRoots.Add(x.AsInsertable.IgnoreColumns(_RootOptions.IgnoreInsertColumns).EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent, _RootOptions.DiffLogBizData).ExecuteReturnEntity());
                        }
                        else
                        {
                            x.AsUpdateable
                                .EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent, _RootOptions.DiffLogBizData)
                                .UpdateColumns(_RootOptions.UpdateColumns)
                                .IgnoreColumns(_RootOptions.IgnoreColumns)
                                .IgnoreNullColumns(_RootOptions.IsIgnoreAllNullColumns)
                                .ExecuteCommandWithOptLockIF(_RootOptions?.IsOptLock, _RootOptions?.IsOptLock);
                            newRoots.Add(item);
                        }
                    }
                    _ParentList = _RootList = newRoots.Cast<object>().ToList();
                }
                else
                {
                    this._Context.Updateable(_Roots)
                        .EnableDiffLogEventIF(_RootOptions.IsDiffLogEvent,_RootOptions.DiffLogBizData)
                        .UpdateColumns(_RootOptions.UpdateColumns)
                        .IgnoreColumns(_RootOptions.IgnoreColumns)
                        .IgnoreNullColumns(_RootOptions.IsIgnoreAllNullColumns)
                        .ExecuteCommandWithOptLockIF(_RootOptions?.IsOptLock, _RootOptions?.IsOptLock);
                }
            }
            else if (_RootOptions != null && _RootOptions?.IsDiffLogEvent == true) 
            {
                this._Context.Updateable(_Roots).EnableDiffLogEvent(_RootOptions.DiffLogBizData).ExecuteCommand();
            }
            else
            {
                this._Context.Updateable(_Roots).ExecuteCommand();
            }
        }

        private void AddContextInfo(string name, bool isRoot)
        {
            if (IsAsNav || isRoot)
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
