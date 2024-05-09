using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using static OrmTest.NewUnitTest;
 
using System.Xml.Linq;

namespace OrmTest
{
    internal class UnitUpdateNavOneToOneFalse
    {
        public static void Init()
        {




            var Db = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = DbType.MySql,
                ConnectionString = Config.ConnectionString,
                IsAutoCloseConnection = true
            }, db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.Write(UtilMethods.GetSqlString(DbType.MySql, sql, pars));
                    Console.WriteLine();
                    Console.WriteLine();
                };
            });

            Db.Ado.ExecuteCommand(@"
DROP TABLE IF EXISTS `school0001`;
            CREATE TABLE `school0001`  (
  `Id` bigint NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY(`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

            INSERT INTO `school0001` VALUES(1001, '小学');

            DROP TABLE IF EXISTS `student0001`;
            CREATE TABLE `student0001`  (
  `Id` bigint NOT NULL,
  `SchoolId` bigint NOT NULL,
  `Name` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsDeleted` bit(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY(`Id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

            INSERT INTO `student0001` VALUES(1, 1001, '软删除', b'1');
            INSERT INTO `student0001` VALUES(2, 1001, '真数据', b'0');");

            //主表一条数据，子表两条数据（同一个SchoolId，其中一条IsDeleted = 1）

            Db.QueryFilter.AddTableFilter<Student>(x => x.IsDeleted == false);

            var entity = new School { Id = 1001, Name = "大学" };
            entity.StudentInfo = new Student { Id = 2, SchoolId = 1001, Name = "导航更新" };

            try
            {
                Db.UpdateNav(entity, new UpdateNavRootOptions { IsInsertRoot = true })
              .Include(x => x.StudentInfo).ExecuteCommand();

                var list=Db.Queryable<School>().Includes(X => X.StudentInfo).ToList();
                var student= Db.Queryable<Student>().ToList();
                if (student.Count != 1) 
                {
                    throw new Exception("unit error");
                }
            }
            catch (Exception)
            {
                throw;
            }

            Console.WriteLine();



        }

        [SugarTable("school0001")]
        public class School
        {
            [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }

            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }

            /// <summary>
            /// 从表
            /// </summary>
            [Navigate(NavigateType.OneToOne, nameof(Id), nameof(Student.SchoolId))]
            public Student StudentInfo { get; set; }
        }

        [SugarTable("student0001")]
        public class Student
        {
            [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
            public long Id { get; set; }

            [SugarColumn(ColumnName = "SchoolId")]
            public long SchoolId { get; set; }

            [SugarColumn(ColumnName = "Name")]
            public string Name { get; set; }

            [SugarColumn(ColumnName = "IsDeleted")]
            public bool IsDeleted { get; set; }
        }
    }
}
