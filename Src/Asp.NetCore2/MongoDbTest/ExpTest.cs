using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbTest
{
    public class ExpTest
    {
        public static void Init() 
        {
            Expression<Func<Order, bool>> exp = it => it.Id == 1 || it.Name == "a";
            var json = MongoNestedTranslator.Translate(exp, null).ToString(); 
            var Order = new Order() { Id = 1 };
            Expression<Func<Order, bool>> exp2 = it => it.Id == Order.Id || it.Name == "a";
            var json2 = MongoNestedTranslator.Translate(exp2, null).ToString();
             
            Expression<Func<Order, bool>> exp3 = it => it.IsValidate == true;
            var json3 = MongoNestedTranslator.Translate(exp3, null).ToString();

            Expression<Func<Order, bool>> exp4 = it => it.IsValidate != true;
            var json24 = MongoNestedTranslator.Translate(exp4, null).ToString();

            Expression<Func<Order, bool>> exp5 = it =>1==it.Id;
            var json5 = MongoNestedTranslator.Translate(exp5, null).ToString();
        }
    }
    public class Order 
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsValidate { get; set; }
    }
}
