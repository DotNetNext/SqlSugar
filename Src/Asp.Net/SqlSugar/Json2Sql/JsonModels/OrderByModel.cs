using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSugar
{
    public class OrderByModel 
    {
        public object FieldName { get; set; }
        public OrderByType OrderByType { get; set; }
        public static List<OrderByModel> Create(params OrderByModel[] orderByModel)
        {
            return orderByModel.ToList();
        }
    }
}
