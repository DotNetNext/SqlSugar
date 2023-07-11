using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace OrmTest
{
    internal class UBulkCopy2
    {
        public static void Init()
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<tdetonator>();
            List<tdetonator> list = new List<tdetonator>();

            int num = 0;

            int index = 1000000;
            db.DbMaintenance.TruncateTable<tdetonator>();
            for (int x = 0; x < 1; x++)

            {

                for (int i = 0; i < 1000000; i++)

                {

                    index++;

                    tdetonator tdetonator = new tdetonator();

                    tdetonator.TClientTask_Id = new Guid("000c1c96-065d-11ee-b3b8-6c3c8c6a4066");

                    //tdetonator.Id = NewId.Next().ToGuid();

                    tdetonator.Id = SnowFlakeSingle.Instance.NextId();

                    tdetonator.Uid = "00000000000011" + index;

                    tdetonator.Gsm = "0000000000011";

                    tdetonator.ProductionTime = DateTime.Now;

                    tdetonator.ClientId = 0;

                    tdetonator.Password1 = "000000";

                    tdetonator.Password2 = "000000";

                    tdetonator.Password3 = "000000";

                    tdetonator.DefaultDelay = 99;

                    tdetonator.DefaultHolgTag = 0;

                    tdetonator.CStatus = true;

                    tdetonator.PStatus = true;

                    tdetonator.TStatus = true;

                    tdetonator.Key1Value = 55;

                    tdetonator.Key2Value = 55;

                    tdetonator.Key3Value = 55;

                    tdetonator.Key4Value = 55;

                    tdetonator.Key5Value = 55;

                    tdetonator.Key6Value = 55;

                    tdetonator.Key7Value = 55;

                    tdetonator.Key8Value = 55;

                    tdetonator.Key9Value = 55;

                    tdetonator.Key10Value = 55;

                    tdetonator.Key11Value = 55;

                    tdetonator.Key12Value = 55;

                    tdetonator.Key13Value = 55;

                    tdetonator.Key14Value = 55;

                    tdetonator.Key15Value = 55;

                    tdetonator.Key16Value = 55;

                    tdetonator.Key17Value = 55;

                    tdetonator.Key18Value = 55;

                    tdetonator.Key19Value = 55;

                    tdetonator.Key20Value = 55;

                    tdetonator.Key21Value = 55;

                    tdetonator.Key22Value = 55;

                    tdetonator.Key23Value = 55;

                    tdetonator.Key24Value = 55;

                    tdetonator.ErrorCode = 1;

                    tdetonator.ErrorMessage = "正常";

                    tdetonator.Synchronization = false;

                    tdetonator.RowStatus = 0;

                    tdetonator.JobNumber = 0;

                    tdetonator.OldFid = "00000000";

                    tdetonator.OldUid = "00000000";

                    tdetonator.OldDefaultDelay = 0;

                    tdetonator.OldDefaultHolgTag = 0;

                    tdetonator.NewFid = "00000000";

                    list.Add(tdetonator);



                } 
        
            }
            db.Fastest<tdetonator>().PageSize(100000).BulkCopy(list);
        }
        public class tdetonator

        {



            /// <summary>

            /// 

            /// </summary>

            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]

            public long Id { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string Uid { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public DateTime ProductionTime { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string Password1 { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string Password2 { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string Password3 { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int DefaultDelay { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int DefaultHolgTag { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public bool? CStatus { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public bool? PStatus { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public bool? TStatus { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key1Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key2Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key3Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key4Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key5Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key6Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key7Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key8Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key9Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key10Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key11Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key12Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key13Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key14Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key15Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key16Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int ErrorCode { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string ErrorMessage { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public bool Synchronization { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public Guid? TUser_Id { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public Guid? TClientTask_Id { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int ClientId { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public DateTime? SyncTime { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key17Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key18Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key19Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key20Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key21Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key22Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key23Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int Key24Value { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int RowStatus { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string BatchCode { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public bool Complement { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string Gsm { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string RowStatusRemark { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int JobNumber { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string OldUid { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int OldDefaultDelay { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public int OldDefaultHolgTag { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string OldFid { get; set; }



            /// <summary>

            /// 

            /// </summary>

            public string NewFid { get; set; }

        }


    }
}
