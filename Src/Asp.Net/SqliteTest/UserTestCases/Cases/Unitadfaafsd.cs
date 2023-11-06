using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace OrmTest
{
    internal class Unitadfaafsd
    {
        public static void Init()
        {
            SqlSugarClient _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = Config.ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true
            },
           db =>
           {
               db.Aop.OnLogExecuting = (sql, pars) =>
               {
                   Console.WriteLine(sql);//输出sql,查看执行sql 性能无影响
               };
           });
            _db.CodeFirst.InitTables<JqdsrdbEntity, JqdsrgxbEntity>();
            _db.InsertNav(new JqdsrdbEntity()
            {
                Jqdsrdbh = "a",
                Jqdsrgxbs = new List<JqdsrgxbEntity>()
                {
                     new JqdsrgxbEntity(){
                      Jqdsrdbh= "a",
                       Id= 1,
                        Sfmc= "a",
                         Sf="a"
                     }
                }
            }).Include(x => x.Jqdsrgxbs).ExecuteCommand();
            var items = _db.Queryable<JqdsrdbEntity>()
                 .Includes(dsr => dsr.Jqdsrgxbs)
                 .Select((dsr) =>
                         new Resp
                         {
                             Sf = dsr.Jqdsrgxbs.Select(sf => sf.Sfmc),
                             IsShowOperationButton = true
                         },
                         isAutoFill: true)
                 .ToListAsync().GetAwaiter().GetResult();

            Check(items);

            var items2 = _db.Queryable<JqdsrdbEntity>()
          .Includes(dsr => dsr.Jqdsrgxbs)
          .LeftJoin<JqdsrdbEntity>((x,y)=>x.Jqdsrdbh==y.Jqdsrdbh)
          .Select((x,y) =>
                  new Resp
                  {
                      Sf = x.Jqdsrgxbs.Select(sf => sf.Sfmc),
                      IsShowOperationButton = true
                  },
                  isAutoFill: true)
          .ToListAsync().GetAwaiter().GetResult();

            Check(items2);

            var items3 = _db.Queryable<JqdsrdbEntity>()
            .Includes(dsr => dsr.Jqdsrgxbs)
            .LeftJoin<JqdsrdbEntity>((x, y) => x.Jqdsrdbh == y.Jqdsrdbh)
            .Select((x, y) =>
                    new Resp
                    {
                        Sf = x.Jqdsrgxbs.Select(sf => sf.Sfmc)  
                    },
                    isAutoFill: true)
            .ToListAsync().GetAwaiter().GetResult();

            if (items3.First().IsShowOperationButton == false) 
            {
                items3.First().IsShowOperationButton = true;
            }
            Check(items3);

            var items4= _db.Queryable<JqdsrdbEntity>()
            .Includes(dsr => dsr.Jqdsrgxbs)
            .LeftJoin<JqdsrdbEntity>((x, y) => x.Jqdsrdbh == y.Jqdsrdbh)
            .Select((x, y) =>
                    new Resp
                    {
                        Sf = x.Jqdsrgxbs.Select(sf => sf.Sfmc)
                    },
                    isAutoFill: true)
            .MergeTable()
            .ToListAsync().GetAwaiter().GetResult();
            if (items4.First().IsShowOperationButton == false)
            {
                items4.First().IsShowOperationButton = true;
            }
            Check(items4);



        }

        private static void Check(List<Resp> items)
        {
            if (items.First().Jqdsrdbh != "a" || items.First().IsShowOperationButton == false ||
            items.First().Sf.Count() == 0)
            {
                throw new Exception("unit error");
            }
        }
    }
    [SugarTable("jqdsrdb")]
    public class JqdsrdbEntity
    {

        [SugarColumn(ColumnName = "jqdsrdbh", IsPrimaryKey = true)]
        public string Jqdsrdbh { get; set; }

        [Navigate(NavigateType.OneToMany, nameof(JqdsrgxbEntity.Jqdsrdbh))]
        public List<JqdsrgxbEntity> Jqdsrgxbs { get; set; }

    }

    [SugarTable("jqdsrgxb")]
    public class JqdsrgxbEntity
    {
        [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
        public long Id { get; set; }

        [SugarColumn(ColumnName = "sf")]
        public string Sf { get; set; }

        [SugarColumn(ColumnName = "sfmc")]
        public string Sfmc { get; set; }

        [SugarColumn(ColumnName = "jqdsrdbh")]
        public string Jqdsrdbh { get; set; }
         
    }

    public class Resp
    {
        public string Jqdsrdbh { get; set; }
        public bool IsShowOperationButton { get; set; }

        public IEnumerable<string> Sf { get; set; }
    }

    public class JqdsrgxbModel
    {
        public string Sf { get; set; }

        public string Sfmc { get; set; }
    }
}
