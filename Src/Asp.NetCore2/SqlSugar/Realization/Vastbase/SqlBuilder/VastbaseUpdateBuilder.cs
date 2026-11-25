using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class VastbaseV5UpdateBuilder : PostgreSQLUpdateBuilder
    {
        private VastbaseV5Mode Mode => VastbaseV5Mode.Require(this.Context);

        public override string ToSqlString()
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                FixEnumStringParameters();
            }
            if (SetValues.IsValuable())
            {
                for (int i = 0; i < SetValues.Count; i++)
                {
                    var sv = SetValues[i];
                    foreach (var col in DbColumnInfoList)
                    {
                        var translated = Builder.GetTranslationColumnName(col.DbColumnName);
                        if (sv.Key.Equals(translated, StringComparison.OrdinalIgnoreCase) && sv.Key != translated)
                        {
                            SetValues[i] = new KeyValuePair<string, string>(translated, sv.Value);
                            break;
                        }
                    }
                }
            }
            var sql = base.ToSqlString();
            if (mode != null && mode.IsBabelfish)
            {
                // 事后重写走同一 pipeline 入口（各步自检前置条件：UPDATE SQL
                // 通常只触发 NormalizeCastTypes；SET 表达式若含 DATE_PART/EXISTS 等特征，
                // 同样会被转换或按漂移规则抛错）。
                sql = VastbaseV5BabelfishRewritePipeline.Apply(sql, new VastbaseV5BabelfishRewritePipeline.RewriteContext());
            }
            return sql;
        }

        // UpdateServerTime 同 InsertServerTime（见 VastbaseV5InsertBuilder）：base GetDate()
        // 按构造默认 PG 方言生成 NOW()，Babelfish 兼容模式(TDS) 须显式 GETDATE()。
        public override string GetDbColumn(DbColumnInfo columnInfo, object name)
        {
            if (columnInfo.UpdateServerTime)
                return Mode.IsBabelfish ? "GETDATE()" : "NOW()";
            return base.GetDbColumn(columnInfo, name);
        }

        // Babelfish 下批量 UPDATE VALUES 的 numeric CAST 拼显式精度（Length/DecimalDigits），
        // 避免 T-SQL decimal(18,0) 默认舍入小数位；PG 原生模式不变。
        protected override string GetCastDbType(DbColumnInfo columnInfo, string dbType)
        {
            var mode = Mode;
            if (mode != null)
            {
                return mode.GetCastDbType(columnInfo, dbType);
            }
            return base.GetCastDbType(columnInfo, dbType);
        }

        protected override string GetJoinUpdate(string columnsString, ref string whereString)
        {
            var mode = Mode;
            if (mode != null && mode.IsBabelfish)
            {
                var tableName = Builder.GetTranslationColumnName(this.TableName);
                this.TableName = Builder.GetTranslationColumnName(this.ShortName);
                var joinString = string.Format(" FROM {0} {1} ", tableName, Builder.GetTranslationColumnName(this.ShortName));
                foreach (var item in this.JoinInfos)
                {
                    joinString += string.Format("\r\n JOIN {0}  {1} ON {2} ",
                        Builder.GetTranslationColumnName(item.TableName),
                        Builder.GetTranslationColumnName(item.ShortName),
                        item.JoinWhere);
                }
                whereString = joinString + "\r\n" + whereString;
                return string.Format(SqlTemplate, GetTableNameString, columnsString, whereString);
            }
            return base.GetJoinUpdate(columnsString, ref whereString);
        }

        private void FixEnumStringParameters()
        {
            if (this.EntityInfo == null || this.SetValues == null || this.Parameters == null) return;

            var enumStringCols = this.EntityInfo.Columns
                .Where(c => c.PropertyInfo != null
                    && UtilMethods.GetUnderType(c.PropertyInfo).IsEnum()
                    && c.DataType != null
                    && IsStringDataType(c.DataType))
                .ToList();
            if (!enumStringCols.Any()) return;

            foreach (var column in enumStringCols)
            {
                var colKey = Builder.GetTranslationColumnName(column.DbColumnName);
                var propKey = Builder.GetTranslationColumnName(column.PropertyName);
                KeyValuePair<string, string>? matchingSV = null;
                foreach (var sv in this.SetValues)
                {
                    if (sv.Key == colKey || sv.Key == propKey
                        || Builder.GetNoTranslationColumnName(sv.Key).Equals(column.DbColumnName, StringComparison.OrdinalIgnoreCase)
                        || Builder.GetNoTranslationColumnName(sv.Key).Equals(column.PropertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        matchingSV = sv;
                        break;
                    }
                }
                if (matchingSV == null) continue;

                var svValue = matchingSV.Value.Value;
                // Extract parameter names from the SetValue SQL fragment (e.g., "[type]=@Const0" → "@Const0")
                var paramMatches = Regex.Matches(svValue, @"@\w+");
                foreach (System.Text.RegularExpressions.Match pm in paramMatches)
                {
                    var paramName = pm.Value;
                    var param = this.Parameters.FirstOrDefault(p => p.ParameterName == paramName);
                    if (param != null && param.Value is long longVal)
                    {
                        var enumType = UtilMethods.GetUnderType(column.PropertyInfo);
                        try
                        {
                            var enumObj = System.Enum.ToObject(enumType, longVal);
                            param.Value = enumObj.ToString();
                            param.DbType = System.Data.DbType.String;
                        }
                        catch { }
                    }
                }
            }

            // Also fix entity-based update parameters (from DbColumnInfoList, not SetValues)
            foreach (var column in enumStringCols)
            {
                var colInfo = this.DbColumnInfoList?.FirstOrDefault(c =>
                    c.PropertyName.Equals(column.PropertyName, StringComparison.OrdinalIgnoreCase)
                    || c.DbColumnName.Equals(column.DbColumnName, StringComparison.OrdinalIgnoreCase));
                if (colInfo == null || colInfo.Value == null) continue;
                if (colInfo.Value is long longVal)
                {
                    var enumType = UtilMethods.GetUnderType(column.PropertyInfo);
                    try
                    {
                        var enumObj = System.Enum.ToObject(enumType, longVal);
                        colInfo.Value = enumObj.ToString();
                    }
                    catch { }
                }
            }
        }

        private static bool IsStringDataType(string dataType)
        {
            if (string.IsNullOrEmpty(dataType)) return false;
            var dt = dataType.ToLowerInvariant();
            return dt.Contains("varchar") || dt.Contains("nvarchar")
                || dt.Contains("char") || dt.Contains("nchar")
                || dt.Contains("text") || dt.Contains("ntext");
        }
    }
}