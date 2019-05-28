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
            Db.CodeFirst.InitTables(typeof(SYS_USER));
            Db.DbMaintenance.TruncateTable<SYS_USER>();
            Db.Insertable(new SYS_USER() { USER_ID=1,USER_ACCOUNT = "a", USER_PWD = "b", USER_NAME = "c", PWD_LASTCHTIME = DateTime.Now, PWD_ERRORCOUNT = 1, PWD_LASTERRTIME = DateTime.Now }).ExecuteCommand();
            Db.Updateable(new SYS_USER() { USER_ID=1, PWD_LASTERRTIME = null }).WhereColumns(it=> new{ it.PWD_ERRORCOUNT, it.PWD_LASTERRTIME }).ExecuteCommand();
            Db.CodeFirst.InitTables(typeof(BoolTest));
            var x = new BoolTest();
            Db.Updateable<BoolTest>().SetColumns(it => new BoolTest() { BoolValue = !it.BoolValue }).Where(it=>it.Id==1).ExecuteCommand();
            Db.Updateable<BoolTest>().SetColumns(it => it.BoolValue == !it.BoolValue  ).Where(it=>it.Id==1).ExecuteCommand();
            Db.Updateable<BoolTest>().SetColumns(it => new BoolTest() { BoolValue = x.BoolValue }).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<BoolTest>().SetColumns(it => it.BoolValue == x.BoolValue).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<BoolTest>().SetColumns(it => new BoolTest() { BoolValue = !x.BoolValue }).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<BoolTest>().SetColumns(it => it.BoolValue == !x.BoolValue).Where(it => it.Id == 1).ExecuteCommand();
            Db.Updateable<BoolTest>(x).ReSetValue(it => it.BoolValue == it.BoolValue).ExecuteCommand();
            Db.Updateable<BoolTest>(x).ReSetValue(it => it.BoolValue == true).ExecuteCommand();
            Db.Updateable<BoolTest>(x).ReSetValue(it => it.BoolValue == !it.BoolValue).ExecuteCommand();
        }
    }

    public class BoolTest
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
    public class SYS_USER 
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
