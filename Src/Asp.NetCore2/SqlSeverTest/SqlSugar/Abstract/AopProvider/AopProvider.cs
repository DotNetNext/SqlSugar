using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class AopProvider
    {
        private AopProvider() { }
        public AopProvider(SqlSugarProvider context)
        {
            this.Context = context;
            this.Context.Ado.IsEnableLogEvent = true;
        }
        private SqlSugarProvider Context { get; set; }
        public Action<DiffLogModel> OnDiffLogEvent { set { this.Context.CurrentConnectionConfig.AopEvents.OnDiffLogEvent = value; } }
        public Action<SqlSugarException> OnError { set { this.Context.CurrentConnectionConfig.AopEvents.OnError = value; } }
        public Action<string, SugarParameter[]> OnLogExecuting { set { this.Context.CurrentConnectionConfig.AopEvents.OnLogExecuting= value; } }
        public Action<string, SugarParameter[]> OnLogExecuted { set { this.Context.CurrentConnectionConfig.AopEvents.OnLogExecuted = value; } }
        public Func<string, SugarParameter[], KeyValuePair<string, SugarParameter[]>> OnExecutingChangeSql { set { this.Context.CurrentConnectionConfig.AopEvents.OnExecutingChangeSql = value; } }
    }
}
