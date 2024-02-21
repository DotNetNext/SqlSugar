using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {

        public static void Json()
        {
            Db.CodeFirst.InitTables<UnitJsonTest>();
            Db.DbMaintenance.TruncateTable<UnitJsonTest>();
            var id= Db.Insertable(new UnitJsonTest() { Order = new Order { Id = 1, Name = "order1" } }).ExecuteReturnIdentity();
            var list = Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order1", list.First().Order.Name, "Json");
            Db.Updateable(new UnitJsonTest() { Id = id, Order = new Order { Id = 2, Name = "order2" } }).ExecuteCommand();
            list= Db.Queryable<UnitJsonTest>().ToList();
            UValidate.Check("order2", list.First().Order.Name, "Json");
            var list2 = Db.Queryable<UnitJsonTest>().ToList();

            string json = @"[

    {

        ""ConditionalList"": [

            {

                ""Key"": -1,

                ""Value"": {

                    ""FieldName"": ""nullableBool"",

                    ""FieldValue"": ""null"",

                    ""ConditionalType"": 6,
                    ""CSharpTypeName"":""bool""

                }

            }

        ]

    }

]";
            var list3 = 
                Db.Queryable<UnitJsonTest>()
                .Where(Db.Utilities.JsonToConditionalModels(json))
                .ToSql().Key;
            if (list3.Trim() != "SELECT `Id`,`Order` FROM `UnitJsonTest`  WHERE  (   `nullableBool` IN (null)   )") 
            {
                throw new Exception("unit error");
            }
            json= json.Replace("\"null\"", "\"0,null,1\"");
            list3 =
            Db.Queryable<UnitJsonTest>()
            .Where(Db.Utilities.JsonToConditionalModels(json))
            .ToSql().Key;
            if (list3.Trim() != "SELECT `Id`,`Order` FROM `UnitJsonTest`  WHERE  (   `nullableBool` IN (0,null,1)   )")
            {
                throw new Exception("unit error");
            }
            Db.CodeFirst.InitTables<UnutBoolTestaaa>();
            var list4 = 
            Db.Queryable<UnutBoolTestaaa>()
            .Where(Db.Utilities.JsonToConditionalModels(json)).ToList();
        }
    }

    public class UnutBoolTestaaa
    {
        public bool nullableBool { get; set; }
    }
    public class UnitJsonTest
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SqlSugar.SugarColumn(ColumnDataType = "varchar(4000)", IsJson = true)]
        public Order Order { get; set; }
    }
}
