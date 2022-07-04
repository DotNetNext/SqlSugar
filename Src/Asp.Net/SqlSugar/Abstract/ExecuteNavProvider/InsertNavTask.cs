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

        public InsertNavTask<Root, TChild>  Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            this.Context = insertNavProvider._Context;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public InsertNavTask<Root, TChild>  Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            this.Context = insertNavProvider._Context;
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => insertNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
    }
    public class InsertNavTask<Root, T> where T : class, new() where Root : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public Func<InsertNavProvider<Root, T>> PreFunc { get;  set; }
        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public InsertNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            InsertNavTask<Root, TChild> result = new InsertNavTask<Root, TChild>();
            Func<InsertNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
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
            Func<InsertNavProvider<Root, Root>> func = () => PreFunc().AsNav();
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
    }

}
