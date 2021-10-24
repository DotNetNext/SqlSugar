using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class SplitTableDeleteProvider<T>  where T : class, new()
    {
        public ISqlSugarClient Context;
        public DeleteableProvider<T> deleteobj;

        public IEnumerable<SplitTableInfo> Tables { get;  set; }

        public int ExecuteCommand()
        {
            var result = 0;
            var sqlobj = deleteobj.ToSql();
      
            foreach (var item in Tables)
            {
                var  newsqlobj = GetSqlObj(sqlobj, item.TableName);
                result +=this.Context.Ado.ExecuteCommand(newsqlobj.Key, newsqlobj.Value);
            }
            return result;
        }

        public async Task<int> ExecuteCommandAsync()
        {
            var result = 0;
            var sqlobj = deleteobj.ToSql();
            foreach (var item in Tables)
            {
                var newsqlobj = GetSqlObj(sqlobj, item.TableName);
                result +=await this.Context.Ado.ExecuteCommandAsync(newsqlobj.Key, newsqlobj.Value);
            }
            return result;
        }

        private KeyValuePair<string, List<SugarParameter>> GetSqlObj(KeyValuePair<string, List<SugarParameter>> keyValuePair,string asName)
        {
            List<SugarParameter> pars = new List<SugarParameter>();
            string sql = keyValuePair.Key;
            if (keyValuePair.Value != null) 
            {
                pars = keyValuePair.Value.Select(it => new SugarParameter(it.ParameterName, it.Value)).ToList();
            }
            sql = Regex.Replace(sql, deleteobj.EntityInfo.DbTableName, asName,RegexOptions.IgnoreCase);
            return new KeyValuePair<string, List<SugarParameter>>(sql,pars);
        }

    }
}
