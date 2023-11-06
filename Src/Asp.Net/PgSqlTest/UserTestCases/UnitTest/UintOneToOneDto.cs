using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UintOneToOneDto
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityService = (s, y) => 
                {
                    if (y.IsPrimarykey != true && y.IsIdentity != true) 
                    {
                        y.IsNullable = true;
                    }
                }
            }; 
            //生成表
            db.CodeFirst.SetStringDefaultLength(50).InitTables<SysUser>();
            db.CodeFirst.SetStringDefaultLength(50).InitTables<SignInRecord>();
            db.DbMaintenance.TruncateTable<SysUser, SignInRecord>();
            //生成数据
            SysUser sysuser = new SysUser()
            {
                Id = 1,
                RealName = "Test",
                Age = 20,
                Phone = "1311111111",
                IdCardNum = "1111",
                Nation = "中国"
            };

            SignInRecord signInRecord = new SignInRecord()
            {
                UserId = 1,
                SignInDate =  (DateTime.Now),
                MorningSignInTime = DateTime.Now,
                MorningSignInAddress = "Addr",
                MorningSignInResult = SignInResultEnum.SignedIn,
                AfternoonSignInTime = DateTime.Now,
                AfternoonSignInAddress = "Addr",
                AfternoonSignInResult = SignInResultEnum.Late,
            };

            db.DbMaintenance.TruncateTable<SysUser>();
            db.DbMaintenance.TruncateTable<SignInRecord>();
            db.Insertable(sysuser).ExecuteCommand();
            db.Insertable(signInRecord).ExecuteCommand();

            //查询测试
            var query = db.Queryable<SignInRecord>()
            //**** Bug1， 当有x => x.sysUser.ToList(it => 时，同时使用Select的话，sysUser不会赋值。去掉Select则可以。

           // .Includes<SysUser>(x => x.sysUser)
            .Includes<SysUser>(x => x.sysUser.ToList(it=>new SysUser() {  Nation=it.Nation }))
            .GroupBy(g => new { g.UserId })
            .Select(it => new SignInRecordOutput
            {
                UserId = it.UserId,
                sysUser = it.sysUser,

                SignInCount = SqlFunc.AggregateCount(SqlFunc.IF(it.MorningSignInResult == SignInResultEnum.SignedIn || it.AfternoonSignInResult == SignInResultEnum.SignedIn).Return(1).End<int>()),
                LateCount = SqlFunc.AggregateCount(SqlFunc.IF(it.MorningSignInResult == SignInResultEnum.Late || it.AfternoonSignInResult == SignInResultEnum.Late).Return(1).End<int>()),
                LeaveCount = SqlFunc.AggregateCount(SqlFunc.IF(it.MorningSignInResult == SignInResultEnum.Leave || it.AfternoonSignInResult == SignInResultEnum.Leave).Return(1).End<int>()),
            }) .ToList();

            if (query.First().sysUser == null) 
            {
                throw new Exception("unit error");
            }
        }

    }
}
