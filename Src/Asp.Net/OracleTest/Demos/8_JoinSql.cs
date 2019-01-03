using OrmTest.Demo;
using OrmTest.Models;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrmTest.Demo
{
    /// <summary>
    /// Secure string operations
    /// </summary>
    public class JoinSql : DemoBase
    {
        public static void Init()
        {
            Where();
            OrderBy();
            ConditionalModel();
        }

        private static void Where()
        {
            var db = GetInstance();
            //Parameterized processing
            string value = "'jack';drop table Student";
            var list = db.Queryable<Student>().Where("name=:name", new { name = value }).ToList();
            //Nothing happened
        }
        private static void ConditionalModel()
        {
            var db = GetInstance();
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "createTime", ConditionalType = ConditionalType.Equal, FieldValue = DateTime.Now.ToString(), FieldValueConvertFunc=it=>Convert.ToDateTime(it) });
            conModels.Add(new ConditionalModel() { FieldName = "createTime", ConditionalType = ConditionalType.GreaterThan, FieldValue = DateTime.Now.ToString(), FieldValueConvertFunc = it => Convert.ToDateTime(it) });
            conModels.Add(new ConditionalModel() { FieldName = "createTime", ConditionalType = ConditionalType.GreaterThanOrEqual, FieldValue = DateTime.Now.ToString(), FieldValueConvertFunc = it => Convert.ToDateTime(it) });
            conModels.Add(new ConditionalModel() { FieldName = "createTime", ConditionalType = ConditionalType.LessThan, FieldValue = DateTime.Now.ToString(), FieldValueConvertFunc = it => Convert.ToDateTime(it) });
            conModels.Add(new ConditionalModel() { FieldName = "Student.id", ConditionalType = ConditionalType.LessThanOrEqual, FieldValue = "1" ,FieldValueConvertFunc= it=>Convert.ToInt32(it)});//id=1
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Like, FieldValue = "1" });// id like '%1%'
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNullOrEmpty });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.In, FieldValue = "1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NotIn, FieldValue = "1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NoEqual, FieldValue = "1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNot, FieldValue = null });// id is not null

            conModels.Add(new ConditionalCollections()
            {
                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, ConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, ConditionalModel> ( WhereType.And,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" })
            }
            });
            var student = db.Queryable<Student>().Where(conModels).ToList();
        }

        private static void OrderBy()
        {
            var db = GetInstance();
            //propertyName is valid
            string propertyName = "Id";
            string dbColumnName = db.EntityMaintenance.GetDbColumnName<Student>(propertyName);
            var list = db.Queryable<Student>().OrderBy(dbColumnName).ToList();

            //propertyName is invalid
            try
            {
                propertyName = "Id'";
                dbColumnName = db.EntityMaintenance.GetDbColumnName<Student>(propertyName);
                var list2 = db.Queryable<Student>().OrderBy(dbColumnName).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
