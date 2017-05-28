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
        public virtual void SetParamterSize(SugarParameter[] parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    this.SetParameterSize(parameter);
                }
            }
        }
        public virtual void SetParameterSize(SugarParameter parameters)
        {
            int size = parameters.Size;
            if (size < 4000)
            {
                parameters.Size = 4000;
            }
        }

        public virtual void SetSqlDbType(PropertyInfo propertyInfo, SugarParameter parameter)
        {

        }

        protected virtual SugarParameter[] GetParameters(object parameters, PropertyInfo[] propertyInfo,string sqlParameterKeyWord)
        {
            List<SugarParameter> listParams = new List<SugarParameter>();
            if (parameters != null)
            {
                var entityType = parameters.GetType();
                var isDic = entityType.IsIn(PubConst.DicArraySO, PubConst.DicArraySS);
                if (isDic)
                {
                    if (entityType == PubConst.DicArraySO)
                    {
                        var newObj = (Dictionary<string, object>)parameters;
                        var pars = newObj.Select(it => new SugarParameter(sqlParameterKeyWord + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParameterSize(par);
                        }
                        listParams.AddRange(pars);
                    }
                    else
                    {
                        var newObj = (Dictionary<string, string>)parameters;
                        var pars = newObj.Select(it => new SugarParameter(sqlParameterKeyWord + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParameterSize(par);
                        }
                        listParams.AddRange(pars); ;
                    }
                }
                else
                {
                    PropertyInfo[] propertiesObj = null;
                    if (propertyInfo != null)
                    {
                        propertiesObj = propertyInfo;
                    }
                    else
                    {
                        propertiesObj = entityType.GetProperties();
                    }
                    string replaceGuid = Guid.NewGuid().ToString();
                    foreach (PropertyInfo r in propertiesObj)
                    {
                        var value = r.GetValue(parameters, null);
                        if (r.PropertyType.IsEnum)
                        {
                            value = Convert.ToInt64(value);
                        }
                        if (value == null || value.Equals(DateTime.MinValue)) value = DBNull.Value;
                        if (r.Name.ToLower().Contains("hierarchyid"))
                        {
                            var par = new SugarParameter(sqlParameterKeyWord + r.Name, SqlDbType.Udt);
                            par.UdtTypeName = "HIERARCHYID";
                            par.Value = value;
                            listParams.Add(par);
                        }
                        else
                        {
                            var par = new SugarParameter(sqlParameterKeyWord + r.Name, value);
                            SetParameterSize(par);
                            if (value == DBNull.Value)
                            {//防止文件类型报错
                                SetSqlDbType(r, par);
                            }
                            listParams.Add(par);
                        }
                    }
                }
            }
            return listParams.ToArray();
        }
    }
}
