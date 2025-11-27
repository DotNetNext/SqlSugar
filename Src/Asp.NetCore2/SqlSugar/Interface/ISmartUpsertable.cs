using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Interface for smart upsert operations with advanced conflict resolution
    /// 具有高级冲突解决功能的智能插入更新操作接口
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface ISmartUpsertable<T> where T : class, new()
    {
        /// <summary>
        /// Specify columns to use for conflict detection (like primary key or unique constraint)
        /// 指定用于冲突检测的列（如主键或唯一约束）
        /// </summary>
        ISmartUpsertable<T> WhereColumns(Expression<Func<T, object>> columns);

        /// <summary>
        /// Specify columns to use for conflict detection by column names
        /// 通过列名指定用于冲突检测的列
        /// </summary>
        ISmartUpsertable<T> WhereColumns(params string[] columnNames);

        /// <summary>
        /// Set the default conflict resolution strategy for all columns
        /// 设置所有列的默认冲突解决策略
        /// </summary>
        ISmartUpsertable<T> SetStrategy(ConflictResolutionStrategy strategy);

        /// <summary>
        /// Set conflict resolution strategy for a specific column
        /// 为特定列设置冲突解决策略
        /// </summary>
        ISmartUpsertable<T> SetColumnStrategy(Expression<Func<T, object>> column, ConflictResolutionStrategy strategy);

        /// <summary>
        /// Set custom update logic for a specific column
        /// 为特定列设置自定义更新逻辑
        /// </summary>
        ISmartUpsertable<T> SetColumnCustomLogic(Expression<Func<T, object>> column, Func<object, object, object> updateFunc);

        /// <summary>
        /// Update only when condition is met
        /// 仅当满足条件时更新
        /// </summary>
        ISmartUpsertable<T> UpdateWhen(Func<T, T, bool> condition);

        /// <summary>
        /// Callback when insert occurs
        /// 插入时的回调
        /// </summary>
        ISmartUpsertable<T> OnInsert(Action<T> callback);

        /// <summary>
        /// Callback when update occurs
        /// 更新时的回调
        /// </summary>
        ISmartUpsertable<T> OnUpdate(Action<T, T> callback);

        /// <summary>
        /// Callback when skip occurs
        /// 跳过时的回调
        /// </summary>
        ISmartUpsertable<T> OnSkip(Action<T> callback);

        /// <summary>
        /// Callback when conflict occurs
        /// 冲突时的回调
        /// </summary>
        ISmartUpsertable<T> OnConflict(Action<T, T> callback);

        /// <summary>
        /// Enable audit trail tracking
        /// 启用审计跟踪
        /// </summary>
        ISmartUpsertable<T> EnableAuditTrail(bool enable = true);

        /// <summary>
        /// Set merge delimiter for string concatenation
        /// 设置字符串连接的合并分隔符
        /// </summary>
        ISmartUpsertable<T> SetMergeDelimiter(string delimiter);

        /// <summary>
        /// Set page size for batch processing
        /// 设置批处理的页面大小
        /// </summary>
        ISmartUpsertable<T> PageSize(int pageSize);

        /// <summary>
        /// Specify table name (for AS operations)
        /// 指定表名（用于AS操作）
        /// </summary>
        ISmartUpsertable<T> AS(string tableName);

        /// <summary>
        /// Execute the smart upsert operation
        /// 执行智能插入更新操作
        /// </summary>
        SmartUpsertResult<T> ExecuteCommand();

        /// <summary>
        /// Execute the smart upsert operation asynchronously
        /// 异步执行智能插入更新操作
        /// </summary>
        Task<SmartUpsertResult<T>> ExecuteCommandAsync();

        /// <summary>
        /// Execute the smart upsert operation asynchronously with cancellation token
        /// 使用取消令牌异步执行智能插入更新操作
        /// </summary>
        Task<SmartUpsertResult<T>> ExecuteCommandAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Get SQL preview without executing
        /// 获取SQL预览而不执行
        /// </summary>
        string ToSqlString();
    }
}