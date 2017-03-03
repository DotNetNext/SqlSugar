using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
namespace SqlSugar
{
    public abstract class LambadaQueryBuilder : IDMLBuilder
    {
        public LambadaQueryBuilder()
        {

        }
        private List<SqlParameter> _QueryPars;
        private List<JoinQueryInfo> _JoinQueryInfos;
        private List<string> _WhereInfos;
        private string _TableNameString;

        public StringBuilder Sql { get; set; }
        public SqlSugarClient Conext { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }
        public string OrderByValue { get; set; }
        public string SelectValue { get; set; }
        public Type EntityType { get; set; }
        public string EntityName { get { return this.EntityType.Name; } }
        public string TableWithString { get; set; }
        public string GroupByValue { get; set; }
        public int WhereIndex { get; set; }
        public int JoinIndex { get; set; }
        public ResolveExpressType ResolveType { get; set; }

        public virtual string SqlTemplate
        {
            get
            {
                return "SELECT {0} FROM {1} {2}";
            }
        }
        public virtual string JoinTemplate
        {
            get
            {
                return " {0} JOIN {1} {2} ON {3} ";
            }
        }
        public virtual string GetTableNameString
        {
            get
            {
                return this.Conext.SqlBuilder.GetTranslationTableName(EntityType.Name);
            }
        }
        public virtual string GetSelectValueString
        {
            get
            {
                if (this.SelectValue.IsNullOrEmpty())
                {
                    string pre = null;
                    if (this.JoinQueryInfos.IsValuable() && this.JoinQueryInfos.Any(it => it.PreShortName.IsValuable())) {
                        pre = this.Conext.SqlBuilder.GetTranslationColumnName(this.JoinQueryInfos.Single(it => it.PreShortName.IsValuable()).PreShortName)+".";
                    }
                    return string.Join(",", this.Conext.Database.DbMaintenance.GetColumnInfosByTableName(this.EntityName).Select(it => pre+this.Conext.SqlBuilder.GetTranslationColumnName(it.ColumnName)));
                }
                else return this.SelectValue;
            }
        }
        public virtual string GetWhereValueString
        {
            get
            {
                if (this.WhereInfos == null) return null;
                else
                {
                    return string.Join(" ", this.WhereInfos);
                }
            }
        }
        public virtual string GetJoinValueString
        {
            get
            {
                if (this.JoinQueryInfos.IsNullOrEmpty()) return null;
                else {
                    return string.Join(" ", this.JoinQueryInfos.Select(it => this.ToJoinString(it)));
                }
            }
        }

        public virtual string ToSqlString()
        {
            Sql = new StringBuilder();
            var tableString = GetTableNameString;
            if (this.JoinQueryInfos.IsValuable()) {
                tableString = tableString + " " + GetJoinValueString;
            }
            Sql.AppendFormat(SqlTemplate, GetSelectValueString, tableString , GetWhereValueString);
            return Sql.ToString();
        }
        public virtual string ToJoinString(JoinQueryInfo joinInfo)
        {
            return string.Format(
                this.JoinTemplate,
                joinInfo.JoinIndex == 1 ? (joinInfo.PreShortName + " " + joinInfo.JoinType.ToString()+" ") : (joinInfo.JoinType.ToString() + " JOIN "),
                joinInfo.TableName,
                joinInfo.ShortName + " " + TableWithString,
                joinInfo.JoinWhere);
        }
        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = PubMethod.IsNullReturnNew(_WhereInfos);
                return _WhereInfos;
            }
            set { _WhereInfos = value; }
        }
     
        public virtual List<SqlParameter> QueryPars
        {
            get
            {
                _QueryPars = PubMethod.IsNullReturnNew(_QueryPars);
                return _QueryPars;
            }
            set { _QueryPars = value; }
        }
        public virtual List<JoinQueryInfo> JoinQueryInfos
        {
            get
            {
                _JoinQueryInfos = PubMethod.IsNullReturnNew(_JoinQueryInfos);
                return _JoinQueryInfos;
            }
            set { _JoinQueryInfos = value; }
        }
        public virtual void Clear()
        {
            this.Skip = 0;
            this.Take = 0;
            this.Sql = null;
            this.WhereIndex = 0;
            this.QueryPars = null;
            this.GroupByValue = null;
            this._TableNameString = null;
            this.WhereInfos = null;
            this.JoinQueryInfos = null;
        }
    }
}
