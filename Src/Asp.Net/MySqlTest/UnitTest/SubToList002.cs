using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{

    public class UnitSubToList002
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            var queryable = db.Queryable<Journal>();
            var abc = queryable
                      .Select(j => new SolrJournalOut
                      {
                          Id = j.Id.SelectAll(),

                          Languages = SqlFunc.Subqueryable<Language>().LeftJoin<JournalLanguage>((l, jl) => l.Id == jl.LanguageId).Where((l, jl) => jl.JournalId == j.Id).ToList(l => l.Name),

                        Subjects = SqlFunc.Subqueryable<Subject>().LeftJoin<JournalSubject>((s, js) => s.Id == js.SubjectId).Where((s, js) => js.JournalId == j.Id).ToList(s => s.Name),
                      });
           var a = abc.ToSqlString();
            if (a != "SELECT *,`j`.`Id` as app_ext_col_0 FROM `Journal` `j`  ") 
            {
                throw new Exception("unit error");
            }
           
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Journal
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }

        [Navigate(typeof(JournalLanguage), nameof(JournalLanguage.JournalId), nameof(JournalLanguage.LanguageId))]
        public List<Language> Languages { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(JournalLanguage.JournalId))]
        public ICollection<JournalLanguage> JournalLanguages { get; set; }

        [Navigate(typeof(JournalSubject), nameof(JournalSubject.JournalId), nameof(JournalSubject.SubjectId))]
        public List<Subject> Subjects { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(JournalSubject.JournalId))]
        public List<JournalSubject> JournalSubjects { get; set; }
    }


    /// <summary>
    ///
    /// </summary>
    public class JournalLanguage
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }

        public long JournalId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(JournalId))]//一对一
        public Journal Journal { get; set; }


        public long LanguageId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(Language))]//一对一
        public Language Language { get; set; }

    }

    public class JournalSubject
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }


        public long JournalId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(JournalId))]//一对一
        public Journal Journal { get; set; }


        public long SubjectId { get; set; }

        [Navigate(NavigateType.OneToOne, nameof(SubjectId))]//一对一
        public Subject Subject { get; set; }
    }

    public class Subject
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }


        /// <summary>
        /// 名称
        /// </summary>

        [MaxLength(500)]
        public string Name { get; set; }
    }

    public class Language
    {
        /// <summary>
        /// 非自增ID
        /// </summary>
        [SugarColumn(ColumnDescription = "ID", IsPrimaryKey = true)]
        public long Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>

        [MaxLength(200)]
        public string Name { get; set; }
    }


    public class SolrJournalOut
    {

        public long Id { get; set; }
        public List<string> Languages { get; set; }
        public List<string> Subjects { get; set; }
    }


}
