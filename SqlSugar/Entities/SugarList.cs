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
        public void Add(string entityName, string dbTableName, string dbTableShortName)
        {
            this.RemoveAll(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingTable() { EntityName = entityName, DbTableName = dbTableName, DbShortTaleName = dbTableShortName });
        }
        public new void Clear()
        {
            this.RemoveAll(it=>true);
        }
    }

    public class IgnoreComumnList : List<IgnoreComumn>
    {
        public void Add(string propertyName, string EntityName)
        {
            this.RemoveAll(it => it.EntityPropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new IgnoreComumn() { EntityPropertyName = propertyName, EntityName = EntityName });
        }

        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }

    public class MappingColumnList : List<MappingColumn>
    {
        public void Add(string propertyName, string dbColumnName, string entityName)
        {
            this.RemoveAll(it => it.EntityPropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingColumn() { EntityPropertyName = propertyName, DbColumnName = dbColumnName, EntityName = entityName });
        }
        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }
}
