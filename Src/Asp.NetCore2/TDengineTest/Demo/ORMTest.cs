﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks; 
using SqlSugar;
using SqlSugar.DbConvert;

namespace TDengineTest
{
    public partial class ORMTest
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

            //固定建表
            CodeFirst(db);

            //自动建表(普通插入)
            InsertUsingTag(db);

            //自动建表(BulkCopy)
            BulkCopy(db);
             
            //生成实体
            //DbFirst(db);

            //简单用例
            Demo1(db);

            //测试用例 （纳秒、微妙、类型）
            UnitTest(db);

            Console.WriteLine("执行完成");
            Console.ReadKey();
        } 
    }

}
