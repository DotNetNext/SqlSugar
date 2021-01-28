using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest
{
    public partial class NewUnitTest
    {
        public static void Insert()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            db.CodeFirst.InitTables<UinitBlukTable>();
            db.Insertable(new List<UinitBlukTable>
            {
                 new UinitBlukTable(){ Id=1,Create=DateTime.Now, Name="00" },
                 new UinitBlukTable(){ Id=2,Create=DateTime.Now, Name="11" }

            }).UseSqlServer().ExecuteBlueCopy();
            var dt = db.Queryable<UinitBlukTable>().ToDataTable();
            dt.Rows[0][0] = 3;
            dt.Rows[1][0] = 4;
            dt.TableName = "[UinitBlukTable]";
            db.Insertable(dt).UseSqlServer().ExecuteBlueCopy();
            db.Insertable(new List<UinitBlukTable2>
            {
                 new UinitBlukTable2(){   Id=5,  Name="55" },
                 new UinitBlukTable2(){    Id=6, Name="66" }

            }).UseSqlServer().ExecuteBlueCopy();
            db.Ado.BeginTran();
            db.Insertable(new List<UinitBlukTable2>
            {
                 new UinitBlukTable2(){   Id=7,  Name="77" },
                 new UinitBlukTable2(){    Id=8, Name="88" }

            }).UseSqlServer().ExecuteBlueCopy();
            var task= db.Insertable(new List<UinitBlukTable2>
            {
                 new UinitBlukTable2(){   Id=9,  Name="9" },
                 new UinitBlukTable2(){    Id=10, Name="10" }

            }).UseSqlServer().ExecuteBlueCopyAsync();
            task.Wait();
            db.Ado.CommitTran();
            var list = db.Queryable<UinitBlukTable>().ToList();
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            if (string.Join("", list.Select(it => it.Id)) != "12345678910")
            {
                throw new Exception("Unit Insert");
            }
            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            for (int i = 1; i <= 20; i++)
            {
                UinitBlukTable data = new UinitBlukTable()
                {
                     Create=DateTime.Now.AddDays(-1),
                     Id=i ,
                     Name =i%3==0?"a":"b"
                };
                list2.Add(data);
            }
            list2.First().Name = null;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            var x=Db.Storageable(list2)
                //.SplitInsert(it => !string.IsNullOrEmpty(it.Item.Name))
                //.SplitUpdate(it =>  string.IsNullOrEmpty(it.Item.Name))
                .SplitDelete(it=>it.Item.Id>10)
                .SplitIgnore(it=>it.Item.Id==2)
                .SplitError(it => it.Item.Id == 3,"id不能等于3")
                .SplitError(it => it.Item.Id == 4, "id不能等于4")
                .SplitError(it => it.Item.Id == 5, "id不能等于5")
                .WhereColumns(it=>it.Id)
                .ToStorage();
            x.AsDeleteable.ExecuteCommand();
            x.AsInsertable.ExecuteCommand();
             x.AsUpdateable.ExecuteCommand();
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }
        public class UinitBlukTable
        {
         
            public int Id { get; set; }
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsNullable =true)]
            public DateTime? Create { get; set; }
        }
        [SqlSugar.SugarTable("UinitBlukTable")]
        public class UinitBlukTable2
        {
            public string Name { get; set; }
            public int Id { get; set; }
        }
 
    }
}
