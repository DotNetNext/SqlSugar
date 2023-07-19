using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar.OceanBaseForOracle
{
    public class OceanBaseForOracleBuilder : SqlBuilderProvider
    {
        public override string SqlParameterKeyWord
        {
            get
            {
                return ":";
            }
        }
        public override string SqlDateNow
        {
            get
            {
                return "sysdate";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select systimestamp from dual";
            }
        }
        public override string SqlTranslationLeft { get { return "\""; } }
        public override string SqlTranslationRight { get { return "\""; } }
        public override string GetTranslationTableName(string name)
        {
            var result = base.GetTranslationTableName(name);
            if (result.Contains("(") && result.Contains(")"))
                return result;
            else
                return result.ToUpper(IsUppper);
        }
        public override string GetTranslationColumnName(string entityName, string propertyName)
        {
            var result = base.GetTranslationColumnName(entityName, propertyName);
            return result.ToUpper(IsUppper);
        }
        public override string GetTranslationColumnName(string propertyName)
        {
            var result = base.GetTranslationColumnName(propertyName);
            return result.ToUpper(IsUppper);
        }
        public override string RemoveParentheses(string sql)
        {
            if (sql.StartsWith("(") && sql.EndsWith(")"))
            {
                sql = sql.Substring(1, sql.Length - 2);
            }

            return sql;
        }
        #region Helper
        public bool IsUppper
        {
            get
            {
                if (this.Context.CurrentConnectionConfig.MoreSettings == null)
                {
                    return true;
                }
                else
                {
                    return this.Context.CurrentConnectionConfig.MoreSettings.IsAutoToUpper == true;
                }
            }
        }
        #endregion

    }

}
