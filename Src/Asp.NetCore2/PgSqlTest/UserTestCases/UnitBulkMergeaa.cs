using SqlSugar;
using System;
using System.Data;
namespace OrmTest
{
    public class UnitBulkMergeaa
    {
        public static void Init()
        {
            SqlSugarClient BDb = NewUnitTest.Db;
            if (BDb.DbMaintenance.IsAnyTable("unittestaaa111",false)) 
            {
                BDb.DbMaintenance.DropTable("unittestaaa111");
            }
            BDb.Ado.ExecuteCommand(@"
CREATE TABLE  ""unittestaaa111"" (

id int8 NOT NULL,

fk_id_1 int8 NULL,

fk_id_2 int8 NULL,

text_nullable_1 text NULL,

CONSTRAINT check_test_fk_id CHECK ((((fk_id_1 IS NULL) AND (fk_id_2 IS NOT NULL)) OR ((fk_id_1 IS NOT NULL) AND (fk_id_2 IS NULL)))),

CONSTRAINT test_pkey PRIMARY KEY (id)

);");
            var dt = new DataTable();
            dt.TableName = "Test"; //设置表名
            dt.Columns.Add(new DataColumn("id", typeof(int)));
            dt.Columns.Add(new DataColumn("fk_id_1", typeof(int)));
            dt.Columns.Add(new DataColumn("fk_id_2", typeof(int)));
            dt.Columns.Add(new DataColumn("text_nullable_1", typeof(string))); 
            var addRow = dt.NewRow(); addRow["id"] = 2; addRow["fk_id_1"] = DBNull.Value; addRow["fk_id_2"] = 2;
            addRow["text_nullable_1"] = null;
            dt.Rows.Add(addRow);//添加数据
            BDb.Fastest<DataTable>().AS("unittestaaa111").BulkMerge(dt, new string[] { "id" }, false);
        }
    }
    public class test
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public int Id { get; set; }

        [SugarColumn(ColumnName = "fk_id_1")]
        public int? Fk_id_1 { get; set; }

        [SugarColumn(ColumnName = "fk_id_2")]
        public int? Fk_id_2 { get; set; }

        [SugarColumn(ColumnName = "text_nullable_1")]
        public string? Text_nullable_1 { get; set; }


    }
}
