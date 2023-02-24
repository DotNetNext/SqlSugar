using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class UnitFilterdafa
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<UintLeft0123, UintRight0123>();
            db.QueryFilter.AddTableFilter<IDEL>(it => it.IsDeleted == false);
            db.Queryable<UintLeft0123>().LeftJoin<UintRight0123>((x, y) => x.Id == y.Id)
                .ToList();
        }
        
    }
    public class UintLeft0123 : IDEL
    {
        public string Id { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "Is_Deleted")]
        public bool IsDeleted { get; set; }
    }
    public class UintRight0123: IDEL
    {
        public string Id { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "Is_Deleted")]
        public bool IsDeleted { get; set; }
    }

    public interface IDEL 
    {
          bool IsDeleted { get; set; }
    }
}
