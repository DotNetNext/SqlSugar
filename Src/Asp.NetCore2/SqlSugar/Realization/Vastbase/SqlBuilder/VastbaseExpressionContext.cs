using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class VastbaseV5ExpressionContext : PostgreSQLExpressionContext
    {
        // 构造默认 PG 方言（base 构造器设 PostgreSQLMethod）；兼容模式仅在 SugarContext
        // 赋值时显式切换。Insert/UpdateServerTime 由 VastbaseInsert/UpdateBuilder 显式生成
        // NOW()/GETDATE()，不依赖构造默认。SugarContext 未赋值时 VastbaseV5Mode.From（模式
        // 唯一来源）返 null，保持 PG 默认（合法构造期窗口）；模式敏感访问经 Require 抛错。

        // 纯 IDbMethods lambda（SqlFunc.GetRandom 等无成员访问的表达式）经
        // MethodCallExpressionResolve_Helper 直接访问 Context.DbMehtods，从不触发惰性 Mode
        // 属性；若等到首次成员访问才解析模式，PG 模式会泄漏 T-SQL 函数（NEWID() →
        // function newid() does not exist）。SugarContext 就绪（每次表达式解析前赋值）即
        // 立即解析模式并据此初始化 DbMehtods（PG 模式得 RANDOM()，Babelfish 得 NEWID()）。
        public override ExpressionOutParameter SugarContext
        {
            get { return base.SugarContext; }
            set
            {
                base.SugarContext = value;
                EnsureModeInitialized();
            }
        }

        private bool _modeInitialized;

        private void EnsureModeInitialized()
        {
            if (_modeInitialized) return;
            var mode = VastbaseV5Mode.From(this.SugarContext?.Context);
            if (mode != null)
            {
                base.DbMehtods = mode.IsBabelfish
                    ? (IDbMethods)new VastbaseV5SqlServerMethod()
                    : new PostgreSQLMethod();
                // PG 模式尊重用户 MoreSettings.PgSqlIsAutoToLower。无脑置 true 会覆盖
                // PgSqlIsAutoToLower=false 的设置（QueryBuilder 在 SugarContext 赋值前已按
                // MoreSettings 写入该值）→ 查询列名被小写化，而 CodeFirst（PgSqlIsAutoToLowerCodeFirst
                // =false）建的列保持精确大小写 → 42703: column "roleid" does not exist。
                // Babelfish 模式保持原语义（false，T-SQL 大小写不敏感按原样）。
                if (mode.IsBabelfish)
                {
                    base.PgSqlIsAutoToLower = false;
                }
                else
                {
                    var moreSettings = this.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings;
                    base.PgSqlIsAutoToLower = moreSettings != null ? moreSettings.PgSqlIsAutoToLower : true;
                }
                _modeInitialized = true;
            }
        }

        private VastbaseV5Mode Mode
        {
            get
            {
                EnsureModeInitialized();
                // 模式敏感访问：SugarContext 未赋值时 Require 直接抛「模式未解析」，
                // 不静默回退 PG 方言。
                return VastbaseV5Mode.Require(this.SugarContext?.Context);
            }
        }

        public override string GetDbColumnName(string entityName, string propertyName)
        {
            var mode = Mode;
            if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                base.PgSqlIsAutoToLower = false;
            return base.GetDbColumnName(entityName, propertyName);
        }

        public override string SqlTranslationLeft
        {
            get
            {
                var mode = Mode;
                if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                    base.PgSqlIsAutoToLower = false;
                return mode.SqlTranslationLeft;
            }
        }

        public override string SqlTranslationRight
        {
            get
            {
                var mode = Mode;
                if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                    base.PgSqlIsAutoToLower = false;
                return mode.SqlTranslationRight;
            }
        }

        public override string GetTranslationText(string name)
        {
            var mode = Mode;
            if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                base.PgSqlIsAutoToLower = false;
            if (mode.IsBabelfish)
                return SqlTranslationLeft + name + SqlTranslationRight;
            return base.GetTranslationText(name);
        }

        public override string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            var mode = Mode;
            if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                base.PgSqlIsAutoToLower = false;
            if (mode.IsBabelfish && entityName != null)
            {
                // Babelfish：显式方括号字面量表名（如 SugarTable("[1.1.dsfadsaf]")，自带括号+点号）
                // 原样返回，避免 base（PostgreSQL 系）isMapping 分支按点拆分生成
                // [[1].[1].[dsfadsaf]] 嵌套括号 → Babelfish 语法错误（33557097）。
                // 须在 mapping 后判断（子查询 SubFromTable 传入的是实体名，方括号在 DbTableName 里）。
                var mappedName = entityName;
                if (isMapping && MappingTables.HasValue())
                {
                    var mappingInfo = MappingTables
                        .FirstOrDefault(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null)
                        mappedName = mappingInfo.DbTableName;
                }
                if (mappedName.StartsWith("[") && mappedName.EndsWith("]"))
                    return mappedName;
            }
            var result = base.GetTranslationTableName(entityName, isMapping);
            if (mode.IsBabelfish && result != null)
            {
                var left = mode.SqlTranslationLeft;
                var right = mode.SqlTranslationRight;
                if (left != null && right != null && result.StartsWith(left) && result.EndsWith(right))
                    result = left + result.Substring(left.Length, result.Length - left.Length - right.Length).TrimEnd() + right;
            }
            return result;
        }

        public override string GetTranslationColumnName(string columnName)
        {
            var mode = Mode;
            if (!mode.AutoToLower && base.PgSqlIsAutoToLower)
                base.PgSqlIsAutoToLower = false;
            return base.GetTranslationColumnName(columnName);
        }
    }

    internal class VastbaseV5SqlServerMethod : SqlServerMethod, IDbMethods
    {
        // 跨库表名三段式：Babelfish 的"数据库"实际是库内 schema 组，两段式 {db}.{t} 会把
        // db 名当 schema 解析（relation "db.t" does not exist）；三段式 {db}.dbo.{t} 可正常
        // 跨库。SqlServerMethod 基类即此实现，此处显式覆写防基类行为漂移。
        public override string GetTableWithDataBase(string dataBaseName, string tableName)
        {
            return $"{dataBaseName}.dbo.{tableName}";
        }

        public override string DateValue(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            if (parameter2.MemberValue.ObjToString() == DateType.Weekday.ToString())
                return string.Format(" (DATEPART(weekday,{0})-1) ", parameter.MemberName);
            return base.DateValue(model);
        }

        public override string DateDiff(MethodCallExpressionModel model)
        {
            var parameter = model.Args[0];
            var parameter2 = model.Args[1];
            var parameter3 = model.Args[2];
            return string.Format(" DATEDIFF({0},{1},{2}) ",
                parameter.MemberValue?.ToString().ToSqlFilter(),
                parameter2.MemberName,
                parameter3.MemberName);
        }

        public override string Pack(string sql)
        {
            const string limitSuffix = " limit 1";
            if (sql.EndsWith(limitSuffix))
            {
                sql = sql.Substring(0, sql.Length - limitSuffix.Length);
                sql = InsertTopAfterDistinctInPack(sql, 1);
            }
            const string limit0Suffix = " limit 0,1";
            if (sql.EndsWith(limit0Suffix))
            {
                sql = sql.Substring(0, sql.Length - limit0Suffix.Length);
                sql = InsertTopAfterDistinctInPack(sql, 1);
            }
            return base.Pack(sql);
        }

        private static string InsertTopAfterDistinctInPack(string sql, int topCount)
        {
            var afterSelect = sql.Substring("SELECT".Length);
            if (afterSelect.TrimStart().StartsWith("distinct ", System.StringComparison.OrdinalIgnoreCase))
            {
                var trimStart = afterSelect.TrimStart();
                var distinctLen = trimStart.IndexOf(' ') + 1;
                var afterDistinct = trimStart.Substring(distinctLen);
                return "SELECT distinct TOP " + topCount + " " + afterDistinct;
            }
            return "SELECT TOP " + topCount + afterSelect;
        }

        // 重新实现 IDbMethods 使接口派发落到本类 ListAny（基类对应成员 private，方法体须
        // 整段自带）。N' 前缀逻辑仅在 Babelfish 模式装配本类后生效，其内 VastbaseV5 判断
        // 恒真；PG 模式走 PostgreSQLMethod，无 N' 前缀。
        public new string ListAny(MethodCallExpressionModel model)
        {
            if (IsArrayAnyParameter(model))
            {
                return ListArrayAny(model);
            }
            StringBuilder sb = new StringBuilder();
            if (model.Args[0].MemberValue != null && (model.Args[0].MemberValue as IList).Count > 0)
            {
                sb.Append(" ( ");
                var listPar = model.Args[1].MemberValue as ListAnyParameter;
                foreach (var item in (model.Args[0].MemberValue as IList))
                {
                    var sql = listPar.Sql;
                    if (sb.Length > 3)
                    {
                        sb.Append("OR");
                    }
                    foreach (var columnInfo in listPar.Columns)
                    {
                        var replace = listPar.ConvetColumnFunc($"{listPar.Name}.{columnInfo.DbColumnName}");
                        if (sql.Contains(replace))
                        {
                            var value = columnInfo.PropertyInfo.GetValue(item);
                            string newValue = null;
                            if (value != null)
                            {
                                if (UtilMethods.IsNumber(columnInfo.UnderType.Name))
                                {
                                    newValue = value.ToString();
                                }
                                else if (value is Enum)
                                {
                                    newValue = Convert.ToInt64(value) + "";
                                }
                                else if (columnInfo.UnderType == SqlSugar.UtilConstants.GuidType)
                                {
                                    newValue = ToGuid(new MethodCallExpressionModel()
                                    {
                                        Args = new List<MethodCallExpressionArgs>()
                                       {
                                            new MethodCallExpressionArgs(){
                                              MemberValue=value.ToSqlValue(),
                                              MemberName=value.ToSqlValue()
                                            }
                                       }
                                    });
                                }
                                else if (columnInfo.UnderType == SqlSugar.UtilConstants.DateType)
                                {
                                    newValue = ToDate(new MethodCallExpressionModel()
                                    {
                                        Args = new List<MethodCallExpressionArgs>()
                                       {
                                            new MethodCallExpressionArgs(){
                                              MemberValue=UtilMethods.GetConvertValue( value).ToSqlValue(),
                                              MemberName=UtilMethods.GetConvertValue( value).ToSqlValue()
                                            }
                                       }
                                    });
                                }
                                else
                                {
                                    newValue = value.ToSqlValue();
                                }
                            }
                            if (columnInfo.UnderType == UtilConstants.StringType
                                && (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.DbType == DbType.SqlServer
                                    || (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.DbType == DbType.VastbaseV5
                                        && VastbaseV5Helper.IsBabelfish(model.Conext?.SugarContext?.Context))))
                            {
                                if (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DisableNvarchar != true)
                                {
                                    if (columnInfo.SqlParameterDbType is System.Data.DbType type && type == System.Data.DbType.AnsiString)
                                    {

                                    }
                                    else if (newValue == null)
                                    {
                                        newValue = "null";
                                    }
                                    else
                                    {
                                        newValue = "N" + newValue;
                                    }
                                }
                            }
                            if (newValue == null) newValue = "null";
                            sql = sql.Replace(replace, newValue);
                            sql = sql.Replace(" = null ", " is null ");
                        }
                    }
                    sb.Append(sql);
                }
                sb.Append(" ) ");
            }
            var result = sb.ToString();
            if (result.IsNullOrEmpty())
            {
                return " 1=2 ";
            }
            else
            {
                return result;
            }
        }

        private static bool IsArrayAnyParameter(MethodCallExpressionModel model)
        {
            var memberValue = model?.Args?.FirstOrDefault()?.MemberValue;
            return UtilMethods.IsValueTypeArray(memberValue);
        }

        private string ListArrayAny(MethodCallExpressionModel model)
        {
            StringBuilder sb = new StringBuilder();
            if (model.Args[0].MemberValue != null && (model.Args[0].MemberValue as IList).Count > 0)
            {
                sb.Append(" ( ");
                var listPar = model.Args[1].MemberValue as ListAnyParameter;
                foreach (var item in (model.Args[0].MemberValue as IList))
                {
                    var sql = listPar.Sql;
                    if (sb.Length > 3)
                    {
                        sb.Append("OR");
                    }
                    foreach (var columnInfo in listPar.Columns)
                    {
                        var value = item;
                        var newValue = "null";
                        if (value != null)
                        {
                            if (columnInfo.DbTableName != "String" && UtilMethods.IsNumber(columnInfo.UnderType.Name))
                            {
                                newValue = value.ToString();
                            }
                            else if (columnInfo.UnderType == SqlSugar.UtilConstants.GuidType)
                            {
                                newValue = ToGuid(new MethodCallExpressionModel()
                                {
                                    Args = new List<MethodCallExpressionArgs>()
                                       {
                                            new MethodCallExpressionArgs(){
                                              MemberValue=value.ToSqlValue(),
                                              MemberName=value.ToSqlValue()
                                            }
                                       }
                                });
                            }
                            else if (columnInfo.UnderType == SqlSugar.UtilConstants.DateType)
                            {
                                newValue = ToDate(new MethodCallExpressionModel()
                                {
                                    Args = new List<MethodCallExpressionArgs>()
                                       {
                                            new MethodCallExpressionArgs(){
                                              MemberValue=UtilMethods.GetConvertValue( value).ToSqlValue(),
                                              MemberName=UtilMethods.GetConvertValue( value).ToSqlValue()
                                            }
                                       }
                                });
                            }
                            else
                            {
                                newValue = value.ToSqlValue();
                                if (columnInfo.EntityName == "String"
                                    && (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.DbType == DbType.SqlServer
                                        || (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.DbType == DbType.VastbaseV5
                                            && VastbaseV5Helper.IsBabelfish(model.Conext?.SugarContext?.Context))))
                                {
                                    if (model.Conext?.SugarContext?.Context?.CurrentConnectionConfig?.MoreSettings?.DisableNvarchar != true)
                                    {
                                        if (model.DataObject is EntityColumnInfo dc && dc.SqlParameterDbType is System.Data.DbType type && type == System.Data.DbType.AnsiString)
                                        {

                                        }
                                        else
                                        {
                                            newValue = "N" + newValue;
                                        }
                                    }
                                }
                            }
                        }
                        if (!sql.Contains(ParameterKeyWord))
                        {
                            sql = sql.Replace(" =)", $" = {newValue})");
                            if (!sql.Contains(newValue))
                            {
                                sql = sql.Replace(" )", $" = {newValue})");
                            }
                        }
                        else
                        {
                            Regex reg = new Regex(ParameterKeyWord + @"MethodConst\d+");
                            sql = reg.Replace(sql, it =>
                            {
                                return " " + newValue + " ";
                            });
                        }

                    }
                    sb.Append(sql);
                }
                sb.Append(" ) ");
            }
            var result = sb.ToString();
            result = result.Replace(" = null)", " is null)");
            if (result.IsNullOrEmpty())
            {
                return " 1=2 ";
            }
            else
            {
                return result;
            }
        }
    }
}