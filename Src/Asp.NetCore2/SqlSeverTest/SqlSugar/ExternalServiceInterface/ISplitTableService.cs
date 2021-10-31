using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public interface ISplitTableService
    {
        List<SplitTableInfo> GetAllTables(ISqlSugarClient db,EntityInfo EntityInfo,List<DbTableInfo> tableInfos);
         string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo);
        string GetTableName(ISqlSugarClient db, EntityInfo EntityInfo, SplitType type);
        string GetTableName(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object fieldValue);
        object GetFieldValue(ISqlSugarClient db, EntityInfo entityInfo, SplitType splitType, object entityValue);
    }
}
