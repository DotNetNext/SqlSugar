using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Security.AccessControl;

namespace SqlSugar 
{
    public partial class DynamicBuilder
    {
        internal List<PropertyMetadata> propertyAttr = new List<PropertyMetadata>();
        internal List<CustomAttributeBuilder> entityAttr = new List<CustomAttributeBuilder>();
        internal string entityName { get; set; }
        internal Type baseType = null;
        internal Type[] interfaces = null;
        internal SqlSugarProvider context;

        public DynamicBuilder(SqlSugarProvider context)
        {
            this.context = context;
        }

        public DynamicProperyBuilder CreateClass(string entityName, SugarTable table=null, Type baseType = null, Type[] interfaces = null,SplitTableAttribute splitTableAttribute=null)
        {
            this.baseType = baseType;
            this.interfaces = interfaces;
            this.entityName = entityName;
            if (table == null) 
            {
                table = new SugarTable() { TableName = entityName };
            }
            this.entityAttr = new List<CustomAttributeBuilder>() { GetEntity(table) };
            if (splitTableAttribute != null) 
            {
                this.entityAttr.Add(GetSplitEntityAttr(splitTableAttribute));
            }
            return new DynamicProperyBuilder() {  baseBuilder=this};
        }

        public  object CreateObjectByType(Type type, Dictionary<string, object> dict)
        {
            // 创建一个默认的空对象
            object obj = Activator.CreateInstance(type);

            // 遍历字典中的每个 key-value 对
            foreach (KeyValuePair<string, object> pair in dict)
            {
                // 获取对象中的属性
                PropertyInfo propertyInfo = type.GetProperty(pair.Key);

                if (propertyInfo == null) 
                {
                    propertyInfo = type.GetProperties().FirstOrDefault(it=>it.Name.EqualCase(pair.Key));
                }

                if (propertyInfo != null)
                {
                    propertyInfo.SetValue(obj, UtilMethods.ChangeType2(pair.Value, propertyInfo.PropertyType));
                }
            }

            // 返回创建的对象
            return obj;
        }

        public  List<object> CreateObjectByType(Type type, List<Dictionary<string, object>> dictList)
        {
            List<object> result = new List<object>();
            foreach (var item in dictList)
            {
                result.Add(CreateObjectByType(type, item));
            }
            return result;
        }
    }
}
