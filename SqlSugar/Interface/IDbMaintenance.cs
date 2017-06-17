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

        #region DDL
        bool DropTable(string tableName);
        bool TruncateTable(string tableName);
        bool CreateTable(string tableName, List<DbColumnInfo> columns);
        bool AddColumnToTable(string tableName, DbColumnInfo column);
        bool AddKey(string tableName,string columnName,bool isPrimaryKey, bool isIdentity);
        bool BackupDataBase(string databaseName,string fullFileName);
        bool DropColumn(string tableName,string columnName);
        #endregion
    }
}
