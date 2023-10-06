using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public class StorageableMethodInfo
    {
        internal SqlSugarProvider Context { get; set; }
        internal MethodInfo MethodInfo { get; set; }
        internal object objectValue { get; set; }
        public int ExecuteCommand()
        {
            if (Context == null) return 0;
            object objectValue = null;
            MethodInfo method = GetSaveMethod(ref objectValue);
            if (method == null) return 0;
            return (int)method.Invoke(objectValue, new object[] { });
        }

        public StorageableAsMethodInfo AsInsertable
        {
            get
            {
                var type = "AsInsertable";
                return GetAs(type);
            }
        }
        public StorageableAsMethodInfo AsUpdateable 
        {
            get
            {
                var type = "AsUpdateable";
                return GetAs(type);
            }
        }

        private StorageableAsMethodInfo GetAs(string type)
        {
            object objectValue = null;
            MethodInfo method = GetSaveMethod(ref objectValue);
            if (method == null) return new StorageableAsMethodInfo(null);
            method = objectValue.GetType().GetMethod("ToStorage");
            objectValue = method.Invoke(objectValue, new object[] { });
            StorageableAsMethodInfo result = new StorageableAsMethodInfo(type);
            result.ObjectValue = objectValue;
            result.Method = method;
            return result;
        }

        private MethodInfo GetSaveMethod(ref object callValue)
        {
            if (objectValue == null)
                return null;
            callValue = MethodInfo.Invoke(Context, new object[] { objectValue });
            return callValue.GetType().GetMyMethod("ExecuteCommand",0);
        }

        public StorageableMethodInfo ToStorage()
        {
            return this;
        }

        public StorageableSplitTableMethodInfo SplitTable()
        {
            object objectValue = null;
            MethodInfo method = GetSaveMethod(ref objectValue);
            if (method == null) return new StorageableSplitTableMethodInfo(null);
            method = objectValue.GetType().GetMethod("SplitTable");
            objectValue = method.Invoke(objectValue, new object[] { });
            StorageableSplitTableMethodInfo result = new StorageableSplitTableMethodInfo(null);
            result.ObjectValue = objectValue;
            result.Method = method;
            return result;
        }
    }

    public class StorageableAsMethodInfo
    {
        private StorageableAsMethodInfo() { }
        private string type;
        public StorageableAsMethodInfo(string type) 
        {
            this.type = type;
        }
        internal object ObjectValue { get;  set; }
        internal MethodInfo Method { get;   set; }
        public int ExecuteCommand()
        {
            if (type == null) return 0;
            PropertyInfo property = ObjectValue.GetType().GetProperty(type);
            var value = property.GetValue(ObjectValue);
            var newObj= value.GetType().GetMethod("ExecuteCommand").Invoke(value, new object[] { });
            return (int)newObj;
        }
    }

    public class StorageableSplitTableMethodInfo
    {
        private StorageableSplitTableMethodInfo() { }
        private string type;
        public StorageableSplitTableMethodInfo(string type)
        {
            this.type = type;
        }
        internal object ObjectValue { get; set; }
        internal MethodInfo Method { get; set; }
        public int ExecuteCommand()
        { 
            var newObj = ObjectValue.GetType().GetMethod("ExecuteCommand").Invoke(ObjectValue, new object[] { });
            return (int)newObj;
        }
    }
}
