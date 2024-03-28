using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrmTest;
using SqlSugar;

namespace OrmTest 
{
    public class UnitWeek
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;

            for (int i = 0; i < 20; i++)
            {
                var date = DateTime.Now.AddDays(i);
                var id = db.Insertable(new Order()
                {
                    CreateTime = date,
                    CustomId = 1,
                    Name = "a",
                    Price = 1,
                     
                })
               .ExecuteReturnIdentity();

                var data1 = db.Queryable<Order>()
                    .In(id).Select(it => it.CreateTime.DayOfWeek).Single();
                Console.WriteLine(db.Queryable<Order>()
                    .In(id).Select(it => it.CreateTime).Single());
                if (data1 != date.DayOfWeek)
                {
                    throw new Exception("DayOfWeek error");
                }
            } 
            db.CodeFirst.SetStringDefaultLength(255).InitTables<UserInfo>();
            db.DbMaintenance.TruncateTable<UserInfo>();
            db.Insertable<UserInfo>(new UserInfo()
            {
                 UserName = "a",
                  UserType= EnumUserType.UserType
            }).ExecuteCommand();

            var userTypes = db.QueryableWithAttr<UserInfo>().Where(e => e.UserType != null).Select(e => e.UserType).ToList();
            if (userTypes.First() != EnumUserType.UserType) 
            {
                throw new Exception("unit error");
            }

            db.CodeFirst.InitTables<UnitBooladfa>();
            List<bool?> bools = new List<bool?>() { true,false };
            db.Queryable<UnitBooladfa>().Where(it => bools.Contains(it.Bool)).ToList();
            if(!db.Queryable<UnitBooladfa>().Where(it => bools.Contains(it.Bool)).ToSqlString().Contains("1,0"))
            {
                throw new Exception("unit error");
            }
        }
    }
    public class UnitBooladfa 
    {
        public bool? Bool { get; set; }
    }
    [SugarTable("unitaser13231")] 
    public class UserInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, ColumnDescription = "主键", ColumnName = "id", IsNullable = false)]
        public long Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [SugarColumn(ColumnDescription = "用户名", ColumnName = "user", IsNullable = false)]
        public string? UserName { get; set; }

        /// <summary>
        /// 用户类型 (可为空)
        /// </summary>
        [SugarColumn(ColumnDescription = "用户类型", ColumnName = "user_type", IsNullable = true)]
        public EnumUserType? UserType { get; set; }
    }
    public enum EnumUserType 
    {
        UserType=1
    }
}
