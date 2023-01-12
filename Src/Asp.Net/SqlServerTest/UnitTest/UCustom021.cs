using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;
namespace OrmTest 
{
    public class UCustom021
    {
        public static void Inti() {
            var db = NewUnitTest.Db;
            db.CodeFirst.SetStringDefaultLength(20).InitTables<UnitStudent111>();
            db.CodeFirst.SetStringDefaultLength(20).InitTables<UnitExam>();

            var id = db.Insertable(new UnitStudent111() { Name = "小a" }).ExecuteReturnIdentity();
            db.Insertable(new UnitExam
            {
                StudentId = id,
                Number = 1,
                Time = DateTime.Now
            }).ExecuteCommand();

            var result2 = db.Queryable<UnitStudent111>()
             .Includes(e => e.Exams.Where(s => s.Time >DateTime.Now).ToList()).ToList();

            Join<MyJoin>();

        }

        public static void Join<T>() where T : IUserLink
        {
            var sql= NewUnitTest.Db
                .Queryable<UserDO, T>((u, a) => new JoinQueryInfos(JoinType.Inner, u.Id == a.UserId))
                .Where((u, a) => u.Id == 1)
                .Select((u, a) => a).ToSql();
            Check.Exception("SELECT a.* FROM [UserDO] [u] Inner JOIN [MyJoin] a ON ( [u].[Id] = [a].[UserId] )   WHERE ( [u].[Id] = @Id0 )" != sql.Key,"unit error");
        }
    }
    public class MyJoin : IUserLink 
    {
        public int UserId { get; set; }
    }
    public interface IUserLink
    {
         int UserId { get; set; }
    }

    public class UserDO
    {
        public int Id { get; set; }
    }

    public class AuthDO : IUserLink
    {
        public int UserId { get; set; }

        public int ObjId { get; set; }
    }
    public class UnitStudent111
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public string Name { get; set; }
        [Navigate(NavigateType.OneToMany, nameof(UnitExam.StudentId))]//BookA表中的studenId
        public List<UnitExam> Exams { get; set; }

    }
    public class UnitExam
    {
        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int Number { get; set; }
        public DateTime Time { get; set; }

    }
}
