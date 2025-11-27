using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar
{
    /// <summary>
    /// Extension methods for Smart Upsert functionality
    /// 智能插入更新功能的扩展方法
    /// </summary>
    public static class SmartUpsertExtensions
    {
        #region SqlSugarClient Extensions

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarClient client, List<T> dataList) where T : class, new()
        {
            return client.Context.SmartUpsert(dataList);
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarClient client, T data) where T : class, new()
        {
            return client.Context.SmartUpsert(new List<T> { data });
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarClient client, T[] dataArray) where T : class, new()
        {
            return client.Context.SmartUpsert(dataArray.ToList());
        }

        #endregion

        #region SqlSugarProvider Extensions

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarProvider provider, List<T> dataList) where T : class, new()
        {
            dataList = dataList.Where(it => it != null).ToList();
            provider.InitMappingInfo<T>();
            var result = new SmartUpsertableProvider<T>(provider, dataList);
            return result;
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarProvider provider, T data) where T : class, new()
        {
            return provider.SmartUpsert(new List<T> { data });
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarProvider provider, T[] dataArray) where T : class, new()
        {
            return provider.SmartUpsert(dataArray.ToList());
        }

        #endregion

        #region SqlSugarScope Extensions

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarScope scope, List<T> dataList) where T : class, new()
        {
            return scope.ScopedContext.SmartUpsert(dataList);
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarScope scope, T data) where T : class, new()
        {
            return scope.ScopedContext.SmartUpsert(new List<T> { data });
        }

        /// <summary>
        /// Smart upsert with advanced conflict resolution strategies
        /// 具有高级冲突解决策略的智能插入更新
        /// </summary>
        public static ISmartUpsertable<T> SmartUpsert<T>(this SqlSugarScope scope, T[] dataArray) where T : class, new()
        {
            return scope.ScopedContext.SmartUpsert(dataArray.ToList());
        }

        #endregion
    }
}