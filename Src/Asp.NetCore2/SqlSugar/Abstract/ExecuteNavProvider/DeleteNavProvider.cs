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
        internal DeleteNavOptions deleteNavOptions;
        public List<Root> _Roots { get; set; }
        public List<object> _ParentList { get; set; }
        public List<object> _RootList { get; set; }
        public EntityInfo _ParentEntity { get; set; }
        public EntityColumnInfo _ParentPkColumn { get; set; }
        public SqlSugarProvider _Context { get; set; }
        internal DeleteNavRootOptions _RootOptions { get; set; }
        public bool _IsDeletedParant { get; set; }
        public List<string> _WhereList = new List<string>();
        public List<SugarParameter> _Parameters = new List<SugarParameter>();
        public DeleteNavProvider<Root, TChild> ThenInclude< TChild>(Expression<Func<T, TChild>> expression)
            where TChild : class, new()
        {
            this._Context.InitMappingInfo<TChild>();
            InitParentList();
            Expression newExp = GetMamber(expression);
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
            return GetResult<TChild>();
        }

        public DeleteNavProvider<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression)
         where TChild : class, new()
        {
            this._Context.InitMappingInfo<TChild>();
            InitParentList();
            Expression newExp = GetMamber(expression);
            var name = ExpressionTool.GetMemberName(newExp);
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
            return GetResult<TChild>();
        }

        private  Expression GetMamber(Expression expression)
        {
            int i = 0;
            Expression newExp =ExpressionTool.GetLambdaExpressionBody(expression);
            while (newExp is MethodCallExpression)
            {
                var callMethod = (newExp as MethodCallExpression);
                ActionMethodCallExpression(callMethod);
                newExp = callMethod.Arguments[0];
                i++;
                Check.Exception(i > 10000, expression + "  is error");
            }
            return newExp;
        }


        private  void ActionMethodCallExpression(MethodCallExpression method)
        {
            var queryBuilder = GetQueryBuilder();
            NavigatManager<T> navigatManager = new NavigatManager<T>()
            {
                Context = this._Context 
            };
            if (method.Method.Name == "ToList") 
            {

            }
            else if (method.Method.Name == "Where")
            {
                navigatManager.CheckHasRootShortName(method.Arguments[0], method.Arguments[1]);
                var exp = method.Arguments[1];
                _WhereList.Add(" " + queryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
            }
            else if (method.Method.Name == "WhereIF")
            {
                var isOk = LambdaExpression.Lambda(method.Arguments[1]).Compile().DynamicInvoke();
                if (isOk.ObjToBool())
                {
                    var exp = method.Arguments[2];
                    navigatManager.CheckHasRootShortName(method.Arguments[1], method.Arguments[2]);
                    _WhereList.Add(" " + queryBuilder.GetExpressionValue(exp, ResolveExpressType.WhereSingle).GetString());
                }
            }
            if (queryBuilder.Parameters != null)
            {
                _Parameters.AddRange(queryBuilder.Parameters);
            }
        }

        private QueryBuilder GetQueryBuilder()
        { 
            return this._Context.Queryable<T>().QueryBuilder;
        }

        private DeleteNavProvider<Root, TChild> GetResult<TChild>() where TChild : class, new()
        {
            return new DeleteNavProvider<Root, TChild>()
            {
                _Context = this._Context,
                _ParentEntity = this._ParentEntity,
                _ParentList = this._ParentList,
                _Roots = this._Roots,
                _ParentPkColumn = this._ParentPkColumn,
                _RootList = this._RootList,
                _IsDeletedParant=this._IsDeletedParant
            };
        }
        public DeleteNavProvider<Root, Root> AsNav()
        {
            return new DeleteNavProvider<Root, Root>
            {
                _Context = _Context,
                _ParentEntity = null,
                _ParentList = null,
                _Roots = _Roots,
                _IsDeletedParant = this._IsDeletedParant,
                _ParentPkColumn = this._Context.EntityMaintenance.GetEntityInfo<Root>().Columns.First(it => it.IsPrimarykey)
            };
        }
        private void InitParentList()
        {
            this._ParentEntity = this._Context.EntityMaintenance.GetEntityInfo<T>();
            if (_RootList == null)
            {
                _RootList = _ParentList = _Roots.Cast<object>().ToList();
            }
            else if (_ParentList == null)
            {
                _ParentList = _RootList;
                var pkColumn = this._Context.EntityMaintenance.GetEntityInfo<T>().Columns.FirstOrDefault(it => it.IsPrimarykey);
                this._ParentPkColumn = pkColumn;
            }

        }
    }
}
