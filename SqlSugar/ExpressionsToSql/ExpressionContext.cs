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
    /// ** qq:610262374
    public class ExpressionContext : ExpResolveAccessory
    {

        #region properties
        public IDbMethods DbMehtods { get; set; }
        public int Index { get; set; }
        public int ParameterIndex { get; set; }
        public MappingColumnList MappingColumns { get; set; }
        public MappingTableList MappingTables { get; set; }
        public List<JoinQueryInfo> JoinQueryInfos { get; set; }

        public bool IsJoin {
            get
            {
               return JoinQueryInfos.IsValuable();
            }
        }
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
        public virtual string GetTranslationTableName(string name,bool isMapping=true)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name Or Column Name"));
            if (name.Contains("[") && name.Contains("]")) return name;
            if (isMapping&&this.MappingTables.IsValuable())
            {
                if (name.Contains("."))
                {
                    var columnInfo = name.Split('.');
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null)
                    {
                        columnInfo[columnInfo.Length - 1] = mappingInfo.EntityName;
                    }
                    return string.Join(".", columnInfo.Select(it => "[" + it + "]"));
                }
                else
                {
                    var mappingInfo = this.MappingTables.FirstOrDefault(it => it.EntityName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    return "[" + (mappingInfo == null ? name : mappingInfo.EntityName) + "]";
                }
            }
            else
            {
                if (name.Contains("."))
                {
                    return string.Join(".", name.Split('.').Select(it => "[" + it + "]"));
                }
                else
                {
                    return "[" + name + "]";
                }
            }
        }
        public virtual string GetTranslationColumnName(string name, bool isMapping = true)
        {
            Check.ArgumentNullException(name, string.Format(ErrorMessage.ObjNotExist, "Table Name Or Column Name"));
            if (name.Contains("[") && name.Contains("]")) return name;
            if (isMapping&&this.MappingColumns.IsValuable())
            {
                if (name.Contains("."))
                {
                    var columnInfo = name.Split('.');
                    var mappingInfo = this.MappingColumns.FirstOrDefault(it => it.EntityPropertyName.Equals(columnInfo.Last(), StringComparison.CurrentCultureIgnoreCase));
                    if (mappingInfo != null) {
                        columnInfo[columnInfo.Length-1] = mappingInfo.DbColumnName;
                    }
                    return string.Join(".", columnInfo.Select(it => "[" + it + "]"));
                }
                else
                {
                    var mappingInfo = this.MappingColumns.FirstOrDefault(it => it.EntityPropertyName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
                    return "[" + (mappingInfo == null ? name : mappingInfo.DbColumnName) + "]";
                }
            }
            else {
                if (name.Contains("."))
                {
                    return string.Join(".", name.Split('.').Select(it => "[" + it + "]"));
                }
                else
                {
                    return "[" + name + "]";
                }
            }
        }
        public virtual void Resolve(Expression expression, ResolveExpressType resolveType)
        {
            this.ResolveType = resolveType;
            this.Expression = expression;
            BaseResolve resolve = new BaseResolve(new ExpressionParameter() { Expression = this.Expression, Context = this });
            resolve.Start();
        }
        public virtual string GetAsString(string asName, string fieldValue)
        {
            return string.Format(" {0} {1} {2} ", GetTranslationTableName(fieldValue), "AS", GetTranslationColumnName(asName,false));
        }
        public virtual string GetAsString(string asName, string fieldValue,string fieldShortName)
        {
            return string.Format(" {0} {1} {2} ", GetTranslationTableName(fieldShortName+"."+ fieldValue), "AS", GetTranslationTableName(asName,false));
        }
        public virtual void Clear()
        {
            base._Result = null;
        }
        #endregion
    }
}
