using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public class Unit01
    {
        public static void Init()
        {
            var DB = new SqlSugarScope(new List<ConnectionConfig>()
        {
            new ConnectionConfig()
            {
                ConfigId = "Main",
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                ConnectionString =Config.ConnectionString
            }
        }, client => { client.Aop.OnLogExecuting = (s, parameters) => Console.WriteLine(s); });

            DB.CodeFirst.InitTables<ULockEntity>();
            var entity = new ULockEntity() { Id = 1, Name = "a", Ver = 0 };
            entity=DB.Insertable<ULockEntity>(entity).ExecuteReturnEntity();
            DB.Updateable(entity)
                .UpdateColumns(s => new { s.Name, s.Enable })
                .ExecuteCommandWithOptLock(true);
            Console.WriteLine("Hello");
        }

        class ULockEntity
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }

            public string Name { get; set; }

            public bool Enable { get; set; }


            [SqlSugar.SugarColumn(IsEnableUpdateVersionValidation = true)] //标识版本字段
            public long Ver { get; set; }

            [SugarColumn(IsIgnore = true)]
            public string ShowName => $@"{Id}_{Name}";
        }
    }
}
