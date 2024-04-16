using Google.Protobuf.WellKnownTypes;
using MySqlX.XDevAPI.Relational;
using OrmTest;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrmTest
{
    public class examination
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int qtype { get; set; }
        public string question { get; set; }
        [SugarColumn(IsJson = true)]
        public List<string> options { get; set; }
        public string answer { get; set; }
    }

    public class examination_info
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        public int exam_id { get; set; }
        public int userid { get; set; }
        public string myanswer;
        public int score { get; set; } = -1;
        public DateTime? submit_time;


        [SugarColumn(IsJson = true)]
        public examination exam { get; set; }
    }

    internal class UnitDtoJsonafda
    {
        public static  void Init()
        {
            // ----------------------sql------------------
            // CREATE DATABASE `cochlea` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
            //            --cochlea.examination definition

            //CREATE TABLE `examination` (
            //  `id` int(11) NOT NULL AUTO_INCREMENT,
            //  `qtype` int(11) NOT NULL,
            //  `question` varchar(100) NOT NULL,
            //  `options` varchar(200) DEFAULT NULL,
            //  `answer` varchar(5) NOT NULL,
            //  PRIMARY KEY(`id`)
            //) ENGINE = InnoDB AUTO_INCREMENT = 21 DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_general_ci COMMENT = '试卷';


            //            --cochlea.examination_info definition

            //CREATE TABLE `examination_info` (
            //  `id` int(11) NOT NULL,
            //  `exam_id` int(11) DEFAULT NULL,
            //  `userid` int(11) DEFAULT NULL,
            //  `myanswer` varchar(100) DEFAULT NULL,
            //  `score` int(11) DEFAULT NULL,
            //  `submit_time` datetime DEFAULT NULL,
            //  PRIMARY KEY(`id`),
            //  KEY `examination_info_userid_IDX` (`userid`) USING BTREE
            //) ENGINE = InnoDB DEFAULT CHARSET = utf8mb4 COLLATE = utf8mb4_general_ci;




            //ConfigurationManager.ConnectionStrings["dbconnstr"].ConnectionString;
            SqlSugarClient Db = NewUnitTest.Db;

  
            Db.Ado.ExecuteCommand("-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)\r\n--\r\n-- Host: localhost    Database: cochlea\r\n-- ------------------------------------------------------\r\n-- Server version\t11.3.2-MariaDB\r\n\r\n/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;\r\n/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;\r\n/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;\r\n/*!50503 SET NAMES utf8mb4 */;\r\n/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;\r\n/*!40103 SET TIME_ZONE='+00:00' */;\r\n/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;\r\n/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;\r\n/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;\r\n/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;\r\n\r\n--\r\n-- Table structure for table `examination`\r\n--\r\n\r\nDROP TABLE IF EXISTS `examination`;\r\n/*!40101 SET @saved_cs_client     = @@character_set_client */;\r\n/*!50503 SET character_set_client = utf8mb4 */;\r\nCREATE TABLE `examination` (\r\n  `id` int(11) NOT NULL AUTO_INCREMENT,\r\n  `qtype` int(11) NOT NULL,\r\n  `question` varchar(100) NOT NULL,\r\n  `options` varchar(200) DEFAULT NULL,\r\n  `answer` varchar(5) NOT NULL,\r\n  PRIMARY KEY (`id`)\r\n) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci COMMENT='试卷';\r\n/*!40101 SET character_set_client = @saved_cs_client */;\r\n\r\n--\r\n-- Dumping data for table `examination`\r\n--\r\n\r\nLOCK TABLES `examination` WRITE;\r\n/*!40000 ALTER TABLE `examination` DISABLE KEYS */;\r\nINSERT INTO `examination` VALUES (1,1,'内耳的听觉感受器是','[\\\"球囊斑\\\",\\\"椭圆囊斑\\\",\\\"壶腹峭\\\",\\\"螺旋器\\\",\\\"前庭阶\\\"]','d'),(2,1,'不属于骨迷路的结构是','[\\\"蜗管\\\",\\\"前庭\\\",\\\"外骨半规管耳蜗\\\",\\\"前骨半规管\\\",\\\"耳蜗\\\"]','a'),(3,1,'前庭蜗器按部位可分为','[\\\"外耳、鼓室和迷路\\\",\\\"耳廓、鼓室和内耳\\\",\\\"外耳、鼓室和耳蜗\\\",\\\"外耳、中耳和内耳\\\",\\\"耳廓、中耳和耳蜗\\\"]','d'),(4,1,'外耳门位于','[\\\"颧骨\\\",\\\"颞骨\\\",\\\"枕骨\\\",\\\"下颌骨\\\",\\\"筛骨\\\"]','b'),(5,1,'鼓室与颅中窝相邻的壁是','[\\\"颈静脉壁\\\",\\\"鼓室盖\\\",\\\"颈动脉壁\\\",\\\"迷路壁\\\",\\\"乳突壁\\\"]','b'),(6,1,'鼓室','[\\\"前壁有面神经管凸\\\",\\\"前庭窗后上方隆起为岬\\\",\\\"后壁有前庭窗、蜗窗\\\",\\\"内侧壁有鼓窦开口\\\",\\\"前壁有咽鼓管开口\\\"]','e'),(7,1,'何者位于中耳内','[\\\"外耳道\\\",\\\"骨半规管\\\",\\\"耳蜗\\\",\\\"听小骨\\\",\\\"鼓膜\\\"]','d'),(8,1,'中耳的组成不包括','[\\\"鼓室\\\",\\\"咽鼓管\\\",\\\"乳突窦\\\",\\\"乳突小房\\\",\\\"前庭\\\"]','e'),(9,1,'鼓室','[\\\"上壁为鼓室盖,邻颅后窝\\\",\\\"前壁上方有咽鼓管通鼻咽部\\\",\\\"下壁为颈动脉壁\\\",\\\"后壁邻颈内静脉始部\\\",\\\"内有两块听小骨肌\\\"]','b'),(10,1,'关于蜗管的描述错误的是','[\\\"位于耳蜗内\\\",\\\"内含内淋巴\\\",\\\"借连合管与球囊相连\\\",\\\"螺旋器位于蜗管的前庭壁(前庭膜上)\\\",\\\"螺旋器位于蜗管的基底膜上\\\"]','d'),(11,1,'与鼓室相通的管道是','[\\\"外耳道\\\",\\\"内耳道\\\",\\\"咽鼓管\\\",\\\"蜗管\\\",\\\"内淋巴管\\\"]','c'),(12,1,'下列有关中耳的描述中，正确的选项是：','[\\\"中耳即鼓室\\\",\\\"中耳全部位于颞骨岩部内\\\",\\\"中耳向内借第二鼓膜与内耳中鼓阶相隔\\\",\\\"中耳向前借咽鼓管通向口咽部\\\",\\\"中耳内粘膜与乳突窦和乳突小房内的粘膜不相连续\\\"]','c'),(13,1,'在鼓室内侧壁可以看到以下结构','[\\\"外半规管凸和鼓膜张肌半管\\\",\\\"面神经管凸和岬\\\",\\\"鼓膜张肌半管和锥隆起\\\",\\\"前庭窗和蜗顶\\\",\\\"蜗窗和蜗顶\\\"]\\r\\n','b'),(14,1,'听骨链由外向内依次是','[\\\"锤骨、砧骨、镫骨\\\",\\\"锤骨、镫骨、砧骨\\\",\\\"砧骨、锤骨、镫骨\\\",\\\"砧骨、镫骨、锤骨\\\",\\\"镫骨、砧骨、锤骨\\\"]\\r\\n','a'),(15,1,'由前内向后外，膜迷路依次分为','[\\\"膜半规管、椭圆囊、球囊和蜗管\\\",\\\"膜半规管、球囊、椭圆囊和蜗管\\\",\\\"蜗管、球囊、椭圆囊和膜半规管\\\",\\\"蜗管、椭圆囊、球囊和膜半规管\\\",\\\"蜗管、球囊、膜半规管和椭圆囊\\\"]','c'),(16,1,'在鼓室前壁可以看到以下结构：','[\\\"鼓膜张肌半管开口和咽鼓管鼓室口\\\",\\\"咽鼓管鼓室口和乳突窦开口\\\",\\\"乳突窦开口\\\",\\\"面神经管凸和咽鼓管鼓室口\\\",\\\"面神经管凸和鼓膜张肌半管开口\\\"]','a'),(17,1,'女性患者，50岁，近期出现眩晕，伴有恶心，呕吐，诊断为耳源性眩晕，病变累及部位可能是：','[\\\"中耳\\\",\\\"骨迷路\\\",\\\"骨半规管\\\",\\\"膜迷路\\\",\\\"基底膜\\\"]','d'),(18,1,'前往高原地带旅游时，耳部常常出现不适，可通过嚼口香糖减轻耳部不适，下列相关分析正确的是：','[\\\"通常咽鼓管咽口处于关闭状态，嚼口香糖使咽口开放\\\",\\\"通常咽鼓管鼓室口处于关闭状态，嚼口香糖使鼓室口开放\\\",\\\"通常咽鼓管鼓室口和咽口处于关闭状态，嚼口香糖使咽鼓管开放\\\",\\\"开放咽鼓管，是鼓室内压力升高\\\",\\\"以上分析都不对\\\"]','a'),(19,1,'某患儿，1岁，感冒后高烧不退，并出现抓耳摇头现象，诊断为急性中耳炎，感冒导致急性中耳炎的解剖学基础是：','[\\\"咽鼓管是鼓室和鼻咽部之间的交通\\\",\\\"咽鼓管是鼓室和口咽部之间的交通\\\",\\\"鼓膜张肌半管是鼓室和口咽部的交通\\\",\\\"鼓室与外耳道相隔\\\",\\\"鼓室与乳突窦和乳突小房相通\\\"]','a'),(20,1,'下列有关骨迷路的描述中，正确的选项是：','[\\\"后半规管和外半规管的单骨脚汇合形成总骨脚\\\",\\\"前半规管弓向前方，后半规管弓向后方，外半规管弓向后外\\\",\\\"前庭阶内容纳外淋巴，鼓阶内容纳内淋巴\\\",\\\"前庭阶和鼓阶不相通\\\",\\\"前庭内侧壁为内耳道的底\\\"]','e');\r\n/*!40000 ALTER TABLE `examination` ENABLE KEYS */;\r\nUNLOCK TABLES;\r\n\r\n--\r\n-- Table structure for table `examination_info`\r\n--\r\n\r\nDROP TABLE IF EXISTS `examination_info`;\r\n/*!40101 SET @saved_cs_client     = @@character_set_client */;\r\n/*!50503 SET character_set_client = utf8mb4 */;\r\nCREATE TABLE `examination_info` (\r\n  `id` int(11) NOT NULL,\r\n  `exam_id` int(11) DEFAULT NULL,\r\n  `userid` int(11) DEFAULT NULL,\r\n  `myanswer` varchar(100) DEFAULT NULL,\r\n  `score` int(11) DEFAULT NULL,\r\n  `submit_time` datetime DEFAULT NULL,\r\n  PRIMARY KEY (`id`),\r\n  KEY `examination_info_userid_IDX` (`userid`) USING BTREE\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;\r\n/*!40101 SET character_set_client = @saved_cs_client */;\r\n\r\n--\r\n-- Dumping data for table `examination_info`\r\n--\r\n\r\nLOCK TABLES `examination_info` WRITE;\r\n/*!40000 ALTER TABLE `examination_info` DISABLE KEYS */;\r\n/*!40000 ALTER TABLE `examination_info` ENABLE KEYS */;\r\nUNLOCK TABLES;\r\n/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;\r\n\r\n/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;\r\n/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;\r\n/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;\r\n/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;\r\n/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;\r\n/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;\r\n/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;\r\n\r\n-- Dump completed on 2024-04-16 11:06:23\r\n");
    
            //
            var exam_info_list = Db.Queryable<examination>()
                .LeftJoin<examination_info>((em, info) => em.id == info.exam_id
                && info.userid == 1)
                .Select((em, info) => new examination_info
                {
                    id = info.id,
                    exam_id = em.id,
                    myanswer = info.myanswer,
                    score = info.score,
                    submit_time = info.submit_time,
                    userid = info.userid,
                    exam = new examination
                    {
                        id = em.id,
                        answer = em.answer,

                        options = em.options,
                        qtype = em.qtype,
                        question = em.question
                    }

                })
                .ToList();
            Db.DbMaintenance.TruncateTable<examination>();
            if (exam_info_list.First().exam.options == null) 
            {
                throw new Exception("unit error");
            }
        }

        public static SqlSugarClient GetSqlSugarClient()
        {
            //string connstr = ConfigurationManager.ConnectionStrings["dbconnstr"].ConnectionString;
            string connstr = "server=localhost;Database=SqlSugar5xTest;Uid=root;Pwd=123456;AllowLoadLocalInfile=true";
            //创建数据库对象 (用法和EF Dappper一样通过new保证线程安全)
            SqlSugarClient Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = connstr,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            },
            db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //获取原生SQL推荐 5.1.4.63  性能OK
                    //Console.WriteLine(UtilMethods.GetNativeSql(sql, pars));
                    //WebLogger.Debug(UtilMethods.GetNativeSql(sql, pars));
                    //获取无参数化SQL 对性能有影响，特别大的SQL参数多的，调试使用
                    Console.WriteLine(UtilMethods.GetSqlString(DbType.SqlServer, sql, pars));
                };

            });
            return Db;
        }
    }
}
