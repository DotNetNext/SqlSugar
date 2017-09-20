using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class AopProvider
    {
        private AopProvider() { }
        public AopProvider(SqlSugarClient context)
        {
            this.Context = context;
            this.Context.Ado.IsEnableLogEvent = true;
        }
        private SqlSugarClient Context { get; set; }
        public Action<Exception> OnError { set { this.Context.Ado.ErrorEvent = value; } }
        public Action<string, SugarParameter[]> OnLogExecuting { set { this.Context.Ado.LogEventStarting = value; } }
        public Action<string, SugarParameter[]> OnLogExecuted { set { this.Context.Ado.LogEventCompleted = value; } }
        public Func<string, SugarParameter[], KeyValuePair<string, SugarParameter[]>> OnExecutingChangeSql { set { this.Context.Ado.ProcessingEventStartingSQL = value; } }
    }
}
