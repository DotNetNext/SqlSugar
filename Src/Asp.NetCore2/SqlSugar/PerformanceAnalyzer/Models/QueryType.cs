using System;

namespace SqlSugar.PerformanceAnalyzer.Models
{
    /// <summary>
    /// Represents the type of SQL query
    /// </summary>
    public enum QueryType
    {
        Select,
        Insert,
        Update,
        Delete,
        Merge,
        CreateTable,
        Other
    }
}