using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class Unitafadsa
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<School>();
            db.CodeFirst.InitTables<School2>();
            db.Queryable<School2>().ToList();
        }
       
         
        /// <summary>
        /// 多主键表
        /// </summary>
        [SugarTable("dbschool")]
        public class School  
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }

            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }
        }
        /// <summary>
        /// 多主键表
        /// </summary>
        [SugarTable("dbschool")]
        public class School2
        {
            [SugarColumn(IsPrimaryKey = true)]
            public string Id { get; set; }

            [SugarColumn(IsPrimaryKey = true)]
            public int SchoolId { get; set; }

            public string Remark { get; set; }
        }
    }
}
