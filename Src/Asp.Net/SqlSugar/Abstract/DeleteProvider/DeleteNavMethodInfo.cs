using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class DeleteNavMethodInfo
    {
        internal object MethodInfos { get; set; }
        internal SqlSugarProvider Context { get; set; }

        public DeleteNavMethodInfo IncludeByNameString(string navMemberName, DeleteNavOptions updateNavOptions = null)
        {
            var type = MethodInfos.GetType().GetGenericArguments()[0];
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type);
            Type properyItemType;
            bool isList;
            Expression exp = UtilMethods.GetIncludeExpression(navMemberName, entityInfo, out properyItemType, out isList);
            var method = this.MethodInfos.GetType().GetMyMethod("Include", 2, isList)
                            .MakeGenericMethod(properyItemType);
            var obj = method.Invoke(this.MethodInfos, new object[] { exp, updateNavOptions });
            this.MethodInfos = obj;
            return this;
        }
        public DeleteNavMethodInfo ThenIncludeByNameString(string navMemberName, DeleteNavOptions updateNavOptions = null)
        {
            var type = MethodInfos.GetType().GetGenericArguments()[1];
            var entityInfo = this.Context.EntityMaintenance.GetEntityInfo(type);
            Type properyItemType;
            bool isList;
            Expression exp = UtilMethods.GetIncludeExpression(navMemberName, entityInfo, out properyItemType, out isList);
            var method = this.MethodInfos.GetType().GetMyMethod("ThenInclude", 2, isList)
                            .MakeGenericMethod(properyItemType);
            var obj = method.Invoke(this.MethodInfos, new object[] { exp, updateNavOptions });
            this.MethodInfos = obj;
            return this;
        }
        public async Task<bool> ExecuteCommandAsync()
        {
            if (Context == null) return false;
            var result = MethodInfos.GetType().GetMethod("ExecuteCommandAsync").Invoke(MethodInfos, new object[] { });
            return await (Task<bool>)result;
        }
        public bool ExecuteCommand()
        {
            if (Context == null) return false;
            var result = MethodInfos.GetType().GetMethod("ExecuteCommand").Invoke(MethodInfos, new object[] { });
            return (bool)result;
        }
    }
}
