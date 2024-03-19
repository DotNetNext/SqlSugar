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

            Console.WriteLine(sql);

            /// SELECT  `x`.`name` AS `Name` , `u`.`name` AS `BName`  FROM `a` x Left JOIN `b` `u` ON ( `x`.`id` = `u`.`Aid` )


        }
    }

}
