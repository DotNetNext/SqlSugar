using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest
{
 
        public class UnitNavInsertIssue
        {
        
            public static void Init()
            {
                ConnectionConfig cfg = new ConnectionConfig()
                {
                    DbType = DbType.MySql,
                    ConnectionString = "随便填一个本地的吧"
                };
              var sqlSugarScope = NewUnitTest.Db;
                sqlSugarScope.DbMaintenance.CreateDatabase();
                sqlSugarScope.CodeFirst.InitTables(typeof(Mail), typeof(MailAttachment));
                Mail mail = new Mail()
                {
                    TraceId = "abcd",
                    Subject = "subject",
                    Content = "content"
                };
                  sqlSugarScope
                    .InsertNav(mail)
                    .Include(p => p.Attachments)
                    .ExecuteCommand();

            sqlSugarScope
               .UpdateNav(mail,new UpdateNavRootOptions() {
                   IgnoreColumns =new  string[]{ "Subject" }
               })
               .Include(p => p.Attachments)
               .ExecuteCommand();
        }
        }

        [SugarTable(nameof(Mail))]
        public class Mail
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SugarColumn(IsNullable = false, Length = 20)]
            public string TraceId { get; set; }
            [SugarColumn(IsNullable = false, Length = 500)]
            public string Subject { get; set; }
            [SugarColumn(IsNullable = false, Length = 2000)]
            public string Content { get; set; }
            [Navigate(NavigateType.OneToMany, nameof(MailAttachment.MailTraceId), nameof(TraceId))]
            public List<MailAttachment> Attachments { get; set; }
        }

        [SugarTable(nameof(MailAttachment))]
        public class MailAttachment
        {
            [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            [SugarColumn(IsNullable = false, Length = 20)]
            public string MailTraceId { get; set; }
            [SugarColumn(IsNullable = false, Length = 100)]
            public string FileName { get; set; }
        }
 
}
