using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TDengineTest
{
    public partial class ORMTest
    {


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
    }
}
