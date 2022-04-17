using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    public class UCustom018
    {
        public static void Init()
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.SqlServer,
                IsAutoCloseConnection = true
            });


            //建表 
            if (!db.DbMaintenance.IsAnyTable("unit_Patent_License", false))
            {
                db.CodeFirst.InitTables<Patent_License>();
            }




            //建表 
            if (!db.DbMaintenance.IsAnyTable("Enterprise", false))
            {
                db.CodeFirst.InitTables<UintEnterprise>();
            }
            db.DbMaintenance.TruncateTable<Patent_License, UintEnterprise>();
            db.Insertable(new Patent_License()
            {
                Id = "1",
                Licensee_Id = 1,
                Licensor_Id = 1
            }).ExecuteCommand();
            db.Insertable(new UintEnterprise()
            {
                Id = 1,
                Createtime = DateTime.Now,
                Name = ""
            }).ExecuteCommand();



            RefAsync<int> totalCount = 0;

            var data = db.Queryable<Patent_License>()

                .LeftJoin<UintEnterprise>((pl, db_licensor) => pl.Licensor_Id == db_licensor.Id)

                .LeftJoin<UintEnterprise>((pl, db_licensor, db_licensee) => pl.Licensee_Id == db_licensee.Id)


                .Select((pl, db_licensor, db_licensee) => new
                {

                    License = pl,

                    LicensorId = (long?)db_licensor.Id,

                    LicensorName = db_licensor.Name,

                    LicenseeId = (long?)db_licensee.Id,

                    LicenseeName = db_licensee.Name

                })

                .ToPageListAsync(1, 2, totalCount).GetAwaiter().GetResult();

        }
        [SugarTable("unit_Patent_License")]
        public class Patent_License

        {



            [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]

            public string Id { get; set; }



            [SugarColumn(ColumnName = "licensor_id")]

            public long Licensor_Id { get; set; }





            [SugarColumn(ColumnName = "licensee_id")]

            public long Licensee_Id { get; set; }

        }

        public class UintEnterprise

        {

            [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]

            public long Id { get; set; }





            [SugarColumn(ColumnName = "name")]

            public string Name { get; set; }



            [SugarColumn(ColumnName = "createtime")]

            public DateTime Createtime { get; set; }

        }
    }
}
