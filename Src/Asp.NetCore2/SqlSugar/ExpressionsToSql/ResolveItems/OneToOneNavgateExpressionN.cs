using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class OneToOneNavgateExpressionN
    {
        #region  Constructor
        public string shorName { get; set; }
        public EntityInfo entityInfo;
        public List<ExpressionItems> items;
        public SqlSugarProvider context;
        public ISqlBuilder builder;
        public OneToOneNavgateExpressionN(SqlSugarProvider context)
        {
            this.context = context;
        }
        #endregion

        #region Api&Core

        public bool IsNavgate(Expression expression)
        {
            if (this.context == null) return false;
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression)
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MemberExpression)
            {
                var memberExp = exp as MemberExpression;
                var childExpression = memberExp.Expression;
                result = ValidateIsJoinMember(result, memberExp, childExpression);
            }
            return result;
        }
        public MapperSql GetMemberSql()
        {
            MapperSql MapperSql = new MapperSql();
            var memberInfo = this.items.Where(it => it.Type == 1).First();
            var subInfos = this.items.Where(it => it.Type == 2).Reverse().ToList();
            var formInfo = subInfos.First();
            var joinInfos = subInfos.Skip(1).ToList();
            var i = 0;
            var masterShortName = formInfo.ThisEntityInfo.DbTableName + i;
            var queryable = this.context.Queryable<object>(ToShortName(masterShortName)).AS(formInfo.ThisEntityInfo.DbTableName).Filter(null,true);
            builder = queryable.SqlBuilder;
            i++;
            var lastShortName = "";
            foreach (var item in joinInfos)
            {
                var shortName = item.ThisEntityInfo.DbTableName + i;
                var pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
                if (item.Nav.Name2.HasValue()) 
                {
                    var nav2NameColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName==item.Nav.Name2);
                    if (nav2NameColumn != null) 
                    {
                        pkColumn = nav2NameColumn;
                    }
                }
                var navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name);
                Check.ExceptionEasy(pkColumn == null, $"{item.ThisEntityInfo.EntityName} need PrimayKey", $"使用导航属性{item.ThisEntityInfo.EntityName} 缺少主键");
                var on = $" {ToShortName(shortName)}.{queryable.SqlBuilder.GetTranslationColumnName(pkColumn.DbColumnName)}={ToShortName(formInfo.ThisEntityInfo.DbTableName + (i - 1))}.{queryable.SqlBuilder.GetTranslationColumnName(navColum.DbColumnName)}";
                if (item.Nav.WhereSql.HasValue()) 
                {
                    on = (on + " AND " + item.Nav.WhereSql);
                }
                queryable.AddJoinInfo(item.ThisEntityInfo.DbTableName,ToShortName(shortName), on, JoinType.Inner);
                ++i;
                lastShortName = shortName;
                formInfo = item;
            }
            var selectProperyInfo = ExpressionTool.GetMemberName(memberInfo.Expression);
            var selectColumnInfo = memberInfo.ParentEntityInfo.Columns.First(it => it.PropertyName == selectProperyInfo);
            queryable.Select($" {ToShortName(lastShortName)}.{queryable.SqlBuilder.GetTranslationColumnName(selectColumnInfo.DbColumnName)}");
            var last = subInfos.First();
            var FirstPkColumn = last.ThisEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            if (last.Nav.Name2.HasValue()) 
            {
                var nav2 = last.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == last.Nav.Name2);
                if (nav2 != null) 
                {
                    FirstPkColumn = nav2;
                }
            }
            Check.ExceptionEasy(FirstPkColumn == null, $"{ last.ThisEntityInfo.EntityName} need PrimayKey", $"使用导航属性{ last.ThisEntityInfo.EntityName} 缺少主键");
            var PkColumn = last.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == last.Nav.Name);
            Check.ExceptionEasy(PkColumn == null, $"{ last.ParentEntityInfo.EntityName} no found {last.Nav.Name}", $"{ last.ParentEntityInfo.EntityName} 不存在 {last.Nav.Name}");
            queryable.Where($" {ToShortName(this.shorName)}.{queryable.SqlBuilder.GetTranslationColumnName(PkColumn.DbColumnName)} = {ToShortName(masterShortName)}.{queryable.SqlBuilder.GetTranslationColumnName(FirstPkColumn.DbColumnName)} ");
            MapperSql.Sql = "( " + queryable.ToSql().Key + " ) ";
            return MapperSql;
        }
        private bool ValidateIsJoinMember(bool result, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var oldChildExpression = childExpression;
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression == null||(child2Expression is ConstantExpression))
                {
                    return false;
                }
                items = new List<ExpressionItems>();
                items.Add(new ExpressionItems() { Type=1 , Expression= memberExp, ParentEntityInfo= this.context.EntityMaintenance.GetEntityInfo(oldChildExpression.Type )});
                items.Add(new ExpressionItems() { Type = 2, Expression = oldChildExpression, ThisEntityInfo=this.context.EntityMaintenance.GetEntityInfo(oldChildExpression.Type), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type) });
                if (items.Any(it => it.Type == 2 && it.Nav == null)) 
                {
                    return false;
                }
                while (child2Expression != null)
                {
                    if (IsClass(child2Expression))
                    {
                        items.Add(new ExpressionItems() { Type = 2, Expression = child2Expression, ThisEntityInfo=this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(GetMemberExpression(child2Expression).Type) });
                        child2Expression = GetMemberExpression(child2Expression);

                    }
                    else if (IsParameter(child2Expression))
                    {
                        shorName = child2Expression.ToString();
                        entityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
                if (!items.Any(it => it.Type == 2 && it.Nav == null))
                {
                    return true;
                }
            }
            return result;
        }
        #endregion

        #region Helper
        private string ToShortName(string name)
        {
            var result = "";
            if (name.ObjToString().Contains("."))
            {
                if (this.context!= null)
                {
                    var sqlBuilder = this.context.Queryable<object>().SqlBuilder;
                    result = sqlBuilder.SqlTranslationLeft + name.Replace(".", "_") + sqlBuilder.SqlTranslationRight;
                    return result;
                }
                else 
                {
                    result = name.Replace(".", "_");
                }
            }
            else
            {
                result= name;
            }
            if (builder == null) return name;
            return builder.GetTranslationColumnName(name);
        }

        private static bool IsParameter(Expression child2Expression)
        {
            return child2Expression.Type.IsClass() && child2Expression is ParameterExpression;
        }

        private static Expression GetMemberExpression(Expression child2Expression)
        {
            return (child2Expression as MemberExpression).Expression;
        }

        private static bool IsClass(Expression child2Expression)
        {
            return child2Expression.Type.IsClass() && child2Expression is MemberExpression;
        }
        #endregion

    }
}
