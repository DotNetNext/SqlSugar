using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class CommonMethodInfo
    {
        internal object Context { get; set; }

        public int ExecuteReturnIdentity()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteReturnIdentity", 0).Invoke(Context, new object[] { });
            return (int)result;
        }
        public async Task<int> ExecuteReturnIdentityAsync()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteReturnIdentityAsync", 0).Invoke(Context, new object[] { });
            return await (Task<int>)result;
        }
        public int ExecuteCommand()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommand", 0).Invoke(Context, new object[] { });
            return (int)result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommandAsync", 0).Invoke(Context, new object[] { });
            return await (Task<int>)result;
        }
    }
    public class SplitMethodInfo
    {
        internal object Context { get; set; }

        public int ExecuteCommand()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommand", 0).Invoke(Context, new object[] { });
            return (int)result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommandAsync", 0).Invoke(Context, new object[] { });
            return await (Task<int>)result;
        }
    }
    public class UpdateCommonMethodInfo
    {
        internal object Context { get; set; }

        public int ExecuteCommand()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommand", 0).Invoke(Context, new object[] { });
            return (int)result;
        }
        public async Task<int> ExecuteCommandAsync()
        {
            if (Context == null) return 0;
            var result = Context.GetType().GetMyMethod("ExecuteCommandAsync", 0).Invoke(Context, new object[] { });
            return await (Task<int>)result;
        }
    }

}
