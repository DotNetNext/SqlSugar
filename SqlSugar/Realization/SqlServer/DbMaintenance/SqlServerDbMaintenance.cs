using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class SqlServerDbMaintenance : DbMaintenanceProvider
    {
        protected override string AddColumnToTableSql
        {
            get
            {
                return "ALERT TABLE {0} ADD {1} {2} {3}";
            }
        }

        protected override string BackupDataBaseSql
        {
            get
            {
                return "";
            }
        }

        protected override string CreateTableSql
        {
            get
            {
                return @"CREATE TABLE {0}
                         {1}";
            }
        }

        protected override string GetColumnInfosByTableNameSql
        {
            get
            {
                string sql = @"SELECT Sysobjects.name AS TableName,
                           syscolumns.Id AS TableId,
                           syscolumns.name AS DbColumnName,
                           systypes.name AS DataType,
                           syscolumns.length AS [Length],
                           sys.extended_properties.[value] AS [ColumnDescription],
                           syscomments.text AS DefaultValue,
                           syscolumns.isnullable AS IsNullable,
	                       columnproperty(syscolumns.id,syscolumns.name,'IsIdentity')as IsIdentity,
                           (CASE
                                WHEN EXISTS
                                       (SELECT 1
                                        FROM sysobjects
                                        WHERE xtype= 'pk'
                                          AND name IN
                                            (SELECT name
                                             FROM sysindexes
                                             WHERE indid IN
                                                 (SELECT indid
                                                  FROM sysindexkeys
                                                  WHERE id = syscolumns.id
                                                    AND colid=syscolumns.colid ))) THEN 1
                                ELSE 0
                            END) AS IsPrimaryKey
                    FROM syscolumns
                    INNER JOIN systypes ON syscolumns.xtype = systypes.xtype
                    LEFT JOIN sysobjects ON syscolumns.id = sysobjects.id
                    LEFT OUTER JOIN sys.extended_properties ON (sys.extended_properties.minor_id = syscolumns.colid
                                                                AND sys.extended_properties.major_id = syscolumns.id)
                    LEFT OUTER JOIN syscomments ON syscolumns.cdefault = syscomments.id
                    WHERE syscolumns.id IN
                        (SELECT id
                         FROM sysobjects
                         WHERE xtype IN('u',
                                        'v') )
                      AND (systypes.name <> 'sysname')
                      AND sysobjects.name='{0}'
                      AND systypes.name<>'geometry'
                      AND systypes.name<>'geography'
                    ORDER BY syscolumns.colid";
                return sql;
            }
        }

        protected override string GetTableInfoListSql
        {
            get
            {
                return @"SELECT s.Name,tbp.value as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0  WHERE s.xtype IN('U')";
            }
        }

        protected override string GetViewInfoListSql
        {
            get
            {
                return @"SELECT s.Name,tbp.value as Description
                            FROM sysobjects s
					     	LEFT JOIN sys.extended_properties as tbp ON s.id=tbp.major_id and tbp.minor_id=0  WHERE s.xtype IN('V')";
            }
        }

        protected override string TruncateTableSql
        {
            get
            {
                return "TRUNCATE TABLE {0}";
            }
        }
    }
}
