using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace TDengineTest
{
    public partial class ORMTest
    {
        /// <summary>
        /// 纳秒
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void NS()
        {
            //说明:
            //字符串中指定TsType=config_ns
            //实体加上 SqlParameterDbType =typeof(DateTime19)

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.TDengine,
                IsAutoCloseConnection = true,
                ConnectionString = "Host=localhost;Port=6030;Username=root;Password=taosdata;Database=nstest;TsType=config_ns"
            });

            //删除库-库上限比太少只能删了测试
            if (db.DbMaintenance.GetDataBaseList().Any(it => it == "nstest"))
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
            var list = db.Queryable<MyTable02_NS>().ToList();

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
        /// <summary>
        /// 微妙
        /// </summary>
        /// <exception cref="Exception"></exception>
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
    }
}
