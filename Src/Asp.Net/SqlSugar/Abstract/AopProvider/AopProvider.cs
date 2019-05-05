using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class AopProvider
    {
        private AopProvider() { }
        public AopProvider(SqlSugarContext context)
        {
            this.Context = context;
            this.Context.Ado.IsEnableLogEvent = true;
        }
        private SqlSugarContext Context { get; set; }
        public Action<DiffLogModel> OnDiffLogEvent { set { this.Context.DiffLogEvent = value; } }
        public Action<SqlSugarException> OnError { set { this.Context.ErrorEvent = value; } }
        public Action<string, SugarParameter[]> OnLogExecuting { set { this.Context.LogEventStarting = value; } }
        public Action<string, SugarParameter[]> OnLogExecuted { set { this.Context.LogEventCompleted = value; } }
        public Func<string, SugarParameter[], KeyValuePair<string, SugarParameter[]>> OnExecutingChangeSql { set { this.Context.ProcessingEventStartingSQL = value; } }
    }
}
