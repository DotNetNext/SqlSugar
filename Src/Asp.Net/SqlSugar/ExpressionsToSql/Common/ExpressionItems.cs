using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    internal class ExpressionItems
    {
        /// <summary>
        /// 1 memeber, 2 class  ,3 method
        /// </summary>
        public int Type { get; set; }
        public EntityInfo ParentEntityInfo { get; set; }
        public EntityInfo ThisEntityInfo { get; set; }
        public Expression Expression { get; set; }
        public Navigate Nav
        {
            get
            {
                if (Expression is MemberExpression)
                {
                    var name = (Expression as MemberExpression).Member.Name;
                    var navColumn = ParentEntityInfo.Columns.FirstOrDefault(it => it.PropertyName == name);
                    return navColumn == null ? null : navColumn.Navigat;
                }
                return null;
            }
        }

    }
}
