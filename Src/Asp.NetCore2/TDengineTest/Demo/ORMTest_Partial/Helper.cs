using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using TDengineDriver;

namespace TDengineTest
{
    public partial class ORMTest
    {

        private static List<MyTable02> GetInsertDatas()
        {
            return new List<MyTable02>() {
            new MyTable02()
            {
                ts = DateTime.Now.AddDays(-1),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试1",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
             new MyTable02()
            {
                ts = DateTime.Now.AddDays(-2),
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = false,
                name = "测试2",
                location = "false",
                phase = Convert.ToSingle(1.1),
                voltage = 222
            },
                new MyTable02()
            {
                ts = DateTime.Now,
                current = Convert.ToSingle(1.1),
                groupId = 1,
                isdelete = true,
                name = "测试3",
                location = "true",
                phase = Convert.ToSingle(1.1),
                voltage = 111
            }
            };
        }
        public class fc_data
        {
            public DateTime upload_time { get; set; }

            public int voltage { get; set; }

            public float temperature { get; set; }

            public int data_id { get; set; }


            public string speed_hex { get; set; }


            public string gateway_mac { get; set; }


            public int ruminate { get; set; }

            public sbyte rssi { get; set; }

        }
        public class MyTable02
        {
            [SugarColumn(IsPrimaryKey = true)]
            public DateTime ts { get; set; }
            public float current { get; set; }
            public bool isdelete { get; set; }
            public string name { get; set; }
            public int voltage { get; set; }
            public float phase { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
            public string location { get; set; }
            [SugarColumn(IsOnlyIgnoreInsert = true, IsOnlyIgnoreUpdate = true)]
            public int groupId { get; set; }
        }
    }
}
