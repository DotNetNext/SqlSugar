using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    public class DeleteBuilder : IDMLBuilder
    {
        private List<string> _WhereInfos;

        public SqlSugarClient Context { get; set; }
        public ILambdaExpressions LambdaExpressions { get; set; }
        public List<SugarParameter> Parameters { get; set; }
        public StringBuilder sql { get; set; }
        public ISqlBuilder Builder { get; set; }
        public string TableName { get; set; }
        public string TableWithString { get; set; }
        public virtual List<string> WhereInfos
        {
            get
            {
                _WhereInfos = PubMethod.IsNullReturnNew(_WhereInfos);
                return _WhereInfos;
            }
            set { _WhereInfos = value; }
        }
        public string SqlTemplate
        {
            get
            {
                return " DELETE FROM {0}{1}";
            }
        }
        public virtual string GetTableNameString
        {
            get
            {
                var result = Builder.GetTranslationTableName(TableName);
                result += PubConst.Space;
                if (this.TableWithString.IsValuable())
                {
                    result += TableWithString + PubConst.Space;
                }
                return result;
            }
        }
        public virtual string GetWhereString
        {
            get
            {
                if (_WhereInfos == null || _WhereInfos.Count == 0) return null;
                string whereString = null;
                int i = 0;
                foreach (var item in _WhereInfos)
                {
                    var isFirst = i == 0;
                    whereString += isFirst ? "WHERE " : "AND ";
                    whereString +=(item + PubConst.Space);
                    ++i;
                }
                return whereString;
            }
        }

        public void Clear()
        {
        }

        public string ToSqlString()
        {
            return string.Format(SqlTemplate, GetTableNameString, GetWhereString);
        }
        public virtual ExpressionResult GetExpressionValue(Expression expression, ResolveExpressType resolveType)
        {
            ILambdaExpressions resolveExpress = this.LambdaExpressions;
            this.LambdaExpressions.Clear();
            resolveExpress.MappingColumns = Context.MappingColumns;
            resolveExpress.MappingTables = Context.MappingTables;
            resolveExpress.IgnoreComumnList = Context.IgnoreColumns;
            resolveExpress.Resolve(expression, resolveType);
            this.Parameters = new List<SugarParameter>();
            this.Parameters.AddRange(resolveExpress.Parameters);
            var reval = resolveExpress.Result;
            return reval;
        }
    }
}
