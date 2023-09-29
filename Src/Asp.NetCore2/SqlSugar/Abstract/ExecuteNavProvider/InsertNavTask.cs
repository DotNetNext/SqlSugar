using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class InsertNavTaskInit<Root, T> where T : class, new() where Root : class, new() 
    {

        internal SqlSugarProvider Context { get; set; }
        internal InsertNavProvider<Root, Root> insertNavProvider { get; set; }
        internal NavContext NavContext { get;  set; }
        public InsertNavMethodInfo IncludeByNameString(string navMemberName, UpdateNavOptions updateNavOptions = null)
        {
            InsertNavMethodInfo result = new InsertNavMethodInfo();
            result.Context = insertNavProvider._Context;
            var entityInfo = result.Context.EntityMaintenance.GetEntityInfo<T>();
            Type properyItemType;
            bool isList;
            Expression exp = UtilMethods.GetIncludeExpression(navMemberName, entityInfo, out properyItemType, out isList);
            var method = this.GetType().GetMyMethod("Include", 2, isList)
                            .MakeGenericMethod(properyItemType);
            var obj = method.Invoke(this, new object[] { exp, updateNavOptions });
            result.MethodInfos = obj;
            return result;
        }
        public InsertNavMethodInfo IncludesAllFirstLayer(params string[] ignoreColumns)
        {
            if (ignoreColumns == null)
            {
                ignoreColumns = new string[] { };
            }
            this.Context = insertNavProvider._Context;
            var navColumns = this.Context.EntityMaintenance.GetEntityInfo<Root>().Columns.Where(it => !ignoreColumns.Contains(it.PropertyName) || !ignoreColumns.Any(z => z.EqualCase(it.DbColumnName))).Where(it => it.Navigat != null).ToList();
            var updateNavs = this;
            InsertNavMethodInfo methodInfo = updateNavs.IncludeByNameString(navColumns[0].PropertyName);
            foreach (var item in navColumns.Skip(1))
            {
                methodInfo = methodInfo.IncludeByNameString(item.PropertyName);
            }
            return methodInfo;
        }
        public InsertNavTask<Root, TChild>  Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            Check.ExceptionEasy(typeof(TChild).FullName.Contains("System.Collections.Generic.List`"), "  need  where T: class, new() ", "需要Class,new()约束，并且类属性中不能有required修饰符");
            this.Context = insertNavProvider._Context;
            insertNavProvider.NavContext = this.NavContext;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild>  Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            this.Context = insertNavProvider._Context;
            insertNavProvider.NavContext = this.NavContext;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }

        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression,InsertNavOptions options) where TChild : class, new()
        {
            Check.ExceptionEasy(typeof(TChild).FullName.Contains("System.Collections.Generic.List`"), "  need  where T: class, new() ", "需要Class,new()约束，并且类属性中不能有required修饰符");
            this.Context = insertNavProvider._Context;
            insertNavProvider.NavContext = this.NavContext;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression, options);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression, InsertNavOptions options) where TChild : class, new()
        {
            this.Context = insertNavProvider._Context;
            insertNavProvider.NavContext =this.NavContext;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression, options);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
    }
    public class InsertNavTask<Root, T> where T : class, new() where Root : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public Func<InsertNavProvider<Root, T>> PreFunc { get;  set; }
        internal NavContext NavContext { get;  set; }

        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => {
                var nav = PreFunc().ThenInclude(expression);
                nav.NavContext = this.NavContext;
                return nav;
            };
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () =>
            {
                var nav = PreFunc().ThenInclude(expression);
                nav.NavContext = this.NavContext;
                return nav;
            };
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }
        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }



        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression,InsertNavOptions options) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression,options);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression, InsertNavOptions options) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression, options);
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression, InsertNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression, options);
        }
        public InsertNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression, InsertNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression, options);
        }

        public Root ExecuteReturnEntity()
        {
            var hasTran = this.Context.Ado.Transaction != null;
            if (hasTran)
            {
               return (Root)PreFunc()?._RootList?.FirstOrDefault();
            }
            else
            {
                Root result = null;
                this.Context.Ado.UseTran(() =>
                {
                    result= (Root)PreFunc()?._RootList?.FirstOrDefault();
                }, ex => throw ex);
                return result;
            }
        }
        public async Task<Root> ExecuteReturnEntityAsync()
        {
            Root result = null;
            await Task.Run(async () =>
            {
                result=ExecuteReturnEntity();
                await Task.Delay(0);
            });
            return result;
        }

        public bool ExecuteCommand()
        {
            var hasTran = this.Context.Ado.Transaction != null;
            if (hasTran)
            {
                PreFunc();
            }
            else 
            {
                this.Context.Ado.UseTran(() =>
                {
                    PreFunc();
                }, ex => throw ex);
            }
            return true;
        }
        public async Task<bool> ExecuteCommandAsync()
        {
            await Task.Run(async () => 
            {
                ExecuteCommand();
                await Task.Delay(0);
            });
            return true;
        }

        private InsertNavTask<Root, Root> AsNav()
        {
            InsertNavTask<Root, Root> result = new InsertNavTask<Root, Root>();
            Func<InsertNavProvider<Root, Root>> func = () => {

                    var navas= PreFunc().AsNav();
                    navas.NavContext = this.NavContext;
                    navas.IsNav = true;
                    return navas;
                };
            result.PreFunc = func;
            result.Context = this.Context;
            result.NavContext = this.NavContext;
            return result;
        }
    }

}
