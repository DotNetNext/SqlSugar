using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Updateable()
        {
            Db.CodeFirst.InitTables(typeof(UnitUser));
            Db.DbMaintenance.TruncateTable<UnitUser>();
            Db.Insertable(new UnitUser() { USER_ID = 1, USER_ACCOUNT = "a", USER_PWD = "b", USER_NAME = "c", PWD_LASTCHTIME = DateTime.Now, PWD_ERRORCOUNT = 1, PWD_LASTERRTIME = DateTime.Now }).ExecuteCommand();
            Db.Updateable(new UnitUser() { USER_ID = 1, PWD_LASTERRTIME = null }).WhereColumns(it => new { it.PWD_ERRORCOUNT, it.PWD_LASTERRTIME }).ExecuteCommand();
            Db.CodeFirst.InitTables(typeof(UnitBoolTest));
            var x = new UnitBoolTest();
            Db.Updateable<UnitBoolTest>().SetColumns(it => new UnitBoolTest() { BoolValue = !it.BoolValue }).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>().SetColumns(it => it.BoolValue == !it.BoolValue).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>().SetColumns(it => new UnitBoolTest() { BoolValue = x.BoolValue }).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>().SetColumns(it => it.BoolValue == x.BoolValue).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>().SetColumns(it => new UnitBoolTest() { BoolValue = !x.BoolValue }).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>().SetColumns(it => it.BoolValue == !x.BoolValue).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<UnitBoolTest>(x).ReSetValue(it => it.BoolValue == it.BoolValue).ExecuteCommand();
            Db.Updateable<UnitBoolTest>(x).ReSetValue(it => it.BoolValue == true).ExecuteCommand();
            Db.Updateable<UnitBoolTest>(x).ReSetValue(it => it.BoolValue == !it.BoolValue).ExecuteCommand();
            Db.Updateable<UnitBoolTest>(x).UpdateColumns(it => new { it.BoolValue }).ExecuteCommand();



            UnitSaveDiary saveDiary = new UnitSaveDiary();
            saveDiary.ID = 2;
            saveDiary.TypeID = 10;
            saveDiary.TypeName = "类型100";
            saveDiary.Title = "标题1000";
            saveDiary.Content = "内容";
            saveDiary.Time = DateTime.Now;
            saveDiary.IsRemind = false;//无论传false/true 最终执行的结果都是以true执行的

            var sql = Db.Updateable<UnitDiary>().SetColumns(it => new UnitDiary()
            {
                IsRemind = saveDiary.IsRemind,
            }).Where(it => it.ID == saveDiary.ID).ToSql();
            UValidate.Check(sql.Key, @"UPDATE [Diary]  SET
            [IsRemind] =  @Const0    WHERE ( [ID] = @ID1 )", "Updateable");


            sql = Db.Updateable<UnitDiary>().SetColumns(it => new UnitDiary()
            {
                TypeID = saveDiary.TypeID,
            }).Where(it => it.ID == saveDiary.ID).ToSql();
            UValidate.Check(sql.Key, @"UPDATE [Diary]  SET
            [TypeID] = @Const0   WHERE ( [ID] = @ID1 )", "Updateable");


            sql = Db.Updateable<UnitDiary>().SetColumns(it => new UnitDiary()
            {
                TypeID = saveDiary.TypeID,
            }).Where(it => it.ID == saveDiary.ID).ToSql();
            UValidate.Check(sql.Key, @"UPDATE [Diary]  SET
            [TypeID] = @Const0   WHERE ( [ID] = @ID1 )", "Updateable");

            sql = Db.Updateable<NullTest>().SetColumns(it => new NullTest()
            {
                p = true

            }).Where(it => it.id == 1).ToSql();
            UValidate.Check(sql.Key, @"UPDATE [NullTest]  SET
            [p] =  @Const0    WHERE ( [id] = @id1 )", "Updateable");
            sql = Db.Updateable<NullTest>().SetColumns(it => new NullTest()
            {
                p2 = true

            }).Where(it => it.id == 1).ToSql();
            UValidate.Check(sql.Key, @"UPDATE [NullTest]  SET
            [p2] = @Const0   WHERE ( [id] = @id1 )", "Updateable");


            Db.Updateable<Order>()
                .SetColumns(it => it.Name == "a")
                .SetColumns(it => it.CreateTime == DateTime.Now)
                .SetColumns(it => it.Price == 1).Where(it => it.Id == 1).ExecuteCommand();


            Db.Updateable<Order>()
               .SetColumns(it => new Order { Name = "a", CreateTime = DateTime.Now })
               .SetColumns(it => it.Price == 1).Where(it => it.Id == 1).ExecuteCommand();



            Db.Updateable<Order>()
             .SetColumns(it => new Order { Name = "a", CreateTime = DateTime.Now })
             .SetColumns(it => new Order() { Price = 1 }).Where(it => it.Id == 1).ExecuteCommand();
 
            Db.Updateable<Order>()
           .SetColumns(it => new Order { Name= it.Id>0  ?"1":"2", CreateTime = DateTime.Now })
            .Where(it => it.Id == 1).ExecuteCommand();

            Db.Updateable<Order>()
           .SetColumns(it => new Order { Name = SqlFunc.IsNull(it.Name,"a")+"b", CreateTime = DateTime.Now })
           .Where(it => it.Id == 1).ExecuteCommand();

            Db.CodeFirst.InitTables<Unitbluecopy>();
            Db.Insertable(new Unitbluecopy()).UseSqlServer().ExecuteBlueCopy();
        }
    }

    public class Unitbluecopy
    {
        public int Id { get; set; }
        [SugarColumn(IsNullable =true)]
        public decimal? x { get; set; }
    }

    public class NullTest
    {
        public int id { get; set; }
        public bool? p { get; set; }
        public bool p2 { get; set; }

    }
    public class UnitSaveDiary
    {
        public int ID { get; set; }
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? Time { get; set; }
        public bool IsRemind { get; set; }
    }
    /// <summary>
    /// 日记表
    /// </summary>
    [SugarTable("Diary")]
    public class UnitDiary
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int ID { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? UserID { get; set; }
        /// <summary>
        /// 日记类型ID
        /// </summary>
        public int? TypeID { get; set; }
        /// <summary>
        /// 日记类型名称
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime? Time { get; set; }
        /// <summary>
        /// 是否提醒
        /// </summary>
        public bool? IsRemind { get; set; }
        /// <summary>
        /// 封面图
        /// </summary>
        public string Cover { get; set; }
        /// <summary>
        /// 是否为系统日记 1:系统日记 0:用户日记
        /// </summary>
        public bool? IsSystem { get; set; }
        /// <summary>
        /// 权重(排序)
        /// </summary>
        public int? Sequence { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsDelete { get; set; }
    }

    public class UnitBoolTest
    {
        [SugarColumn(IsPrimaryKey =true)]
        public int Id { get; set; }
        public bool BoolValue { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// 普通用户表
    /// </summary>
    [Serializable]
    public class UnitUser 
    {
        private System.Int64? _USER_ID;
        /// <summary>
        /// GUID主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public System.Int64? USER_ID { get { return this._USER_ID; } set { this._USER_ID = value; } }

        private System.String _USER_ACCOUNT;
        /// <summary>
        /// 用户账号，不可重名，即使是假删除了，亦不可重复
        /// </summary>
        public System.String USER_ACCOUNT { get { return this._USER_ACCOUNT; } set { this._USER_ACCOUNT = value; } }

        private System.String _USER_PWD;
        /// <summary>
        /// 用户密码
        /// </summary>
        public System.String USER_PWD
        {
            get { return this._USER_PWD; }
            set { this._USER_PWD = value; }
        }
        /// <summary>
        /// 不允许用户密码序列化，可以反序列化
        /// </summary>
        /// <returns></returns>
        public bool ShouldSerializeUSER_PWD()
        {
            return false;
        }

        private System.String _USER_NAME;
        /// <summary>
        /// 用户姓名
        /// </summary>

        public System.String USER_NAME { get { return this._USER_NAME; } set { this._USER_NAME = value; } }

        private System.Int32 _USER_STATUS;
        /// <summary>
        /// 用户状体：10正常；20锁定；99已删除
        /// </summary>
        public System.Int32 USER_STATUS { get { return this._USER_STATUS; } set { this._USER_STATUS = value; } }

        private System.DateTime _PWD_LASTCHTIME;
        /// <summary>
        /// 最后一次密码更新时间
        /// </summary>
        public System.DateTime PWD_LASTCHTIME { get { return this._PWD_LASTCHTIME; } set { this._PWD_LASTCHTIME = value; } }

        private System.Int32? _PWD_ERRORCOUNT;
        /// <summary>
        /// 密码错误次数，达到定义的次数就锁定
        /// </summary>
		public System.Int32? PWD_ERRORCOUNT { get { return this._PWD_ERRORCOUNT; } set { this._PWD_ERRORCOUNT = value ?? 0; } }

        private System.DateTime? _PWD_LASTERRTIME = null;
        /// <summary>
        /// 密码最后一次错误时间，满足定义的时间差之后，可自动解锁，如20分钟后自动解锁，亦作为累次错误次数的时间差比对基础
        /// 允许为空
        /// </summary>
        public System.DateTime? PWD_LASTERRTIME { get { return this._PWD_LASTERRTIME; } set { this._PWD_LASTERRTIME = value; } }
    }
}
