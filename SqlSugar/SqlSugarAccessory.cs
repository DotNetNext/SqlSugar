using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace SqlSugar
{
    public partial class SqlSugarAccessory
    {
        public string EntityNamespace { get; set; }
        public IConnectionConfig CurrentConnectionConfig { get; set; }
        public Dictionary<string, object> TempItems { get; set; }
        public Guid ContextID { get; set; }
        public MappingTableList MappingTables = new MappingTableList();
        public MappingColumnList MappingColumns = new MappingColumnList();

        protected ISqlBuilder _SqlBuilder;
        protected IDb _Ado;
        protected ILambdaExpressions _LambdaExpressions;
        protected object _Queryable;
        protected object _Sqlable;

        protected void InitConstructor()
        {
            this.ContextID = Guid.NewGuid();
            if (this.CurrentConnectionConfig is AttrbuitesCofnig)
            {
                this.InitAttributeMappingTables();
                this.InitAttributeMappingColumns();
            }
        }
        protected void InitAttributeMappingTables()
        {
            string cacheKey = "Context.InitAttributeMappingTables";
            CacheFactory.Action<List<MappingTable>>(cacheKey,
            (cm, key) =>
            {
                this.MappingTables.AddRange(cm[key]);
            },
            (cm, key) =>
            {
                var classes = Assembly.Load(this.EntityNamespace.Split('.').First()).GetTypes();
                List<MappingTable> mappingList = new List<MappingTable>();
                foreach (var item in classes)
                {
                    if (item.Namespace == this.EntityNamespace)
                    {
                        var sugarTableObj = item.GetCustomAttributes(typeof(SugarTable), true).Where(it => it is SugarTable).SingleOrDefault();
                        if (sugarTableObj.IsValuable())
                        {
                            var sugarTable = (SugarTable)sugarTableObj;
                            if (item.Name != sugarTable.TableName)
                                mappingList.Add(new MappingTable() { EntityName = item.Name, DbTableName = sugarTable.TableName });
                        }
                    }
                }
                this.MappingTables.AddRange(mappingList);
                return mappingList;
            });
        }

        protected void InitAttributeMappingColumns()
        {
            string cacheKey = "Context.InitAttributeMappingColumns";
            CacheFactory.Action<List<MappingColumn>>(cacheKey,
            (cm, key) =>
            {
                this.MappingColumns.AddRange(cm[cacheKey]);
            },
            (cm, key) =>
            {
                var assembly = Assembly.Load(this.EntityNamespace.Split('.').First());
                var entityTypeArray = assembly.GetTypes();
                List<MappingColumn> mappingList = new List<MappingColumn>();
                foreach (var entityType in entityTypeArray)
                {
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityType.Name, StringComparison.CurrentCultureIgnoreCase));
                    var tableName = mappingInfo.IsNullOrEmpty() ? entityType.Name : mappingInfo.DbTableName;
                    var entityName = mappingInfo.IsNullOrEmpty() ? entityType.Name : mappingInfo.EntityName;
                    if (entityType.Namespace == this.EntityNamespace)
                        foreach (var item in assembly.GetType(this.EntityNamespace + "." + entityName).GetProperties())
                        {
                            var sugarColumnObjs = item.GetCustomAttributes(typeof(SugarColumn), true).Where(it => it is SugarColumn).ToList();
                            foreach (var sugarColumnObj in sugarColumnObjs)
                            {
                                var sugarColumn = (SugarColumn)sugarColumnObj;
                                if (sugarColumn.ColumnName.IsValuable())
                                {
                                    if (item.Name != sugarColumn.ColumnName && sugarColumn.ColumnName.IsValuable())
                                    {
                                        this.MappingColumns.Add(item.Name, sugarColumn.ColumnName, tableName);
                                    }
                                }
                            }
                        }
                }
                this.MappingColumns.AddRange(mappingList);
                return mappingList;
            });
        }

        protected List<JoinQueryInfo> GetJoinInfos(Expression joinExpression, SqlSugarClient context,params  Type [] entityTypeArray)
        {
            ExpressionContext exp = new ExpressionContext();
            exp.Resolve(joinExpression, ResolveExpressType.Join);
            var reval = exp.Result.GetResultArray();
            return null;
        }
    }
}
