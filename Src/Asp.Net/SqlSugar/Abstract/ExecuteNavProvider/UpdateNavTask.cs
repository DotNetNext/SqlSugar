using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class UpdateNavTaskInit<Root, T> where T : class, new() where Root : class, new()
    {

        internal SqlSugarProvider Context { get; set; }
        internal UpdateNavProvider<Root, Root> UpdateNavProvider { get; set; }

        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            this.Context = UpdateNavProvider._Context;
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => UpdateNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            this.Context = UpdateNavProvider._Context;
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => UpdateNavProvider.ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }

        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression,UpdateNavOptions options) where TChild : class, new()
        {
            this.Context = UpdateNavProvider._Context;
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => UpdateNavProvider.ThenInclude(expression,options);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            this.Context = UpdateNavProvider._Context;
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => UpdateNavProvider.ThenInclude(expression,options);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
    }
    public class UpdateNavTask<Root, T> where T : class, new() where Root : class, new()
    {
        public SqlSugarProvider Context { get; set; }
        public Func<UpdateNavProvider<Root, T>> PreFunc { get; set; }


        #region +1
        public UpdateNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression) where TChild : class, new()
        {
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression) where TChild : class, new()
        {
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression);
        }

        #endregion


        #region +2
        public UpdateNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, TChild>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression, options);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> ThenInclude<TChild>(Expression<Func<T, List<TChild>>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            UpdateNavTask<Root, TChild> result = new UpdateNavTask<Root, TChild>();
            Func<UpdateNavProvider<Root, TChild>> func = () => PreFunc().ThenInclude(expression, options);
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, TChild>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression, options);
        }
        public UpdateNavTask<Root, TChild> Include<TChild>(Expression<Func<Root, List<TChild>>> expression, UpdateNavOptions options) where TChild : class, new()
        {
            return AsNav().ThenInclude(expression, options);
        } 
        #endregion


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

        private UpdateNavTask<Root, Root> AsNav()
        {
            UpdateNavTask<Root, Root> result = new UpdateNavTask<Root, Root>();
            Func<UpdateNavProvider<Root, Root>> func = () => PreFunc().AsNav();
            result.PreFunc = func;
            result.Context = this.Context;
            return result;
        }
    }

}
