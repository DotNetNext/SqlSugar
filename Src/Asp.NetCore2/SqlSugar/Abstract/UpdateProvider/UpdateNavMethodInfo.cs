using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class UpdateNavMethodInfo
    {
        internal object MethodInfos { get;  set; }
        internal SqlSugarProvider Context { get;  set; }

        public UpdateNavMethodInfo IncludeByNameString(string navMemberName, UpdateNavOptions updateNavOptions = null)
        {
            var type = MethodInfos.GetType().GetGenericArguments()[0];
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type); 
            Type properyItemType;
            Expression exp = UtilMethods.GetIncludeExpression(navMemberName, entityInfo, out properyItemType);
            var method = this.GetType().GetMyMethod("Include", 2)
                            .MakeGenericMethod(properyItemType);
            var obj = method.Invoke(this, new object[] { exp, updateNavOptions });
            this.MethodInfos = obj; 
            return this;
        }
        public UpdateNavMethodInfo ThenIncludeByNameString(string IncludeByNameString, UpdateNavOptions updateNavOptions = null)
        {
            var type = MethodInfos.GetType().GetGenericArguments()[0];
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type);
            Type properyItemType;
            Expression exp = UtilMethods.GetIncludeExpression(navMemberName, entityInfo, out properyItemType);
            var method = this.GetType().GetMyMethod("ThenInclude", 2)
                            .MakeGenericMethod(properyItemType);
            var obj = method.Invoke(this, new object[] { exp, updateNavOptions });
            this.MethodInfos = obj;
            return this;
        }
        public async Task<bool> ExecuteCommandAsync() 
        {
            if (Context == null) return false;
            var result = MethodInfos.GetType().GetMethod("ExecuteCommandAsync").Invoke(MethodInfos, new object[] { });
            return await (Task<bool>)result;
        }
        public bool  ExecuteCommand()
        {
            if (Context == null) return false;
            var result = MethodInfos.GetType().GetMethod("ExecuteCommand").Invoke(MethodInfos, new object[] { });
            return (bool)result;
        }
    }
}
