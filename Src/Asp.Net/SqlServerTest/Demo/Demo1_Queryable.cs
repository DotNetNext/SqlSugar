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
            ConfiQuery();
        }

        private static void ConfiQuery()
        {
            var db = GetInstance();
            List<DataDictionary> datas = new List<DataDictionary>();
            datas.Add(new DataDictionary() { Code="1", Name="男",Type="sex" });
            datas.Add(new DataDictionary() { Code = "2", Name = "女", Type = "sex" });
            datas.Add(new DataDictionary() { Code = "1", Name = "南通市", Type = "city" });
            datas.Add(new DataDictionary() { Code = "2", Name = "苏州市", Type = "city" });
            datas.Add(new DataDictionary() { Code = "1", Name = "江苏省", Type = "province" });
            datas.Add(new DataDictionary() { Code = "2", Name = "湖南省", Type = "province" });
            db.CodeFirst.InitTables<DataDictionary>();
            db.CodeFirst.InitTables<Person>();
            db.DbMaintenance.TruncateTable<DataDictionary>();
            db.Insertable(datas).ExecuteCommand();

            if (!db.ConfigQuery.Any()) 
            {
                var types= db.Queryable<DataDictionary>().Select(it => it.Type).Distinct().ToList();
                foreach (var type in types)
                {
                    db.ConfigQuery.SetTable<DataDictionary>(it => it.Code, it => it.Name, type, it => it.Type == type);
                }

                db.ConfigQuery.SetTable<Order>(it => it.Id, it => it.Name, "01", it => it.Id > 1);
                db.ConfigQuery.SetTable<Order>(it => it.Id, it => it.Name, "02", it => it.Id > 2);
                db.ConfigQuery.SetTable<Order>(it => it.Id, it => it.Name, null);
            }


            var res=db.Queryable<Person>().Select(it => new Person()
            {
                 Id=it.Id.SelectAll(),
                 SexName=it.SexId.GetConfigValue<DataDictionary>("sex"),
                 ProviceName = it.SexId.GetConfigValue<DataDictionary>("province"),
                 CityName = it.SexId.GetConfigValue<DataDictionary>("city"),
            }).ToList();//也支持支持写在Where或者Orderby
 
            var list = db.Queryable<OrderItem>().Select(it => new OrderItem
            {
                ItemId = it.ItemId.SelectAll(),
                OrderName = it.OrderId.GetConfigValue<Order>("01")
            }).ToList();
            var list2 = db.Queryable<OrderItem>().Select(it => new OrderItem
            {
                ItemId = it.ItemId.SelectAll(),
                OrderName = it.OrderId.GetConfigValue<Order>("02")
            }).ToList();
            var list3 = db.Queryable<OrderItem>().Select(it => new OrderItem
            {
                ItemId = it.ItemId.SelectAll(),
                OrderName = it.OrderId.GetConfigValue<Order>()
            }).ToList();

            var list4 = db.Queryable<OrderItem>().Select(it => new OrderItem
            {
                ItemId = it.ItemId.SelectAll(),
                OrderName = it.OrderId.GetConfigValue<Order>()
            })
            .Where(it=>it.OrderId.GetConfigValue<Order>()== "order1")
            .OrderBy(it=>it.OrderId.GetConfigValue<Order>()).ToList();

            var list5 = db.Queryable<Order, OrderItem>((o, i) => o.Id == i.OrderId)
                        .OrderBy((o,i)=>i.OrderId.GetConfigValue<Order>(),OrderByType.Desc)
                        .Select<ViewOrder>((o,i)=>new ViewOrder() { 
                           Id= o.Id.SelectAll(),
                           Name=i.OrderId.GetConfigValue<Order>()
                        })
                        .ToList();
        }

        private static void EasyExamples()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Examples Start ####");
            var db = GetInstance();
            var dbTime = db.GetDate();
            var getAll = db.Queryable<Order>().Where(it=> SqlFunc.EqualsNull(it.Name,null)).ToList();
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
            var getDicionary = db.Queryable<Order>().ToDictionary(it => it.Id, it => it.Name);
            var getDicionaryList = db.Queryable<Order>().ToDictionaryList();
            var getTest = db.Queryable<Order>().Where(it =>string.IsNullOrWhiteSpace( it.Name)).ToList();
            var test01 = db.Queryable<Order>().PartitionBy(it => it.Id).ToList();
            var q1 = db.Queryable<Order>().Take(1);
            var q2 = db.Queryable<Order>().Take(2);
            var test02 = db.Union(q1, q2).ToList();
            var test03 = db.Queryable<Order>().Take(1).ToList();
            var dp = DateTime.Now;
            var test05 = db.Queryable<Order>().Where(it => it.CreateTime.Month==  dp.Month).ToList();
            var test06 = db.Queryable<Order>()
                   .ToPivotTable(it => it.Id, it => it.Name, it => it.Sum(x => x.Price));

            var test07 = db.Queryable<Order>()
            .ToPivotList(it => it.Id, it => it.Name, it => it.Sum(x => x.Price));

            var test08 = db.Queryable<Order>()
            .ToPivotJson(it => it.Id, it => it.Name, it => it.Sum(x => x.Price));

            var test09 = db.Queryable<Order>().PartitionBy(it=>it.Id).ToPageListAsync(1,2,0);
            test09.Wait();

            int c = 0;
            var test10 = db.Queryable<Order>().ToPageList(1, 2, ref c);
            var test11 = db.Queryable<Order>().GroupBy(it=>new { it.CreateTime.Year }).Select(it=>it.CreateTime.Year).ToList();
            var test12 = db.Queryable<Order>().GroupBy(it =>  it.CreateTime.Date ).Select(it => it.CreateTime.Date).ToList();
            var test13 = db.Queryable<Order>().GroupBy(it => new { it.CreateTime.Date ,it.CreateTime.Year,it.CreateTime.Minute })
                .Select(it => new { it.CreateTime.Date, it.CreateTime.Year, it.CreateTime.Minute }).ToList();
            var test14 = db.Queryable<Order>()
                .GroupBy(it =>   it.CreateTime.Year )
                 .GroupBy(it => it.CreateTime.Second)
                     .GroupBy(it => it.CreateTime.Date)
                .Select(it => new {
                    it.CreateTime.Year,
                    it.CreateTime.Second,
                    it.CreateTime.Date
                }).ToList();
            var test15 = db.Queryable<Order, Order>((o, i) => new JoinQueryInfos(
              JoinType.Left, o.Name == SqlFunc.ToString(SqlFunc.MergeString(",", i.Name, ","))
            ))
            .Select<ViewOrder>().ToList();
            var test16 = db.Queryable<Order>().Select(it => SqlFunc.SqlServer_DateDiff("day", DateTime.Now.AddDays(-1), DateTime.Now)).ToList();
            var test17 =  
               db.Queryable<Order>()
               .Select<Order>()
               .MergeTable()
              .Select(it => new ViewOrder()
              {
                  Name = SqlFunc.Subqueryable<Order>().Select(s => s.Name)
              }).ToList(); ;
            var test18 = db.UnionAll(
               db.Queryable<Order>() ,
               db.Queryable<Order>() 
              ) 
              .Select(it=>new ViewOrder(){ 
                  Name=SqlFunc.Subqueryable<Order>().Select(s=>s.Name)
               }).ToList();
            var test19 = db.Queryable<Order>().Select<ViewOrder>().ToList();
            var test20 = db.Queryable<Order>().LeftJoin<Custom>((o, cs) =>o.Id==cs.Id)
                .ToDictionary(it => it.Id, it => it.Name);

            var test21 = db.Queryable<Order>().Where(it=>it.Id.ToString()==1.ToString()).Select(it => it.CreateTime.ToString("24")).First();
            Console.WriteLine("#### Examples End ####");
        }

        private static void ReturnType()
        {
            Console.WriteLine("");
            Console.WriteLine("#### ReturnType Start ####");
            var db = GetInstance();
            List<Order> list = db.Queryable<Order>().ToList();

            var x2=db.Ado.SqlQueryAsync<Order>("select * from [Order] ");
            x2.Wait();
            var x22 = db.Ado.GetScalarAsync("select * from [Order] ");
            x22.Wait();
            var x222 = db.Ado.ExecuteCommandAsync("select * from [Order] ");
            x222.Wait();
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

            var list1 = db.Queryable<Order>().Select(it => new
            {
                id = SqlFunc.Subqueryable<Custom>().Where(s => s.Id == 1).Sum(s => s.Id) * 1
            }).ToList();

            var list2 = db.Queryable<Order>().Where(it =>
            SqlFunc.Subqueryable<OrderItem>() 
             .LeftJoin<OrderItem>((i,z)=>i.ItemId==z.ItemId)
             .InnerJoin<OrderItem>((i,z,y) => i.ItemId == z.ItemId)
             .InnerJoin<OrderItem>((i,z,y,h) => i.ItemId == z.ItemId)
             .InnerJoin<OrderItem>((i, z, y, h, n) => i.ItemId == z.ItemId)
             .Where((i, z) => i.ItemId == z.ItemId)
             .Any()
            ).ToList();
 
            var list3 = db.Queryable<Order>().Select(it => new
            {
                customName = SqlFunc.Subqueryable<Custom>().Where(s=>s.Id==it.CustomId).GroupBy(s=>s.Name).Having(s=>SqlFunc.AggregateCount(s.Id)>0).Select(s => s.Name) 
            }).ToList();


            var exp = Expressionable.Create<Custom>().And(s => s.Id==1).ToExpression();
            var list4 = db.Queryable<Order>().Select(it => new
            {
                customName = SqlFunc.Subqueryable<Custom>().Where(exp).Where(exp).GroupBy(s => s.Name).Having(s => SqlFunc.AggregateCount(s.Id) > 0).Select(s => s.Name)
            }).ToList();


            var list5 = db.Queryable<Order>().Where(it =>
        SqlFunc.Subqueryable<OrderItem>()
         .LeftJoin<OrderItem>((i, y) => i.ItemId == y.ItemId)
         .InnerJoin<OrderItem>((i, z) => i.ItemId == z.ItemId)
         .Where(i => i.ItemId == 1)
          .Any()
        ).ToList();
            Console.WriteLine("#### Subquery End ####");
        }

        private static void SqlFuncTest()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlFunc Start ####");
            var db = GetInstance();
            var index= db.Queryable<Order>().Select(it => SqlFunc.CharIndex("a", "cccacc")).First();
            var list = db.Queryable<Order>().Select(it =>new ViewOrder()
            {

                Id = SqlFunc.AggregateSum(SqlFunc.IF(it.Id > 0).Return(1).End(0))
            }).ToList();
            var list2 = db.Queryable<Order>().Where(it=>it.CreateTime.Date==it.CreateTime).Select(it => new
            {
                date = it.CreateTime.Date,
                datetime = DateTime.Now.Date
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


            db.Insertable(new Tree() { Id = 222, Name = "child11", ParentId = 11 }).ExecuteCommand();
            var tree = db.Queryable<Tree>().ToTree(it => it.Child, it => it.ParentId, 0); 
            var allchilds= db.Queryable<Tree>().ToChildList(it => it.ParentId, 0);
            var allchilds1 = db.Queryable<Tree>().ToChildList(it => it.ParentId, 1);
            var allchilds2= db.Queryable<Tree>().ToChildList(it => it.ParentId, 2);
            var parentList = db.Queryable<Tree>().ToParentList(it => it.ParentId, 22);
            var parentList2 = db.Queryable<Tree>().ToParentList(it => it.ParentId, 222);
            var parentList3 = db.Queryable<Tree>().ToParentList(it => it.ParentId, 2);

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

            var list = db.Queryable<dynamic>().AS("order ").Where("id=id", new { id = 1 }).ToList();

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


            var query3 = db.Union(
                                   db.Queryable<Order>().Where(it => it.Name.Contains("a")), 
                                   db.Queryable<Order>().Where(it => it.Name.Contains("b"))
                                 ).ToList();



            var query4 = db.Queryable<Order,OrderItem,Custom>(
                              db.Queryable<Order>().Where(it => it.Name.Contains("a")),
                              db.Queryable<OrderItem>().Where(it => it.CreateTime>DateTime.Now),
                              db.Queryable<Custom>().Where(it => it.Name.Contains("b")),
                              JoinType.Left, (o, i, c) => o.Id==i.OrderId,
                              JoinType.Left,(o,i,c)=>o.CustomId==c.Id

                            ).Select(o=>o).ToList();


            var query5 = db.Queryable<Order>()
                            .InnerJoin<Custom>((o, cus) => o.CustomId == cus.Id)
                            .InnerJoin<OrderItem>((o, cus, oritem) => o.Id == oritem.OrderId)
                            .Where((o) => o.Id == 1)
                            .Select((o, cus) => new ViewOrder {  Id=o.Id, CustomName = cus.Name })
                            .ToList();

            var query6 = db.Queryable(db.Queryable<Order>()).LeftJoin<OrderItem>((m, i) => m.Id == i.OrderId)
                .ToList();


            var query7 = db.Queryable(db.Queryable<Order>().Select<Order>().MergeTable()).LeftJoin<OrderItem>((m, i) => m.Id == i.OrderId)
                .ToList();


            var query8 = db.Queryable<Order>()
                .LeftJoin(db.Queryable<Custom>().Where(it=>it.Id==1),(o,i)=>o.CustomId==i.Id)
                .LeftJoin(db.Queryable<OrderItem>().Where(it=>it.OrderId==2),(o,i,item)=>item.OrderId==o.Id)
                .LeftJoin(db.Queryable<Order>().Where(it => it.Id >0), (o, i, item, od) => od.Id == o.Id)
                .Select(o => o).ToList();
    
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
            var list5 = db.Queryable<Order>().Where("id=@id or name like '%'+@name+'%' ", new { id = 1, name = "jack" }).ToList();



            /*** By dynamic***/

            //id=1
            var conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" });//id=1
            var student = db.Queryable<Order>().Where(conModels).ToList();

            //Complex use case
            List<IConditionalModel> Order = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" });//id=1
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
                //new  KeyValuePair<WhereType, ConditionalModel>( WhereType.And ,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "1" }),
                new  KeyValuePair<WhereType, ConditionalModel> (WhereType.Or,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" }),
                new  KeyValuePair<WhereType, ConditionalModel> ( WhereType.And,new ConditionalModel() { FieldName = "id", ConditionalType = ConditionalType.Equal, FieldValue = "2" })
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
                DbType = SqlSugar.DbType.SqlServer,
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
