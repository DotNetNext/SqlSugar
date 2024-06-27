using SqlSugar;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;
using SqlSugar.Xugu;

namespace Data_logic
{
    /// <summary>
    /// 业务逻辑基类
    /// </summary>
    /// <typeparam name="T">与数据库中表的结构一致的模型类型</typeparam>
    public class BaseDataLogic<T> where T : class, new()
    {
        /// <summary>
        /// 主键个数
        /// </summary>
        private int primarykeyCount = -1;
        /// <summary>
        /// 主键属性信息
        /// </summary>
        private PropertyInfo primarykeyInfo = null;
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseDataLogic()
        {
            //获取主键个数
            if (primarykeyCount == -1)
            {
                var list = db.EntityMaintenance.GetEntityInfo<T>().Columns.Where(d => d.IsPrimarykey == true);
                primarykeyCount = list.Count();
                primarykeyInfo = primarykeyCount == 1 ? list.FirstOrDefault().PropertyInfo : null;
            }
            //*
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Debugger.Log(1, "SQL", sql);
            };
            //*/
        }
        /// <summary>
        /// SqlSugar 操作对象，单例用SqlSugarClient，否则用SqlSugarClient
        /// </summary>
        protected static SqlSugarScope db = new SqlSugarScope(new ConnectionConfig()
        {
            ConnectionString = "IP=10.1.1.1;DB=SYSTEM;User=SYSDBA;PWD=SYSDBA;Port=5138;AUTO_COMMIT=on;CHAR_SET=UTF8",//CHAR_SET=GBK
            DbType = DbType.Custom.UseXugu(),
            IsAutoCloseConnection = true,
            //ConfigureExternalServices = new ConfigureExternalServices() { SqlFuncServices = SqlFuncCustom.Methods }
        });
        /// <summary>
        /// 从数据库中生成模型类文件
        /// </summary>
        /// <param name="path">文件放置的位置</param>
        /// <param name="nameSpace">默认命名空间</param>
        /// <param name="allTable">是否生成所有表。是则包含所有T开头的表，否则只生成当前对象名称相同的表</param>
        public  void CreateModel(string path, string nameSpace = "Data.Model", bool allTable = false)
        {
            db.DbFirst.Where(c => allTable ? c.StartsWith("T") : typeof(T).Name == c)
                .IsCreateAttribute().IsCreateDefaultValue()
                .CreateClassFile(path, nameSpace);
        }
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <typeparam name="TModel">视图模型类型</typeparam>
        /// <param name="model">视图模型，仅包含模型中有的且值不为Null的字段</param>
        /// <returns>是否添加成功，已存在视为false</returns>
        public async Task<bool> Add<TModel>(TModel model) where TModel : class
        {
            if (db.Queryable<T>().In(primarykeyInfo.GetValue(model.Adapt<T>())).Any()) return false;
            return await db.Insertable<T>(model)
                .IgnoreColumns(ignoreNullColumn: true)
                .ExecuteReturnBigIdentityAsync().ContinueWith(t =>
                t.Result>0
                );
        }
        /// <summary>
        /// 根据主键或根据条件更新数据
        /// </summary>
        /// <typeparam name="TModel">视图模型类型</typeparam>
        /// <param name="model">视图模型，仅包含模型中有的且值不为Null的字段</param>
        /// <param name="where">查询条件，当条件为Null时为主键更新（T必须配置主键），否则为条件批量更新</param>
        /// <returns>更新是否成功，不存在视为false</returns>
        /// <exception cref="Exception">类型T未配置主键或有多个主键</exception>
        public async Task<bool> Update<TModel>(TModel model, Expression<Func<T, bool>> where = null) where TModel : class
        {
            if (where != null && this.primarykeyCount != 1) throw new Exception($"类型'{typeof(T).Name}'未配置主键或有多个主键");
            var expression = db.Updateable<T>(model);
            if (where != null) expression = expression.Where(where);
            return await expression
                .IgnoreColumns(ignoreAllNullColumns: true)
                .ExecuteCommandHasChangeAsync();
        }
        /// <summary>
        /// 根据主键插入或更新一条数据
        /// </summary>
        /// <typeparam name="TModel">视图模型类型（必须配置主键）</typeparam>
        /// <param name="model">视图模型，仅包含模型中有的且值不为Null的字段</param>
        /// <returns>更新是否成功</returns>
        /// <exception cref="Exception">类型T未配置主键或有多个主键</exception>
        public async Task<bool> AddOrUpdate<TModel>(TModel model) where TModel : class, new()
        {
            if (this.primarykeyCount != 1) throw new Exception($"类型'{typeof(T).Name}'未配置主键或有多个主键");

            if (!await db.Queryable<T>().In(primarykeyInfo.GetValue(model.Adapt<T>())).AnyAsync()) return await Add(model);
            else return await Update(model);
            /*
            var o = db.Storageable(model.Adapt<T>()).ToStorage();
            o.AsInsertable.ExecuteCommand();//不存在插入
            o.AsUpdateable.ExecuteCommand();//存在更新

            return await db.Storageable(model.Adapt<T>())
                .ExecuteCommandAsync()
                .ContinueWith(t => t.Result > 0);
             */
        }
        /// <summary>
        /// 根据主键删除数据
        /// </summary>
        /// <typeparam name="TKey">主键的类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>删除是否成功</returns>
        /// <exception cref="Exception">类型T未配置主键或有多个主键</exception>
        public async Task<bool> Delete<TKey>(TKey id)
        {
            if (this.primarykeyCount != 1) throw new Exception($"类型'{typeof(T).Name}'未配置主键或有多个主键");
            //不存在时视为删除成功
            if (!await db.Queryable<T>().In(id).AnyAsync()) return true;
            return await db.Deleteable<T>(id).ExecuteCommandHasChangeAsync();
        }
        /// <summary>
        /// 根据查询条件删除数据
        /// </summary>
        /// <param name="where">查询条件表达式</param>
        /// <returns>删除是否成功</returns>
        public async Task<bool> Delete(Expression<Func<T, bool>> where)
        {
            //不存在时视为删除成功
            if (!await db.Queryable<T>().Where(where).AnyAsync()) return true;
            return await db.Deleteable<T>(where).ExecuteCommandHasChangeAsync();
        }
        /// <summary>
        /// 根据单个主键获取一条数据
        /// </summary>
        /// <typeparam name="TModel">要返回的模型类型</typeparam>
        /// <typeparam name="TKey">主键的类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>返回指定主键值的数据</returns>
        /// <exception cref="Exception">类型T未配置主键或有多个主键</exception>
        public async Task<TModel> Single<TModel, TKey>(TKey id)
        {
            if (this.primarykeyCount != 1) throw new Exception($"类型'{typeof(T).Name}'未配置主键或有多个主键");
            return await db.Queryable<T>().InSingleAsync(id).ContinueWith(t => t.Result.Adapt<TModel>());
        }
        /// <summary>
        /// 根据条件获取唯一数据
        /// </summary>
        /// <typeparam name="TModel">要返回的模型类型</typeparam>
        /// <typeparam name="TKey">主键的类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>返回符合指定条件的唯一数据，不唯一时将抛出异常</returns>
        public async Task<TModel> Single<TModel, TKey>(Expression<Func<T, bool>> where)
        {
            return await db.Queryable<T>().Where(where).Select<TModel>().SingleAsync();
        }
        /// <summary>
        /// 获取符合条件的分页数据
        /// </summary>
        /// <typeparam name="TModel">列表模型的类型</typeparam>
        /// <param name="where">条件表达式</param>
        /// <param name="order">排序表达式</param>
        /// <param name="orderType">排序方向，默认顺序</param>
        /// <param name="pageNum">页码，从1开始</param>
        /// <param name="pageSize">每页条数，默认20</param>
        /// <returns>符合条件的分页数据列表，以及总条数</returns>
        public async Task<(List<TModel>, int)> List<TModel>(Expression<Func<T, bool>> where = null
            , Expression<Func<T, object>> order = null, OrderByType orderType = OrderByType.Asc
            , int pageNum = 1, int pageSize = 20, bool noPager = false)
        {
            var expression = db.Queryable<T>();
            if (where != null && where.ToString() != "it => True") expression = expression.Where(where);
            if (order != null) expression = expression.OrderBy(order, orderType);

            var expressionTModel = expression.Select<TModel>();
            if (noPager) return await expressionTModel.ToListAsync().ContinueWith(d => (d.Result, d.Result.Count));
            else return await ToPagedList<TModel>(expressionTModel, pageNum, pageSize);
        }
        /// <summary>
        /// 通用分页
        /// </summary>
        /// <typeparam name="TModel">列表模型的类型</typeparam>
        /// <param name="expression">查询表达式</param>
        /// <param name="pageNum">页码，从1开始</param>
        /// <param name="pageSize">每页条数，默认20</param>
        /// <returns>返回查询表达式分页后的数据列表，以及总条数</returns>
        protected static async Task<(List<TModel>, int)> ToPagedList<TModel>(ISugarQueryable<TModel> expression, int pageNum = 1, int pageSize = 20)
        {
            RefAsync<int> totalCount = 0;
            return await expression
                .ToPageListAsync/*ToOffsetPageAsync//2012以上才支持*/(pageNum, pageSize, totalCount)
                .ContinueWith(d => (d.Result, totalCount));
        }
        /// <summary>
        /// 获取动态where条件对象
        /// </summary>
        /// <returns>动态where条件对象</returns>
        public Expressionable<T> CreateWhere() => Expressionable.Create<T>();
    }
}