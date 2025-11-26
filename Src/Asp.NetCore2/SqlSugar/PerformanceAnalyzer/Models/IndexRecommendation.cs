using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Represents an index recommendation for improving query performance
    /// </summary>
    public class IndexRecommendation
    {
        public Guid Id { get; set; }
        public string TableName { get; set; }
        public List<string> Columns { get; set; }
        public string Reason { get; set; }
        public string CreateIndexSql { get; set; }
        public int EstimatedImpactPercent { get; set; }
        public int QueryCount { get; set; }
        public double TotalQueryTime { get; set; }
        public List<string> AffectedQueries { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public IndexRecommendation()
        {
            Id = Guid.NewGuid();
            Columns = new List<string>();
            AffectedQueries = new List<string>();
            CreatedAt = DateTime.UtcNow;
        }
        
        public void GenerateCreateIndexSql(DbType dbType)
        {
            if (string.IsNullOrEmpty(TableName) || Columns == null || !Columns.Any())
                return;
                
            var indexName = $"IX_{TableName}_{string.Join("_", Columns)}";
            var columnList = string.Join(", ", Columns);
            
            switch (dbType)
            {
                case DbType.SqlServer:
                    CreateIndexSql = $"CREATE NONCLUSTERED INDEX [{indexName}] ON [{TableName}] ({columnList})";
                    break;
                case DbType.MySql:
                    CreateIndexSql = $"CREATE INDEX `{indexName}` ON `{TableName}` ({columnList})";
                    break;
                case DbType.PostgreSQL:
                    CreateIndexSql = $"CREATE INDEX {indexName} ON {TableName} ({columnList})";
                    break;
                case DbType.Oracle:
                    CreateIndexSql = $"CREATE INDEX {indexName} ON {TableName} ({columnList})";
                    break;
                default:
                    CreateIndexSql = $"CREATE INDEX {indexName} ON {TableName} ({columnList})";
                    break;
            }
        }
    }
}