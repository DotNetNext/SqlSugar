using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbMaintenance
    {
        SqlSugarProvider Context { get; set; }

        #region DML
        List<string> GetDataBaseList(SqlSugarClient db);
        List<string> GetDataBaseList();
        List<DbTableInfo> GetViewInfoList(bool isCache=true);
        List<DbTableInfo> GetTableInfoList(bool isCache=true);
        List<DbColumnInfo> GetColumnInfosByTableName(string tableName,bool isCache=true);
        List<string> GetIsIdentities(string tableName);
        List<string> GetPrimaries(string tableName);
        List<string> GetProcList(string dbName);
        List<string> GetIndexList(string tableName);
        List<string> GetFuncList();
        List<string> GetTriggerNames(string tableName);
        List<string> GetDbTypes();
        #endregion

        #region Check
        bool IsAnyTable(string tableName, bool isCache = true);
        bool IsAnyColumn(string tableName, string column, bool isCache = true);
        bool IsPrimaryKey(string tableName, string column);
        bool IsPrimaryKey(string tableName, string column,bool isCache=true);
        bool IsIdentity(string tableName, string column);
        bool IsAnyConstraint(string ConstraintName);
        bool IsAnySystemTablePermissions();
        bool IsAnyProcedure(string procName);
        #endregion

        #region DDL
        bool AddDefaultValue(string tableName,string columnName,string defaultValue);
        bool CreateIndex(string tableName, string [] columnNames, bool isUnique=false);
        bool CreateIndex(string tableName, string[] columnNames, string IndexName, bool isUnique = false);
        bool DropTable(string tableName);
        bool DropView(string viewName);
        bool DropIndex(string indexName);
        bool DropFunction(string funcName);
        bool DropProc(string procName);
        bool DropTable(params string[] tableName);
        bool DropTable(params Type[] tableEntityTypes);
        bool DropTable<T>();
        bool DropTable<T,T2>();
        bool DropTable<T, T2,T3>();
        bool DropTable<T, T2, T3,T4>();
        bool TruncateTable(string tableName);
        bool TruncateTable(params string[] tableName);
        bool TruncateTable(params Type[] tableEntityType);
        bool TruncateTable<T>();
        bool TruncateTable<T,T2>();
        bool TruncateTable<T, T2, T3>();
        bool TruncateTable<T, T2, T3,T4>();
        bool TruncateTable<T, T2, T3, T4,T5>();
        bool CreateTable(string tableName, List<DbColumnInfo> columns,bool isCreatePrimaryKey=true);
        bool AddColumn(string tableName, DbColumnInfo column);
        bool UpdateColumn(string tableName, DbColumnInfo column);
        bool AddPrimaryKey(string tableName,string columnName);
        bool AddPrimaryKeys(string tableName, string [] columnNames);
        bool AddPrimaryKeys(string tableName, string[] columnNames,string pkName);
        bool DropConstraint(string tableName, string constraintName);
        bool BackupDataBase(string databaseName,string fullFileName);
        bool BackupTable(string oldTableName, string newTableName, int maxBackupDataRows = int.MaxValue);
        bool DropColumn(string tableName,string columnName);
        bool RenameColumn(string tableName, string oldColumnName, string newColumnName);
        bool AddRemark(EntityInfo entity);
        void AddIndex(EntityInfo entityInfo);
        void AddDefaultValue(EntityInfo entityInfo);
        bool IsAnyDefaultValue(string tableName, string columnName);
        bool IsAnyIndex(string indexName);
        bool AddColumnRemark(string columnName,string tableName,string description);
        bool DeleteColumnRemark(string columnName, string tableName);
        bool IsAnyColumnRemark(string columnName, string tableName);
        bool AddTableRemark( string tableName, string description);
        bool DeleteTableRemark(string tableName);
        bool IsAnyTableRemark(string tableName);
        bool RenameTable(string oldTableName,string newTableName);
        /// <summary>
        ///by current connection string
        /// </summary>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        bool CreateDatabase(string databaseDirectory = null);
        /// <summary>
        /// by databaseName
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="databaseDirectory"></param>
        /// <returns></returns>
        bool CreateDatabase(string databaseName,string databaseDirectory = null);
        #endregion
    }
}
