using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;


namespace TDengineTest
{
    public partial class ORMTest
    {
        public static void DbFirst(SqlSugarClient db)
        {
            db.DbFirst.CreateClassFile("c:\\Demo\\11", "Models");
        }
    }
}
