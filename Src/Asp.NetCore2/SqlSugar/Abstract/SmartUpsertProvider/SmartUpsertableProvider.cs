using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SqlSugar
{
    /// <summary>
    /// Smart upsert provider with advanced conflict resolution strategies
    /// 具有高级冲突解决策略的智能插入更新提供程序
    /// </summary>
    public class SmartUpsertableProvider<T> : ISmartUpsertable<T> where T : class, new()
    {
        #region Properties
        
        public SqlSugarProvider Context { get; set; }
        public List<T> DataList { get; set; }
        
        private ConflictResolutionStrategy _defaultStrategy = ConflictResolutionStrategy.UpdateAll;
        private List<string> _whereColumns = new List<string>();
        private Dictionary<string, ColumnStrategyConfig<T>> _columnStrategies = new Dictionary<string, ColumnStrategyConfig<T>>();
        private Func<T, T, bool> _updateCondition;
        private Action<T> _onInsertCallback;
        private Action<T, T> _onUpdateCallback;
        private Action<T> _onSkipCallback;
        private Action<T, T> _onConflictCallback;
        private bool _enableAuditTrail = false;
        private string _mergeDelimiter = ", ";
        private int _pageSize = 1000;
        private string _tableName;

        #endregion

        #region Constructor

        public SmartUpsertableProvider(SqlSugarProvider context, List<T> dataList)
        {
            this.Context = context;
            this.DataList = dataList ?? new List<T>();
        }

        #endregion

        #region Configuration Methods

        public ISmartUpsertable<T> WhereColumns(Expression<Func<T, object>> columns)
        {
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            var columnNames = Context.Utilities.ExpressionToString(columns, ResolveExpressType.FieldMultiple, null);
            _whereColumns = columnNames.Split(',').Select(c => c.Trim()).ToList();
            return this;
        }

        public ISmartUpsertable<T> WhereColumns(params string[] columnNames)
        {
            _whereColumns = columnNames.ToList();
            return this;
        }

        public ISmartUpsertable<T> SetStrategy(ConflictResolutionStrategy strategy)
        {
            _defaultStrategy = strategy;
            return this;
        }

        public ISmartUpsertable<T> SetColumnStrategy(Expression<Func<T, object>> column, ConflictResolutionStrategy strategy)
        {
            var columnName = Context.Utilities.ExpressionToString(column, ResolveExpressType.FieldSingle, null);
            
            if (!_columnStrategies.ContainsKey(columnName))
            {
                _columnStrategies[columnName] = new ColumnStrategyConfig<T>();
            }
            
            _columnStrategies[columnName].ColumnName = columnName;
            _columnStrategies[columnName].Strategy = strategy;
            
            return this;
        }

        public ISmartUpsertable<T> SetColumnCustomLogic(Expression<Func<T, object>> column, Func<object, object, object> updateFunc)
        {
            var columnName = Context.Utilities.ExpressionToString(column, ResolveExpressType.FieldSingle, null);
            
            if (!_columnStrategies.ContainsKey(columnName))
            {
                _columnStrategies[columnName] = new ColumnStrategyConfig<T>();
            }
            
            _columnStrategies[columnName].ColumnName = columnName;
            _columnStrategies[columnName].Strategy = ConflictResolutionStrategy.Custom;
            _columnStrategies[columnName].CustomUpdateFunc = updateFunc;
            
            return this;
        }

        public ISmartUpsertable<T> UpdateWhen(Func<T, T, bool> condition)
        {
            _updateCondition = condition;
            return this;
        }

        public ISmartUpsertable<T> OnInsert(Action<T> callback)
        {
            _onInsertCallback = callback;
            return this;
        }

        public ISmartUpsertable<T> OnUpdate(Action<T, T> callback)
        {
            _onUpdateCallback = callback;
            return this;
        }

        public ISmartUpsertable<T> OnSkip(Action<T> callback)
        {
            _onSkipCallback = callback;
            return this;
        }

        public ISmartUpsertable<T> OnConflict(Action<T, T> callback)
        {
            _onConflictCallback = callback;
            return this;
        }

        public ISmartUpsertable<T> EnableAuditTrail(bool enable = true)
        {
            _enableAuditTrail = enable;
            return this;
        }

        public ISmartUpsertable<T> SetMergeDelimiter(string delimiter)
        {
            _mergeDelimiter = delimiter;
            return this;
        }

        public ISmartUpsertable<T> PageSize(int pageSize)
        {
            _pageSize = pageSize;
            return this;
        }

        public ISmartUpsertable<T> AS(string tableName)
        {
            _tableName = tableName;
            return this;
        }

        #endregion

        #region Execution Methods

        public SmartUpsertResult<T> ExecuteCommand()
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new SmartUpsertResult<T> { TotalCount = DataList.Count };

            try
            {
                // Validate configuration
                ValidateConfiguration();

                // Process in batches
                Context.Utilities.PageEach(DataList, _pageSize, pageItems =>
                {
                    ProcessBatch(pageItems.ToList(), result);
                });

                stopwatch.Stop();
                result.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new SmartUpsertError<T>
                {
                    Message = "Smart upsert operation failed",
                    Exception = ex
                });
                result.ErrorCount++;
            }

            return result;
        }

        public async Task<SmartUpsertResult<T>> ExecuteCommandAsync()
        {
            return await Task.Run(() => ExecuteCommand());
        }

        public async Task<SmartUpsertResult<T>> ExecuteCommandAsync(CancellationToken cancellationToken)
        {
            Context.Ado.CancellationToken = cancellationToken;
            return await ExecuteCommandAsync();
        }

        public string ToSqlString()
        {
            // Generate SQL preview
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            var tableName = _tableName ?? entityInfo.DbTableName;
            
            return $"-- Smart Upsert Preview for {tableName}\n" +
                   $"-- Strategy: {_defaultStrategy}\n" +
                   $"-- Where Columns: {string.Join(", ", _whereColumns)}\n" +
                   $"-- Total Records: {DataList.Count}\n" +
                   $"-- Page Size: {_pageSize}";
        }

        #endregion

        #region Private Methods

        private void ValidateConfiguration()
        {
            if (!_whereColumns.Any())
            {
                // Auto-detect primary key columns
                var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
                var pkColumns = entityInfo.Columns.Where(c => c.IsPrimarykey).ToList();
                
                if (pkColumns.Any())
                {
                    _whereColumns = pkColumns.Select(c => c.PropertyName).ToList();
                }
                else
                {
                    throw new Exception("WhereColumns must be specified or entity must have primary key");
                }
            }
        }

        private void ProcessBatch(List<T> batchItems, SmartUpsertResult<T> result)
        {
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            
            // Build where expression for existing records
            var existingRecords = GetExistingRecords(batchItems);
            var existingDict = BuildExistingDictionary(existingRecords);

            foreach (var item in batchItems)
            {
                try
                {
                    var key = BuildKey(item);
                    
                    if (existingDict.ContainsKey(key))
                    {
                        // Record exists - handle update
                        var existing = existingDict[key];
                        
                        if (_defaultStrategy == ConflictResolutionStrategy.ThrowOnConflict)
                        {
                            throw new Exception($"Conflict detected for key: {key}");
                        }
                        
                        if (_defaultStrategy == ConflictResolutionStrategy.SkipOnConflict)
                        {
                            result.SkippedCount++;
                            result.SkippedItems.Add(item);
                            _onSkipCallback?.Invoke(item);
                            continue;
                        }

                        // Check update condition
                        if (_updateCondition != null && !_updateCondition(existing, item))
                        {
                            result.SkippedCount++;
                            result.SkippedItems.Add(item);
                            _onSkipCallback?.Invoke(item);
                            continue;
                        }

                        // Perform update
                        _onConflictCallback?.Invoke(existing, item);
                        PerformUpdate(existing, item);
                        result.UpdatedCount++;
                        result.UpdatedItems.Add(item);
                        _onUpdateCallback?.Invoke(existing, item);
                    }
                    else
                    {
                        // Record doesn't exist - perform insert
                        PerformInsert(item);
                        result.InsertedCount++;
                        result.InsertedItems.Add(item);
                        _onInsertCallback?.Invoke(item);
                    }
                }
                catch (Exception ex)
                {
                    result.ErrorCount++;
                    result.Errors.Add(new SmartUpsertError<T>
                    {
                        Item = item,
                        Message = ex.Message,
                        Exception = ex
                    });
                }
            }
        }

        private List<T> GetExistingRecords(List<T> batchItems)
        {
            var queryable = Context.Queryable<T>();
            
            if (!string.IsNullOrEmpty(_tableName))
            {
                queryable = queryable.AS(_tableName);
            }

            // Build IN clause for where columns
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            var whereColumnInfos = entityInfo.Columns.Where(c => _whereColumns.Contains(c.PropertyName)).ToList();

            if (whereColumnInfos.Count == 1)
            {
                // Single column - use IN
                var column = whereColumnInfos.First();
                var values = batchItems.Select(item => column.PropertyInfo.GetValue(item)).ToList();
                
                var exp = Expressionable.Create<T>();
                exp = exp.Or(it => SqlFunc.ContainsArray(values.ToArray(), column.PropertyInfo.GetValue(it)));
                
                return queryable.Where(exp.ToExpression()).ToList();
            }
            else
            {
                // Multiple columns - build OR conditions
                var exp = Expressionable.Create<T>();
                
                foreach (var item in batchItems)
                {
                    var itemExp = Expressionable.Create<T>();
                    foreach (var column in whereColumnInfos)
                    {
                        var value = column.PropertyInfo.GetValue(item);
                        itemExp = itemExp.And(it => column.PropertyInfo.GetValue(it).Equals(value));
                    }
                    exp = exp.Or(itemExp.ToExpression());
                }
                
                return queryable.Where(exp.ToExpression()).ToList();
            }
        }

        private Dictionary<string, T> BuildExistingDictionary(List<T> existingRecords)
        {
            var dict = new Dictionary<string, T>();
            
            foreach (var record in existingRecords)
            {
                var key = BuildKey(record);
                dict[key] = record;
            }
            
            return dict;
        }

        private string BuildKey(T item)
        {
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            var keyParts = new List<string>();
            
            foreach (var columnName in _whereColumns)
            {
                var column = entityInfo.Columns.FirstOrDefault(c => c.PropertyName == columnName);
                if (column != null)
                {
                    var value = column.PropertyInfo.GetValue(item);
                    keyParts.Add(value?.ToString() ?? "NULL");
                }
            }
            
            return string.Join("|", keyParts);
        }

        private void PerformInsert(T item)
        {
            var insertable = Context.Insertable(item);
            
            if (!string.IsNullOrEmpty(_tableName))
            {
                insertable = insertable.AS(_tableName);
            }
            
            insertable.ExecuteCommand();
        }

        private void PerformUpdate(T existing, T incoming)
        {
            var entityInfo = Context.EntityMaintenance.GetEntityInfo<T>();
            var updateable = Context.Updateable(incoming);
            
            if (!string.IsNullOrEmpty(_tableName))
            {
                updateable = updateable.AS(_tableName);
            }

            // Apply column strategies
            var columnsToUpdate = new List<string>();
            
            foreach (var column in entityInfo.Columns)
            {
                if (_whereColumns.Contains(column.PropertyName))
                    continue; // Skip where columns
                
                var strategy = _columnStrategies.ContainsKey(column.PropertyName)
                    ? _columnStrategies[column.PropertyName].Strategy
                    : _defaultStrategy;

                var shouldUpdate = ShouldUpdateColumn(existing, incoming, column, strategy);
                
                if (shouldUpdate)
                {
                    columnsToUpdate.Add(column.PropertyName);
                    
                    // Apply strategy-specific logic
                    ApplyColumnStrategy(existing, incoming, column, strategy);
                }
            }

            if (columnsToUpdate.Any())
            {
                updateable.UpdateColumns(columnsToUpdate.ToArray())
                         .WhereColumns(_whereColumns.ToArray())
                         .ExecuteCommand();
            }
        }

        private bool ShouldUpdateColumn(T existing, T incoming, EntityColumnInfo column, ConflictResolutionStrategy strategy)
        {
            var incomingValue = column.PropertyInfo.GetValue(incoming);
            var existingValue = column.PropertyInfo.GetValue(existing);

            switch (strategy)
            {
                case ConflictResolutionStrategy.UpdateAll:
                case ConflictResolutionStrategy.UpdateSpecified:
                    return true;

                case ConflictResolutionStrategy.UpdateNonNull:
                    return incomingValue != null;

                case ConflictResolutionStrategy.UpdateIfGreater:
                    if (incomingValue is IComparable comparableIncoming && existingValue is IComparable comparableExisting)
                    {
                        return comparableIncoming.CompareTo(comparableExisting) > 0;
                    }
                    return false;

                case ConflictResolutionStrategy.UpdateIfLess:
                    if (incomingValue is IComparable comparableIncoming2 && existingValue is IComparable comparableExisting2)
                    {
                        return comparableIncoming2.CompareTo(comparableExisting2) < 0;
                    }
                    return false;

                case ConflictResolutionStrategy.SkipOnConflict:
                    return false;

                case ConflictResolutionStrategy.IncrementOnConflict:
                case ConflictResolutionStrategy.DecrementOnConflict:
                case ConflictResolutionStrategy.Custom:
                    return true;

                default:
                    return true;
            }
        }

        private void ApplyColumnStrategy(T existing, T incoming, EntityColumnInfo column, ConflictResolutionStrategy strategy)
        {
            var incomingValue = column.PropertyInfo.GetValue(incoming);
            var existingValue = column.PropertyInfo.GetValue(existing);

            switch (strategy)
            {
                case ConflictResolutionStrategy.IncrementOnConflict:
                    if (incomingValue is int intIncoming && existingValue is int intExisting)
                    {
                        column.PropertyInfo.SetValue(incoming, intExisting + intIncoming);
                    }
                    else if (incomingValue is decimal decIncoming && existingValue is decimal decExisting)
                    {
                        column.PropertyInfo.SetValue(incoming, decExisting + decIncoming);
                    }
                    else if (incomingValue is double dblIncoming && existingValue is double dblExisting)
                    {
                        column.PropertyInfo.SetValue(incoming, dblExisting + dblIncoming);
                    }
                    break;

                case ConflictResolutionStrategy.DecrementOnConflict:
                    if (incomingValue is int intIncoming2 && existingValue is int intExisting2)
                    {
                        column.PropertyInfo.SetValue(incoming, intExisting2 - intIncoming2);
                    }
                    else if (incomingValue is decimal decIncoming2 && existingValue is decimal decExisting2)
                    {
                        column.PropertyInfo.SetValue(incoming, decExisting2 - decIncoming2);
                    }
                    else if (incomingValue is double dblIncoming2 && existingValue is double dblExisting2)
                    {
                        column.PropertyInfo.SetValue(incoming, dblExisting2 - dblIncoming2);
                    }
                    break;

                case ConflictResolutionStrategy.Custom:
                    if (_columnStrategies.ContainsKey(column.PropertyName))
                    {
                        var config = _columnStrategies[column.PropertyName];
                        if (config.CustomUpdateFunc != null)
                        {
                            var newValue = config.CustomUpdateFunc(existingValue, incomingValue);
                            column.PropertyInfo.SetValue(incoming, newValue);
                        }
                    }
                    break;
            }
        }

        #endregion
    }
}