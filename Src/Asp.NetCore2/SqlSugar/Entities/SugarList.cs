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
            this.RemoveAll(it => true);
        }
    }

    public class IgnoreColumnList : List<IgnoreColumn>
    {
        public void Add(string propertyName, string EntityName)
        {
            this.RemoveAll(it => it.EntityName == EntityName && it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new IgnoreColumn() { PropertyName = propertyName, EntityName = EntityName });
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
            this.RemoveAll(it => it.EntityName == entityName && it.PropertyName.Equals(propertyName, StringComparison.CurrentCultureIgnoreCase));
            this.Add(new MappingColumn() { PropertyName = propertyName, DbColumnName = dbColumnName, EntityName = entityName });
        }
        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }


    public class QueueList : List<QueueItem>
    {
        public void Add(string sql, SugarParameter[] parameters)
        {
            this.Add(new QueueItem() { Sql = sql, Parameters = parameters });
        }
        public void Add(string sql, List<SugarParameter> parameters)
        {
            if (parameters == null)
                parameters = new List<SugarParameter>();
            this.Add(new QueueItem() { Sql = sql, Parameters = parameters.ToArray() });
        }
        public new void Clear()
        {
            this.RemoveAll(it => true);
        }
    }
}
