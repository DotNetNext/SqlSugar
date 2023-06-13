using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OrmTest.UCustom010;

namespace OrmTest 
{
    internal class UQueryableByObject
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            var type = typeof(Order);
            var data=db.QueryableByObject(type)
                .Where("id=1").ToList();
            var data2=db.QueryableByObject(type)
                .Where("id=@id",new {id=1 }).ToList();
            var data3 = db.QueryableByObject(type)
                .OrderBy("id").Where("id=@id", new { id = 1 }).ToList();
            var data4 = db.QueryableByObject(type)
                .AS("order","o")
                .GroupBy("id").Having("count(name)>0").Select("Id").ToList();
            var data5 = db.QueryableByObject(type)
                .AS("order")
                .GroupBy("id").Having("count(name)>0").Select("Id").ToList();

            var data6= db.QueryableByObject(type,"o")
             .AddJoinInfo("order","y","o.id=y.id",SqlSugar.JoinType.Left)
             .GroupBy("o.id").Having("count(o.name)>0").Select("o.Id").ToList();

            var having = ObjectFuncModel.Create("GreaterThan", ObjectFuncModel.Create("AggregateAvg", "o.id"), "{int}:1");
            var data62 = db.QueryableByObject(type, "o")
             .AddJoinInfo(typeof(Custom), "y", "o.id=y.id", SqlSugar.JoinType.Left)
             .GroupBy("o.id").Having(having).Select("o.Id").ToList();

            List<OrderByModel> orderList = OrderByModel.Create
                                 (
                                 new OrderByModel() { FieldName = "o.id", OrderByType = OrderByType.Desc },
                                 new OrderByModel() { FieldName = "o.name" }//用到函数看标题6
                                 ); 
            var data63 = db.QueryableByObject(type, "o")
             .AddJoinInfo(typeof(Custom), "y", "o.id=y.id", SqlSugar.JoinType.Left)
             .OrderBy(orderList) .Select("o.Id").ToList();
            var groupList = GroupByModel.Create(new GroupByModel() { FieldName = "o.id" }); //Sql: `id`

            var selector = new List<SelectModel>() {
            new SelectModel(){AsName = "id1",FiledName = "o.id"},
            new SelectModel(){ AsName="id2", FiledName = "o.id"}};
            var conModels = new List<IConditionalModel>();
            conModels.Add(new ConditionalModel { FieldName = "o.id", ConditionalType = ConditionalType.Equal, FieldValue = "1" });
            var data64 = db.QueryableByObject(type, "o")
              .AddJoinInfo(typeof(Custom), "y", "o.id=y.id", SqlSugar.JoinType.Left)
             // .Where(conModels)
              .Where(conModels,true)
              .GroupBy(groupList).Having(having).Select(selector).ToList();

            var data65 = db.QueryableByObject(type, "o")
            .AddJoinInfo(typeof(Custom), "y", "o.id=y.id", SqlSugar.JoinType.Left)
            // .Where(conModels)
            .Where(conModels)
            .GroupBy(groupList).Having(having).Select(selector).ToList();


            var whereFunc = ObjectFuncModel.Create("Format", "o.id", ">", "{int}:1", "&&", "o.name", "=", "{string}:a");
            var data66 = db.QueryableByObject(type, "o")
            .AddJoinInfo(typeof(Custom), "y", "o.id=y.id", SqlSugar.JoinType.Left)
            .Where(whereFunc)
            .GroupBy(groupList).Having(having).Select(selector).ToList();
        }
    }
}
