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
        List<DbTableInfo> GetViewInfoList(bool isCache=true);
        List<DbTableInfo> GetTableInfoList(bool isCache=true);
        List<DbColumnInfo> GetColumnInfosByTableName(string tableName,bool isCache=true);
        List<string> GetIsIdentities(string tableName);
        List<string> GetPrimaries(string tableName);
        #endregion

        #region Check
        bool IsAnyTable(string tableName, bool isCache = true);
        bool IsAnyColumn(string tableName, string column);
        bool IsPrimaryKey(string tableName, string column);
        bool IsIdentity(string tableName, string column);
        bool IsAnyConstraint(string ConstraintName);
        bool IsAnySystemTablePermissions();
        #endregion

        #region DDL
        bool DropTable(string tableName);
        bool TruncateTable(string tableName);
        bool CreateTable(string tableName, List<DbColumnInfo> columns,bool isCreatePrimaryKey=true);
        bool AddColumn(string tableName, DbColumnInfo column);
        bool UpdateColumn(string tableName, DbColumnInfo column);
        bool AddPrimaryKey(string tableName,string columnName);
        bool AddPrimaryKeys(string tableName, string [] columnNames);
        bool DropConstraint(string tableName, string constraintName);
        bool BackupDataBase(string databaseName,string fullFileName);
        bool BackupTable(string oldTableName, string newTableName, int maxBackupDataRows = int.MaxValue);
        bool DropColumn(string tableName,string columnName);
        bool RenameColumn(string tableName, string oldColumnName, string newColumnName);
        bool AddRemark(EntityInfo entity);
        bool AddColumnRemark(string columnName,string tableName,string description);
        bool DeleteColumnRemark(string columnName, string tableName);
        bool IsAnyColumnRemark(string columnName, string tableName);
        bool AddTableRemark( string tableName, string description);
        bool DeleteTableRemark(string tableName);
        bool IsAnyTableRemark(string tableName);
        bool RenameTable(string oldTableName,string newTableName);
        #endregion
    }
}
