using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqlSugar
{
    public partial class SqliteProvider : AdoProvider
    {
        public override void ExecuteBefore(string sql, SugarParameter[] parameters)
        {
            if (sql.IsValuable() && parameters.IsValuable())
            {
                foreach (var parameter in parameters)
                {
                    //Compatible with.NET CORE parameters case
                    var name = parameter.ParameterName;
                    if (!sql.Contains(name) && Regex.IsMatch(sql, "(" + name + "$)" + "|(" + name + @"[ ,\,])", RegexOptions.IgnoreCase)) {
                       parameter.ParameterName=Regex.Match(sql, "(" + name + "$)" + "|(" + name + @"[ ,\,])", RegexOptions.IgnoreCase).Value;
                    }
                }
            }
            if (this.IsEnableLogEvent)
            {
                Action<string, string> action = LogEventStarting;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, null);
                    }
                    else
                    {
                        action(sql, this.Context.RewritableMethods.SerializeObject(parameters.Select(it => new { key = it.ParameterName, value = it.Value.ObjToString() })));
                    }
                }
            }
        }
    }
}
