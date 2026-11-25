using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SubLeftJoin : ISubOperation
    {

        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get { return "LeftJoin"; }
        }

        public Expression Expression
        {
            get; set;
        }

        public int Sort
        {
            get
            {
                return 302;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression)
        {
            var exp = expression as MethodCallExpression;
            var argExp = exp.Arguments[0];
            var name =this.Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters[0].Name);
            var parameter = (argExp as LambdaExpression).Parameters.Last();
            foreach (var item in (argExp as LambdaExpression).Parameters)
            {
                Context.InitMappingInfo(item.Type);
            }
            this.Context.RefreshMapping();
            var tableName= Context.GetTranslationTableName(parameter.Type.Name, true);
            if (this.Context.IsAsAttr == true) 
            {
                var db = this.Context.SugarContext.Context;
                var entityInfo = db.EntityMaintenance.GetEntityInfo(parameter.Type);
                var queryable = ((QueryableProvider<object>)(db.Queryable<object>()));
                tableName = queryable.GetTableName(entityInfo, tableName);
            }
            if (exp.Arguments.Count == 2 && exp.Arguments.Last().HasValue()) 
            {
                tableName=Context.GetTranslationTableName(ExpressionTool.DynamicInvoke(exp.Arguments.Last())+"");
            }
            var joinString =string.Format(" {2} LEFT JOIN {1} {0} ",
                 this.Context.GetTranslationColumnName(parameter.Name), 
                 tableName,
                 null);
            // NOLOCK 提示仅限两类源：SqlServer 保持原生语义；Vastbase 仅 Babelfish 兼容模式(TDS)
            // 接受（T-SQL 提示语法；PG 方言无此语法，生成即 42601）。括号化子查询源的提示会落
            // 到派生表别名后被拒（42601），故抑制。
            var nolockConfig = this.Context?.SugarContext?.Context?.CurrentConnectionConfig;
            var nolockEligible =
                nolockConfig?.DbType == DbType.SqlServer
                || (nolockConfig?.DbType == DbType.VastbaseV5
                    && VastbaseV5Helper.IsBabelfish(this.Context?.SugarContext?.Context)
                    && !tableName.TrimStart().StartsWith("("));
            if (nolockEligible && nolockConfig?.MoreSettings?.IsWithNoLockSubquery == true)
            {
                joinString = $"{joinString} {SqlWith.NoLock} ";
            }
            var result = joinString+ "ON " + SubTools.GetMethodValue(Context, argExp, ResolveExpressType.WhereMultiple);
            //var selfParameterName = Context.GetTranslationColumnName((argExp as LambdaExpression).Parameters.First().Name) + UtilConstants.Dot;
            this.Context.JoinIndex++;
            new SubSelect() { Context = this.Context }.SetShortName(exp, "+");
            //result = result.Replace(selfParameterName, SubTools.GetSubReplace(this.Context));
            return result;
        }
    }
}
