using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteNavTaskInit<Root,T> where T : class, new() where Root : class, new()
    {
        internal List<T> Roots { get;   set; }
        internal SqlSugarProvider Context { get; set; }
        internal DeleteNavProvider<Root, Root> deleteNavProvider { get; set; }
        public DeleteNavMethodInfo IncludesAllFirstLayer(params string[] ignoreColumns)
        {
            if (ignoreColumns == null)
            {
                ignoreColumns = new string[] { };
            }
            this.Context = deleteNavProvider._Context;
            var navColumns = this.Context.EntityMaintenance.GetEntityInfo<Root>().Columns.Where(it => !ignoreColumns.Contains(it.PropertyName) || !ignoreColumns.Any(z => z.EqualCase(it.DbColumnName))).Where(it => it.Navigat != null).ToList();
            var updateNavs = this;
            DeleteNavMethodInfo methodInfo = updateNavs.IncludeByNameString(navColumns[0].PropertyName);
            foreach (var item in navColumns.Skip(1))
            {
                methodInfo = methodInfo.IncludeByNameString(item.PropertyName);
            }
            return methodInfo;
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            this.Context = deleteNavProvider._Context;
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => deleteNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavMethodInfo IncludeByNameString(string navMemberName, UpdateNavOptions updateNavOptions = null)
        {
            DeleteNavMethodInfo result = new DeleteNavMethodInfo();
            result.Context = deleteNavProvider._Context;
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
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            this.Context = deleteNavProvider._Context;
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => deleteNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression,DeleteNavOptions deleteNavOptions) where TChild : class, new()
        {
            var result= Include(expression);
            deleteNavProvider.deleteNavOptions = deleteNavOptions;
            return result;
        }
    }
    public class DeleteNavTask<Root, T> where T : class, new() where Root : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public Func<DeleteNavProvider<Root, T>> PreFunc { get; set; }
        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression, DeleteNavOptions deleteNavOptions) where TChild : class, new()
        {
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => {
                var dev = PreFunc();
                dev.deleteNavOptions = deleteNavOptions;
                return dev.ThenInclude(expression);
            };
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression,DeleteNavOptions deleteNavOptions) where TChild : class, new()
        {
            DeleteNavTask<Root, TChild> result = new DeleteNavTask<Root, TChild>();
            Func<DeleteNavProvider<Root, TChild>> func = () => {
                 var dev = PreFunc();
                 dev.deleteNavOptions = deleteNavOptions;
                 return dev.ThenInclude(expression);
                };
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression, DeleteNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression,options);
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }
        public DeleteNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression,DeleteNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression, options);
        }
        public bool ExecuteCommand()
        {
            PreFunc();

            var hasTran = this.Context.Ado.Transaction != null;
            if (hasTran)
            {
                ExecTasks();
            }
            else
            {
                this.Context.Ado.UseTran(() =>
                {
                    ExecTasks();

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

        private DeleteNavTask<Root, Root> AsNav()
        {
            DeleteNavTask<Root, Root> result = new DeleteNavTask<Root, Root>();
            Func<DeleteNavProvider<Root, Root>> func = () => PreFunc().AsNav();
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        private void ExecTasks() 
        {
            var tasks=(List<Action>)this.Context.TempItems["_DeleteNavTask"];
            tasks.Reverse();
            foreach (var task in tasks) 
            {
                task();
            }
            this.Context.TempItems.Remove("_DeleteNavTask");
        }
    }
}
