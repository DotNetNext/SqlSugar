using SqlSugar;
using System;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitDynamicCoread12321
    {
        /// <summary>
        /// 管理系统角色
        ///</summary>
        [SugarTable("a", IsDisabledUpdateAll = true)]
        public class AT
        {

            /// <summary>
            /// 角色名称 
            ///</summary>
            [SugarColumn(ColumnName = "id")]
            public long Id { get; set; }

            /// <summary>
            /// 角色名称 
            ///</summary>
            [SugarColumn(ColumnName = "name", Length = 100)]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 角色描述
            ///</summary>
            [SugarColumn(ColumnName = "desc", Length = 100)]
            public string Desc { get; set; } = string.Empty;

            public string BName { get; set; }

        }

        /// <summary>
        /// 管理系统角色
        ///</summary>
        [SugarTable("b", IsDisabledUpdateAll = true)]
        public class BT
        {

            /// <summary>
            /// 角色名称 
            ///</summary>
            [SugarColumn(ColumnName = "id")]
            public long Id { get; set; }


            /// <summary>
            /// 角色名称 
            ///</summary>
            [SugarColumn(ColumnName = "aid")]
            public long Aid { get; set; }

            /// <summary>
            /// 角色名称 
            ///</summary>
            [SugarColumn(ColumnName = "name", Length = 100)]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            /// 角色描述
            ///</summary>
            [SugarColumn(ColumnName = "desc", Length = 100)]
            public string Desc { get; set; } = string.Empty;

        }
        public class UnitInsetSql
        {
            [SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int Id { get; set; }
            [SugarColumn(InsertSql ="cast('{0}' as varchar(100))", UpdateSql = "cast('{0}' as varchar(200))")] 
            public string Name { get; set; } 
        }

        public static  void Init()
        {

            StaticConfig.DynamicExpressionParserType = typeof(DynamicExpressionParser);
            var db = NewUnitTest.Db;
             

            var shortNames = DynamicParameters.Create("x", typeof(AT), "u", typeof(BT));
            var sql = db.QueryableByObject(typeof(AT), "x")
                .AddJoinInfo(typeof(BT),
                    shortNames,
                    $"x.Id==u.Aid",
                    JoinType.Left)
                .Select(shortNames, $"new (x.Name as Name,u.Name as BName)", typeof(AT))
                .ToSql().Key;
            if (sql.Contains( "Aid")) 
            {
                throw new Exception("unit error");
            }
            
         

            var shortNames2 = DynamicParameters.Create("x", typeof(OrderItem), "u", typeof(OrderItem),"u2", typeof(OrderItem));
            var sql2 = db.QueryableByObject(typeof(OrderItem), "x")
                 .AddJoinInfo(typeof(OrderItem), DynamicParameters.Create("x", typeof(OrderItem), "u", typeof(OrderItem)), $"x.OrderId==u.OrderId", JoinType.Left)
                  .AddJoinInfo(typeof(OrderItem), DynamicParameters.Create("x", typeof(OrderItem), "u", typeof(OrderItem), "u2", typeof(OrderItem)), $"x.OrderId==u2.OrderId", JoinType.Left)
                 // .Select(shortNames, $"new (x.Name as Name,u.Name as BName)", typeof(AT))
                .ToSql().Key;
            if (sql2.Trim()!=("SELECT [x].[ItemId],[x].[OrderId],[x].[Price],[x].[CreateTime] FROM [OrderDetail] [x] Left JOIN [OrderDetail] [u] ON ( [x].[OrderId] = [u].[OrderId] )  Left JOIN [OrderDetail] [u2] ON ( [x].[OrderId] = [u2].[OrderId] )"))
            {
                throw new Exception("unit error");
            }

            db.CodeFirst.InitTables<UnitInsetSql>();
            db.Insertable(new UnitInsetSql()).ExecuteCommand();
            db.Updateable(new UnitInsetSql() { Id=1}).ExecuteCommand();
            Console.WriteLine(sql);

            /// SELECT  `x`.`name` AS `Name` , `u`.`name` AS `BName`  FROM `a` x Left JOIN `b` `u` ON ( `x`.`id` = `u`.`Aid` )


        }
    }

}
