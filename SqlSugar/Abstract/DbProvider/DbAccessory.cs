using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
namespace SqlSugar
{
    public partial class DbAccessory
    {
        protected IDbBind _DbBind;
        protected IDbFirst _DbFirst;
        protected ICodeFirst _CodeFirst;
        protected IDbMaintenance _DbMaintenance;
        protected IDbConnection _DbConnection;
        public virtual void SetParSize(SqlParameter[] pars)
        {
            if (pars != null)
            {
                foreach (var par in pars)
                {
                    this.SetParSize(par);
                }
            }
        }
        public virtual void SetParSize(SqlParameter par)
        {
            int size = par.Size;
            if (size < 4000)
            {
                par.Size = 4000;
            }
        }

        public virtual void SetSqlDbType(PropertyInfo prop, SqlParameter par)
        {
            var isNullable = false;
            var isByteArray = PubMethod.GetUnderType(prop, ref isNullable) == typeof(byte[]);
            if (isByteArray)
            {
                par.SqlDbType = SqlDbType.VarBinary;
            }
        }

        protected virtual SqlParameter[] GetParameters(object obj, PropertyInfo[] propertyInfo,string sqlParameterKeyWord)
        {
            List<SqlParameter> listParams = new List<SqlParameter>();
            if (obj != null)
            {
                var type = obj.GetType();
                var isDic = type.IsIn(PubConst.DicArraySO, PubConst.DicArraySS);
                if (isDic)
                {
                    if (type == PubConst.DicArraySO)
                    {
                        var newObj = (Dictionary<string, object>)obj;
                        var pars = newObj.Select(it => new SqlParameter(sqlParameterKeyWord + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
                        }
                        listParams.AddRange(pars);
                    }
                    else
                    {

                        var newObj = (Dictionary<string, string>)obj;
                        var pars = newObj.Select(it => new SqlParameter(sqlParameterKeyWord + it.Key, it.Value));
                        foreach (var par in pars)
                        {
                            SetParSize(par);
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
                        propertiesObj = type.GetProperties();
                    }
                    string replaceGuid = Guid.NewGuid().ToString();
                    foreach (PropertyInfo r in propertiesObj)
                    {
                        var value = r.GetValue(obj, null);
                        if (r.PropertyType.IsEnum)
                        {
                            value = Convert.ToInt64(value);
                        }
                        if (value == null || value.Equals(DateTime.MinValue)) value = DBNull.Value;
                        if (r.Name.ToLower().Contains("hierarchyid"))
                        {
                            var par = new SqlParameter(sqlParameterKeyWord + r.Name, SqlDbType.Udt);
                            par.UdtTypeName = "HIERARCHYID";
                            par.Value = value;
                            listParams.Add(par);
                        }
                        else
                        {
                            var par = new SqlParameter(sqlParameterKeyWord + r.Name, value);
                            SetParSize(par);
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
