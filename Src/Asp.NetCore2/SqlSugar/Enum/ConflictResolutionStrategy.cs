using System;

namespace SqlSugar
{
    /// <summary>
    /// Conflict resolution strategy for smart batch upsert operations
    /// 智能批量插入更新操作的冲突解决策略
    /// </summary>
    public enum ConflictResolutionStrategy
    {
        /// <summary>
        /// Update all columns on conflict (default)
        /// 冲突时更新所有列（默认）
        /// </summary>
        UpdateAll = 0,

        /// <summary>
        /// Update only specified columns on conflict
        /// 冲突时仅更新指定列
        /// </summary>
        UpdateSpecified = 1,

        /// <summary>
        /// Update only if source value is not null
        /// 仅当源值不为空时更新
        /// </summary>
        UpdateNonNull = 2,

        /// <summary>
        /// Update only if source value is greater (for numeric/date fields)
        /// 仅当源值更大时更新（用于数字/日期字段）
        /// </summary>
        UpdateIfGreater = 3,

        /// <summary>
        /// Update only if source value is less (for numeric/date fields)
        /// 仅当源值更小时更新（用于数字/日期字段）
        /// </summary>
        UpdateIfLess = 4,

        /// <summary>
        /// Skip update on conflict, keep existing data
        /// 冲突时跳过更新，保留现有数据
        /// </summary>
        SkipOnConflict = 5,

        /// <summary>
        /// Throw exception on conflict
        /// 冲突时抛出异常
        /// </summary>
        ThrowOnConflict = 6,

        /// <summary>
        /// Use custom update logic
        /// 使用自定义更新逻辑
        /// </summary>
        Custom = 7,

        /// <summary>
        /// Increment numeric values on conflict
        /// 冲突时递增数值
        /// </summary>
        IncrementOnConflict = 8,

        /// <summary>
        /// Decrement numeric values on conflict
        /// 冲突时递减数值
        /// </summary>
        DecrementOnConflict = 9
    }
}