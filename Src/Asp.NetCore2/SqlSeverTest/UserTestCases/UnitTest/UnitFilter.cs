using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrmTest
{
    internal class UnitFilter
    {

        public static void Init() 
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = $"Data Source=unitdffa.db;",
                DbType = DbType.Sqlite,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                MoreSettings = new ConnMoreSettings
                {
                    IsAutoDeleteQueryFilter = true, // 启用删除查询过滤器
                }
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(UtilMethods.GetNativeSql(sql,pars));
                Console.WriteLine();
            };

            //添加过滤器
            db.QueryFilter.AddTableFilter<IDeletedFilter>(u => u.IsDelete == false);
            db.CodeFirst.InitTables<DeviceEntity, DeviceBrandEntity>();

            //导航查询会出问题，提示对象为空
            //如果把第22行代码注释掉就没问题
            var devices = db.Queryable<DeviceEntity>()
                     .IncludeLeftJoin(a => a.DeviceBrand)
                     //.ClearFilter()
                     .Select(a => new
                     {
                         Id = a.Id,
                         Name = a.Name,
                         DeviceBrandId = a.DeviceBrandId,
                         BrandName = a.DeviceBrand.Name,
                     }).ToList();

            var devices2 = db.Queryable<DeviceEntity>("a")
                   
                   .AddJoinInfo(typeof(DeviceBrandEntity),"o", "o.id==a.DeviceBrandId")
                   //.ClearFilter()
                   .Select(a => new
                   {
                       Id = a.Id,
                       Name = a.Name,
                       DeviceBrandId = a.DeviceBrandId,
                       BrandName = a.DeviceBrand.Name,
                   }).ToList();
            db.CodeFirst.InitTables<DeviceEntity2, DeviceBrandEntity2>();
            var list= db.Queryable<DeviceEntity2, DeviceBrandEntity2>((x, y) => 
                              new JoinQueryInfos(JoinType.Left,x.Id==y.Id))
                .ToList();

            var list2 = db.Queryable<DeviceEntity2, DeviceBrandEntity2>((x, y) =>
                            x.Id == y.Id )
            .Select(x=>new DeviceEntity2
            { 
              Id=x.Id
            })
            .ToList();
        }
        /// <summary>
        /// 仪器
        /// </summary>
        [SugarTable("Device")]
        public class DeviceEntity : IDeletedFilter
        {
            /// <summary>
            /// id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 仪器品牌id
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public string DeviceBrandId { get; set; }

            /// <summary>
            /// 设备品牌
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(DeviceBrandId))]
            public DeviceBrandEntity DeviceBrand { get; set; }

            /// <summary>
            /// 是否删除
            /// </summary>
            public bool IsDelete { get; set; }
        }


        /// <summary>
        /// 仪器
        /// </summary>
        [SugarTable("Device2")]
        public class DeviceEntity2 : IDeletedFilter
        {
            /// <summary>
            /// id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 仪器品牌id
            /// </summary>
            [SugarColumn(IsNullable = true)]
            public string DeviceBrandId { get; set; }

            /// <summary>
            /// 设备品牌
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(DeviceBrandId))]
            public DeviceBrandEntity DeviceBrand { get; set; }

            /// <summary>
            /// 是否删除
            /// </summary>
            [SugarColumn(ColumnName = "Is_Delete")]
            public bool IsDelete { get; set; }
        }

        internal interface IDeletedFilter
        {
            /// <summary>
            /// 假删除
            /// </summary>
            bool IsDelete { get; set; }
        }

        /// <summary>
        /// 仪器品牌
        /// </summary>
        [SugarTable("DeviceBrand")]
        public class DeviceBrandEntity
        {
            /// <summary>
            /// id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// 仪器品牌
        /// </summary>
        [SugarTable("DeviceBrand2")]
        public class DeviceBrandEntity2:IDeletedFilter
        {
            /// <summary>
            /// id
            /// </summary>
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }
            /// <summary>
            /// 名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 是否删除
            /// </summary>
            [SugarColumn(ColumnName = "Is_Delete")]
            public bool IsDelete { get; set; }
        }


    }
}
