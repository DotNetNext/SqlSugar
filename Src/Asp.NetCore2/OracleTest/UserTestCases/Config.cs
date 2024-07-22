using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Config
    {
        public static string ConnectionString = DbHelper.Connection;
        public static string ConnectionString2 = ConnectionString;
        public static string ConnectionString3 = ConnectionString;
    }
}
