using Apache.IoTDB;
using Apache.IoTDB.Data;
using Apache.IoTDB.DataStructure;
using SqlSugar.IoTDB;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace SqlSugar
{
    public sealed class IoTDBFastBuilder : FastBuilder, IFastBuilder
    {
        public IoTDBFastBuilder()
        {
            DbFastestProperties = new DbFastestProperties();
        }

        public async Task<int> ExecuteBulkCopyAsync(DataTable data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (data.Rows.Count == 0) return 0;

            var connection = (IoTDBConnection)this.Context.Ado.Connection;
            var tablet = BuildTablet(data,
                IoTDBConnectionOptions.Parse(this.Context.CurrentConnectionConfig.ConnectionString),
                this.FastEntityInfo);
            try
            {
                this.Context.Ado.CheckConnection();
                var status = await connection.SessionPool.InsertTabletAsync(tablet).ConfigureAwait(false);
                if (status != 0)
                    throw new InvalidOperationException("IoTDB tablet insert failed with status " + status.ToString(CultureInfo.InvariantCulture) + ".");
                return data.Rows.Count;
            }
            finally
            {
                CloseDb();
            }
        }

        internal static Tablet BuildTablet(DataTable data, IoTDBConnectionOptions options, EntityInfo entityInfo)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrWhiteSpace(data.TableName))
                throw new ArgumentException("Set the IoTDB device path with Fastest().AS(...) before bulk copy.", nameof(data));

            var timeColumns = data.Columns.Cast<DataColumn>()
                .Where(column => IsTimeColumn(column.ColumnName))
                .ToList();
            if (timeColumns.Count != 1)
                throw new NotSupportedException("IoTDB bulk copy requires exactly one time column named 'time' or 'timestamp'.");

            var timeColumn = timeColumns[0];
            var measurementColumns = data.Columns.Cast<DataColumn>()
                .Where(column => !ReferenceEquals(column, timeColumn))
                .ToList();
            if (measurementColumns.Count == 0)
                throw new NotSupportedException("IoTDB bulk copy requires at least one measurement column.");

            var devicePath = options.GetDevicePath(data.TableName);
            var measurements = measurementColumns
                .Select(column => options.GetMeasurementName(column.ColumnName))
                .ToList();
            if (measurements.Any(name => name.Contains(".")))
                throw new ArgumentException("IoTDB tablet measurement names must be single path nodes.", nameof(data));
            if (measurements.Distinct(StringComparer.OrdinalIgnoreCase).Count() != measurements.Count)
                throw new ArgumentException("IoTDB tablet contains duplicate measurement names.", nameof(data));

            var dataTypes = measurementColumns
                .Select(column => ResolveDataType(column, FindEntityColumn(entityInfo, column.ColumnName)))
                .ToList();
            var timestamps = new System.Collections.Generic.List<long>(data.Rows.Count);
            var rows = new System.Collections.Generic.List<System.Collections.Generic.List<object>>(data.Rows.Count);
            foreach (DataRow row in data.Rows)
            {
                timestamps.Add(ToEpochMilliseconds(row[timeColumn]));
                var values = new System.Collections.Generic.List<object>(measurementColumns.Count);
                for (var index = 0; index < measurementColumns.Count; index++)
                {
                    values.Add(ConvertValue(row[measurementColumns[index]], dataTypes[index])!);
                }
                rows.Add(values);
            }

            return new Tablet(devicePath, measurements, dataTypes, rows, timestamps);
        }

        public override Task<int> UpdateByTempAsync(string tableName, string tempName, string[] updateColumns, string[] whereColumns)
        {
            throw new NotSupportedException("IoTDB tree measurements do not support relational bulk update.");
        }

        public override Task CreateTempAsync<T>(DataTable data)
        {
            throw new NotSupportedException("IoTDB tree measurements do not support relational temporary tables.");
        }

        public override Task<int> Merge<T>(string tableName, DataTable data, EntityInfo entityInfo, string[] whereColumns, string[] updateColumns, System.Collections.Generic.List<T> datas)
        {
            throw new NotSupportedException("IoTDB tree measurements do not support relational bulk merge.");
        }

        private static EntityColumnInfo? FindEntityColumn(EntityInfo entityInfo, string columnName)
        {
            return entityInfo?.Columns?.FirstOrDefault(column =>
                string.Equals(column.DbColumnName, columnName, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(column.PropertyName, columnName, StringComparison.OrdinalIgnoreCase));
        }

        private static TSDataType ResolveDataType(DataColumn column, EntityColumnInfo entityColumn)
        {
            var declaredType = entityColumn?.DataType?.Trim();
            if (!string.IsNullOrEmpty(declaredType))
            {
                switch (declaredType.ToUpperInvariant())
                {
                    case "BOOLEAN":
                    case "BOOL": return TSDataType.BOOLEAN;
                    case "INT32":
                    case "INTEGER":
                    case "INT": return TSDataType.INT32;
                    case "INT64":
                    case "BIGINT": return TSDataType.INT64;
                    case "FLOAT":
                    case "REAL": return TSDataType.FLOAT;
                    case "DOUBLE":
                    case "DECIMAL":
                    case "NUMERIC": return TSDataType.DOUBLE;
                    case "TEXT":
                    case "VARCHAR":
                    case "CHAR": return TSDataType.TEXT;
                    case "STRING": return TSDataType.STRING;
                    case "BLOB":
                    case "BINARY": return TSDataType.BLOB;
                    case "TIMESTAMP": return TSDataType.TIMESTAMP;
                    case "DATE": return TSDataType.DATE;
                }
            }

            var type = Nullable.GetUnderlyingType(column.DataType) ?? column.DataType;
            if (type == typeof(bool)) return TSDataType.BOOLEAN;
            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint)) return TSDataType.INT32;
            if (type == typeof(long) || type == typeof(ulong)) return TSDataType.INT64;
            if (type == typeof(float)) return TSDataType.FLOAT;
            if (type == typeof(double) || type == typeof(decimal)) return TSDataType.DOUBLE;
            if (type == typeof(byte[])) return TSDataType.BLOB;
            if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return TSDataType.TIMESTAMP;
            if (type == typeof(string) || type == typeof(char) || type == typeof(Guid)) return TSDataType.TEXT;

            throw new NotSupportedException("IoTDB bulk copy does not support measurement type '" + column.DataType.FullName + "' for '" + column.ColumnName + "'.");
        }

        private static object? ConvertValue(object value, TSDataType dataType)
        {
            if (value == null || value == DBNull.Value) return null;
            switch (dataType)
            {
                case TSDataType.BOOLEAN: return Convert.ToBoolean(value, CultureInfo.InvariantCulture);
                case TSDataType.INT32: return Convert.ToInt32(value, CultureInfo.InvariantCulture);
                case TSDataType.INT64: return Convert.ToInt64(value, CultureInfo.InvariantCulture);
                case TSDataType.FLOAT: return Convert.ToSingle(value, CultureInfo.InvariantCulture);
                case TSDataType.DOUBLE: return Convert.ToDouble(value, CultureInfo.InvariantCulture);
                case TSDataType.TEXT:
                case TSDataType.STRING:
                    return Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty;
                case TSDataType.BLOB:
                    if (value is byte[] bytes) return bytes;
                    throw new InvalidCastException("IoTDB BLOB measurements require byte[] values.");
                case TSDataType.TIMESTAMP: return ToEpochMilliseconds(value);
                case TSDataType.DATE:
                    if (value is DateTime dateTime) return dateTime;
                    if (value is DateTimeOffset dateTimeOffset) return dateTimeOffset.DateTime;
                    return DateTime.Parse(Convert.ToString(value, CultureInfo.InvariantCulture) ?? throw new InvalidCastException("IoTDB DATE value cannot be converted to text."), CultureInfo.InvariantCulture);
                default: throw new NotSupportedException("Unsupported IoTDB measurement type: " + dataType + ".");
            }
        }

        private static long ToEpochMilliseconds(object value)
        {
            if (value == null || value == DBNull.Value)
                throw new ArgumentException("IoTDB time values cannot be null.");
            if (value is DateTime dateTime)
                return IoTDBParameterBinder.ToUnixTimeMilliseconds(dateTime);
            if (value is DateTimeOffset dateTimeOffset)
                return dateTimeOffset.ToUnixTimeMilliseconds();
            if (value is string text && DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var parsed))
                return parsed.ToUnixTimeMilliseconds();
            return Convert.ToInt64(value, CultureInfo.InvariantCulture);
        }

        private static bool IsTimeColumn(string name)
        {
            return string.Equals(name, "time", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "timestamp", StringComparison.OrdinalIgnoreCase);
        }
    }
}
