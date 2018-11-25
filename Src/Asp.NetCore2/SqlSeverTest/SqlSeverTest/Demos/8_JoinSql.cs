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
            SelectMerge();
            ConditionalModel();
            JoinExp();
            Clone();
        }

        private static void Clone()
        {
            var db = GetInstance();
            var qy = db.Queryable<Student>().Where(it => 1 == 1);

            var list1 = qy.Clone().Where(it => it.Id == 1).ToList();
            var list2 = qy.Clone().Where(it => it.Id == 2).ToList();



            var qy2 = db.Queryable<Student,School>((st,sc)=>new object[]{
                JoinType.Left,st.SchoolId==sc.Id
            }).Where((st,sc)=>st.Id == 1);

            var join0 = qy2.Clone().Where((st, sc) => sc.Id == 222).Select(st=>st.Id).ToList();
            var join1 = qy2.Clone().Where((st,sc) => st.Id== 1111).ToList();
            var join2 = qy2.Clone().Where((st,sc)=>sc.Id==222).ToList();
        }

        private static void JoinExp()
        {
            var db = GetInstance();

            var exp= Expressionable.Create<Student>()
                .OrIF(1==1,it => it.Id == 11)
                .And(it=>it.Id==1)
                .AndIF(2==2,it => it.Id == 1)
                .Or(it =>it.Name == "a1").ToExpression();
            var list=db.Queryable<Student>().Where(exp).ToList();
        }

        private static void ConditionalModel()
        {
            var db = GetInstance();
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" });//id=1
            conModels.Add(new ConditionalModel() { FieldName = "Student.id", ConditionalType = ConditionalType.Equal, FieldValue = "1" });//id=1
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Like, FieldValue = "1" });// id like '%1%'
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNullOrEmpty });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.In,FieldValue="1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NotIn, FieldValue = "1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NoEqual, FieldValue = "1,2,3" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNot,FieldValue=null});// id is not null

            conModels.Add(new ConditionalCollections() { ConditionalList=new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, ConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, ConditionalModel> ( WhereType.And,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" })
            }
            });
            var student = db.Queryable<Student>().Where(conModels).ToList();
        }

        private static void SelectMerge()
        {
            var db = GetInstance();
            //page join
            var pageJoin = db.Queryable<Student, School>((st, sc) => new object[] {
              JoinType.Left,st.SchoolId==sc.Id
            })
            .Where(st => st.Id==1)
            .Where(st => st.Id==2)
            .Select((st, sc) => new { id = st.Id, name = sc.Name })
            .MergeTable().Where(XXX => XXX.id == 1).OrderBy("name asc").ToList();// Prefix, is, not, necessary, and take the columns in select

        }
        private static void Where()
        {
            var db = GetInstance();
            //Parameterized processing
            string value = "'jack';drop table Student";
            var list = db.Queryable<Student>().Where("name=@name", new { name = value }).ToList();
            //Nothing happened
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
