using System;
using System.Linq.Expressions;

namespace SqlSugar
{
    /// <summary>
    /// Column-specific strategy configuration for smart upsert
    /// 智能插入更新的列级策略配置
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class ColumnStrategyConfig<T> where T : class, new()
    {
        /// <summary>
        /// Column name
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Conflict resolution strategy for this column
        /// 此列的冲突解决策略
        /// </summary>
        public ConflictResolutionStrategy Strategy { get; set; }

        /// <summary>
        /// Custom update function for this column
        /// 此列的自定义更新函数
        /// </summary>
        public Func<object, object, object> CustomUpdateFunc { get; set; }

        /// <summary>
        /// Merge delimiter for string concatenation (used with MergeValues strategy)
        /// 字符串连接的合并分隔符（与MergeValues策略一起使用）
        /// </summary>
        public string MergeDelimiter { get; set; } = ", ";
    }
}