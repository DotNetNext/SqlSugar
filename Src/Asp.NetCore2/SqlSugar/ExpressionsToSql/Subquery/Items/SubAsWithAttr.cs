using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

namespace SqlSugar 
{
    public class SubAsWithAttr : ISubOperation
    {
        public bool HasWhere
        {
            get; set;
        }

        public string Name
        {
            get
            {
                return "AsWithAttr";
            }
        }

        public Expression Expression
        {
            get; set;
        }


        public int Sort
        {
            get
            {
                return 200;
            }
        }

        public ExpressionContext Context
        {
            get; set;
        }

        public string GetValue(Expression expression = null)
        {
            var exp = expression as MethodCallExpression;
            var type = exp.Type.GetGenericArguments()[0];
            var db = this.Context.SugarContext.Context;
            var entityInfo= db.EntityMaintenance.GetEntityInfo(type);
            var tableName = entityInfo.DbTableName; 
            var queryable= ((QueryableProvider<object>)(db.Queryable<object>()));
            var expString = queryable.GetTableName(entityInfo, tableName); 
            return "$SubAs:" + expString;
        }
    }
}
