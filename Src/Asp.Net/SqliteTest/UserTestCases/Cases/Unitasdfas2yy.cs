using SqlSugar;
using System;
using System.Data; 

namespace OrmTest
{
    internal class Unitdfaafas
    {
        /// <summary>
        /// 测试案例: BulkCopy中Datatable列名如果有空格报错
        /// </summary>
        /// <param name="args"></param>
       public  static void Init()
        {
            var Db = NewUnitTest.Db;
            Db.DbMaintenance.CreateDatabase();
            Db.CurrentConnectionConfig.MoreSettings = new ConnMoreSettings() {  
                 IsCorrectErrorSqlParameterName=true
            };
            var datatable = new DataTable();
            datatable.Columns.Add("Name", typeof(string));
            datatable.Columns.Add("Name 1", typeof(string));//列名有空格

            for (int i = 0; i < 10; i++)
            {
                var row = datatable.NewRow();
                row["Name"] = "Name" + i;
                row["Name 1"] = "Name 1" + i;
                datatable.Rows.Add(row);
            }

            var tableName = "Testaaaa";
            var dynamicProperyBuilder = Db.DynamicBuilder().CreateClass(tableName, new SugarTable());
            dynamicProperyBuilder.CreateProperty("Id", typeof(int), new SugarColumn()
            {
                IsPrimaryKey = true,
                IsIdentity = true
            });

            foreach (DataColumn datatableColumn in datatable.Columns)
            {
                dynamicProperyBuilder.CreateProperty(datatableColumn.ColumnName.Replace(" ",""), typeof(string), new SugarColumn()
                {
                    ColumnDataType = StaticConfig.CodeFirst_BigString,
                    ColumnName = datatableColumn.ColumnName,
                    IsNullable = true
                });
            }

            var dynamicClass = dynamicProperyBuilder.BuilderType();
            Db.CodeFirst.InitTables(dynamicClass);


            //BulkCopy中Datatable列名如果有空格会报错！
            var result = Db.Fastest<DataTable>().AS(tableName).BulkCopy(datatable);

        }

         
    }
}
 
 