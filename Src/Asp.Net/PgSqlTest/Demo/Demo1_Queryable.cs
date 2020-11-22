using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace OrmTest
{
    public class Demo1_Queryable
    {

        public static void Init()
        {
            EasyExamples();
            QueryConditions();
            JoinTable();
            Async();
            NoEntity();
            Mapper();
            SqlFuncTest();
            Subquery();
            ReturnType();
        }

        private static void EasyExamples()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Examples Start ####");
            var db = GetInstance();
            var dbTime = db.GetDate();
            var getAll = db.Queryable<Order>().ToList();
            var getOrderBy = db.Queryable<Order>().OrderBy(it => it.Name,OrderByType.Desc).ToList();
            var getOrderBy2 = db.Queryable<Order>().OrderBy(it => it.Id).OrderBy(it => it.Name, OrderByType.Desc).ToList();
            var getOrderBy3 = db.Queryable<Order>().OrderBy(it =>new { it.Name,it.Id}).ToList();
            var getRandom = db.Queryable<Order>().OrderBy(it => SqlFunc.GetRandom()).First();
            var getByPrimaryKey = db.Queryable<Order>().InSingle(2);
            var getSingleOrDefault = db.Queryable<Order>().Where(it => it.Id == 1).Single();
            var getFirstOrDefault = db.Queryable<Order>().First();
            var getByWhere = db.Queryable<Order>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByWhere2 = db.Queryable<Order>().Where(it => it.Id == DateTime.Now.Year).ToList();
            var getByFuns = db.Queryable<Order>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
            var getByFuns2 = db.Queryable<Order>().GroupBy(it => it.Name).Select(it => SqlFunc.AggregateDistinctCount(it.Price)).ToList();
            Console.WriteLine("#### Examples End ####");
        }

        private static void ReturnType()
        {
            Console.WriteLine("");
            Console.WriteLine("#### ReturnType Start ####");
            var db = GetInstance();
            List<Order> list = db.Queryable<Order>().ToList();

            Order item = db.Queryable<Order>().First(it => it.Id == 1);

            DataTable dataTable = db.Queryable<Order>().Select(it => it.Id).ToDataTable();

            var json = db.Queryable<Order>().ToJson();

            List<int> listInt = db.Queryable<Order>().Select(it => it.Id).ToList();

            var dynamic = db.Queryable<Order>().Select<dynamic>().ToList();

            var viewModel = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
                    JoinType.Left, o.Id == i.OrderId  ,
                    JoinType.Left, o.CustomId == c.Id 
                ))
                .Select<ViewOrder>().ToList();

            var newDynamic = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
                   JoinType.Left, o.Id == i.OrderId,
                   JoinType.Left, o.CustomId == c.Id
               ))
                .Select((o, i, c) => new { orderName = o.Name, cusName=c.Name }).ToList();

            var newClass = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
                   JoinType.Left, o.Id == i.OrderId,
                   JoinType.Left, o.CustomId == c.Id
               ))
                .Select((o, i, c) => new ViewOrder {  Name=o.Name,  CustomName=c.Name }).ToList();


            var oneClass = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
              JoinType.Left, o.Id == i.OrderId,
              JoinType.Left, o.CustomId == c.Id
            ))
           .Select((o, i, c) => c).ToList();

            var twoClass = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
            JoinType.Left, o.Id == i.OrderId,
            JoinType.Left, o.CustomId == c.Id
            ))
           .Select((o, i, c) => new { o,i}).ToList();

            List<Dictionary<string, object>> ListDic = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
                  JoinType.Left, o.Id == i.OrderId,
                  JoinType.Left, o.CustomId == c.Id
                ))
                .Select<ExpandoObject>().ToList().Select(it => it.ToDictionary(x => x.Key, x => x.Value)).ToList();
            Console.WriteLine("#### ReturnType End ####");
        }

        private static void Subquery()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Subquery Start ####");
            var db = GetInstance();

            var list = db.Queryable<Order>().Take(10).Select(it => new
            {
                customName=SqlFunc.Subqueryable<Custom>().Where("it.CustomId=id").Select(s=>s.Name),
                customName2 = SqlFunc.Subqueryable<Custom>().Where("it.CustomId = id").Where(s => true).Select(s => s.Name)
            }).ToList();

            var list2 = db.Queryable<Order>().Where(it => SqlFunc.Subqueryable<OrderItem>().Where(i => i.OrderId == it.Id).Any()).ToList();

            Console.WriteLine("#### Subquery End ####");
        }

        private static void SqlFuncTest()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlFunc Start ####");
            var db = GetInstance();
            var index= db.Queryable<Order>().Select(it => SqlFunc.Contains("a", "cccacc")).First();
            var list = db.Queryable<Order>().Select(it => new ViewOrder()
            {

                Id = SqlFunc.AggregateSum(SqlFunc.IF(it.Id > 0).Return(1).End(0))
            }).ToList();
            var list2 = db.Queryable<Order>().Select(it => new
            {
                date = SqlFunc.ToDateShort(it.CreateTime),
                datetime = SqlFunc.ToDate(it.CreateTime)
            }).ToList();
            Console.WriteLine("#### SqlFunc  End ####");
        }

        private static void Mapper()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Mapper Start ####");
            var db = GetInstance();
            //Creater Table
            db.CodeFirst.InitTables(typeof(Tree));
            db.DbMaintenance.TruncateTable("tree");
            db.Insertable(new Tree() { Id = 1, Name = "root" }).ExecuteCommand();
            db.Insertable(new Tree() { Id = 11, Name = "child1",ParentId=1 }).ExecuteCommand();
            db.Insertable(new Tree() { Id = 12, Name = "child2",ParentId=1 }).ExecuteCommand();
            db.Insertable(new Tree() { Id = 2, Name = "root" }).ExecuteCommand();
            db.Insertable(new Tree() { Id = 22, Name = "child3", ParentId = 2 }).ExecuteCommand();

            // Same property name mapping,Both entities have parentId
            var list = db.Queryable<Tree>().Mapper(it => it.Parent, it => it.ParentId).ToList();


            //If both entities have parentId, I don't want to associate with parentId.
            var list1 =db.Queryable<Tree>()
                                     //parent=(select * from parent where id=it.parentid)
                                     .Mapper(it=>it.Parent,it=>it.ParentId, it=>it.Parent.Id)
                                     //Child=(select * from parent where ParentId=it.id)
                                     .Mapper(it => it.Child, it => it.Id, it => it.Parent.ParentId)
                                     .ToList();
            //one to one
            var list2 = db.Queryable<OrderItemInfo>().Mapper(it => it.Order, it => it.OrderId).ToList();

            //one to many
            var list3 = db.Queryable<Order>().Mapper(it => it.Items, it => it.Items.First().OrderId).ToList();

            //many to many
            db.CodeFirst.InitTables<A, B, ABMapping>();

            db.Insertable(new A() { Name = "A" }).ExecuteCommand();
            db.Insertable(new B() { Name = "B" }).ExecuteCommand();
            db.Insertable(new ABMapping() { AId = 1, BId = 1 }).ExecuteCommand();

            var  list4 = db.Queryable<ABMapping>()
              .Mapper(it => it.A, it => it.AId)
              .Mapper(it => it.B, it => it.BId).ToList();

            //Manual mode
            var result = db.Queryable<OrderInfo>().Take(10).Select<ViewOrder>().Mapper((itemModel, cache) =>
            {
                var allItems = cache.Get(orderList => {
                    var allIds = orderList.Select(it => it.Id).ToList();
                    return db.Queryable<OrderItem>().Where(it => allIds.Contains(it.OrderId)).ToList();//Execute only once
                });
                itemModel.Items = allItems.Where(it => it.OrderId==itemModel.Id).ToList();//Every time it's executed
            }).ToList();

            Console.WriteLine("#### End Start ####");
        }

        private static void NoEntity()
        {
            Console.WriteLine("");
            Console.WriteLine("#### No Entity Start ####");
            var db = GetInstance();

            var list = db.Queryable<dynamic>().AS("order").Where("id=id", new { id = 1 }).ToList();

            var list2 = db.Queryable<dynamic>("o").AS("order").AddJoinInfo("OrderDetail", "i", "o.id=i.OrderId").Where("id=id", new { id = 1 }).Select("o.*").ToList();
            Console.WriteLine("#### No Entity End ####");
        }

        private static void JoinTable()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Join Table Start ####");
            var db = GetInstance();

            //Simple join
            var list = db.Queryable<Order, OrderItem, Custom>((o, i, c) => o.Id == i.OrderId&&c.Id == o.CustomId)
                         .Select<ViewOrder>()
                         .ToList();

            //Join table
            var list2 = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
             JoinType.Left, o.Id == i.OrderId,
             JoinType.Left, c.Id == o.CustomId
            ))
           .Select<ViewOrder>().ToList();

            //Join queryable
            var query1 = db.Queryable<Order, OrderItem>((o, i) => new JoinQueryInfos(
              JoinType.Left, o.Id == i.OrderId
            ))
            .Where(o => o.Name == "jack");

            var query2 = db.Queryable<Custom>();
            var list3=db.Queryable(query1, query2,JoinType.Left, (p1, p2) => p1.CustomId == p2.Id).Select<ViewOrder>().ToList();

            Console.WriteLine("#### Join Table End ####");
        }

        private static void QueryConditions()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Query Conditions Start ####");

            SqlSugarClient db = GetInstance();

            /*** By expression***/

            //id=@id
            var list = db.Queryable<Order>().Where(it => it.Id == 1).ToList();
            //id=@id or name like '%'+@name+'%'
            var list2 = db.Queryable<Order>().Where(it => it.Id == 1 || it.Name.Contains("jack")).ToList();


            //Create expression 
            var exp = Expressionable.Create<Order>()
                            .And(it => it.Id == 1)
                            .Or(it => it.Name.Contains("jack")).ToExpression();
            var list3 = db.Queryable<Order>().Where(exp).ToList();


            /*** By sql***/

            //id=@id
            var list4 = db.Queryable<Order>().Where("id=@id", new { id = 1 }).ToList();
            //id=@id or name like '%'+@name+'%'
            var list5 = db.Queryable<Order>().Where("id=@id or name like @name ", new { id = 1, name = "%jack%" }).ToList();



            /*** By dynamic***/

            //id=1
            var conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" , FieldValueConvertFunc=it=>Convert.ToInt32(it) });//id=1
            var student = db.Queryable<Order>().Where(conModels).ToList();

            //Complex use case
            List<IConditionalModel> Order = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1", FieldValueConvertFunc = it => Convert.ToInt32(it) });//id=1
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Like, FieldValue = "1", FieldValueConvertFunc = it => Convert.ToInt32(it) });// id like '%1%'
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNullOrEmpty });
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.In, FieldValue = "1,2,3" });
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NotIn, FieldValue = "1,2,3" });
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.NoEqual, FieldValue = "1,2,3" });
            //conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.IsNot, FieldValue = null });// id is not null

            conModels.Add(new ConditionalCollections()
            {
                ConditionalList = new List<KeyValuePair<WhereType, SqlSugar.ConditionalModel>>()//  (id=1 or id=2 and id=1)
            {
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, ConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" , FieldValueConvertFunc = it => Convert.ToInt32(it) }),
                new  KeyValuePair<WhereType, ConditionalModel> ( WhereType.And,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" ,FieldValueConvertFunc = it => Convert.ToInt32(it)})
            }
            });
            var list6 = db.Queryable<Order>().Where(conModels).ToList();

            /*** Conditional builder ***/

            // use whereif
            string name = "";
            int id = 1;
            var query = db.Queryable<Order>()
                            .WhereIF(!string.IsNullOrEmpty(name), it => it.Name.Contains(name))
                            .WhereIF(id > 0, it => it.Id == id).ToList();
            //clone new Queryable
            var query2 = db.Queryable<Order>().Where(it => it.Id == 1);
            var list7 = query2.Clone().Where(it => it.Name == "jack").ToList();//id=1 and name = jack
            var list8 = query2.Clone().Where(it => it.Name == "tom").ToList();//id=1 and name = tom

            Console.WriteLine("#### Condition Screening End ####");



        }

        private static void Async()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Async Start ####");

            SqlSugarClient db = GetInstance();
            var task1 = db.Queryable<Order>().FirstAsync();
            task1.Wait();
            var task2 = db.Queryable<Order>().Where(it => it.Id == 1).ToListAsync();

        
            task2.Wait();

            Console.WriteLine("#### Async End ####");
        }

        private static SqlSugarClient GetInstance()
        {
            return new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.PostgreSQL,
                ConnectionString = Config.ConnectionString,
                InitKeyType = InitKeyType.Attribute,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(sql);
                        Console.WriteLine(string.Join(",", p?.Select(it => it.ParameterName + ":" + it.Value)));
                    }
                }
            });
        }
    }
}
