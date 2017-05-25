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
        public SqlSugarClient Context { get; set; }
        public string EntityNamespace { get; set; }
        public IConnectionConfig CurrentConnectionConfig { get; set; }
        public Dictionary<string, object> TempItems { get; set; }
        public Guid ContextID { get; set; }
        public MappingTableList MappingTables = new MappingTableList();
        public MappingColumnList MappingColumns = new MappingColumnList();
        public IgnoreComumnList IgnoreColumns = new IgnoreComumnList();

        protected ISqlBuilder _SqlBuilder;
        protected EntityProvider _EntityProvider;
        protected IAdo _Ado;
        protected ILambdaExpressions _LambdaExpressions;
        protected IRewritableMethods _RewritableMethods;
        protected IDbFirst _DbFirst;
        protected ICodeFirst _CodeFirst;
        protected IDbMaintenance _DbMaintenance;

        protected void InitConstructor()
        {
            this.ContextID = Guid.NewGuid();
            if (this.CurrentConnectionConfig is AttributeConfig)
            {
                string cacheKey = "Context.InitAttributeMappingTables";
                CacheFactory.Action<Tuple<MappingTableList, MappingColumnList, IgnoreComumnList>>(cacheKey,
                (cm, key) =>
                {
                    var cacheInfo = cm[key];
                    this.MappingTables = cacheInfo.Item1;
                    this.MappingColumns = cacheInfo.Item2;
                    this.IgnoreColumns = cacheInfo.Item3;
                },
               (cm, key) =>
               {
                   var entities = this.Context.EntityProvider.GetAllEntities();
                   foreach (var entity in entities)
                   {
                       if (entity.Type.IsAnonymousType()) continue;
                       if (entity.DbTableName!=null&&!entity.DbTableName.Equals(entity.EntityName))
                       {
                           this.MappingTables.Add(entity.EntityName, entity.DbTableName);
                       }
                       foreach (var column in entity.Columns)
                       {
                           if (column.IsIgnore)
                           {
                               this.IgnoreColumns.Add(column.PropertyName, column.EnitytName);
                           }
                           else
                           {
                               if (!column.DbColumnName.Equals(column.PropertyName))
                               {
                                   this.MappingColumns.Add(column.PropertyName, column.DbColumnName, column.EnitytName);
                               }
                           }
                       }
                   }
                   var result= Tuple.Create<MappingTableList, MappingColumnList, IgnoreComumnList>(this.MappingTables,this.MappingColumns,this.IgnoreColumns);
                   return this.Context.RewritableMethods.TranslateCopy(result);
               });
            }
        }
        protected List<JoinQueryInfo> GetJoinInfos(Expression joinExpression, ref string shortName, params Type[] entityTypeArray)
        {
            List<JoinQueryInfo> reval = new List<JoinQueryInfo>();
            var lambdaParameters = ((LambdaExpression)joinExpression).Parameters.ToList();
            ExpressionContext exp = new ExpressionContext();
            exp.MappingColumns = this.Context.MappingColumns;
            exp.MappingTables = this.Context.MappingTables;
            exp.Resolve(joinExpression, ResolveExpressType.Join);
            int i = 0;
            var joinArray = exp.Result.GetResultArray();
            foreach (var type in entityTypeArray)
            {
                var isFirst = i == 0;
                ++i;
                JoinQueryInfo joinInfo = new JoinQueryInfo();
                var hasMappingTable = exp.MappingTables.IsValuable();
                MappingTable mappingInfo = null;
                if (hasMappingTable)
                {
                    mappingInfo = exp.MappingTables.FirstOrDefault(it => it.EntityName.Equals(type.Name, StringComparison.CurrentCultureIgnoreCase));
                    joinInfo.TableName = mappingInfo != null ? mappingInfo.DbTableName : type.Name;
                }
                else
                {
                    joinInfo.TableName = type.Name;
                }
                if (isFirst)
                {
                    var firstItem = lambdaParameters.First();
                    lambdaParameters.Remove(firstItem);
                    shortName = firstItem.Name;
                }
                var joinString = joinArray[i * 2 - 2];
                joinInfo.ShortName = lambdaParameters[i - 1].Name;
                joinInfo.JoinType = (JoinType)Enum.Parse(typeof(JoinType), joinString);
                joinInfo.JoinWhere = joinArray[i * 2 - 1];
                joinInfo.JoinIndex = i;
                reval.Add((joinInfo));
            }
            return reval;
        }

        protected void CreateQueryable<T>(ISugarQueryable<T> result) where T : class, new()
        {
            var sqlBuilder = InstanceFactory.GetSqlbuilder(CurrentConnectionConfig);
            result.Context = this.Context;
            result.SqlBuilder = sqlBuilder;
            result.SqlBuilder.QueryBuilder = InstanceFactory.GetQueryBuilder(CurrentConnectionConfig);
            result.SqlBuilder.QueryBuilder.Builder = sqlBuilder;
            result.SqlBuilder.Context = result.SqlBuilder.QueryBuilder.Context = this.Context;
            result.SqlBuilder.QueryBuilder.EntityType = typeof(T);
            result.SqlBuilder.QueryBuilder.EntityName = typeof(T).Name;
            result.SqlBuilder.QueryBuilder.LambdaExpressions = InstanceFactory.GetLambdaExpressions(CurrentConnectionConfig);
        }

    }
}
