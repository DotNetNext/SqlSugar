﻿using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqliteBuilder : SqlBuilderProvider
    {
        public override string SqlTranslationLeft { get { return "`"; } }
        public override string SqlTranslationRight { get { return "`"; } }
        public override string SqlDateNow
        {
            get
            {
                return "DATETIME('now') ";
            }
        }
        public override string FullSqlDateNow
        {
            get
            {
                return "select DATETIME('now') ";
            }
        }
        public override string RemoveParentheses(string sql)
        {
            if (sql.StartsWith("(") && sql.EndsWith(")"))
            {
                sql = sql.Substring(1, sql.Length - 2);
            }

            return sql;
        }
    }
}
