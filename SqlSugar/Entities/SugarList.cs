using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class MappingTableList : List<MappingTable>  
    {
        public void Add(string entityName, string dbTableName)
        {
            this.RemoveAll(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingTable() { EntityName = entityName, DbTableName = dbTableName });
        }
        public void Add(string entityName, string dbTableName,string dbTableShortName)
        {
            this.RemoveAll(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingTable() { EntityName = entityName, DbTableName = dbTableName, DbShortTaleName =dbTableShortName});
        }
    }

    public class IgnoreComumnList : List<IgnoreComumn> {
        public void Add(string EntityPropertyName,string EntityName)
        {
            this.RemoveAll(it => it.EntityPropertyName.Equals(EntityPropertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new IgnoreComumn() { EntityPropertyName = EntityPropertyName,  EntityName=EntityName});
        }
    }

    public class MappingColumnList: List<MappingColumn> 
    {
        public void Add(string EntityPropertyName, string dbColumnName,string dbTableName)
        {
            this.RemoveAll(it => it.EntityPropertyName.Equals(EntityPropertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingColumn() { EntityPropertyName = EntityPropertyName, DbColumnName = dbColumnName, DbTableName= dbTableName });
        }
    }
}
