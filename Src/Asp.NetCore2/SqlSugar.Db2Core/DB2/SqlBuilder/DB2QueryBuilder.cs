using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar.DB2
{
    public partial class DB2QueryBuilder : QueryBuilder
    {
        /// <summary>
        /// 静态常量正则表达式
        /// </summary>
        private const string constRegex = "@.*const.*";

        #region Sql Template
        public override string PageTempalte
        {
            get
            {
                /*
                 SELECT * FROM TABLE WHERE CONDITION ORDER BY ID DESC LIMIT 10 offset 0
                 */
                var template = "SELECT {0} FROM {1} {2} {3} {4} LIMIT {6} offset {5}";
                return template;
            }
        }
        public override string DefaultOrderByTemplate
        {
            get
            {
                return "ORDER BY NOW() ";
            }
        }

        #endregion

        #region Common Methods
        public override string GetTableNameString
        {
            get
            {
                if (this.TableShortName != null)
                {
                    this.TableShortName = Builder.GetTranslationColumnName(this.TableShortName);
                }
                return base.GetTableNameString;
            }
        }

        //public override string GetWhereValueString
        //{
        //    get
        //    {
        //        return base.GetWhereValueString.Replace("\"", "".ToUpper()); ;
        //    }
        //}

        //public override string GetJoinValueString
        //{
        //    get
        //    {
        //        if (this.JoinQueryInfos.IsNullOrEmpty()) return null;
        //        else
        //        {
        //            return string.Join(UtilConstants.Space, this.JoinQueryInfos.Select(it => this.ToJoinString(it).Replace("\"", "").ToUpper()));
        //        }
        //    }
        //}

        public override bool IsComplexModel(string sql)
        {
            return Regex.IsMatch(sql, @"AS ""\w+\.\w+""") || Regex.IsMatch(sql, @"AS ""\w+\.\w+\.\w+""");
        }
        public override string ToSqlString()
        {
            base.AppendFilter();
            string oldOrderValue = this.OrderByValue;
            string result = null;
            sql = new StringBuilder();
            sql.AppendFormat(SqlTemplate, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString);
            if (IsCount) { return sql.ToString(); }
            if (Skip != null && Take == null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, (Skip != null || Take != null) ? null : GetOrderByString, Skip.ObjToInt(), long.MaxValue);
            }
            else if (Skip == null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, 0, Take.ObjToInt());
            }
            else if (Skip != null && Take != null)
            {
                if (this.OrderByValue == "ORDER BY ") this.OrderByValue += GetSelectValue.Split(',')[0];
                result = string.Format(PageTempalte, GetSelectValue, GetTableNameString, GetWhereValueString, GetGroupByString + HavingInfos, GetOrderByString, Skip.ObjToInt() > 0 ? Skip.ObjToInt() : 0, Take);
            }
            else
            {
                result = sql.ToString();
            }
            this.OrderByValue = oldOrderValue;
            result = GetSqlQuerySql(result);
            if (result.IndexOf("-- No table") > 0)
            {
                return "-- No table";
            }
            if (TranLock != null)
            {
                result = result + TranLock;
            }
            return result;
        }

        #endregion

        #region Get SQL Partial
        public override string GetSelectValue
        {
            get
            {
                string result = string.Empty;
                if (this.SelectValue == null || this.SelectValue is string)
                {
                    result = GetSelectValueByString();
                }
                else
                {
                    result = GetSelectValueByExpression();
                }
                if (this.SelectType == ResolveExpressType.SelectMultiple)
                {
                    this.SelectCacheKey = this.SelectCacheKey + string.Join("-", this.JoinQueryInfos.Select(it => it.TableName));
                }
                if (IsDistinct)
                {
                    result = "distinct " + result;
                }
                if (this.SubToListParameters != null && this.SubToListParameters.Any())
                {
                    result = SubToListMethod(result);
                }
                result = ConstanParameterized(result);
                return result;
                //return result.Replace("\"", "").ToUpper();
            }
        }

        /// <summary>
        /// 常量参数化
        /// </summary>
        /// <returns></returns>
        private string ConstanParameterized(string selectSql)
        {
            var selectParams = selectSql.Split(",").Select(p =>
            {
                if (!Regex.IsMatch(p, constRegex)) return p;
                var parameterItems = this.Parameters.Where(t => p.Contains(t.ParameterName)).ToList();
                if (!parameterItems.Any()) return p;

                var paramSql = p;
                parameterItems.ForEach(parameter =>
                {
                    var dbType = DB2DbBind.MappingDbTypesConst.FirstOrDefault(t => parameter.DbType == t.Value);
                    if (default(KeyValuePair<string, System.Data.DbType>).Equals(dbType)) return;
                    var parameterized = string.Empty;
                    if (dbType.Value == System.Data.DbType.String)
                    {
                        var size = string.IsNullOrEmpty(parameter.Value?.ToString()) ? 1 : System.Text.Encoding.Default.GetBytes(parameter.Value?.ToString()).Length;
                        parameterized = $"{dbType.Key.ToUpper()}({size})";
                    }
                    else
                    {
                        parameterized = $"{dbType.Key.ToUpper()}";
                    }
                    paramSql = paramSql.Replace(parameter.ParameterName, $" CAST({parameter.ParameterName} AS {parameterized}) ");
                });
                return paramSql;
            });
            return string.Join(",", selectParams);
        }

        #endregion
    }
}
