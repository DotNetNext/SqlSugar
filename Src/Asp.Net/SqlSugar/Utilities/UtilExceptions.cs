using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
namespace SqlSugar
{
    public class SqlSugarException : Exception
    {
        public  string Sql { get; set; }
        public  object Parametres { get; set; }
        public new Exception InnerException;
        public new string  StackTrace;
        public new MethodBase TargetSite;
        public new string Source;

        public SqlSugarException(string message)
            : base(message){}

        public SqlSugarException(SqlSugarProvider context,string message, string sql)
            : base(message) {
            this.Sql = sql;
        }

        public SqlSugarException(SqlSugarProvider context, string message, string sql, object pars)
            : base(message) {
            this.Sql = sql;
            this.Parametres = pars;
        }

        public SqlSugarException(SqlSugarProvider context, Exception ex, string sql, object pars)
            : base(ex.Message)
        {
            this.Sql = sql;
            this.Parametres = pars;
            this.InnerException = ex.InnerException;
            this.StackTrace = ex.StackTrace;
            this.TargetSite = ex.TargetSite;
            this.Source = ex.Source;
        }

        public SqlSugarException(SqlSugarProvider context, string message, object pars)
            : base(message) {
            this.Parametres = pars;
        }
    }
    public class VersionExceptions : SqlSugarException
    {
        public VersionExceptions(string message)
            : base(message){ }
    }
}
