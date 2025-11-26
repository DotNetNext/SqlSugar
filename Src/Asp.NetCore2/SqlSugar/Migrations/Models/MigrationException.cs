using System;

namespace SqlSugar.Migrations
{
    public class MigrationException : Exception
    {
        public long? Version { get; set; }
        public string MigrationName { get; set; }

        public MigrationException(string message) : base(message) { }

        public MigrationException(string message, Exception innerException) 
            : base(message, innerException) { }

        public MigrationException(long version, string migrationName, string message) 
            : base($"Migration {version} ({migrationName}): {message}")
        {
            Version = version;
            MigrationName = migrationName;
        }

        public MigrationException(long version, string migrationName, string message, Exception innerException)
            : base($"Migration {version} ({migrationName}): {message}", innerException)
        {
            Version = version;
            MigrationName = migrationName;
        }
    }
}
