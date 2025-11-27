using System;
using System.Collections.Generic;

namespace SqlSugar
{
    /// <summary>
    /// Result of smart upsert operation with detailed statistics
    /// 智能插入更新操作的结果，包含详细统计信息
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class SmartUpsertResult<T> where T : class, new()
    {
        /// <summary>
        /// Total number of records processed
        /// 处理的总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Number of records inserted
        /// 插入的记录数
        /// </summary>
        public int InsertedCount { get; set; }

        /// <summary>
        /// Number of records updated
        /// 更新的记录数
        /// </summary>
        public int UpdatedCount { get; set; }

        /// <summary>
        /// Number of records skipped
        /// 跳过的记录数
        /// </summary>
        public int SkippedCount { get; set; }

        /// <summary>
        /// Number of records that caused errors
        /// 导致错误的记录数
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// List of inserted items
        /// 插入的项目列表
        /// </summary>
        public List<T> InsertedItems { get; set; } = new List<T>();

        /// <summary>
        /// List of updated items
        /// 更新的项目列表
        /// </summary>
        public List<T> UpdatedItems { get; set; } = new List<T>();

        /// <summary>
        /// List of skipped items
        /// 跳过的项目列表
        /// </summary>
        public List<T> SkippedItems { get; set; } = new List<T>();

        /// <summary>
        /// List of errors that occurred
        /// 发生的错误列表
        /// </summary>
        public List<SmartUpsertError<T>> Errors { get; set; } = new List<SmartUpsertError<T>>();

        /// <summary>
        /// Execution time in milliseconds
        /// 执行时间（毫秒）
        /// </summary>
        public long ExecutionTimeMs { get; set; }

        /// <summary>
        /// Whether the operation was successful
        /// 操作是否成功
        /// </summary>
        public bool IsSuccess => ErrorCount == 0;
    }

    /// <summary>
    /// Error information for smart upsert operation
    /// 智能插入更新操作的错误信息
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class SmartUpsertError<T> where T : class, new()
    {
        /// <summary>
        /// The item that caused the error
        /// 导致错误的项目
        /// </summary>
        public T Item { get; set; }

        /// <summary>
        /// Error message
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Exception details
        /// 异常详情
        /// </summary>
        public Exception Exception { get; set; }
    }
}