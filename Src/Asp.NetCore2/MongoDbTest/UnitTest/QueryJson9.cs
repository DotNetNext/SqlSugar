using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SqlSugar.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
using System.ComponentModel;

namespace MongoDbTest
{
    public class QueryJson9
    {
        internal static void Init()
        {
            var db = DbHelper.GetNewDb();
            db.CodeFirst.InitTables<SysUser>();
            db.DbMaintenance.TruncateTable<SysUser>();

            InsertSampleUser(db);
            InsertSampleUser2(db);
            var val0 = db.Queryable<SysUser>().ToList();
            var val1 = db.Queryable<SysUser>().Where(it => it.Name.ToString().Contains("部")).ToList();
            if (val1.Count != 1) Cases.ThrowUnitError();
            var val2 = db.Queryable<SysUser>().Where(it => it.EntInfo.Any(x => x.DeptName.Any())).ToList();
            if (val2.Count != 1) Cases.ThrowUnitError();

            db.CodeFirst.InitTables<MinuteData>();
            var item = new Dictionary<string, MinuteDataItem>();
            item.Add("a", new MinuteDataItem() { Value=1 });
            item.Add("a2", new MinuteDataItem() { Value = 2 });
            db.DbMaintenance.TruncateTable<MinuteData>();
            db.Insertable(new MinuteData()
            {
                StationCode = "a",
                DataItems = item,
                CreateDateTime = DateTime.Parse("2025-09-15 00:00:01")
            }).ExecuteCommand();
            var list = db.Queryable<MinuteData>().First();
            if(list.DataItems.Count!=2) Cases.ThrowUnitError();
            if (list.CreateDateTime!= DateTime.Parse("2025-09-15 00:00:01")) Cases.ThrowUnitError();
        }

        private static void InsertSampleUser(SqlSugarClient db)
        {
            var user = new SysUser()
            {
                Name = "部",
                EntInfo = new List<SysEntDept>()
                {
                    new SysEntDept()
                    {
                        DeptId = new List<string>() { ObjectId.GenerateNewId().ToString() },
                        EntId = ObjectId.GenerateNewId().ToString(),
                        DeptName = new List<string>() { "研发部" },
                    }
                }
            };
            db.Insertable(user).ExecuteCommand();
        }

        private static void InsertSampleUser2(SqlSugarClient db)
        {
            var user = new SysUser()
            {
                Name = "xx",
                EntInfo = new List<SysEntDept>()
                {
                    new SysEntDept()
                    {
                        DeptId = new List<string>() { ObjectId.GenerateNewId().ToString() },
                        EntId = ObjectId.GenerateNewId().ToString(),
                        DeptName = null,
                    }
                }
            };
            db.Insertable(user).ExecuteCommand();
        }

        private static void InsertSampleUser3(SqlSugarClient db)
        {
            var user = new SysUser()
            {
                Name = "xx",
                EntInfo = new List<SysEntDept>()
                {
                    new SysEntDept()
                    {
                        DeptId = new List<string>() { ObjectId.GenerateNewId().ToString() },
                        EntId = ObjectId.GenerateNewId().ToString(),
                        DeptName =new List<string>(){  },
                    }
                }
            };
            db.Insertable(user).ExecuteCommand();
        }
        /// <summary>
        /// 用户表
        /// </summary>
        [SugarTable("sys_user", "用户表")]
        [Tenant("0")]
        public class SysUser : MongoDbBase
        {
            /// <summary>
            /// 用户类型（00系统用户）
            /// </summary>
            [SugarColumn(Length = 2, ColumnDescription = "用户类型（00系统用户）", DefaultValue = "00")]
            public string UserType { get; set; } = "00";

            public string Avatar { get; set; }

            [SugarColumn(Length = 50, ColumnDescription = "用户邮箱")]
            public string Email { get; set; }


            /// <summary>
            /// 手机号
            /// </summary>
            public string Phonenumber { get; set; }

            /// <summary>
            /// 用户性别（0男 1女 2未知）
            /// </summary>
            public int Sex { get; set; }

            /// <summary>
            /// 删除标志（0代表存在 2代表删除）
            /// </summary>
            [SugarColumn(DefaultValue = "0")]
            public int DelFlag { get; set; }

            /// <summary>
            /// 最后登录IP
            /// </summary>
            [SugarColumn(IsOnlyIgnoreInsert = true)]
            public string LoginIP { get; set; }

            /// <summary>
            /// 部门Id
            /// </summary>
            [SugarColumn(DefaultValue = default, ColumnDataType = nameof(ObjectId))]
            public string DeptId { get; set; }

            /// <summary>
            /// 企业集合
            /// </summary>
            [SugarColumn(IsJson = true)]
            public List<SysEntDept> EntInfo { get; set; } = new();


            /// <summary>
            /// 是否已删除
            /// </summary>
            public bool IsAvaliable { get; set; } = true;

            #region 表额外字段

            /// <summary>
            /// 拥有角色个数
            /// </summary>
            //[SugarColumn(IsIgnore = true)]
            //public int RoleNum { get; set; }
            [SugarColumn(IsIgnore = true)]
            public string DeptName { get; set; }


            [SugarColumn(IsIgnore = true)]
            public string WelcomeMessage
            {
                get
                {
                    int now = DateTime.Now.Hour;

                    if (now > 0 && now <= 6)
                    {
                        return "午夜好";
                    }
                    else if (now > 6 && now <= 11)
                    {
                        return "早上好";
                    }
                    else if (now > 11 && now <= 14)
                    {
                        return "中午好";
                    }
                    else if (now > 14 && now <= 18)
                    {
                        return "下午好";
                    }
                    else
                    {
                        return "晚上好";
                    }
                }
            }

            [SugarColumn(IsIgnore = true)] public string WelcomeContent { get; set; }

            /// <summary>
            /// 角色id集合
            /// </summary>
            [SugarColumn(IsIgnore = true)]
            public List<string> RoleIds { get; set; }
            public string Name { get;   set; }

            #endregion
        }


        /// <summary>
        /// 用户表里企业与部门信息
        /// </summary>
        public class SysEntDept
        {
            /// <summary>
            /// 企业Id
            /// </summary>
            [BsonRepresentation(BsonType.ObjectId)]
            [SugarColumn(ColumnDataType = nameof(ObjectId))]
            public string EntId { get; set; }

            /// <summary>
            /// 企业名称
            /// </summary>
            public string EntName { get; set; }

            /// <summary>
            /// 部门Id集合
            /// </summary>                
            [BsonRepresentation(BsonType.ObjectId)]
            [SugarColumn(IsJson = true, ColumnDataType = nameof(ObjectId))]
            public List<string> DeptId { get; set; } = new();

            /// <summary>
            /// 部门名称集合
            /// </summary>        
            [SugarColumn(IsJson = true)]
            public List<string> DeptName { get; set; } = new();

            /// <summary>
            /// 部门备注集合
            /// </summary>        
            [SugarColumn(IsJson = true)]
            public List<string> DeptRmk { get; set; } = new();
        }

        [SugarTable("d_minute")]
        public class MinuteData : MongoDbBase
        {
            public string StationCode { get; set; }
            public DateTime CreateDateTime { get; set; }

            [SugarColumn(IsJson = true)]
            public Dictionary<string, MinuteDataItem> DataItems { get; set; }
        }

        public class MinuteDataItem
        {
            public double Value { get; set; }
            public string Flag { get; set; }
        }

    }
}