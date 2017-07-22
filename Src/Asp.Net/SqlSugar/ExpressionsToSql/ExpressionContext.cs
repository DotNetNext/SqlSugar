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

        #region properties
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
        public int Index { get; set; }
        public int ParameterIndex { get; set; }
        public MappingColumnList MappingColumns { get; set; }
        public MappingTableList MappingTables { get; set; }
        public IgnoreColumnList IgnoreComumnList { get; set; }
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
        #endregion

        #region public functions
        public virtual string GetTranslationTableName(string entityName, bool isMapping = true)
        {
            Check.ArgumentNullException(entityName, string.Format(ErrorMessage.ObjNotExist, "Table Name"));
            if (IsTranslationText(entityName)) return entityName;
            if (isMapping && this.MappingTables.IsValuable())
            {
                if (entityName.Contains("."))
                {
                    var columnInfo = entityName.Split('.');
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null)
                    {
                        columnInfo[columnInfo.Length - 1] = mappingInfo.EntityName;
                    }
                    return string.Join(".", columnInfo.Select(it => GetTranslationText(it)));
                }
                else
                {
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(entityName, StringComparison.CurrentCultureIgnoreCase));
                    return "[" + (mappingInfo == null ? entityName : mappingInfo.EntityName) + "]";
                }
            }
            else
            {
                if (entityName.Contains("."))
                {
                    return string.Join(".", entityName.Split('.').Select(it => GetTranslationText(it)));
                }
                else
                {
                    return GetTranslationText(entityName);
                }
            }
        }
        public virtual string GetTranslationColumnName(string columnName)
        {
            Check.ArgumentNullException(columnName, string.Format(ErrorMessage.ObjNotExist, "column Name"));
            if (columnName.Substring(0, 1) == this.SqlParameterKeyWord)
            {
                return columnName;
            }
            if (IsTranslationText(columnName)) return columnName;
            if (columnName.Contains("."))
            {
                return string.Join(".", columnName.Split('.').Select(it => GetTranslationText(it)));
            }
            else
            {
                return GetTranslationText(columnName);
            }
        }
        public virtual string GetDbColumnName(string entityName, string propertyName)
        {
            if (this.MappingColumns.IsValuable())
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
            return name.Contains("[") && name.Contains("]");
        }
        public virtual string GetTranslationText(string name)
        {
            return "[" + name + "]";
        }
        public virtual void Resolve(Expression expression, ResolveExpressType resolveType)
        {
            this.ResolveType = resolveType;
            this.Expression = expression;
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { CurrentExpression = this.Expression, Context = this });
            resolve.Start();
        }
        public virtual string GetAsString(string asName, string fieldValue)
        {
            if (fieldValue.Contains(".*")|| fieldValue=="*") return fieldValue;
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
        public virtual void Clear()
        {
            base._Result = null;
            base._Parameters = new List<SugarParameter>();
        }
        public ExpressionContext GetCopyContext()
        {
            ExpressionContext copyContext = (ExpressionContext)Activator.CreateInstance(this.GetType(), true);
            copyContext.Index = this.Index;
            copyContext.ParameterIndex = this.ParameterIndex;
            return copyContext;
        }
        #endregion
    }
}
