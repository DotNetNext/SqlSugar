using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class OneToManyNavgateExpressionN
    {
        #region  Constructor
        public SqlSugarProvider context;
        public string shorName;
        public EntityInfo entityInfo;
        public List<ExpressionItems> items;
        public string whereSql;
        public MethodCallExpressionResolve methodCallExpressionResolve;
        public OneToManyNavgateExpressionN(SqlSugarProvider context, MethodCallExpressionResolve methodCallExpressionResolve)
        {
            this.context = context;
            this.methodCallExpressionResolve= methodCallExpressionResolve;
    }
        #endregion

        internal bool IsNavgate(Expression expression)
        {
            var result = false;
            var exp = expression;
            if (exp is UnaryExpression) 
            {
                exp = (exp as UnaryExpression).Operand;
            }
            if (exp is MethodCallExpression) 
            {
                var memberExp=exp as MethodCallExpression;
                if (memberExp.Method.Name.IsIn("Any","Count") &&  memberExp.Arguments.Count>0 && memberExp.Arguments[0] is MemberExpression ) 
                {
                    result = ValiteOneManyCall(result, memberExp, memberExp.Arguments[0] as MemberExpression, memberExp.Arguments[0]);
                    if (memberExp.Arguments.Count > 1)
                    {
                        whereSql = GetWhereSql(memberExp);
                    }
                }
            }
            return result;
        }
        private bool ValiteOneManyCall(bool result,MethodCallExpression callExpression, MemberExpression memberExp, Expression childExpression)
        {
            if (childExpression != null && childExpression is MemberExpression)
            {
                var oldChildExpression = childExpression;
                var child2Expression = (childExpression as MemberExpression).Expression;
                if (child2Expression == null || (child2Expression is ConstantExpression))
                {
                    return false;
                }
                items = new List<ExpressionItems>();
                var childType = oldChildExpression.Type;
                if (!childType.FullName.IsCollectionsList()) 
                {
                    return false;
                }
                childType = childType.GetGenericArguments()[0];
                items.Add(new ExpressionItems() { Type = 3, Expression = callExpression, ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(childType) });
                items.Add(new ExpressionItems() { Type = 2, Expression = oldChildExpression, ThisEntityInfo = this.context.EntityMaintenance.GetEntityInfo(childType), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type) });
                if (items.Any(it => it.Type == 2 && it.Nav == null))
                {
                    return false;
                }
                while (child2Expression != null)
                {
                    if (IsClass(child2Expression))
                    {
                        items.Add(new ExpressionItems() { Type = 2, Expression = child2Expression, ThisEntityInfo = this.context.EntityMaintenance.GetEntityInfo(child2Expression.Type), ParentEntityInfo = this.context.EntityMaintenance.GetEntityInfo(GetMemberExpression(child2Expression).Type) });
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
        public object GetSql()
        {
            MapperSql MapperSql = new MapperSql();
            var memberInfo = this.items.Where(it => it.Type == 3).First();
            var subInfos = this.items.Where(it => it.Type == 2).Reverse().ToList();
            var formInfo = subInfos.First();
            var joinInfos = subInfos.Skip(1).ToList();
            var i = 0;
            var masterShortName = formInfo.ThisEntityInfo.DbTableName + i;
            var queryable = this.context.Queryable<object>(masterShortName).AS(formInfo.ThisEntityInfo.DbTableName);
            i++;
            var lastShortName = "";
            var index = 0;
            foreach (var item in joinInfos)
            {
                if (item.Nav.NavigatType == NavigateType.OneToMany)
                {
                    lastShortName = OneToMany(ref formInfo, ref i, queryable, ref index, item);
                }
                else if (item.Nav.NavigatType == NavigateType.OneToOne)
                {
                    lastShortName = OneToOne(ref formInfo, ref i, queryable, ref index, item);
                }
                else
                {
                    lastShortName = ManyToMany(ref formInfo, ref i, queryable, ref index, item); 
                }
            }
            var isAny = (memberInfo.Expression as MethodCallExpression).Method.Name == "Any";
            queryable.Select(isAny ? "1" : " COUNT(1) ");
            var last = subInfos.First();
            var FirstPkColumn = last.ThisEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            FirstPkColumn = GetFirstPkColumn(last, FirstPkColumn);
            Check.ExceptionEasy(FirstPkColumn == null, $"{ last.ThisEntityInfo.EntityName} need PrimayKey", $"使用导航属性{ last.ThisEntityInfo.EntityName} 缺少主键");
            var PkColumn = last.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == last.Nav.Name);
            Check.ExceptionEasy(PkColumn == null, $"{ last.ParentEntityInfo.EntityName} no found {last.Nav.Name}", $"{ last.ParentEntityInfo.EntityName} 不存在 {last.Nav.Name}");
            queryable.Where($" {this.shorName}.{ queryable.SqlBuilder.GetTranslationColumnName(PkColumn.DbColumnName)} = {masterShortName}.{queryable.SqlBuilder.GetTranslationColumnName(FirstPkColumn.DbColumnName)} ");
            queryable.WhereIF(this.whereSql.HasValue(), GetWhereSql1(this.whereSql,lastShortName, joinInfos, queryable.SqlBuilder));
            MapperSql.Sql = $"( {queryable.ToSql().Key} ) ";
            if (isAny)
            {
                MapperSql.Sql = $" EXISTS( {MapperSql.Sql}) ";

            }
            return MapperSql;
        }

        private static EntityColumnInfo GetFirstPkColumn(ExpressionItems last, EntityColumnInfo FirstPkColumn)
        {
            if (last.Nav.NavigatType == NavigateType.OneToOne && last.Nav.Name2.HasValue())
            {
                var name2 = last.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == last.Nav.Name2);
                if (name2 != null)
                {
                    FirstPkColumn = name2;
                }
            }
            return FirstPkColumn;
        }

        private static string OneToMany(ref ExpressionItems formInfo, ref int i, ISugarQueryable<object> queryable, ref int index, ExpressionItems item)
        {
            string lastShortName;
            var shortName = item.ThisEntityInfo.DbTableName + i;
            EntityColumnInfo pkColumn;
            EntityColumnInfo navColum;
            //if (index == 0)
            //{
                pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name);
                navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
                if (item.Nav.Name2.HasValue()) 
                {
                    navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name2);
                }
            //}
            //else
            //{
            //    pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            //    navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name);
            //}
            Check.ExceptionEasy(pkColumn == null, $"{item.ThisEntityInfo.EntityName} need PrimayKey", $"使用导航属性{item.ThisEntityInfo.EntityName} 缺少主键");
            var on = $" {shortName}.{queryable.SqlBuilder.GetTranslationColumnName(pkColumn.DbColumnName)}={formInfo.ThisEntityInfo.DbTableName + (i - 1)}.{queryable.SqlBuilder.GetTranslationColumnName(navColum.DbColumnName)}";
            queryable.AddJoinInfo(item.ThisEntityInfo.DbTableName, shortName, on, JoinType.Inner);
            ++i;
            index++;
            lastShortName = shortName;
            formInfo = item;
            return lastShortName;
        }

        private static string OneToOne(ref ExpressionItems formInfo, ref int i, ISugarQueryable<object> queryable, ref int index, ExpressionItems item)
        {
            string lastShortName;
            var shortName = item.ThisEntityInfo.DbTableName + i;
            EntityColumnInfo pkColumn;
            EntityColumnInfo navColum;
            //if (index == 0)
            //{
            //    pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name);
            //    navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            //}
            //else
            //{
                pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it =>it.IsPrimarykey );
                navColum = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name);
                if (item.Nav.Name2.HasValue())
                {
                   pkColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == item.Nav.Name2);
                }
            //}
            Check.ExceptionEasy(pkColumn == null, $"{item.ThisEntityInfo.EntityName} need PrimayKey", $"使用导航属性{item.ThisEntityInfo.EntityName} 缺少主键");
            var on = $" {shortName}.{queryable.SqlBuilder.GetTranslationColumnName(pkColumn.DbColumnName)}={formInfo.ThisEntityInfo.DbTableName + (i - 1)}.{queryable.SqlBuilder.GetTranslationColumnName(navColum.DbColumnName)}";
            queryable.AddJoinInfo(item.ThisEntityInfo.DbTableName, shortName, on, JoinType.Inner);
            ++i;
            index++;
            lastShortName = shortName;
            formInfo = item;
            return lastShortName;
        }


        private string ManyToMany(ref ExpressionItems formInfo, ref int i, ISugarQueryable<object> queryable, ref int index, ExpressionItems item)
        {
            string lastShortName;
            var bshortName = item.ThisEntityInfo.DbTableName + i;
            EntityColumnInfo AidColumn;
            EntityColumnInfo BidColumn;

            BidColumn = item.ThisEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);
            AidColumn = item.ParentEntityInfo.Columns.FirstOrDefault(it => it.IsPrimarykey);

            var abEntity =this.context.EntityMaintenance.GetEntityInfo(item.Nav.MappingType);
            var Ab_Aid = abEntity.Columns.FirstOrDefault(it => item.Nav.MappingAId == it.PropertyName);
            var Ab_Bid = abEntity.Columns.FirstOrDefault(it => item.Nav.MappingBId == it.PropertyName);

            Check.ExceptionEasy(AidColumn == null, $" {AidColumn.EntityName} need primary key ", $"{AidColumn.EntityName}需要主键");
            Check.ExceptionEasy(AidColumn == null, $" {BidColumn.EntityName} need primary key ", $"{BidColumn.EntityName}需要主键");

            var abShort = abEntity.EntityName + "_1";
            var abOn = $" {abShort}.{queryable.SqlBuilder.GetTranslationColumnName(Ab_Aid.DbColumnName)}={formInfo.ThisEntityInfo.DbTableName + (i - 1)}.{queryable.SqlBuilder.GetTranslationColumnName(AidColumn.DbColumnName)}";
            queryable.AddJoinInfo(abEntity.DbTableName, abShort, abOn, JoinType.Inner);
            var On = $" {bshortName}.{queryable.SqlBuilder.GetTranslationColumnName(BidColumn.DbColumnName)}={abShort}.{queryable.SqlBuilder.GetTranslationColumnName(Ab_Bid.DbColumnName)}";
            queryable.AddJoinInfo(BidColumn.DbTableName, bshortName, On, JoinType.Inner);
            ++i;
            index++;
            lastShortName = bshortName;
            formInfo = item;
            return lastShortName;
        }

        #region Helper
        private string GetWhereSql1(string wheresql,string lastShortName, List<ExpressionItems> joinInfos,ISqlBuilder sqlBuilder)
        {
            var sql = wheresql;
            if (sql == null) return sql;
            joinInfos.Last().ThisEntityInfo.Columns.ForEach(it =>
            {
                if (it.DbColumnName != null)
                {
                    if (this.whereSql.Contains("." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)))
                    {
                        var regex = @"\w+\." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)
                            .Replace(sqlBuilder.SqlTranslationLeft, "\\" + sqlBuilder.SqlTranslationLeft)
                            .Replace(sqlBuilder.SqlTranslationRight, "\\" + sqlBuilder.SqlTranslationRight)
                            .Replace("\\\\","\\");

                        if (!regex.IsMatch(this.whereSql)) 
                        {
                            regex = $@"\{sqlBuilder.SqlTranslationLeft}\w+\{sqlBuilder.SqlTranslationRight}\." + sqlBuilder.GetTranslationColumnName(it.DbColumnName)
                            .Replace(sqlBuilder.SqlTranslationLeft, "\\" + sqlBuilder.SqlTranslationLeft)
                            .Replace(sqlBuilder.SqlTranslationRight, "\\" + sqlBuilder.SqlTranslationRight)
                            .Replace("\\\\", "\\");
                        }

                        this.whereSql =Regex.Replace(this.whereSql, regex,
                             lastShortName + "." + sqlBuilder.GetTranslationColumnName(it.DbColumnName));
                    }
                    else
                    {
                        var oldWhere = this.whereSql;
                        var newWhere = this.whereSql.Replace(sqlBuilder.GetTranslationColumnName(it.DbColumnName),
                            lastShortName + "." + sqlBuilder.GetTranslationColumnName(it.DbColumnName));
                        if (oldWhere != newWhere && oldWhere.TrimStart().StartsWith("( (SELECT "))
                        {

                        }
                        else 
                        {
                            this.whereSql = newWhere;
                        }
                    }
                }
            });
            return this.whereSql;
        }

        private string GetWhereSql(MethodCallExpression memberExp)
        {
            var whereExp = memberExp.Arguments[1];
            var result = this.methodCallExpressionResolve.GetNewExpressionValue(whereExp);
            return result;
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
