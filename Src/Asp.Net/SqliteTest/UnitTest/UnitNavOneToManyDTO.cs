using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    internal class UnitNavOneToManyDTO
    {
        public static void Init() 
        {
            Test(NewUnitTest.Db).GetAwaiter().GetResult();
        }
        private static async Task Test(ISqlSugarClient db)
        {
            db.CodeFirst.InitTables<JyywEquityEntity, JyywHonorEntity, JyywMemberEntity>();
            db.DbMaintenance.TruncateTable<JyywEquityEntity, JyywHonorEntity, JyywMemberEntity>();

            db.CodeFirst.InitTables<JyywPeopleEntity, JyywReportEntity>();
            db.DbMaintenance.TruncateTable<JyywPeopleEntity, JyywReportEntity>();

            {

                List<JyywHonorEntity> jyywHonor = new List<JyywHonorEntity>();
                jyywHonor.Add(new JyywHonorEntity
                {
                    Id = "451911727693431045",
                    PeopleId = "3208101010101",
                    Date = DateTime.Now,
                    ProvideUnit = "授予单位",
                    Level = "市级",
                    CollectionUnit = "446454257080598469",
                });
                await db.Insertable(jyywHonor).ExecuteCommandAsync();

                List<JyywEquityEntity> jyywEquity = new List<JyywEquityEntity>();
                jyywEquity.Add(new JyywEquityEntity
                {
                    Id = "451911727668265221",
                    PeopleId = "3208101010101",
                    Type = "抚恤",
                    Date = DateTime.Now,
                    ProvideUnit = "局",
                    Level = "市级",
                    Money = 500,
                    CollectionUnit = "446454257080598469",
                });
                jyywEquity.Add(new JyywEquityEntity
                {
                    Id = "451911727668265222",
                    PeopleId = "3208101010101",
                    Type = "抚恤",
                    Date = DateTime.Now,
                    ProvideUnit = "局",
                    Level = "市级",
                    Money = 500,
                    CollectionUnit = "446454257080598469",
                });
                jyywEquity.Add(new JyywEquityEntity
                {
                    Id = "451911727668265223",
                    PeopleId = "3208101010101",
                    Type = "抚恤",
                    Date = DateTime.Now,
                    ProvideUnit = "局",
                    Level = "市级",
                    Money = 500,
                    CollectionUnit = "446454257080598469",
                });
                await db.Insertable(jyywEquity).ExecuteCommandAsync();

                List<JyywMemberEntity> jyywMember = new List<JyywMemberEntity>();
                jyywMember.Add(new JyywMemberEntity
                {
                    Id = "451911727714402565",
                    PeopleId = "3208101010101",
                    MemberName = "王",
                    Relationship = "母亲",
                    BirthDate = DateTime.Now,
                    Hometown = "上海青浦",
                    WorkUnit = "市局",
                    CollectionUnit = "446454257080598469",
                });
                jyywMember.Add(new JyywMemberEntity
                {
                    Id = "452011164222619589",
                    PeopleId = "3208101010101",
                    MemberName = "1",
                    Relationship = "父亲",
                    BirthDate = DateTime.Now,
                    Hometown = "2",
                    WorkUnit = "3",
                    CollectionUnit = "446454257080598469",
                });
                await db.Insertable(jyywMember).ExecuteCommandAsync();

                List<JyywPeopleEntity> jyywPeople = new List<JyywPeopleEntity>();
                jyywPeople.Add(new JyywPeopleEntity
                {
                    Id = "451911727638905093",
                    CollectionDate = DateTime.Now,
                    CollectionUnit = "446454257080598469",
                    Name = "王",
                    Status = "健在",
                    Sex = "男",
                    Nation = "汉族",
                    Hometown = "AAAA",
                    IdNumber = "3208101010101",
                    Phone = "15251711733",
                    WorkUnit = "公司",
                    Addr = "路",
                    BriefStoryTag = "[\"和歹徒搏斗\"]",
                    BriefStoryContent = "简要事迹",
                    SacrificeInjuryTag = "[\"和歹徒搏斗\",\"帮助困难\"]",
                    SacrificeInjuryContent = "牺牲或伤残情况",
                    PracticalDifficulties = "家庭生活实际困难",
                    BasicLife = "无",
                    MedicalTreatment = "无",
                    Employment = "无",
                    Education = "无",
                    Housing = "无",
                });
                jyywPeople.Add(new JyywPeopleEntity
                {
                    Id = "452011164168093637",
                    CollectionDate = DateTime.Now,
                    CollectionUnit = "446454257080598469",
                    Name = "王",
                    Status = "健在",
                    Sex = "男",
                    Nation = "汉族",
                    Hometown = "AAAA",
                    IdNumber = "3208101010101",
                    Phone = "15251711733",
                    WorkUnit = "公司",
                    Addr = "路",
                    BriefStoryTag = "[\"和歹徒搏斗\"]",
                    BriefStoryContent = "简要事迹",
                    SacrificeInjuryTag = "[\"和歹徒搏斗\",\"帮助困难\"]",
                    SacrificeInjuryContent = "牺牲或伤残情况",
                    PracticalDifficulties = "家庭生活实际困难",
                    BasicLife = "无",
                    MedicalTreatment = "无",
                    Employment = "无",
                    Education = "无",
                    Housing = "无",
                });
                await db.Insertable(jyywPeople).ExecuteCommandAsync();

                List<JyywReportEntity> jyywReport = new List<JyywReportEntity>();
                jyywReport.Add(new JyywReportEntity
                {
                    Id = "451911727731179781",
                    PeopleId = "3208101010101",
                    Date = DateTime.Now,
                    ProvideUnit = "电视台",
                    Level = "市级",
                    CollectionUnit = "446454257080598469",
                });
                await db.Insertable(jyywReport).ExecuteCommandAsync();
            }

  

            var data = await db.Queryable<JyywPeopleEntity>()
                .Includes(it => it.JyywEquityList)
                .Includes(it => it.JyywHonorList)
                .Includes(it => it.JyywMemberList)
                .Includes(it => it.JyywReportList)
                .Where(it => it.Id == "451911727638905093")
                .Select(it => new
                {
                    it = it.Id,
                    tableField142 = it.JyywEquityList,
                    tableField125 = it.JyywHonorList,
                    tableField131 = it.JyywMemberList,
                    tableField155 = it.JyywReportList,
                }).ToListAsync();

            if (data.First().it != "451911727638905093"
                || data.First().tableField125.Count == 0) 
            {
                throw new Exception("unit error");
            }
        }

        /// <summary>
        /// 权益保护实体.
        /// </summary>
        [SugarTable("jyyw_equity")]
        public class JyywEquityEntity
        {
            /// <summary>
            /// 主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 人员表主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_PeopleId", ColumnDescription = "人员表主键", IsNullable = true)]
            public string PeopleId { get; set; }

            /// <summary>
            /// 权益类型.
            /// </summary>
            [SugarColumn(ColumnName = "F_Type", ColumnDescription = "权益类型", IsNullable = true)]
            public string Type { get; set; }

            /// <summary>
            /// 发放时间.
            /// </summary>
            [SugarColumn(ColumnName = "F_Date", ColumnDescription = "发放时间", IsNullable = true)]
            public DateTime Date { get; set; }

            /// <summary>
            /// 发放单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_ProvideUnit", ColumnDescription = "发放单位", IsNullable = true)]
            public string ProvideUnit { get; set; }

            /// <summary>
            /// 级别.
            /// </summary>
            [SugarColumn(ColumnName = "F_Level", ColumnDescription = "级别", IsNullable = true)]
            public string Level { get; set; }

            /// <summary>
            /// 金额.
            /// </summary>
            [SugarColumn(ColumnName = "F_Money", ColumnDescription = "金额", IsNullable = true)]
            public decimal Money { get; set; }

            /// <summary>
            /// 备注.
            /// </summary>
            [SugarColumn(ColumnName = "F_Remark", ColumnDescription = "备注", IsNullable = true)]
            public string Remark { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionUnit", ColumnDescription = "采集单位", IsNullable = true)]
            public string CollectionUnit { get; set; }

        }

        /// <summary>
        /// 曾获荣誉实体.
        /// </summary>
        [SugarTable("jyyw_honor")]
        public class JyywHonorEntity
        {
            /// <summary>
            /// 主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 人员表主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_PeopleId", ColumnDescription = "人员表主键", IsNullable = true)]
            public string PeopleId { get; set; }

            /// <summary>
            /// 授予时间.
            /// </summary>
            [SugarColumn(ColumnName = "F_Date", ColumnDescription = "授予时间", IsNullable = true)]
            public DateTime Date { get; set; }

            /// <summary>
            /// 授予单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_ProvideUnit", ColumnDescription = "授予单位", IsNullable = true)]
            public string ProvideUnit { get; set; }

            /// <summary>
            /// 荣誉级别.
            /// </summary>
            [SugarColumn(ColumnName = "F_Level", ColumnDescription = "荣誉级别", IsNullable = true)]
            public string Level { get; set; }

            /// <summary>
            /// 备注.
            /// </summary>
            [SugarColumn(ColumnName = "F_Remark", ColumnDescription = "备注", IsNullable = true)]
            public string Remark { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionUnit", ColumnDescription = "采集单位", IsNullable = true)]
            public string CollectionUnit { get; set; }

        }

        /// <summary>
        /// 家庭成员实体.
        /// </summary>
        [SugarTable("jyyw_member")]
        public class JyywMemberEntity
        {
            /// <summary>
            /// 主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 人员表主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_PeopleId", ColumnDescription = "人员表主键", IsNullable = true)]
            public string PeopleId { get; set; }

            /// <summary>
            /// 家庭成员姓名.
            /// </summary>
            [SugarColumn(ColumnName = "F_MemberName", ColumnDescription = "家庭成员姓名", IsNullable = true)]
            public string MemberName { get; set; }

            /// <summary>
            /// 关系.
            /// </summary>
            [SugarColumn(ColumnName = "F_Relationship", ColumnDescription = "关系", IsNullable = true)]
            public string Relationship { get; set; }

            /// <summary>
            /// 出生年月.
            /// </summary>
            [SugarColumn(ColumnName = "F_BirthDate", ColumnDescription = "出生年月", IsNullable = true)]
            public DateTime BirthDate { get; set; }

            /// <summary>
            /// 籍贯.
            /// </summary>
            [SugarColumn(ColumnName = "F_Hometown", ColumnDescription = "籍贯", IsNullable = true)]
            public string Hometown { get; set; }

            /// <summary>
            /// 工作单位及职业.
            /// </summary>
            [SugarColumn(ColumnName = "F_WorkUnit", ColumnDescription = "工作单位及职业", IsNullable = true)]
            public string WorkUnit { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionUnit", ColumnDescription = "采集单位", IsNullable = true)]
            public string CollectionUnit { get; set; }

        }

        /// <summary>
        /// jyyw_people实体.
        /// </summary>
        [SugarTable("jyyw_people")]
        public class JyywPeopleEntity
        {
            /// <summary>
            /// 主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 采集时间.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionDate", ColumnDescription = "采集单位", IsNullable = true)]
            public DateTime CollectionDate { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionUnit", ColumnDescription = "采集单位", IsNullable = true)]
            public string CollectionUnit { get; set; }

            /// <summary>
            /// 姓名.
            /// </summary>
            [SugarColumn(ColumnName = "F_Name", ColumnDescription = "姓名", IsNullable = true)]
            public string Name { get; set; }

            /// <summary>
            /// 人员状态.
            /// </summary>
            [SugarColumn(ColumnName = "F_Status", ColumnDescription = "人员状态", IsNullable = true)]
            public string Status { get; set; }

            /// <summary>
            /// 性别.
            /// </summary>
            [SugarColumn(ColumnName = "F_Sex", ColumnDescription = "性别", IsNullable = true)]
            public string Sex { get; set; }

            /// <summary>
            /// 民族.
            /// </summary>
            [SugarColumn(ColumnName = "F_Nation", ColumnDescription = "民族", IsNullable = true)]
            public string Nation { get; set; }

            /// <summary>
            /// 籍贯.
            /// </summary>
            [SugarColumn(ColumnName = "F_Hometown", ColumnDescription = "籍贯", IsNullable = true)]
            public string Hometown { get; set; }

            /// <summary>
            /// 身份证号码.
            /// </summary>
            [SugarColumn(ColumnName = "F_IdNumber", ColumnDescription = "身份证号码", IsNullable = true)]
            public string IdNumber { get; set; }

            /// <summary>
            /// 手机号.
            /// </summary>
            [SugarColumn(ColumnName = "F_Phone", ColumnDescription = "手机号", IsNullable = true)]
            public string Phone { get; set; }

            /// <summary>
            /// 工作单位及职业(职务).
            /// </summary>
            [SugarColumn(ColumnName = "F_WorkUnit", ColumnDescription = "工作单位及职业(职务)", IsNullable = true)]
            public string WorkUnit { get; set; }

            /// <summary>
            /// 户籍及家庭住址.
            /// </summary>
            [SugarColumn(ColumnName = "F_Addr", ColumnDescription = "户籍及家庭住址", IsNullable = true)]
            public string Addr { get; set; }

            /// <summary>
            /// 简要事迹-标签.
            /// </summary>
            [SugarColumn(ColumnName = "F_BriefStoryTag", ColumnDescription = "简要事迹-标签", IsNullable = true)]
            public string BriefStoryTag { get; set; }

            /// <summary>
            /// 简要事迹-内容.
            /// </summary>
            [SugarColumn(ColumnName = "F_BriefStoryContent", ColumnDescription = "简要事迹-内容", IsNullable = true)]
            public string BriefStoryContent { get; set; }

            /// <summary>
            /// 在见义勇为中牺牲或伤残情况-标签.
            /// </summary>
            [SugarColumn(ColumnName = "F_SacrificeInjuryTag", ColumnDescription = "在见义勇为中牺牲或伤残情况-标签", IsNullable = true)]
            public string SacrificeInjuryTag { get; set; }

            /// <summary>
            /// 在见义勇为中牺牲或伤残情况-内容.
            /// </summary>
            [SugarColumn(ColumnName = "F_SacrificeInjuryContent", ColumnDescription = "在见义勇为中牺牲或伤残情况-内容", IsNullable = true)]
            public string SacrificeInjuryContent { get; set; }

            /// <summary>
            /// 家庭生活实际困难.
            /// </summary>
            [SugarColumn(ColumnName = "F_PracticalDifficulties", ColumnDescription = "家庭生活实际困难", IsNullable = true)]
            public string PracticalDifficulties { get; set; }

            /// <summary>
            /// 基本生活.
            /// </summary>
            [SugarColumn(ColumnName = "F_BasicLife", ColumnDescription = "基本生活", IsNullable = true)]
            public string BasicLife { get; set; }

            /// <summary>
            /// 医疗.
            /// </summary>
            [SugarColumn(ColumnName = "F_MedicalTreatment", ColumnDescription = "医疗", IsNullable = true)]
            public string MedicalTreatment { get; set; }

            /// <summary>
            /// 就业.
            /// </summary>
            [SugarColumn(ColumnName = "F_Employment", ColumnDescription = "就业", IsNullable = true)]
            public string Employment { get; set; }

            /// <summary>
            /// 教育.
            /// </summary>
            [SugarColumn(ColumnName = "F_Education", ColumnDescription = "教育", IsNullable = true)]
            public string Education { get; set; }

            /// <summary>
            /// 住房.
            /// </summary>
            [SugarColumn(ColumnName = "F_Housing", ColumnDescription = "住房", IsNullable = true)]
            public string Housing { get; set; }

            /// <summary>
            /// .
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(JyywEquityEntity.PeopleId), nameof(IdNumber))]
            public List<JyywEquityEntity> JyywEquityList { get; set; }

            /// <summary>
            /// .
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(JyywHonorEntity.PeopleId), nameof(IdNumber))]
            public List<JyywHonorEntity> JyywHonorList { get; set; }

            /// <summary>
            /// .
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(JyywMemberEntity.PeopleId), nameof(IdNumber))]
            public List<JyywMemberEntity> JyywMemberList { get; set; }

            /// <summary>
            /// .
            /// </summary>
            [Navigate(NavigateType.OneToMany, nameof(JyywReportEntity.PeopleId), nameof(IdNumber))]
            public List<JyywReportEntity> JyywReportList { get; set; }

        }

        /// <summary>
        /// 媒体宣传实体.
        /// </summary>
        [SugarTable("jyyw_report")]
        public class JyywReportEntity
        {
            /// <summary>
            /// 主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_Id", IsPrimaryKey = true)]
            public string Id { get; set; }

            /// <summary>
            /// 人员表主键.
            /// </summary>
            [SugarColumn(ColumnName = "F_PeopleId", ColumnDescription = "人员表主键", IsNullable = true)]
            public string PeopleId { get; set; }

            /// <summary>
            /// 宣传时间.
            /// </summary>
            [SugarColumn(ColumnName = "F_Date", ColumnDescription = "宣传时间", IsNullable = true)]
            public DateTime Date { get; set; }

            /// <summary>
            /// 媒体名称或频道.
            /// </summary>
            [SugarColumn(ColumnName = "F_ProvideUnit", ColumnDescription = "媒体名称或频道", IsNullable = true)]
            public string ProvideUnit { get; set; }

            /// <summary>
            /// 媒体等级.
            /// </summary>
            [SugarColumn(ColumnName = "F_Level", ColumnDescription = "媒体等级", IsNullable = true)]
            public string Level { get; set; }

            /// <summary>
            /// 备注.
            /// </summary>
            [SugarColumn(ColumnName = "F_Remark", ColumnDescription = "备注", IsNullable = true)]
            public string Remark { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            [SugarColumn(ColumnName = "F_CollectionUnit", ColumnDescription = "采集单位", IsNullable = true)]
            public string CollectionUnit { get; set; }

        }

        /// <summary>
        /// jyyw_people详情输出参数.
        /// </summary>
        public class JyywPeopleDetailOutput
        {
            /// <summary>
            /// 主键.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// 采集时间.
            /// </summary>
            public string collectionDate { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            public string collectionUnit { get; set; }

            /// <summary>
            /// 姓名.
            /// </summary>
            public string name { get; set; }

            /// <summary>
            /// 人员状态.
            /// </summary>
            public string status { get; set; }

            /// <summary>
            /// 性别.
            /// </summary>
            public string sex { get; set; }

            /// <summary>
            /// 民族.
            /// </summary>
            public string nation { get; set; }

            /// <summary>
            /// 籍贯.
            /// </summary>
            public string hometown { get; set; }

            /// <summary>
            /// 身份证号码.
            /// </summary>
            public string idNumber { get; set; }

            /// <summary>
            /// 手机号.
            /// </summary>
            public string phone { get; set; }

            /// <summary>
            /// 工作单位及职业(职务).
            /// </summary>
            public string workUnit { get; set; }

            /// <summary>
            /// 户籍及家庭住址.
            /// </summary>
            public string addr { get; set; }

            /// <summary>
            /// 简要事迹-标签.
            /// </summary>
            public string briefStoryTag { get; set; }

            /// <summary>
            /// 简要事迹-内容.
            /// </summary>
            public string briefStoryContent { get; set; }

            /// <summary>
            /// 在见义勇为中牺牲或伤残情况-标签.
            /// </summary>
            public string sacrificeInjuryTag { get; set; }

            /// <summary>
            /// 在见义勇为中牺牲或伤残情况-内容.
            /// </summary>
            public string sacrificeInjuryContent { get; set; }

            /// <summary>
            /// 家庭生活实际困难.
            /// </summary>
            public string practicalDifficulties { get; set; }

            /// <summary>
            /// 基本生活.
            /// </summary>
            public string basicLife { get; set; }

            /// <summary>
            /// 医疗.
            /// </summary>
            public string medicalTreatment { get; set; }

            /// <summary>
            /// 就业.
            /// </summary>
            public string employment { get; set; }

            /// <summary>
            /// 教育.
            /// </summary>
            public string education { get; set; }

            /// <summary>
            /// 住房.
            /// </summary>
            public string housing { get; set; }

            /// <summary>
            /// .
            /// </summary>
            public List<JyywEquityDetailOutput> tableField142 { get; set; }

            /// <summary>
            /// .
            /// </summary>
            public List<JyywHonorDetailOutput> tableField125 { get; set; }

            /// <summary>
            /// .
            /// </summary>
            public List<JyywMemberDetailOutput> tableField131 { get; set; }

            /// <summary>
            /// .
            /// </summary>
            public List<JyywReportDetailOutput> tableField155 { get; set; }
        }

        /// <summary>
        /// 权益保护详情输出参数.
        /// </summary>
        public class JyywEquityDetailOutput
        {
            /// <summary>
            /// 人员表主键.
            /// </summary>
            public string peopleId { get; set; }

            /// <summary>
            /// 权益类型.
            /// </summary>
            public string type { get; set; }

            /// <summary>
            /// 发放时间.
            /// </summary>
            public string date { get; set; }

            /// <summary>
            /// 发放单位.
            /// </summary>
            public string provideUnit { get; set; }

            /// <summary>
            /// 级别.
            /// </summary>
            public string level { get; set; }

            /// <summary>
            /// 金额.
            /// </summary>
            public decimal money { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            public string collectionUnit { get; set; }
        }

        /// <summary>
        /// 曾获荣誉详情输出参数.
        /// </summary>
        public class JyywHonorDetailOutput
        {
            /// <summary>
            /// 人员表主键.
            /// </summary>
            public string peopleId { get; set; }

            /// <summary>
            /// 授予时间.
            /// </summary>
            public string date { get; set; }

            /// <summary>
            /// 授予单位.
            /// </summary>
            public string provideUnit { get; set; }

            /// <summary>
            /// 荣誉级别.
            /// </summary>
            public string level { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            public string collectionUnit { get; set; }
        }

        /// <summary>
        /// 家庭成员详情输出参数.
        /// </summary>
        public class JyywMemberDetailOutput
        {
            /// <summary>
            /// 人员表主键.
            /// </summary>
            public string peopleId { get; set; }

            /// <summary>
            /// 家庭成员姓名.
            /// </summary>
            public string memberName { get; set; }

            /// <summary>
            /// 关系.
            /// </summary>
            public string relationship { get; set; }

            /// <summary>
            /// 出生年月.
            /// </summary>
            public string birthDate { get; set; }

            /// <summary>
            /// 籍贯.
            /// </summary>
            public string hometown { get; set; }

            /// <summary>
            /// 工作单位及职业.
            /// </summary>
            public string workUnit { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            public string collectionUnit { get; set; }
        }

        /// <summary>
        /// 媒体宣传详情输出参数.
        /// </summary>
        public class JyywReportDetailOutput
        {
            /// <summary>
            /// 人员表主键.
            /// </summary>
            public string peopleId { get; set; }

            /// <summary>
            /// 宣传时间.
            /// </summary>
            public string date { get; set; }

            /// <summary>
            /// 媒体名称或频道.
            /// </summary>
            public string provideUnit { get; set; }

            /// <summary>
            /// 媒体等级.
            /// </summary>
            public string level { get; set; }

            /// <summary>
            /// 采集单位.
            /// </summary>
            public string collectionUnit { get; set; }
        }
    }
}
