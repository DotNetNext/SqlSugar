using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
namespace SqlSugar
{
    ///<summary>
    /// ** description：Expression to sql 
    /// ** author：sunkaixuan
    /// ** date：2017/1/14
    /// ** email:610262374@qq.com
    public class ExpressionContext : ExpResolveAccessory
    {
        #region Fields
        private bool _IsSingle = true;
        private IDbMethods _DbMehtods { get; set; }
        #endregion

        #region Properties
        public IDbMethods DbMehtods
        {
            get
            {
                if (_DbMehtods == null)
                {
                    _DbMehtods = new DefaultDbMethod();
                }
                return _DbMehtods;
            }
            set
            {
                _DbMehtods = value;
            }
        }
        public int SubQueryIndex { get; set; }
        public int JoinIndex { get; set; }
        public int Index { get; set; }
        public int ParameterIndex { get; set; }
        public string SingleTableNameSubqueryShortName{ get;  set; }
        public MappingColumnList MappingColumns { get; set; }
        public MappingTableList MappingTables { get; set; }
        public IgnoreColumnList IgnoreComumnList { get; set; }
        public bool PgSqlIsAutoToLower { get; set; }
        public List<SqlFuncExternal> SqlFuncServices { get; set; }
        public bool IsSingle
        {
            get
            {
                return _IsSingle;
            }
            set
            {
                _IsSingle = value;
            }
        }
        public bool IsJoin
        {
            get
            {
                return !IsSingle;
            }
        }
        public List<JoinQueryInfo> JoinQueryInfos { get; set; }
        public ResolveExpressType ResolveType { get; set; }
        public Expression Expression { get; set; }
        public ExpressionResult Result
        {
            get
            {
                if (base._Result == null)
                {
                    this.Result = new ExpressionResult(this.ResolveType);
                }
                return base._Result;
            }
            set
            {
                this._Result = value;
            }
        }
        public List<SugarParameter> Parameters
        {
            get
            {
                if (base._Parameters == null)
                    base._Parameters = new List<SugarParameter>();
                return base._Parameters;
            }
            set
            {
                base._Parameters = value;
            }
        }
        public virtual string SqlParameterKeyWord
        {
            get
            {
                return "@";
            }
        }
        public virtual string SqlTranslationLeft { get { return "["; } }
        public virtual string SqlTranslationRight { get { return "]"; } }
        public virtual Action<Type> InitMappingInfo { get; set; }
        public virtual Action RefreshMapping { get; set; }
        #endregion

        #region Core methods 
        public void Resolve(Expression expression, ResolveExpressType resolveType)
        {
            this.ResolveType = resolveType;
            this.Expression = expression;
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { CurrentExpression = this.Expression, Context = this });
            resolve.Start();
        }
        public void Clear()
        {
            base._Result = null;
            base._Parameters = new List<SugarParameter>();
        }
        public ExpressionContext GetCopyContext()
        {
            ExpressionContext copyContext = (ExpressionContext)Activator.CreateInstance(this.GetType(), true);
            copyContext.Index = this.Index;
            copyContext.InitMappingInfo = this.InitMappingInfo;
            copyContext.RefreshMapping = this.RefreshMapping;
            copyContext.ParameterIndex = this.ParameterIndex;
            copyContext.PgSqlIsAutoToLower = this.PgSqlIsAutoToLower;
            copyContext.IsSingle = this.IsSingle;
            return copyContext;
        }
        public ExpressionContext GetCopyContextWithMapping()
        {
            ExpressionContext copyContext = (ExpressionContext)Activator.CreateInstance(this.GetType(), true);
            copyContext.Index = this.Index;
            copyContext.ParameterIndex = this.ParameterIndex;
            copyContext.MappingColumns = this.MappingColumns;
            copyContext.MappingTables = this.MappingTables;
            copyContext.IgnoreComumnList = this.IgnoreComumnList;
            copyContext.SqlFuncServices = this.SqlFuncServices;
            copyContext.InitMappingInfo = this.InitMappingInfo;
            copyContext.RefreshMapping = this.RefreshMapping;
            copyContext.PgSqlIsAutoToLower = this.PgSqlIsAutoToLower;
            copyContext.IsSingle = this.IsSingle;
            return copyContext;
        }
        #endregion

        #region Override methods
        public virtual string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (IsTranslationText(entityName)) return entityName;
            isMapping = isMapping && this.MappingTables.HasValue();
            var isComplex = entityName.Contains(UtilConstants.Dot);
            if (isMapping && isComplex)
            {
                var columnInfo = entityName.Split(UtilConstants.DotChar);
                var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                if (mappingInfo != null)
                {
                    columnInfo[columnInfo.Length - 1] = mappingInfo.EntityName;
                }
                return string.Join(UtilConstants.Dot, columnInfo.Select(it => GetTranslationText(it)));
            }
            else if (isMapping)
            {
                var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                var name = (mappingInfo == null ? entityName : mappingInfo.DbTableName);
                if (name.Contains("."))
                {
                    return string.Join(".", name.Split('.').Select(it => SqlTranslationLeft + it + SqlTranslationRight));
                }
                else
                {
                    return SqlTranslationLeft + name + SqlTranslationRight;
                }
            }
            else if (isComplex)
            {
                return string.Join(UtilConstants.Dot, entityName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
            }
            else
            {
                return GetTranslationText(entityName);
            }
        }
        public virtual string GetTranslationColumnName(string columnName)
        {
            Check.ArgumentNullException(columnName, string.Format(ErrorMessage.ObjNotExist, "Column Name"));
            if (columnName.Substring(0, 1) == this.SqlParameterKeyWord)
            {
                return columnName;
            }
            if (IsTranslationText(columnName)) return columnName;
            if (columnName.Contains(UtilConstants.Dot))
            {
                return string.Join(UtilConstants.Dot, columnName.Split(UtilConstants.DotChar).Select(it => GetTranslationText(it)));
            }
            else
            {
                return GetTranslationText(columnName);
            }
        }
        public virtual string GetDbColumnName(string entityName, string propertyName)
        {
            if (this.MappingColumns.HasValue())
            {
                var mappingInfo = this.MappingColumns.SingleOrDefault(it => it.EntityName == entityName && it.PropertyName == propertyName);
                return mappingInfo == null ? propertyName : mappingInfo.DbColumnName;
            }
            else
            {
                return propertyName;
            }
        }
        public virtual bool IsTranslationText(string name)
        {
            var result = name.IsContainsIn(SqlTranslationLeft, SqlTranslationRight, UtilConstants.Space, ExpressionConst.LeftParenthesis, ExpressionConst.RightParenthesis);
            return result;
        }
        public virtual string GetTranslationText(string name)
        {
            return SqlTranslationLeft + name + SqlTranslationRight;
        }
        public virtual string GetAsString(string asName, string fieldValue)
        {
            if (fieldValue.Contains(".*") || fieldValue == "*") return fieldValue;
            return string.Format(" {0} {1} {2} ", GetTranslationColumnName(fieldValue), "AS", GetTranslationColumnName(asName));
        }

        public virtual string GetEqString(string eqName, string fieldValue)
        {
            return string.Format(" {0} {1} {2} ", GetTranslationColumnName(eqName), "=", GetTranslationColumnName(fieldValue));
        }

        public virtual string GetAsString(string asName, string fieldValue, string fieldShortName)
        {
            if (fieldValue.Contains(".*") || fieldValue == "*") return fieldValue;
            return string.Format(" {0} {1} {2} ", GetTranslationColumnName(fieldShortName + "." + fieldValue), "AS", GetTranslationColumnName(asName));
        }
        #endregion
    }
}
