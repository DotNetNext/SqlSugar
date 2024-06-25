using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
namespace SqlSugar
{
    public abstract partial class SqlBuilderProvider : SqlBuilderAccessory, ISqlBuilder
    {
        #region  Variable
        private string[] SqlSplicingOperator = new string[] { ">", ">=", "<", "<=", "(", ")", "!=", "<>", "not", "=", "||", "&&", "&", "|", "null", "is", "isnot", "like", "nolike", "+", "-", "*", "/", "%" , "$casewhen", "$then", "$when", "$else", "$end" };
        #endregion

        #region Root
        private string GetSqlPart(object value, List<SugarParameter> pars)
        {
            Check.Exception(value == null, $" FiledName is error ");
            if (IsSqlSplicingOperator(value))
            {
                return GetSqlSplicingOperator(value);
            }
            else if (IsString(value))
            {
                return GetSqlPartByString(value, pars);
            }
            else if (IsListObject(value))
            {
                return GetSqlPartByListObject(value, pars);
            }
            else if (IsObjectFunc(value))
            {
                return GetSqlPartByObjectFuncModel(value, pars);
            }
            else
            {
                return GetSqlPartError(value);
            }
        }

        #endregion

        #region Level2 

        private static string GetSqlSplicingOperator(object value)
        {
            var result = value.ObjToString();
            if (result == "||") return "OR";
            else if (result == "&&") return "AND";
            else if (result.EqualCase("isnot")) return " IS NOT ";
            else if (result.EqualCase("$casewhen")) return " CASE WHEN ";
            else if (result.EqualCase("$then")) return " THEN ";
            else if (result.EqualCase("$when")) return " WHEN ";
            else if (result.EqualCase("$else")) return " ELSE ";
            else if (result.EqualCase("$end")) return " END ";
            return result;
        }
        private static string GetSqlPartError(object value)
        {
            Check.Exception(value == null, $" {value} is error ");
            return null;
        }
        private string GetSqlPartByObjectFuncModel(object value, List<SugarParameter> pars)
        {
            var data = value as ObjectFuncModel;
            var obj = FuncModelToSql(data);
            pars.AddRange(obj.Value);
            return obj.Key;
        }
        private string GetSqlPartByListObject(object value, List<SugarParameter> pars)
        {
            var list = (value as List<object>);
            if (list.Count == 1)
            {
                return GetSqlPart(list.First(), pars).ObjToString();
            }
            else
            {
                Check.Exception(value == null, $" {value} is error ");
                return null;
            }
        }
        private string GetSqlPartByString(object value, List<SugarParameter> pars)
        {
            var valueString = value.ObjToString().Trim();
            if (Json2SqlHelper.IsSqlValue(valueString))
            {
                return GetParameterName(pars, valueString);
            }
            else
            {
                return this.GetTranslationColumnName(value.ObjToString().ToCheckField());
            }
        }
        #endregion

        #region Level3
        private string GetSplicingOperator(string valueString)
        {
            var parvalue = Regex.Match(valueString, @"\@s\:(.+)").Groups[1].Value;
            if (parvalue == null) parvalue = "";
            parvalue = parvalue.Trim();
            if (parvalue.ToLower().IsIn(SqlSplicingOperator))
            {
                return parvalue;
            }
            else
            {
                Check.ExceptionEasy($"{valueString} is error ", $"{valueString} 不是有效的拼接符号,拼接符号有:and、or、>=、<=、>、<、=、(、)");
            }
            return parvalue;
        }
        private string GetParameterName(List<SugarParameter> pars, string valueString)
        {
            object parvalue = Json2SqlHelper.GetValue(valueString);
            SugarParameter parameter = new SugarParameter("@p" + pars.Count(), parvalue);
            var type = Json2SqlHelper.GetType(valueString);
            parvalue = UtilMethods.ConvertDataByTypeName(type, parvalue.ObjToString());
            var parname = GetParameterName(pars, parvalue);
            return parname;
        }
        internal int GetParameterNameIndex = 100;

        private string GetParameterName(List<SugarParameter> pars, object parvalue)
        {
            var parname = "@p" + pars.Count() + "_" + (GetParameterNameIndex) + $"{this.QueryBuilder?.LambdaExpressions?.ParameterIndex}";
            SugarParameter parameter = new SugarParameter(parname, parvalue);
            pars.Add(parameter);
            GetParameterNameIndex++;
            if (this.QueryBuilder != null)
                this.QueryBuilder.LambdaExpressions.ParameterIndex++;
            return parname;
        }
        #endregion

        #region Helper

        private static bool IsListObject(object value)
        {
            return value.GetType() == typeof(List<object>);
        }
        private static bool IsString(object value)
        {
            return value.GetType() == typeof(string);
        }
        private static bool IsObjectFunc(object value)
        {
            return value is ObjectFuncModel;
        }
        private bool IsSqlSplicingOperator(object value)
        {
            return SqlSplicingOperator.Contains(value.ObjToString());
        }
        #endregion

    }
}
