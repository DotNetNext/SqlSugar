using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public static class EntityColumnExtension
    {
        public static EntityColumnable<T> IfTable<T>(this EntityColumnInfo entityColumnInfo) 
        {
            EntityColumnable<T> result = new EntityColumnable<T>();
            result.entityColumnInfo = entityColumnInfo;
            result.IsTable = entityColumnInfo.EntityName == typeof(T).Name;
            return result;
        }
    }

    public class EntityColumnable<T>
    {
        public EntityColumnInfo entityColumnInfo { get; set; }

        public bool IsTable { get;  set; }

        public EntityColumnable<T> UpdateProperty(Expression<Func<T,object>> propertyExpression,Action<EntityColumnInfo> updateAction) 
        {
            var name = ExpressionTool.GetMemberName(propertyExpression);
            if (entityColumnInfo.PropertyName == name && IsTable) 
            {
                updateAction(entityColumnInfo);
            }
            return this;
        }
        public EntityColumnable<T> OneToOne(Expression<Func<T, object>> propertyExpression,string firstName, string lastName=null)
        {
            var name = ExpressionTool.GetMemberName(propertyExpression);
            if (entityColumnInfo.PropertyName == name && IsTable)
            {
                entityColumnInfo.Navigat = new Navigate(NavigateType.OneToOne, firstName, lastName);
                entityColumnInfo.IsIgnore = true;
            }
            return this;
        }
        public EntityColumnable<T> OneToMany(Expression<Func<T, object>> propertyExpression, string firstName, string lastName)
        {
            var name = ExpressionTool.GetMemberName(propertyExpression);
            if (entityColumnInfo.PropertyName == name && IsTable)
            {
                entityColumnInfo.Navigat = new Navigate(NavigateType.OneToMany, firstName, lastName);
                entityColumnInfo.IsIgnore = true;
            }
            return this;
        }

        public EntityColumnable<T> ManyToMany(Expression<Func<T, object>> propertyExpression,Type mapppingType, string mapppingTypeAid, string mapppingTypeBid)
        {
            var name = ExpressionTool.GetMemberName(propertyExpression);
            if (entityColumnInfo.PropertyName == name && IsTable)
            {
                entityColumnInfo.Navigat = new Navigate(mapppingType, mapppingTypeAid, mapppingTypeBid);
                entityColumnInfo.IsIgnore = true;
            }
            return this;
        }
    }
}
