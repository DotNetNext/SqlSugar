using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
namespace SqlSugar.MongoDb
{
    public class ParameterExpressionTractor
    {
        private MongoNestedTranslatorContext context;
        private ExpressionVisitorContext visitorContext;

        public ParameterExpressionTractor(MongoNestedTranslatorContext context, ExpressionVisitorContext visitorContext)
        {
            this.context = context;
            this.visitorContext = visitorContext;
        }

        internal BsonValue Extract(ParameterExpression parameterExpression)
        {
            BsonValue bson = new BsonDocument();
            if (IsSelect())
            {
                var isJoinTable = this.context?.queryBuilder?.JoinQueryInfos?.Any(s => s.ShortName.EqualCase(parameterExpression.Name)) == true;
                foreach (var item in this.context.context.EntityMaintenance.GetEntityInfo(parameterExpression.Type).Columns)
                {
                    // 跳过忽略的列
                    if (item.IsIgnore) continue;

                    if (isJoinTable)
                    {
                        // 构建 "$x.y"
                        string mongoFieldPath = "$" + parameterExpression.Name + "." + item.DbColumnName;

                        // 设置别名
                        string alias = item.DbColumnName; // 或者你自己定义别名逻辑

                        ((BsonDocument)bson).Add(alias, mongoFieldPath);
                    }
                    else
                    {
                        ((BsonDocument)bson).Add(item.DbColumnName, 1);
                    }
                }
            }
            return bson;
        }

        private bool IsSelect()
        {
            return this.context.resolveType.IsIn(ResolveExpressType.SelectSingle, ResolveExpressType.SelectMultiple);
        }
    }
}
