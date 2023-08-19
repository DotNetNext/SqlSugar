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
            SqlFuncTest();
            ReturnType();
        }

        private static void EasyExamples()
        {
            Console.WriteLine("");
            Console.WriteLine("#### Examples Start ####");
            var db = GetInstance();
            var dbTime = db.GetDate();
            var getAll = db.Queryable<Order>().ToList();
            var getLike = db.Queryable<Order>().Where(it=>it.Name.Contains("order1")).ToList();
            var getYYYY = db.Queryable<Order>().Select(it => it.CreateTime.ToString("yyyy-MM-dd")).ToList();
            var getOrderBy = db.Queryable<Order>().OrderBy(it => it.Name,OrderByType.Desc).ToList();
            var getOrderBy2 = db.Queryable<Order>().OrderBy(it => it.Id).OrderBy(it => it.Name, OrderByType.Desc).ToList();
            var getOrderBy3 = db.Queryable<Order>().OrderBy(it =>new { it.Name,it.Id}).ToList();
            //var getRandom = db.Queryable<Order>().OrderBy(it => SqlFunc.GetRandom()).First();
            var getSingleOrDefault = db.Queryable<Order>().Where(it => it.Id == 1).Single();
            var getFirstOrDefault = db.Queryable<Order>().First();
            var getByWhere = db.Queryable<Order>().Where(it => it.Id == 1 || it.Name == "a").ToList();
            var getByWhere2 = db.Queryable<Order>().Where(it => it.Id == DateTime.Now.Year).ToList();
            var getByFuns = db.Queryable<Order>().Where(it => SqlFunc.IsNullOrEmpty(it.Name)).ToList();
           // var getByFuns2 = db.Queryable<Order>().GroupBy(it => it.Name).Select(it => SqlFunc.AggregateDistinctCount(it.Price)).ToList();
            var btime = Convert.ToDateTime("2021-1-1");
            var etime = Convert.ToDateTime("2022-1-12");
            var test01 = db.Queryable<Order>().Select(it =>  SqlFunc.DateDiff(DateType.Year,btime, etime)).ToList();
            var test02 = db.Queryable<Order>().Select(it => SqlFunc.DateDiff(DateType.Day, btime, etime)).ToList();
            var test03 = db.Queryable<Order>().Select(it => SqlFunc.DateDiff(DateType.Month, btime, etime)).ToList();
            //var test04 = db.Queryable<Order>().Select(it => SqlFunc.DateDiff(DateType.Second, DateTime.Now, DateTime.Now.AddMinutes(2))).ToList();
            var q1 = db.Queryable<Order>().Take(1);
            var q2 = db.Queryable<Order>().Take(2);
            var test05 = db.UnionAll(q1, q2).ToList();
            var test06 = db.Queryable<Order>().ToList();
            var getList = db.Queryable<Order>().GroupBy(z => z.Id).Select(it => new {
                count = SqlFunc.AggregateCount(it.Id)
            }).MergeTable().Where(it => it.count > 1).ToList();
            if (db.DbMaintenance.IsAnyTable("users", false))
            {
                db.DbMaintenance.DropTable("users");
            }
            if (!db.DbMaintenance.IsAnyTable("users", false))
            {
                db.CodeFirst.InitTables<Users>();
            }
 
           var list = new List<Users>();
            for (var i = 0; i < 1000; i++)
            {

                list.Add(new Users
                {
                    Sid=SnowFlakeSingle.Instance.NextId(),
                    createtime = DateTime.Now,
                    username = "161718",
                    password = "161718",
                });
            }
            db.Insertable(list).ExecuteCommand();

            var list2 = db.Queryable<Users>().ToList();

            Console.WriteLine("#### Examples End ####");
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

            List<long> listInt = db.Queryable<Order>().Select(it => it.Id).ToList();

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


           // var oneClass = db.Queryable<Order, OrderItem, Custom>((o, i, c) => new JoinQueryInfos(
           //   JoinType.Left, o.Id == i.OrderId,
           //   JoinType.Left, o.CustomId == c.Id
           // ))
           //.Select((o, i, c) => c).ToList();

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
 

        private static void SqlFuncTest()
        {
            Console.WriteLine("");
            Console.WriteLine("#### SqlFunc Start ####");
            var db = GetInstance();
            var index= db.Queryable<Order>().Select(it => SqlFunc.Contains("a", "cccacc")).First();
            //var list = db.Queryable<Order>().Select(it => new ViewOrder()
            //{

            //    Id = SqlFunc.AggregateSum(SqlFunc.IF(it.Id > 0).Return(1).End(0))
            //}).ToList();
            var list2 = db.Queryable<Order>().Select(it => new
            {
                date = SqlFunc.ToDateShort(it.CreateTime),
                datetime = SqlFunc.ToDate(it.CreateTime)
            }).ToList();
            Console.WriteLine("#### SqlFunc  End ####");
        }
 

        private static void NoEntity()
        {
            Console.WriteLine("");
            Console.WriteLine("#### No Entity Start ####");
            var db = GetInstance();

            var list = db.Queryable<dynamic>().AS("order_1").Where("id=id", new { id = 1 }).ToList();

            var list2 = db.Queryable<dynamic>("o").AS("order_1").AddJoinInfo("OrderDetail_1", "i", "o.id=i.OrderId").Where("id=id", new { id = 1 }).Select("o.*").ToList();
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
            db.CodeFirst.InitTables<Tree>();
            //无限级高性能导航映射
            var treeRoot = db.Queryable<Tree>().Where(it => it.Id == 1).ToList();
            db.ThenMapper(treeRoot, item =>
            {
                item.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => item.Id, item).ToList();
            });
            db.ThenMapper(treeRoot.SelectMany(it => it.Child), it =>
            {
                it.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => it.Id, it).ToList();
            });
            db.ThenMapper(treeRoot.SelectMany(it => it.Child).SelectMany(it => it.Child), it =>
            {
                it.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => it.Id, it).ToList();
            });
            db.ThenMapper(treeRoot.SelectMany(it => it.Child).SelectMany(it => it.Child).SelectMany(it => it.Child), it =>
            {
                it.Child = db.Queryable<Tree>().SetContext(x => x.ParentId, () => it.Id, it).ToList();
            });
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
                DbType = SqlSugar.DbType.QuestDB,
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
