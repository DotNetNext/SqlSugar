using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar
{
    public enum DataFilterType
    {
        UpdateByObject = 0,
        InsertByObject = 1,
        DeleteByObject =2
    }
    public class DataFilterModel 
    {
        public DataFilterType OperationType { get; set; }
        public EntityColumnInfo EntityColumnInfo { get; set; }
        public object EntityValue { get; set; }
        public string PropertyName { get { return EntityColumnInfo.PropertyInfo.Name; } }
        public string EntityName { get { return EntityColumnInfo.EntityName; } }


        public void SetValue(object value)
        {
            try
            {
                var type = EntityColumnInfo.PropertyInfo.PropertyType;
                if (value != null && value.GetType() != type)
                {
                    value = UtilMethods.ChangeType2(value, type);
                }
                this.EntityColumnInfo.PropertyInfo.SetValue(EntityValue, value);
            }
            catch (Exception ex)
            {
                Check.ExceptionEasy($" SetValue error in DataExecuting {EntityName} . {ex.Message}", $" DataExecuting 中 SetValue出错 {EntityName} 。 {ex.Message}");
            }
        }
        public bool IsAnyAttribute<T>() where T : Attribute
        {
            return this.EntityColumnInfo.PropertyInfo.GetCustomAttribute<T>() != null;
        }
        public T GetAttribute<T>() where T : Attribute
        {
            return this.EntityColumnInfo.PropertyInfo.GetCustomAttribute<T>();
        }
    }
    public class DataAfterModel
    {

        public List<EntityColumnInfo> EntityColumnInfos { get; set; }
        public object EntityValue { get; set; }
        public EntityInfo Entity { get; set; }
        public object GetValue(string propertyName)
        {
            var propety=EntityColumnInfos.FirstOrDefault(it => it.PropertyName == propertyName);
            Check.ExceptionEasy(propety==null,$"Aop.DataExecuted error . { Entity.EntityName} no property {propertyName}.", $"Aop.DataExecuted 出错 {Entity.EntityName}不存在属性{propertyName}");
            return propety.PropertyInfo.GetValue(EntityValue);
        }
        public void SetValue(string propertyName,object value)
        {
            var propety = EntityColumnInfos.FirstOrDefault(it => it.PropertyName == propertyName);
            Check.ExceptionEasy(propety == null, $"Aop.DataExecuted error . { Entity.EntityName} no property {propertyName}.", $"Aop.DataExecuted 出错 {Entity.EntityName}不存在属性{propertyName}");
            propety.PropertyInfo.SetValue(EntityValue,value);
        }
    }
}
