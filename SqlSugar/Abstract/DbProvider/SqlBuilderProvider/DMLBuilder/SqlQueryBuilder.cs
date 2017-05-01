using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqlQueryBuilder : IDMLBuilder
    {
        public SqlSugarClient Context { get; set; }

        private string _Fields { get; set; }
        public string Fields
        {
            get
            {
                if (this._Fields.IsNullOrEmpty())
                {
                    this._Fields = Regex.Match(this.sql.ToString(), @"select(.*?)from", RegexOptions.IgnoreCase).Groups[1].Value;
                    if (this._Fields.IsNullOrEmpty())
                    {
                        this._Fields = "*";
                    }
                }
                return this._Fields;
            }
            set
            {
                _Fields = value;
            }
        }

        private StringBuilder _Sql;
        public StringBuilder sql
        {
            get
            {
                _Sql = PubMethod.IsNullReturnNew(_Sql);
                return _Sql;
            }
            set
            {
                _Sql = value;
            }
        }

        public string SqlTemplate
        {
            get
            {
                return null;
            }
        }

        private List<SugarParameter> _Parameters;
        public List<SugarParameter> Parameters
        {
            get
            {
                _Parameters = PubMethod.IsNullReturnNew(_Parameters);
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        public string ToSqlString()
        {
            return sql.ToString();
        }
        public void Clear()
        {
            this.sql = null;
        }
    }
}
