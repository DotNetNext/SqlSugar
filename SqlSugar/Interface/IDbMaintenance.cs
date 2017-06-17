using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbMaintenance
    {
        SqlSugarClient Context { get; set; }

        #region DML
        List<DbTableInfo> GetViewInfoList();

        List<DbTableInfo> GetTableInfoList();

        List<DbColumnInfo> GetColumnInfosByTableName(string tableName);

        List<string> GetIsIdentities(string tableName);

        List<string> GetPrimaries(string tableName);
        #endregion

        #region Check
        bool IsAnyTable(string tableName);
        bool IsAnyColumn(string tableName, string column);
        bool IsPrimaryKey(string tableName, string column);
        bool IsIdentity(string tableName, string column);
        #endregion

        #region Get Sql
        string GetCreateTableSql(string tableName, List<DbColumnInfo> columns);
        #endregion

        #region DDL
        bool TruncateTable(string tableName);

        bool CreateTable(string tableName, List<DbColumnInfo> columns);

        bool AddColumnToTable(string tableName, DbColumnInfo column);

        bool BackupDataBase();
        #endregion
    }
}
