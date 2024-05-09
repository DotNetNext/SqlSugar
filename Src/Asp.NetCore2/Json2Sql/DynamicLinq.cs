using Npgsql;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Test;

namespace Test
{
    partial class Program
    {
        public static void DynamicLinq(ISqlSugarClient db) 
        {

            
             //程序启动时注册
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
             
            db.QueryFilter
             .AddTableFilter(typeof(Order), "it", $" it.Name={ "jack" } ") 
             .AddTableFilter(typeof(Order), "it", $" it.Id={1} ");  
            db.Queryable<Order>().ToList();

        } 
    }
     
}
