using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class UpdateMethodInfo
    {
        internal SqlSugarProvider Context { get; set; }
        internal MethodInfo MethodInfo { get; set; }
        internal object objectValue { get; set; }

        public int ExecuteCommand()
        {
            if (Context == null) return 0;
            var inertable=MethodInfo.Invoke(Context, new object[] { objectValue });
            var result= inertable.GetType().GetMethod("ExecuteCommand").Invoke(inertable,new object[] { });
            return (int)result;
        }

        public async Task<int> ExecuteCommandAsync()
        {
            if (Context == null) return 0;
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var result = inertable.GetType().GetMyMethod("ExecuteCommandAsync",0).Invoke(inertable, new object[] { });
            return await (Task<int>)result;
        }
        public UpdateCommonMethodInfo IgnoreColumns(params string[] ignoreColumns)
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("IgnoreColumns", 1,typeof(string[]));
            var result = newMethod.Invoke(inertable, new object[] { ignoreColumns });
            return new UpdateCommonMethodInfo()
            {
                Context = result
            };
        }

        public UpdateCommonMethodInfo UpdateColumns(params string[] updateColumns)
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("UpdateColumns", 1, typeof(string[]));
            var result = newMethod.Invoke(inertable, new object[] { updateColumns });
            return new UpdateCommonMethodInfo()
            {
                Context = result
            };
        }

        public UpdateCommonMethodInfo AS(string tableName)
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("AS", 1, typeof(string));
            var result = newMethod.Invoke(inertable, new object[] { tableName });
            return new UpdateCommonMethodInfo()
            {
                Context = result
            };
        }
        public UpdateCommonMethodInfo SplitTable()
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("SplitTable", 0);
            var result = newMethod.Invoke(inertable, new object[] { });
            return new UpdateCommonMethodInfo()
            {
                Context = result
            };
        }
    }
}
