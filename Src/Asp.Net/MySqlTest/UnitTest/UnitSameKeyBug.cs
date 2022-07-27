using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    internal class UnitSameKeyBug
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables(typeof(ServiceManage), typeof(UserInfo));
            db.DbMaintenance.TruncateTable<ServiceManage, UserInfo>();
            #region 第一次插入数据
            int a = db.Insertable(new UserInfo() { ContacAll = new List<ContactInfo>() { new ContactInfo() {  Name="a"} }, Name = "a" }).ExecuteCommand();
            int b = db.Insertable(new UserInfo() { ContacAll = new List<ContactInfo>() { new ContactInfo() { Name = "z" } }, Name = "b" }).ExecuteCommand();

            int c = db.Insertable(new ServiceManage() { ProjectName = "ceshi", userId = 1, careId = 2 }).ExecuteCommand();
            #endregion
            var list = db.Queryable<ServiceManage>()
                .LeftJoin<UserInfo>((s, ss) => s.userId == ss.Id)
                .LeftJoin<UserInfo>((s, ss, sss) => s.careId == sss.Id)
                .Select((s, ss, sss) => new ServiceManagementViewModel
                { serviceManage = s, userInfo = ss, careUserInfo=sss  }).ToList();
            if (!list.First().careUserInfo.ContacAll.Any()) 
            {
                throw new Exception("unit error");
            }

        }

    }
    public class UserInfo
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [SugarColumn(Length = 20, IsNullable = true)]
        public string  Name { get; set; }
        [SugarColumn(IsJson = true)]
        public List<ContactInfo> ContacAll { get; set; }
    }
    public class ContactInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 关系
        /// </summary>
        public string Relationship { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 备用手机号
        /// </summary>
        public string Phone2 { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

    }
    public class ServiceManagementViewModel
    {
        public ServiceManage serviceManage { get; set; }
        public UserInfo userInfo { get; set; }
        public UserInfo careUserInfo { get; set; }
    }
    public class ServiceManage
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }

        [SugarColumn(Length = 20, IsNullable = true)]
        public string  ProjectName { get; set; }
        public int userId { get; set; }
        public int careId { get; set; }

    }
}
