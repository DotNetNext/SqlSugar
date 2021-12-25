using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class UCustom03
    {
        public static void Init()
        {
            Demo1();
        }

        private static void Demo1()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NoEqual, FieldValue = "1" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNot, FieldValue = null });// id is not null

            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> ( WhereType.And, new ConditionalTree(){
                  ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.And,new ConditionalModel(){
                          FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    })

                  }
                })
            }
            });
            var list6 =db .Queryable<Order>().Where(conModels).ToList();
        }
    }
}
