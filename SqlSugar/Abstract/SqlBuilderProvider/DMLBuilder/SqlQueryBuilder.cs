using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlSugar
{
    public class SqlQueryBuilder:IDMLBuilder
    {
        private string _Fields { get; set; }
        public string Fields
        {
            get
            {
                if (this._Fields.IsNullOrEmpty())
                {
                    this._Fields = Regex.Match(this.Sql.ToString(), @"select(.*?)from", RegexOptions.IgnoreCase).Groups[1].Value;
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
        public StringBuilder Sql
        {
            get {
                _Sql = PubMethod.IsNullReturnNew(_Sql);
                return _Sql;
            }
            set {
                _Sql = value;
            }
        }

        public SqlSugarClient Conext { get; set; }

        public string SqlTemplate
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ToSqlString()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            this.Sql = null;
        }
    }
}
