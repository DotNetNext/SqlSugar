using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SqlSugar
{
    public class MapperExpressionResolve
    {
        private Expression expression;
        private List<MapperExpression> mappers;
        private InvalidOperationException ex;
        private SqlSugarProvider context;
        private QueryBuilder querybuiler;
        private ISqlBuilder sqlBuilder;
        private string sql;
        public MapperExpressionResolve(Expression expression, InvalidOperationException ex)
        {
            this.expression = expression;
            this.ex = ex;
            this.mappers = CallContext.MapperExpression.Value;
            Error01();
            var isMember = expression is MemberExpression;
            if (isMember)
            {
                ResolveMember();
            }
            else
            {
                ResolveList();
            }
        }

        private void ResolveList()
        {
            var methodExpression = expression as MethodCallExpression;
            var callName = methodExpression.Method.Name;
            var exp= methodExpression.Arguments[0] as MemberExpression;
            ThrowTrue(exp == null);
            var childExpression = exp;
            MapperExpression mapper = GetMapperMany(exp);
            var fillInfo = GetFillInfoMany(childExpression, mapper);
            var mappingFild1Info = GetMappingFild1ManyInfo(childExpression, mapper);
            var mappingFild1Info2 = GetMappingFild2Info(childExpression, mapper);
            //var SelectInfo = GetSelectInfo(expression);
            this.context.InitMappingInfo(childExpression.Expression.Type);
            var entity = this.context.EntityMaintenance.GetEntityInfo(childExpression.Expression.Type);
            oneToMany(methodExpression, callName, entity, childExpression.Expression.ToString(), fillInfo, mappingFild1Info, mappingFild1Info2);
        }

        private void ResolveMember()
        {
            var exp = expression as MemberExpression;
            ThrowTrue(exp.Expression == null);
            var childExpression = exp.Expression;
            MapperExpression mapper = GetMapper(exp);
            var fillInfo=GetFillInfo(childExpression, mapper);
            var mappingFild1Info = GetMappingFild1Info(childExpression, mapper);
            var mappingFild1Info2 = GetMappingFild2Info(childExpression, mapper);
            var SelectInfo = GetSelectInfo(expression);
            var entity = this.context.EntityMaintenance.GetEntityInfo(childExpression.Type);

            var isExMapper = mappingFild1Info2!=null;
            var isFillFild1SameType = fillInfo.Type == mappingFild1Info.Type;
            var isSameProperty = false;

            if (isExMapper)
            {
                ExtMapper(fillInfo, mappingFild1Info, mappingFild1Info2, SelectInfo);
            }
            else if (isSameProperty)
            {

            }
            else if (isFillFild1SameType)
            {
                throw new NotSupportedException(expression.ToString());
            }
            else
            {
                oneToOne(fillInfo, mappingFild1Info, mappingFild1Info2,SelectInfo);
            }
        }

   
        private void oneToOne(MapperExpressionInfo fillInfo, MapperExpressionInfo mappingFild1Info, MapperExpressionInfo mappingFild1Info2, MapperExpressionInfo selectInfo)
        {
            var pkColumn = selectInfo.EntityInfo.Columns.Where(it => it.IsPrimarykey == true).FirstOrDefault();
            if (pkColumn == null)
            {
                pkColumn = selectInfo.EntityInfo.Columns.First();
            }
            var tableName = sqlBuilder.GetTranslationTableName(fillInfo.EntityInfo.DbTableName);
            var whereLeft = sqlBuilder.GetTranslationColumnName(pkColumn.DbColumnName);
            var whereRight = sqlBuilder.GetTranslationColumnName(mappingFild1Info.FieldString);
            this.sql = this.context.Queryable<object>()
                                                        .AS(tableName)
                                                        .Where(string.Format(" {0}={1} ",whereLeft , whereRight))
                                                        .Select(sqlBuilder.GetTranslationColumnName(selectInfo.FieldName)).ToSql().Key;
        }

        private void oneToMany(MethodCallExpression methodCallExpression,string methodName,EntityInfo mainEntity,string shortName,MapperExpressionInfo fillInfo, MapperExpressionInfo mappingFild1Info, MapperExpressionInfo mappingFild1Info2)
        {
            var pkColumn = mainEntity.Columns.FirstOrDefault(it => it.IsPrimarykey == true);
            if (pkColumn == null)
            {
                pkColumn = mainEntity.Columns.FirstOrDefault();
            }
            var tableName = sqlBuilder.GetTranslationTableName(fillInfo.EntityInfo.DbTableName);
            var whereLeft = sqlBuilder.GetTranslationColumnName(mappingFild1Info.FieldString);
            var whereRight = sqlBuilder.GetTranslationColumnName(shortName + "." + pkColumn.DbColumnName);
            string whereExpression = GetWhereExpression(methodCallExpression);
            if (methodName == "Any")
            {
                this.sql = " (" + this.context.Queryable<object>()
                                                            .AS(tableName)
                                                            .Where(string.Format(" {0}={1} ", whereLeft, whereRight))
                                                            .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression)
                                                            .Select("COUNT(1)").ToSql().Key + ")>0 ";
            }
            else
            {
                this.sql = this.context.Queryable<object>()
                                                         .AS(tableName)
                                                         .Where(string.Format(" {0}={1} ", whereLeft, whereRight))
                                                         .WhereIF(!string.IsNullOrEmpty(whereExpression), whereExpression)
                                                         .Select("COUNT(1)").ToSql().Key;
            }
        }

        private string GetWhereExpression(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression.Arguments.Count <= 1)
                return null;
            var exp= methodCallExpression.Arguments[1];
            var querybuiler=InstanceFactory.GetQueryBuilder(this.context.CurrentConnectionConfig);
            querybuiler.LambdaExpressions = InstanceFactory.GetLambdaExpressions(this.context.CurrentConnectionConfig);
            querybuiler.Builder = InstanceFactory.GetSqlbuilder(this.context.CurrentConnectionConfig);
            querybuiler.Builder.Context = querybuiler.Context;
            querybuiler.Builder.QueryBuilder = querybuiler;
            querybuiler.Context = this.context;
            var expValue=querybuiler.GetExpressionValue(exp, ResolveExpressType.WhereMultiple);
            var paramterName = (exp as LambdaExpression).Parameters[0].Name;
            var sql = expValue.GetResultString();
            sql = sql.Replace(querybuiler.Builder.GetTranslationColumnName(paramterName) + ".", "");
            if (querybuiler.Parameters != null && querybuiler.Parameters.Count > 0)
            {
                foreach (var item in querybuiler.Parameters)
                {
                    sql = sql.Replace(item.ParameterName,item.Value.ObjToString().ToSqlValue());
                }
            }
            return sql;
        }

        private MapperExpressionInfo GetSelectInfo(Expression expression)
        {
      
            var field = expression;
            if (field is UnaryExpression)
            {
                field = (field as UnaryExpression).Operand;
            }
            var type = ((field as MemberExpression).Expression).Type;
            this.context.InitMappingInfo(type);
            var name = (field as MemberExpression).Member.Name;
            var entity = this.context.EntityMaintenance.GetEntityInfo(type);
            var fieldName = entity.Columns.First(it => it.PropertyName == name).DbColumnName;
            return new MapperExpressionInfo()
            {
                Type = type,
                FieldName = fieldName,
                EntityInfo= entity
            };
        }

        private MapperExpressionInfo GetMappingFild2Info(Expression childExpression, MapperExpression mapper)
        {
            if (mapper.MappingField2Expression == null)
                return null;
            var exp = mapper.MappingField2Expression;
            var field = (exp as LambdaExpression).Body;
            if (field is UnaryExpression)
            {
                field = (field as UnaryExpression).Operand;
            }
            var type = ((field as MemberExpression).Expression).Type;
            this.context.InitMappingInfo(type);
            var name = (field as MemberExpression).Member.Name;
            var entity = this.context.EntityMaintenance.GetEntityInfo(type);
            var fieldName = entity.Columns.First(it => it.PropertyName == name).DbColumnName;
            return new MapperExpressionInfo()
            {
                Type = type,
                FieldName = fieldName
            };
        }

        private MapperExpressionInfo GetMappingFild1Info(Expression childExpression, MapperExpression mapper)
        {
            var exp = mapper.MappingField1Expression;
            var field = (exp as LambdaExpression).Body;
            if (field is UnaryExpression)
            {
                field = (field as UnaryExpression).Operand;
            }
            var type = ((field as MemberExpression).Expression).Type;
            this.context.InitMappingInfo(type);
            var name =(field as MemberExpression).Member.Name;
            var entity = this.context.EntityMaintenance.GetEntityInfo(type);
            var fieldName=entity.Columns.First(it => it.PropertyName == name).DbColumnName;
            var array = (field as MemberExpression).ToString().Split('.').ToList();
            array[array.Count()-1] = fieldName;
            var filedString = string.Join(".", array);
            return new MapperExpressionInfo()
            {
                 Type=type,
                 FieldName = fieldName,
                 FieldString= filedString,
                 EntityInfo= entity
            };
        }

        private MapperExpressionInfo GetFillInfo(Expression childExpression, MapperExpression mapper)
        {
            this.querybuiler = mapper.QueryBuilder;
            this.context = mapper.Context;
            this.sqlBuilder = mapper.SqlBuilder;
            if (this.querybuiler.TableShortName.IsNullOrEmpty())
            {
                this.querybuiler.TableShortName = (childExpression as MemberExpression).Expression.ToString();
            }
            this.context.InitMappingInfo(childExpression.Type);
            return new MapperExpressionInfo() {
                 EntityInfo=this.context.EntityMaintenance.GetEntityInfo(childExpression.Type)
            };
        }

        private MapperExpression GetMapper(MemberExpression exp)
        {
            var mapper= mappers.Where(it => it.Type == MapperExpressionType.oneToOne)
                .Reverse()
                .Where(it => (it.FillExpression as LambdaExpression).Body.ToString() == exp.Expression.ToString()).FirstOrDefault();
            ThrowTrue(mapper == null);
            return mapper;
        }

        public string GetMemberName(MemberExpression memberExpression)
        {
            return "";
        }

        private void  ExtMapper(MapperExpressionInfo fillInfo, MapperExpressionInfo mappingFild1Info, MapperExpressionInfo mappingFild1Info2, MapperExpressionInfo selectInfo)
        {
            var tableName = sqlBuilder.GetTranslationTableName(fillInfo.EntityInfo.DbTableName);
            var whereLeft = sqlBuilder.GetTranslationColumnName(mappingFild1Info2.FieldName);
            var whereRight = sqlBuilder.GetTranslationColumnName(mappingFild1Info.FieldString);
            this.sql = this.context.Queryable<object>()
                                                        .AS(tableName)
                                                        .Where(string.Format(" {0}={1} ", whereLeft, whereRight))
                                                        .Select(sqlBuilder.GetTranslationColumnName(selectInfo.FieldName)).ToSql().Key;
        }

        public MapperSql GetSql()
        {
            return new MapperSql() { Sql = " (" + this.sql + ") " };
        }

        private MapperExpression GetMapperMany(MemberExpression exp)
        {
            var mapper = mappers.Where(it => it.Type == MapperExpressionType.oneToN)
                .Reverse()
                .Where(it => (it.FillExpression as LambdaExpression).Body.ToString() == exp.ToString()).FirstOrDefault();
            ThrowTrue(mapper == null);
            return mapper;
        }
        private MapperExpressionInfo GetFillInfoMany(Expression childExpression, MapperExpression mapper)
        {
            this.querybuiler = mapper.QueryBuilder;
            this.context = mapper.Context;
            this.sqlBuilder = mapper.SqlBuilder;
            if (this.querybuiler.TableShortName.IsNullOrEmpty())
            {
                this.querybuiler.TableShortName = (childExpression as MemberExpression).Expression.ToString();
            }
            var type = (childExpression as MemberExpression).Type.GetGenericArguments()[0];
            this.context.InitMappingInfo(type);
            return new MapperExpressionInfo()
            {
                EntityInfo = this.context.EntityMaintenance.GetEntityInfo(type)
            };
        }
        private MapperExpressionInfo GetMappingFild1ManyInfo(Expression childExpression, MapperExpression mapper)
        {
            var exp = mapper.MappingField1Expression;
            var field = (exp as LambdaExpression).Body;
            if (field is UnaryExpression)
            {
                field = (field as UnaryExpression).Operand;
            }
            var type = ((field as MemberExpression).Expression).Type;
            this.context.InitMappingInfo(type);
            var name = (field as MemberExpression).Member.Name;
            var entity = this.context.EntityMaintenance.GetEntityInfo(type);
            var fieldName = entity.Columns.First(it => it.PropertyName == name).DbColumnName;
            //var array = (field as MemberExpression).ToString().Split('.').ToList();
            //array[array.Count() - 1] = fieldName;
            //var filedString = string.Join(".", array);
            return new MapperExpressionInfo()
            {
                Type = type,
                FieldName = fieldName,
                FieldString = fieldName,
                EntityInfo = entity
            };
        }

        void Error01()
        {
            Check.Exception(mappers == null, ErrorMessage.GetThrowMessage(expression.ToString() + "no support", "当前表达式" + expression.ToString() + "必须在Mapper之后使用"));
        }
        void ThrowTrue(bool isError)
        {
            Check.Exception(isError, ErrorMessage.GetThrowMessage(expression.ToString() + "no support", "不支持表达式" + expression.ToString()+ " 1.检查当前表达式中的别名是否与Mapper中的一致 2.目前只支持 1对1 Mapper下的 Where "));
        }
    }

    public class MapperSql
    {
        public string Sql { get; set; }
    }

    public class MapperExpressionInfo
    {
        public Type Type { get; set; }
        public EntityInfo EntityInfo { get; set; }
        public string FieldName { get;  set; }
        public string FieldString { get;  set; }
    }
}