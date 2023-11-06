using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace OrmTest
{
    internal class UnitCreateNavClass
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var typeBilder = db.DynamicBuilder().CreateClass("table1dfafa1231", new SugarTable() { });

            //可以循环添加列
            typeBilder.CreateProperty("Id", typeof(int), new SugarColumn() { IsPrimaryKey = true, IsIdentity = true });
            typeBilder.CreateProperty("Name", typeof(string), new SugarColumn() { });
            //一对一
            typeBilder.CreateProperty("OrderInfo",typeof(Order), navigate: new Navigate(NavigateType.OneToOne, "Id" ));//和导航查询一配置法
            //一对多
            typeBilder.CreateProperty("OrderInfos", typeof(List<Order>), navigate: new Navigate(NavigateType.OneToMany, nameof(Order.Id)));//和导航查询一配置法
            typeBilder.WithCache(); //缓存Key 表名+字段名称相加


            //创建类
            var type = typeBilder.BuilderType();

            //建表
            db.CodeFirst.InitTables(type);

            //设置动态表达式
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);

            var dic=  new Dictionary<string,object> { { "Name", "jack" } };
            var insertObj = db.DynamicBuilder().CreateObjectByType(type, dic);
            db.InsertableByObject(insertObj).ExecuteCommand();

            var list=db.QueryableByObject(type)
                .Where("it", $"it=>it.OrderInfo.Id==1")
                .Where("it",$"it=>it.OrderInfos.Any()")
                .ToList();

            var list2 = db.QueryableByObject(type)
              .Includes("OrderInfo")
              .Includes("OrderInfos")
              .ToList();
        }
    }
}
