using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UCustomConditionalFunc
    {
        public static void Init()
        {
            var conditionEmail = new ConditionalModel { FieldName = "Email", ConditionalType = ConditionalType.Like, FieldValue = "aaa" };
            var conditionUserType = new ConditionalModel { FieldName = "UserType", FieldValue = "1", CustomConditionalFunc = new UserTypeConditionalFunc() };

            var db = NewUnitTest.Db;
            Demo1(conditionEmail, conditionUserType, db);
            Demo2(conditionEmail, conditionUserType, db);
            Demo3(conditionEmail, conditionUserType, db);
        }

        private static void Demo1(ConditionalModel conditionEmail, ConditionalModel conditionUserType, SqlSugarClient db)
        {
            var conditions = new List<IConditionalModel>();
            conditions.Add(conditionEmail);
            conditions.Add(new ConditionalCollections
            {
                ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>
                    {
                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, conditionUserType)  // 这里key用[Or|Null]都没用
                    }
            });
            db.CodeFirst.InitTables<AppUser>();
            var query = db.Queryable<AppUser>().Where(conditions, true).ToList();
        }
        private static void Demo2(ConditionalModel conditionEmail, ConditionalModel conditionUserType, SqlSugarClient db)
        {
            var conditions = new List<IConditionalModel>();
            conditions.Add(conditionEmail);
            conditions.Add(new ConditionalCollections
            {
                ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>
                    {
                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.Or, conditionUserType)  // 这里key用[Or|Null]都没用
                    }
            });
            db.CodeFirst.InitTables<AppUser>();
            var query = db.Queryable<AppUser>().Where(conditions, true).ToList();
        }
        private static void Demo3(ConditionalModel conditionEmail, ConditionalModel conditionUserType, SqlSugarClient db)
        {
            var conditions = new List<IConditionalModel>();
         //   conditions.Add(conditionEmail);
            conditions.Add(new ConditionalCollections
            {
                ConditionalList = new List<KeyValuePair<WhereType, ConditionalModel>>
                    {
                                   new KeyValuePair<WhereType, ConditionalModel>(WhereType.And, conditionUserType)  // 这里key用[Or|Null]都没用
                    }
            });
            db.CodeFirst.InitTables<AppUser>();
            var query = db.Queryable<AppUser>().Where(conditions, true).ToList();
        }
    }
    [SugarTable("AppUseradfafa")]
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public int UserType { get; set; }
    }

    public class UserTypeConditionalFunc : ICustomConditionalFunc
    {
        public KeyValuePair<string, SugarParameter[]> GetConditionalSql(ConditionalModel conditionalModel, int index)
        {
            var sql = $"UserType = {conditionalModel.FieldValue}";
            return new KeyValuePair<string, SugarParameter[]>(sql, Array.Empty<SugarParameter>());
        }
    }
}
