using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UinitUpdateNavOneToOne
    {

        public static void Init() 
        {
            var db = NewUnitTest.Db;
            //建表 
            if (!db.DbMaintenance.IsAnyTable("UserTest", false))
            {
                db.CodeFirst.InitTables<UserTest>();
            }
            else 
            {
                db.DbMaintenance.TruncateTable<UserTest, ShopDataTest, AddressTest>();
            }
            //建表 
            if (!db.DbMaintenance.IsAnyTable("ShopDataTest", false))
            {
                db.CodeFirst.InitTables<ShopDataTest>();
            }
            //建表 
            if (!db.DbMaintenance.IsAnyTable("AddressTest", false))
            {
                db.CodeFirst.InitTables<AddressTest>();
            }


            //while (true)
            //{
                string uName = "测试用户名";
                if (!db.Queryable<UserTest>().Where(u => u.Name == uName).Any())
                {
                    UserTest userDataAdd = new UserTest();
                    userDataAdd.Name = uName;
                    db.Insertable(userDataAdd).ExecuteCommand();
                }

                //用例代码 

                //if (userData.ShopDataTest==null)
                //{

                //}
                UserTest userData = db.Queryable<UserTest>().Where(u => u.Name == uName)
                 .Includes(u => u.ShopDataTest, u => u.AddressTest)
                 .First();
                userData.ShopId = 0;
                userData.ShopDataTest = new ShopDataTest();
                userData.ShopDataTest.Name = "店铺名3";
                userData.ShopDataTest.GuId = 0;
                userData.ShopDataTest.AddressTest = new AddressTest()
                {
                    Name = "地址名称",
                };
                bool b = db.UpdateNav(userData)
                    .Include(u => u.ShopDataTest)
                    .ThenInclude(u => u.AddressTest)
                    .ExecuteCommand();

                UserTest userData2 = db.Queryable<UserTest>().Where(u => u.Name == uName)
                   .Includes(u => u.ShopDataTest, u => u.AddressTest)
                   .First();

                var address= userData2.ShopDataTest.AddressTest;

           // }

        }
        [SugarTable("AddressTest")]
        public class AddressTest
        {
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            public long GuId { get; set; }

            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }
        }
        [SugarTable("ShopDataTest")]
        public class ShopDataTest
        {
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            public long GuId { get; set; }


            [SugarColumn(ColumnName = "AddressId")]
            public long AddressId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(AddressId))]
            public AddressTest AddressTest { get; set; }

            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }
        }
        [SugarTable("UserTest")]
        public class UserTest
        {
            [SugarColumn(ColumnName = "GuId", IsPrimaryKey = true, IsIdentity = true)]
            public long GuId { get; set; }

            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }


            [SugarColumn(ColumnName = "ShopId")]
            public long ShopId { get; set; }

            [Navigate(NavigateType.OneToOne, nameof(ShopId))]
            public ShopDataTest ShopDataTest { get; set; }
        }
    }
}
