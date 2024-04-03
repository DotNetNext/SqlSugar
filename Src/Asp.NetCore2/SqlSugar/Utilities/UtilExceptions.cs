using System;
namespace SqlSugar
{
    public class SqlSugarException : Exception
    {
        public  string Sql { get; set; }
        public  object Parametres { get; set; }

        public SqlSugarException(string message, Exception ex)
            : base(message, ex)
        {
        }
        public SqlSugarException(string message)
            : base(message) { }

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
            : base(ex.Message,ex)
        {
            this.Sql = sql;
            this.Parametres = pars;
            this.Source = ex.Source;
        }

        public SqlSugarException(SqlSugarProvider context, string message, object pars)
            : base(message) {
            this.Parametres = pars;
        }
    }
    public class VersionExceptions : SqlSugarException
    {
        public VersionExceptions(string message, Exception ex)
            : base(message, ex) { }
        public VersionExceptions(string message)
            : base(message) { }
    }
}
