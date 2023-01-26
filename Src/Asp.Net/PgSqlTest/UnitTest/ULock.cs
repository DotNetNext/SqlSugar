using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class ULock
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            db.CodeFirst.InitTables<ULockEntity>();
            db.DbMaintenance.TruncateTable<ULockEntity>();
            var dt= DateTime.Now;
            var id=db.Insertable(new ULockEntity()
            {
                Name = "oldName",
                Ver= dt
            }).ExecuteReturnIdentity();

            var data = db.Updateable(new ULockEntity()
            {
                Id = id,
                Ver= dt,
                Name = "newname",
           
            }).ExecuteCommandWithOptLock();
            if (data != 1) { throw new Exception("unit error"); };
            var data2 = db.Updateable(new ULockEntity()
            {
                Id = id,
                Name = "newname2",
 
            }).ExecuteCommandWithOptLock();
            if (data2 != 0) { throw new Exception("unit error"); };

            var data3 = db.Updateable(new ULockEntity()
            {
                Id = id,
                Name = "newname2",
           
            })
            .UpdateColumns(z=>z.Name).ExecuteCommandWithOptLock();
            if (data3 != 0) { throw new Exception("unit error"); }
            var ver = db.Queryable<ULockEntity>().InSingle(id);
            var data4 = db.Updateable(new ULockEntity()
            {
                Id = id,
                Name = "newname2",
                Ver = ver.Ver
            })
            .UpdateColumns(z => z.Name).ExecuteCommandWithOptLock();
            if (data4 != 1) { throw new Exception("unit error"); }
        }

        public class ULockEntity
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true,IsIdentity =true)]
            public int Id { get; set; }
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation  = true, ColumnDataType ="timestamp" )]
            public DateTime Ver { get; set; }
        }
    }
}
