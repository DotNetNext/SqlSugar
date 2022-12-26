using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

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
    }
}
