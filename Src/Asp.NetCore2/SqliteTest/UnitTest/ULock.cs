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
            var id = db.Insertable(new ULockEntity()
            {
                Name = "oldName",
            }).ExecuteReturnIdentity();

            var data = db.Updateable(new ULockEntity()
            {
                Id = id,
                Name = "newname",
                Ver = 0
            }).ExecuteCommandWithOptLock();
            if (data != 1) { throw new Exception("unit error"); };
            var data2 = db.Updateable(new ULockEntity()
            {
                Id = id,
                Name = "newname2",
                Ver = 0
            }).ExecuteCommandWithOptLock();
            if (data2 != 0) { throw new Exception("unit error"); };
        }

        public class ULockEntity
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)]
            public long Ver { get; set; }
        }
    }
}
