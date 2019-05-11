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

        #region  Fields
        private string _Fields;
        private StringBuilder _Sql;
        private List<SugarParameter> _Parameters;
        #endregion

        #region Properties
        public SqlSugarProvider Context { get; set; }
        public string Fields
        {
            get
            {
                if (this._Fields.IsNullOrEmpty())
                {
                    this._Fields = Regex.Match(this.sql.ObjToString().Replace("\n", string.Empty).Replace("\r", string.Empty).Trim(), @"select(.*?)from", RegexOptions.IgnoreCase).Groups[1].Value;
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
        public StringBuilder sql
        {
            get
            {
                _Sql = UtilMethods.IsNullReturnNew(_Sql);
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
        public List<SugarParameter> Parameters
        {
            get
            {
                _Parameters = UtilMethods.IsNullReturnNew(_Parameters);
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }
        #endregion

        #region Methods
        public string ToSqlString()
        {
            return sql.ToString();
        }
        public void Clear()
        {
            this.sql = null;
        } 
        #endregion
    }
}
