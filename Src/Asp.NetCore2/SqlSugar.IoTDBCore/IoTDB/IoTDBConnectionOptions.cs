using System;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace SqlSugar.IoTDB
{
    public sealed class IoTDBConnectionOptions
    {
        private static readonly Regex DatabasePattern = new Regex(@"^root(?:\.[A-Za-z0-9_]+)+$", RegexOptions.Compiled);

        public string ProviderConnectionString { get; private set; }
        public string Database { get; private set; }

        private IoTDBConnectionOptions(string providerConnectionString, string database)
        {
            ProviderConnectionString = providerConnectionString;
            Database = database;
        }

        public static IoTDBConnectionOptions Parse(string connectionString)
        {
            var values = new DbConnectionStringBuilder { ConnectionString = connectionString ?? string.Empty };
            var database = GetAndRemove(values, "Database");
            var rootPath = GetAndRemove(values, "RootPath");
            database = database ?? rootPath ?? "root.sg";
            var host = GetAndRemove(values, "Host");
            if (!string.IsNullOrWhiteSpace(host) && !values.ContainsKey("DataSource"))
            {
                values["DataSource"] = host;
            }

            if (!DatabasePattern.IsMatch(database))
            {
                throw new ArgumentException("IoTDB Database/RootPath must be a qualified root path such as root.sg.", nameof(connectionString));
            }

            return new IoTDBConnectionOptions(values.ConnectionString, database);
        }

        public string GetDevicePath(string name)
        {
            var normalized = name?.Trim().Trim('`', '"') ?? string.Empty;
            if (!Regex.IsMatch(normalized, @"^(?:root\.)?[A-Za-z0-9_]+(?:\.[A-Za-z0-9_]+)*$"))
            {
                throw new ArgumentException("IoTDB device path contains unsupported identifier characters.", nameof(name));
            }

            if (normalized.StartsWith("root.", StringComparison.OrdinalIgnoreCase))
            {
                if (!normalized.StartsWith(Database + ".", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(normalized, Database, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException("Device path must be inside the configured IoTDB database.", nameof(name));
                }
                return normalized;
            }

            return Database + "." + normalized;
        }

        public string GetMeasurementName(string name)
        {
            var normalized = name?.Trim().Trim('`', '"') ?? string.Empty;
            if (!Regex.IsMatch(normalized, @"^[A-Za-z0-9_]+(?:\.[A-Za-z0-9_]+)*$"))
            {
                throw new ArgumentException("IoTDB measurement contains unsupported identifier characters.", nameof(name));
            }
            return normalized.Equals("timestamp", StringComparison.OrdinalIgnoreCase) ? "time" : normalized;
        }

        private static string? GetAndRemove(DbConnectionStringBuilder values, string key)
        {
            foreach (string existing in values.Keys)
            {
                if (existing.Equals(key, StringComparison.OrdinalIgnoreCase))
                {
                    var value = Convert.ToString(values[existing]);
                    values.Remove(existing);
                    return value;
                }
            }
            return null;
        }
    }
}
