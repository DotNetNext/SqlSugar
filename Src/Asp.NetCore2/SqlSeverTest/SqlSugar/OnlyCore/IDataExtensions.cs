﻿using System;
using System.Text.RegularExpressions;
using System.Linq;
namespace SqlSugar
{
    public interface IDataAdapter
    {
        void Fill(DataSet ds);
    }
    public partial class SqliteProvider : AdoProvider
    {
        public override void ExecuteBefore(string sql, SugarParameter[] parameters)
        {
            if (sql.HasValue() && parameters.HasValue())
            {
                foreach (var parameter in parameters)
                {
                    //Compatible with.NET CORE parameters case
                    var name = parameter.ParameterName;
                    if (!sql.Contains(name) && Regex.IsMatch(sql, "(" + name + "$)" + "|(" + name + @"[ ,\,])", RegexOptions.IgnoreCase))
                    {
                        parameter.ParameterName = Regex.Match(sql, "(" + name + "$)" + "|(" + name + @"[ ,\,])", RegexOptions.IgnoreCase).Value;
                    }
                }
            }
            if (this.IsEnableLogEvent)
            {
                Action<string, SugarParameter[]> action = LogEventStarting;
                if (action != null)
                {
                    if (parameters == null || parameters.Length == 0)
                    {
                        action(sql, new SugarParameter[] { });
                    }
                    else
                    {
                        action(sql,parameters);
                    }
                }
            }
        }
    }
}
