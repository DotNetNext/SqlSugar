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
        InsertByObject = 1
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
            var type = EntityColumnInfo.PropertyInfo.PropertyType;
            if (value != null && value.GetType() != type) 
            {
                value = UtilMethods.ChangeType2(value, type);
            }
            this.EntityColumnInfo.PropertyInfo.SetValue(EntityValue, value);
        }
    }
}
