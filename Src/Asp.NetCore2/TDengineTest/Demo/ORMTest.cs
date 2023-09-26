using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SqlSugar; 
using TDengineDriver; 

namespace OrmTest
{
    public class ORMTest
    {

        public static void Init()
        {
            var db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.TDengine,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true,
                AopEvents = new AopEvents
                {
                    OnLogExecuting = (sql, p) =>
                    {
                        Console.WriteLine(UtilMethods.GetNativeSql(sql, p));
                    }
                }
            });
             
            //简单用例
            Demo1(db);

            //测试用例
            UnitTest(db);
        }

        private static void Demo1(SqlSugarClient db)
        {
            //建库
            db.DbMaintenance.CreateDatabase();

            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  St01 (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT, isdelete BOOL, name BINARY(64)) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable02 using St01 tags('California.SanFrancisco',1)");


            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable02 ");


            //插入单条子表
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommand();

            //批量插入子表
            db.Insertable(GetInsertDatas()).ExecuteCommand();


            //查询子表(主表字段也能查出来)
            var list = db.Queryable<MyTable02>().OrderBy(it => it.ts).ToList();

            //条件查询
            var list2 = db.Queryable<MyTable02>().Where(it => it.name == "测试2").ToList();
            var list22 = db.Queryable<MyTable02>().Where(it => it.voltage == 222).ToList();
            var list222 = db.Queryable<MyTable02>().Where(it => it.phase == 1.2).ToList();
            var list2222 = db.Queryable<MyTable02>().Where(it => it.isdelete == true).ToList();

            //模糊查询
            var list3 = db.Queryable<MyTable02>().Where(it => it.name.Contains("a")).ToList();

            //时间差函数 
            var list31 = db.Queryable<MyTable02>().Select(it =>
            new
            {
                diff = SqlFunc.DateDiff(DateType.Day, it.ts, DateTime.Now),
                time=it.ts
            }).ToList();

            //时间加1天
            var list32 = db.Queryable<MyTable02>().Select(it =>
              new
              {
                  addTime = SqlFunc.DateAdd(it.ts,1, DateType.Day),
                  oldime = it.ts
              }).ToList();

            //自定义函数:实现时间加1天
            var list33 = db.Queryable<MyTable02>().Select(it =>
              new
              {
                  addTime =SqlFunc.MappingColumn<DateTime>(" `ts`+1d "),
                  oldime = it.ts
              }).ToList();

            //分页
            var Count = 0;
            var list4 = db.Queryable<MyTable02>().Where(it => it.voltage == 111)
                .ToPageList(1, 2, ref Count);

            //删除子表
            var ts = list.First().ts;
            var de = DateTime.Now.AddYears(-1);
            var count = db.Deleteable<MyTable02>().Where(it => it.ts > de).ExecuteCommand();


            //异步
            db.Insertable(new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommandAsync().GetAwaiter().GetResult();

            var list100 = db.Queryable<MyTable02>().ToListAsync().GetAwaiter().GetResult();

            //联表查询:不支持left join只能这样
            var list101= db.Queryable<MyTable02, MyTable02>((x, y) => x.ts == y.ts)
                .Select((x,y) => new
                {
                    xts=x.ts,
                    yts=y.ts
                }).ToList();
            //联表查询在分页
            var list102 = db.Queryable<MyTable02, MyTable02>((x, y) => x.ts == y.ts)
            .Select((x, y) => new
            {
                xts = x.ts,
                yts = y.ts
            }).ToPageList(1,2);
        }
        private static void UnitTest(SqlSugarClient db)
        {
            //类型测试
            DbType(db);
            DbType2(db);

            //纳秒
            NS();

            //微秒
            US();
        }

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

        private static void NS()
        {
            //说明:
            //字符串中指定TsType=config_ns
            //实体加上 SqlParameterDbType =typeof(DateTime19)

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() {
                DbType=SqlSugar.DbType.TDengine,
                  IsAutoCloseConnection=true,
                ConnectionString = "Host=localhost;Port=6030;Username=root;Password=taosdata;Database=nstest;TsType=config_ns" });

            //删除库-库上限比太少只能删了测试
            if(db.DbMaintenance.GetDataBaseList().Any(it=>it== "nstest"))
            {
                db.Ado.ExecuteCommand("drop  database nstest");
            }

            db.DbMaintenance.CreateDatabase();//创建纳秒库
            
            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  St01 (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT, isdelete BOOL, name BINARY(64)) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable02 using St01 tags('California.SanFrancisco',1)");


            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable02 ");


            //插入单条子表
            db.Insertable(new MyTable02_NS()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommand();
            var list=db.Queryable<MyTable02_NS>().ToList();

            db.Ado.ExecuteCommand(@" CREATE TABLE aoi_recordsuper (fcreatetime timestamp, fsncode VARCHAR(50), fmachineno VARCHAR(50),lotno VARCHAR(50),billno VARCHAR(50),fresult VARCHAR(50)
 ,fproduct VARCHAR(50),machinetime VARCHAR(50),id VARCHAR(50),uploadtype VARCHAR(50),dimbin VARCHAR(50)) 
 TAGS (workstation VARCHAR(50));");

            var dt2=Convert.ToDateTime("2023-09-23 08:00:00.12345");
            db.Ado.ExecuteCommand(@"
   

INSERT INTO aoirecord_2309 USING aoi_recordsuper TAGS (""AOI"") 
VALUES ('2023-09-23', ""10.2"", ""A219"",""2309050001"",""B03123"",""OK"",""681-ABC"",""2023-9-5"",""123"",""首测"",""A"")
(now, ""10.2"", ""A219"",""2309050001"",""B03123"",""OK"",""681-ABC"",""2023-9-5"",""123"",""首测"",""A"")");

            var date = Convert.ToDateTime("2023-9-25 00:00:00");
            var list3=db.Queryable<T_TD_AOIRecord>().ToList();
            var list2 = db.Queryable<T_TD_AOIRecord>()
                .Where(t => t.fcreatetime >= date).ToList();

            if (list2.Count != 1) 
            {
                throw new Exception("unit error");
            }

        }
        private static void US()
        {
            //clear static config
            SqlSugar.TDengine.TDengineProvider._IsIsNanosecond = false;
            SqlSugar.TDengine.TDengineProvider._IsMicrosecond = false;

            //说明:
            //字符串中指定TsType=config_ns
            //实体加上 SqlParameterDbType =typeof(DateTime19)
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.TDengine,
                IsAutoCloseConnection = true,
                ConnectionString = "Host=localhost;Port=6030;Username=root;Password=taosdata;Database=nstest;TsType=config_us"
            });
            //删除库-库上限比太少只能删了测试
            db.Ado.ExecuteCommand("drop   database nstest");

            db.DbMaintenance.CreateDatabase();//创建纳秒库

            //建超级表
            db.Ado.ExecuteCommand("CREATE STABLE IF NOT EXISTS  St01 (ts TIMESTAMP, current FLOAT, voltage INT, phase FLOAT, isdelete BOOL, name BINARY(64)) TAGS (location BINARY(64), groupId INT)");

            //创建子表
            db.Ado.ExecuteCommand(@"create table IF NOT EXISTS  MyTable02 using St01 tags('California.SanFrancisco',1)");


            //查询子表
            var dt = db.Ado.GetDataTable("select * from MyTable02 ");


            //插入单条子表
            db.Insertable(new MyTable02_US()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "haha",
                location = "aa",
                phase = Convert.ToSingle(1.2),
                voltage = 11
            }).ExecuteCommand();
            var list = db.Queryable<MyTable02_US>().ToList();

            db.Ado.ExecuteCommand(@" CREATE TABLE aoi_recordsuper (fcreatetime timestamp, fsncode VARCHAR(50), fmachineno VARCHAR(50),lotno VARCHAR(50),billno VARCHAR(50),fresult VARCHAR(50)
 ,fproduct VARCHAR(50),machinetime VARCHAR(50),id VARCHAR(50),uploadtype VARCHAR(50),dimbin VARCHAR(50)) 
 TAGS (workstation VARCHAR(50));");

            var dt2 = Convert.ToDateTime("2023-09-23 08:00:00.12345");
            db.Ado.ExecuteCommand(@"
   

INSERT INTO aoirecord_2309 USING aoi_recordsuper TAGS (""AOI"") 
VALUES ('2023-09-23', ""10.2"", ""A219"",""2309050001"",""B03123"",""OK"",""681-ABC"",""2023-9-5"",""123"",""首测"",""A"")
(now, ""10.2"", ""A219"",""2309050001"",""B03123"",""OK"",""681-ABC"",""2023-9-5"",""123"",""首测"",""A"")");

            var date = Convert.ToDateTime("2023-9-25 00:00:00");
            var list3 = db.Queryable<T_TD_AOIRecord>().ToList();
            var list2 = db.Queryable<T_TD_AOIRecord>()
                .Where(t => t.fcreatetime >= date).ToList();

            if (list2.Count != 1)
            {
                throw new Exception("unit error");
            }
        }

        private static List<MyTable02> GetInsertDatas()
        {
            return new List<MyTable02>() {
            new MyTable02()
            {
                ts = DateTime.Now.AddDays(-1),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试1",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
             new MyTable02()
            {
                ts = DateTime.Now.AddDays(-2),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试2",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
                new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "测试3",
                location = "true",
                phase = Convert.ToSingle(1.1),
                voltage = 111
            }
            };
        }
        public class fc_data
        {
            public DateTime upload_time { get; set; }

            public int voltage { get; set; }

            public float temperature { get; set; }

            public int data_id { get; set; }


            public string speed_hex { get; set; }


            public string gateway_mac { get; set; }


            public int ruminate { get; set; }

            public sbyte rssi { get; set; }

        }
        public class MyTable02
        {
            [SugarColumn(IsPrimaryKey =true)]
            public DateTime ts { get; set; }
            public float current { get; set; }
            public bool isdelete { get; set; }
            public string name { get; set; }
            public int voltage { get; set; }
            public float phase { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert =true,IsOnlyIgnoreUpdate =true)]
            public string location { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
            public int groupId { get; set; }
        } 

    }

}
