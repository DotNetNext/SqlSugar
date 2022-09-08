using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
    [SqlSugar.SugarTable("users")]
    public class Users
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long Sid { get; set; } 

        public DateTime  createtime { get; set; }
        public string username { get; set; }

        public string password { get; set; }
    }
}
