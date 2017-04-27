using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public partial interface IDbMaintenance
    {
        SqlSugarClient Context { get; set; }

        List<DbTableInfo> GetViewInfoList();

        List<DbTableInfo> GetTableInfoList();

        List<DbColumnInfo> GetColumnInfosByTableName(string tableName);

        string GetSinglePrimaryFiled(string tableName);

        bool TruncateTable(string tableName);

        bool CreateTable(string tableName, List<DbColumnInfo> columns);

        bool AddColumnToTable(string tableName, DbColumnInfo column);

        bool BackupDataBase();
    }
}
