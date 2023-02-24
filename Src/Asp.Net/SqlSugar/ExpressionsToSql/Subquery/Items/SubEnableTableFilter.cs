using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace SqlSugar
{
    public class SubEnableTableFilter : ISubOperation
    {
        public ExpressionContext Context
        {
            get;set;
        }

        public Expression Expression
        {
            get;set;
        }

        public bool HasWhere
        {
            get;set;
        }

        public string Name
        {
            get
            {
                return "EnableTableFilter";
            }
        }

        public int Sort
        {
            get
            {
                return 402;
            }
        }

        public string GetValue(Expression expression)
        {
            var result = "";
            if (this.Context.SugarContext != null)
            {
                var db = this.Context.SugarContext.Context;
                BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
                Type type = this.Context.SubTableType;
                var isWhere = HasWhere;
                if (db.QueryFilter.GeFilterList != null) {
                    foreach (var item in db.QueryFilter.GeFilterList)
                    {
                        PropertyInfo field = item.GetType().GetProperty("exp", flag);
                        if (field != null)
                        {
                            Type ChildType = item.GetType().GetProperty("type", flag).GetValue(item, null) as Type;
                            if (ChildType == type|| (ChildType.IsInterface&&type.GetInterfaces().Contains(ChildType)))
                            {
                                var entityInfo = db.EntityMaintenance.GetEntityInfo(ChildType);
                                var exp = field.GetValue(item, null) as Expression;
                                var whereStr = isWhere ? " AND " : " WHERE ";
                                isWhere = true;
                                result += (whereStr + SubTools.GetMethodValue(Context, exp, ResolveExpressType.WhereSingle));
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}
