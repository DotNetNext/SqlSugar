using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET7Test
{
    public class Userinfo021
    {
        [SqlSugar.SugarColumn( IsIdentity =true)]
        public int id { get; set; }
        public string user_name { get; set; }
        public string pwd { get; set; }
        public int create_user_id { get; set; }
        public DateTime gmt_modified { get; set; }
        public bool deleted { get; set; }
    }

}
