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
            Demo8();
            Demo7();
            Demo6();
            Demo5();
            Demo4();
            Demo3();
            Demo2();
            Demo1();
        }

        private static void Demo5()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NoEqual, FieldValue = "1" });
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNot, FieldValue = null });
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }
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
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }


        }

        private static void Demo2()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
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
            
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }


        }


        private static void Demo3()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Null,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> ( WhereType.And, new ConditionalTree(){
                  ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="ID", ConditionalType=ConditionalType.Equal, FieldValue="1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Or,new ConditionalModel(){
                          FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    })

                  }
                })
            }
            }); 
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }

        }

        private static void Demo4()
        {
            var db = NewUnitTest.Db;
            List<ConditionalTree> conModels = new List<ConditionalTree>();
            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Null,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.And,new ConditionalModel() { FieldName = "name", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> (
                  WhereType.And, new ConditionalTree(){
                      ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>()
                  {
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="price", ConditionalType=ConditionalType.Equal, FieldValue="1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.And,new ConditionalModel(){
                          FieldName = "CustomId", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    })

                  }
                })
            }
            });
            var json = db.Context.Utilities.SerializeObject(conModels);

            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();

            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);

            if (json != json2) 
            {
                throw new Exception("unit error");
            }
        }

        private static void Demo6()
        {
            var db = NewUnitTest.Db;
            List<ConditionalTree> conModels = new List<ConditionalTree>();
            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Null,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.And,new ConditionalModel() { FieldName = "name", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, IConditionalModel> (
                  WhereType.And, new ConditionalTree(){
                      ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>()
                  {
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="price", ConditionalType=ConditionalType.Equal, FieldValue="1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.And,new ConditionalModel(){
                          FieldName = "CustomId", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    })

                  }
                })
            }
            });
            var json = db.Context.Utilities.SerializeObject(conModels);

            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(it=>it.Id==1).Where(conditionalModels).ToList();

            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);

            if (json != json2)
            {
                throw new Exception("unit error");
            }
        }

        private static void Demo7()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
                {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                  new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Null,new ConditionalModel() { FieldName = "name", ConditionalType = ConditionalType.Equal, FieldValue = "name" }),
                
                  new  KeyValuePair<WhereType, IConditionalModel> ( WhereType.And, new ConditionalTree(){
                  ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="customid", ConditionalType=ConditionalType.Equal, FieldValue="1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Or,new ConditionalModel(){
                          FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Or,new ConditionalTree(){
                          ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                              new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="customid", ConditionalType=ConditionalType.Equal, FieldValue="1"
                           }
                              
                              ),
                              
                      }
                    })


                  }
                }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.And,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
            }
            });
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }
        }

        private static void Demo8()
        {
            var db = NewUnitTest.Db;
            List<IConditionalModel> conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalTree()
            {
                ConditionalList = new List<KeyValuePair<WhereType, IConditionalModel>>()//  (id=1 or id=2 and id=1)
                {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                  new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Null,new ConditionalModel() { FieldName = "name", ConditionalType = ConditionalType.Equal, FieldValue = "name" }),

                  new  KeyValuePair<WhereType, IConditionalModel> ( WhereType.And, new ConditionalTree(){
                  ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="customid", ConditionalType=ConditionalType.Equal, FieldValue="1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.And,new ConditionalModel(){
                          FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1"
                    }),
                    new KeyValuePair<WhereType, IConditionalModel>(WhereType.Or,new ConditionalTree(){
                          ConditionalList=new List<KeyValuePair<WhereType, IConditionalModel>>(){
                              new KeyValuePair<WhereType, IConditionalModel>(WhereType.Null,new ConditionalModel(){
                         FieldName="customid", ConditionalType=ConditionalType.Equal, FieldValue="1"
                           }

                              ),

                      }
                    })


                  }
                }),
                new  KeyValuePair<WhereType, IConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
            }
            });
            var json = db.Context.Utilities.SerializeObject(conModels);
            var conditionalModels = db.Context.Utilities.JsonToConditionalModels(json);
            var list6 = db.Queryable<Order>().Where(conditionalModels).ToList();
            var json2 = db.Context.Utilities.SerializeObject(conditionalModels);
            if (json != json2)
            {
                throw new Exception("unit error");
            }
        }
    }
}
