using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestDemo.Entitys;
namespace OrmTest
{
    public class Unitdfaysfa
    {
        public static void Init()
        {
            GetPerson().GetAwaiter().GetResult();
        }
        public static async Task<List<PersonPageModelDto>> GetPerson()
        {


            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<PersonInfo, PersonReginfo, PersonJobTitle, PersonCreditLevel>();
            var listQuery = new List<ISugarQueryable<PersonPageModelDto>>();

            var queryReginfo = db.Queryable<PersonInfo>()
                .LeftJoin<PersonReginfo>((p, r) => p.Id == r.PersonId)
                .Select((p, r) => new PersonPageModelDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IdCard = p.IdCard,
                    CertNum = r.CertNum
                });

            var queryTitle = db.Queryable<PersonInfo>()
                .LeftJoin<PersonJobTitle>((p, r) => p.Id == r.PersonId)
                .Select((p, r) => new PersonPageModelDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    IdCard = p.IdCard,
                    CertNum = r.CertNo
                });

            var query = db.UnionAll(queryTitle, queryReginfo).MergeTable().Where(x => SqlFunc.Subqueryable<PersonCreditLevel>().Where(s => s.IdCard == x.IdCard && s.Level == 8).NotAny());


            var pagelist = await query.Clone().CountAsync();
            var pagelist2 =   query.Clone().Count();
            return null;
        }
    }
}
