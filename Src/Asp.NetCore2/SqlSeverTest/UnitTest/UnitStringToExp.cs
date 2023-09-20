using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Text;

namespace OrmTest
{
    internal class UnitStringToExp
    {
        public static void Init()
        {
            Test01();
            Test02();
        }

        private static void Test01()
        {
            //Clear
            StaticConfig.DynamicExpressionParserType = null;
            StaticConfig.DynamicExpressionParsingConfig = null;

            //程序启动时配置 5.1.4.106
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            var db = NewUnitTest.Db;
            var exp = DynamicCoreHelper.GetWhere<Order>("it", $"it=>it.Name== {"jack"}");
            var getAll22 = db.Queryable<Order>().Where(exp).ToList();


        }
        private static void Test02()
        {

            var db = NewUnitTest.Db;
            //Test Data
            db.CodeFirst.InitTables<UnitPerson011, UnitAddress011>();
            db.DbMaintenance.TruncateTable<UnitPerson011, UnitAddress011>();

            var address = new UnitAddress011
            {
                Street = "123 Main Street"
            };
            int addressId = db.Insertable(address).ExecuteReturnIdentity();

            // 创建 UnitPerson011 对象并插入记录
            var person = new UnitPerson011
            {
                Name = "John Doe",
                AddressId = addressId
            };
            int personId = db.Insertable(person).ExecuteReturnIdentity();

            var list = db.Queryable<UnitPerson011>().Includes(x => x.Address).ToList();

            //Clear
            StaticConfig.DynamicExpressionParserType = null;
            StaticConfig.DynamicExpressionParsingConfig = null;

            //程序启动时配置 5.1.4.106
            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            StaticConfig.DynamicExpressionParsingConfig = new ParsingConfig()
            {
                CustomTypeProvider = new SqlSugarTypeProvider()
            };
            var exp = DynamicCoreHelper.GetWhere<UnitPerson011>("it", $"it=>it.Address.Street={"a"}");
            var list2=db.Queryable<UnitPerson011>().Where(exp).ToList();
             
            //导航属性动态
            var list3 = db.Queryable<UnitPerson011>().Where("it", $"SqlFunc.Exists(it.Address.Id)").ToList();
            //普通条件动态
            var list4 = db.Queryable<UnitPerson011>().Where("it", $"it.Name={"a"}").ToList();
            //动态类+动态条件
            var list5=db.QueryableByObject(typeof(UnitPerson011)).Where("it", $"it.Address.Id=={1}").ToList();
        }
        public class SqlSugarTypeProvider : DefaultDynamicLinqCustomTypeProvider
        {
            public override HashSet<Type> GetCustomTypes()
            {
                var customTypes = base.GetCustomTypes();
                customTypes.Add(typeof(SqlFunc));
                return customTypes;
            }
        }
        [SugarTable("UnitPerson0afdaa1x1g1")]
        public class UnitPerson011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public int AddressId { get; set; }
            [SqlSugar.Navigate(SqlSugar.NavigateType.OneToOne, nameof(AddressId))]
            public UnitAddress011 Address { get; set; }
        }
        [SugarTable("UnitAddress011a11fasfa")]
        public class UnitAddress011
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Street { get; set; }
        }
    }
}
 
