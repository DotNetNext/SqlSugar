using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public class InsertableProvider<T> : IInsertable<T> where T : class, new()
    {
        public SqlSugarClient Context { get; set; }
        public bool IsMappingTable { get { return this.Context.MappingTables != null && this.Context.MappingTables.Any(); } }
        public bool IsMappingColumns { get { return this.Context.MappingColumns != null && this.Context.MappingColumns.Any(); } }
        public IDb Db { get { return Context.Database; } }
        public ISqlBuilder SqlBuilder { get; set; }
        public InsertBuilder InsertBuilder { get { return this.InsertBuilder; } }
        public EntityInfo EntityInfo { get; set; }
        private List<string> IgnoreColumnList { get; set; }
        private List<string> InsertColumList { get; set; }
        public T[] InsertObjs { get; set; }
        public int ExecuteCommand()
        {
            return Db.ExecuteCommand(InsertBuilder.ToSqlString(), InsertBuilder.Parameters);
        }
        public KeyValuePair<string, List<SugarParameter>> ToSql()
        {
            string sql = InsertBuilder.ToSqlString();
            return new KeyValuePair<string, List<SugarParameter>>(sql, InsertBuilder.Parameters);
        }

        public int ExecuteReutrnIdentity()
        {
            return Db.GetInt(InsertBuilder.ToSqlString(), InsertBuilder.Parameters);
        }

        public IInsertable<T> IgnoreColumns(Expression<Func<T, object[]>> columns)
        {
            throw new NotImplementedException();
        }

        public IInsertable<T> InsertColumns(Expression<Func<T, object[]>> columns)
        {
            return this;
        }

        public IInsertable<T> With(string lockString)
        {
            return this;
        }

        public IInsertable<T> Where(bool isInsertNull)
        {
            return this;
        }
    }
}
