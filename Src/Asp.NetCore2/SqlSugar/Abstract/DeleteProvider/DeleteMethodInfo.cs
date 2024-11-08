﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteMethodInfo
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
            return  await(Task<int>)result;
        }

        public CommonMethodInfo AS(string tableName)
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("AS", 1, typeof(string));
            var result = newMethod.Invoke(inertable, new object[] { tableName });
            return new CommonMethodInfo()
            {
                Context = result
            };
        }
        public CommonMethodInfo EnableDiffLogEvent(object businessData = null)
        {
            if (Context == null)
            {
                return new CommonMethodInfo();
            }
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("EnableDiffLogEvent", 1, typeof(object));
            var result = newMethod.Invoke(inertable, new object[] { businessData });
            return new CommonMethodInfo()
            {
                Context = result
            };
        }
        public CommonMethodInfo SplitTable()
        {
            var inertable = MethodInfo.Invoke(Context, new object[] { objectValue });
            var newMethod = inertable.GetType().GetMyMethod("SplitTable", 0);
            var result = newMethod.Invoke(inertable, new object[] { });
            return new CommonMethodInfo()
            {
                Context = result
            };
        }
    }
}
