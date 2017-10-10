using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public partial class AdoAccessory
    {
        protected IDbBind _DbBind;
        protected IDbFirst _DbFirst;
        protected ICodeFirst _CodeFirst;
        protected IDbMaintenance _DbMaintenance;
        protected IDbConnection _DbConnection;

        protected virtual SugarParameter[] GetParameters(object parameters, PropertyInfo[] propertyInfo, string sqlParameterKeyWord)
        {
            List<SugarParameter> result = new List<SugarParameter>();
            if (parameters != null)
            {
                var entityType = parameters.GetType();
                var isDictionary = entityType.IsIn(UtilConstants.DicArraySO, UtilConstants.DicArraySS);
                if (isDictionary)
                    DictionaryToParameters(parameters, sqlParameterKeyWord, result, entityType);
                else if (parameters is List<SugarParameter>)
                {
                    result = (parameters as List<SugarParameter>);
                }
                else if (parameters is SugarParameter[])
                {
                    result = (parameters as SugarParameter[]).ToList();
                }
                else
                {
                    Check.Exception(!entityType.IsAnonymousType(), "The parameter format is wrong. \nUse new{{xx=xx, xx2=xx2}}  or \nDictionary<string, object> or \nSugarParameter [] ");
                    ProperyToParameter(parameters, propertyInfo, sqlParameterKeyWord, result, entityType);
                }
            }
            return result.ToArray();
        }
        protected void ProperyToParameter(object parameters, PropertyInfo[] propertyInfo, string sqlParameterKeyWord, List<SugarParameter> listParams, Type entityType)
        {
            PropertyInfo[] properties = null;
            if (propertyInfo != null)
                properties = propertyInfo;
            else
                properties = entityType.GetProperties();

            foreach (PropertyInfo properyty in properties)
            {
                var value = properyty.GetValue(parameters, null);
                if (properyty.PropertyType.IsEnum())
                    value = Convert.ToInt64(value);
                if (value == null || value.Equals(DateTime.MinValue)) value = DBNull.Value;
                if (properyty.Name.ToLower().Contains("hierarchyid"))
                {
                    var parameter = new SugarParameter(sqlParameterKeyWord + properyty.Name, SqlDbType.Udt);
                    parameter.UdtTypeName = "HIERARCHYID";
                    parameter.Value = value;
                    listParams.Add(parameter);
                }
                else
                {
                    var parameter = new SugarParameter(sqlParameterKeyWord + properyty.Name, value);
                    listParams.Add(parameter);
                }
            }
        }
        protected void DictionaryToParameters(object parameters, string sqlParameterKeyWord, List<SugarParameter> listParams, Type entityType)
        {
            if (entityType == UtilConstants.DicArraySO)
            {
                var dictionaryParameters = (Dictionary<string, object>)parameters;
                var sugarParameters = dictionaryParameters.Select(it => new SugarParameter(sqlParameterKeyWord + it.Key, it.Value));
                listParams.AddRange(sugarParameters);
            }
            else
            {
                var dictionaryParameters = (Dictionary<string, string>)parameters;
                var sugarParameters = dictionaryParameters.Select(it => new SugarParameter(sqlParameterKeyWord + it.Key, it.Value));
                listParams.AddRange(sugarParameters); ;
            }
        }
    }
}