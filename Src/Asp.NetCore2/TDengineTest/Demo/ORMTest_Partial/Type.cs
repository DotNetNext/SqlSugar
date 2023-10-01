using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text; 

namespace TDengineTest
{
    public partial class ORMTest
    {

        private static void DbType2(SqlSugarClient db)
        {
            //建库

            //db.Ado.ExecuteCommand("CREATE DATABASE IF NOT EXISTS db WAL_RETENTION_PERIOD 3600");

            db.Ado.ExecuteCommand(@"CREATE DATABASE  IF NOT EXISTS db123456 

KEEP 1080 

DURATION 10 

BUFFER 16 

WAL_LEVEL 1 

CACHEMODEL 'both'");//不支持   UPDATE 1  选项



            //建超级表

            //            DeviceType INT, 

            // DeviceCode BINARY(12), 

            db.Ado.ExecuteCommand(

 @"CREATE STABLE IF NOT EXISTS  ConveryTable 

(

	ts TIMESTAMP,  



	Action INT,

	TaskNo INT,

	Stime TIMESTAMP,

	Etime TIMESTAMP,

	GoodsType INT,

	BarCode NCHAR(64),

	FromNode NCHAR(24),

	ToNode NCHAR(24),

	Speed FLOAT,

	AccSpeed FLOAT,

	DecSpeed FLOAT,

	Field1 NCHAR(256),	

	Field2 NCHAR(256),

	Field3 NCHAR(256),

	Field4 NCHAR(256),

	Field5 NCHAR(256),

	Field6 NCHAR(256),

	

	Remark  NCHAR(500) 

) 

	TAGS 

	(

		DeviceType INT,  DeviceCode BINARY(12)

	)");



            var random = new Random();



            string tablename = "t1118_" + (1000 + 99);

            db.Ado.ExecuteCommand("create table IF NOT EXISTS  t1118_1099 using ConveryTable tags(1,'1099')");

            var curdb = db;

            List<ConveryTable> rows = new List<ConveryTable>();

            for (int k = 0; k < 10; k++)

            {

                rows.Add(new ConveryTable()

                {

                    ts = DateTime.Now.AddDays(random.Next(1, 9)),

                    AccSpeed = random.Next(1, 9),

                    Action = 2,

                    BarCode = "1111" + k,

                    DecSpeed = random.Next(1, 9),

                    DeviceCode = "1001",

                    DeviceType = 1,

                    Etime = DateTime.Now.AddSeconds(k),

                    FromNode = "001001001",

                    ToNode = "002002" + k.ToString("d3"),

                    GoodsType = 3,

                    TaskNo = random.Next(1, 900),

                    Speed = random.Next(7, 90),

                    Stime = DateTime.Now,

                });

            }

            curdb.Insertable(rows).AS(tablename).ExecuteCommand(); ;
            curdb.Queryable<ConveryTable>().AS(tablename).ToList();
        }

        private static void DbType(SqlSugarClient db)
        {
            //更多类型查询测试
            db.Ado.ExecuteCommand(@"
                                CREATE STABLE IF NOT EXISTS  `fc_data` (
                                `upload_time` TIMESTAMP, 
                                `voltage` SMALLINT,
                                `temperature` FLOAT,
                                `data_id` SMALLINT,
                                `speed_hex` VARCHAR(80),
                                `gateway_mac` VARCHAR(8),
                                `ruminate` SMALLINT, 
                                `rssi` TINYINT) TAGS (`tag_id` VARCHAR(12))");
            var list = db.Queryable<fc_data>().ToList();
            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  fc_data01 using `fc_data` tags('1')");
            db.Insertable(new fc_data()
            {
                data_id = 1,
                gateway_mac = "mac",
                rssi = 11,
                ruminate = 1,
                speed_hex = "x",
                temperature = 1,
                upload_time = DateTime.Now,
                voltage = 1

            }).AS("fc_data01").ExecuteCommand();

            var list2 = db.Queryable<fc_data>().AS("fc_data01").ToList();
        }
    }
}
