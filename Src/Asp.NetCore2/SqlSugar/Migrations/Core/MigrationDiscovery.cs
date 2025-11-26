using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SqlSugar.Migrations
{
    public class MigrationDiscovery
    {
        private readonly MigrationConfig _config;

        public MigrationDiscovery(MigrationConfig config)
        {
            _config = config;
        }

        public List<MigrationInfo> DiscoverMigrations()
        {
            var assembly = _config.MigrationAssembly ?? Assembly.GetCallingAssembly();
            var migrationTypes = assembly.GetTypes()
                .Where(t => typeof(Migration).IsAssignableFrom(t) && !t.IsAbstract)
                .Where(t => string.IsNullOrEmpty(_config.MigrationNamespace) || 
                           t.Namespace?.StartsWith(_config.MigrationNamespace) == true)
                .ToList();

            var migrations = new List<MigrationInfo>();

            foreach (var type in migrationTypes)
            {
                var attr = (MigrationAttribute)Attribute.GetCustomAttribute(type, typeof(MigrationAttribute));
                if (attr != null)
                {
                    migrations.Add(new MigrationInfo
                    {
                        Version = attr.Version,
                        Description = attr.Description ?? type.Name,
                        MigrationType = type,
                        ClassName = type.FullName,
                        IsApplied = false
                    });
                }
            }

            return migrations.OrderBy(m => m.Version).ToList();
        }
    }
}
