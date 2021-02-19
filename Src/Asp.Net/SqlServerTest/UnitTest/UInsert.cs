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

            db.CodeFirst.InitTables<UinitBlukTable>();
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
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
            db.Insertable(new UinitBlukTable() { Id = 2, Name = "b", Create = DateTime.Now }).ExecuteCommand();
            var x=Db.Storageable(list2)
                .SplitInsert(it => it.NotAny())
                .SplitUpdate(it => it.Any())
                .SplitDelete(it=>it.Item.Id>10)
                .SplitIgnore(it=>it.Item.Id==1)
                .SplitError(it => it.Item.Id == 3,"id不能等于3")
                .SplitError(it => it.Item.Id == 4, "id不能等于4")
                .SplitError(it => it.Item.Id == 5, "id不能等于5")
                .SplitError(it => it.Item.Name==null, "name不能等于")
                .WhereColumns(it=> new { it.Id })
                .ToStorage();
             x.AsDeleteable.ExecuteCommand();
             x.AsInsertable.ExecuteCommand();
             x.AsUpdateable.ExecuteCommand();
            foreach (var item in x.ErrorList)
            {
                Console.Write(item.StorageMessage+" ");
            }
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            IDemo1();
            IDemo2();
            IDemo3();
            IDemo4();
            IDemo5();
            IDemo6();
        }

        private static void IDemo6()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();

            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            list2.Add(new UinitBlukTable() { Id = 1, Name = "a" });
            list2.Add(new UinitBlukTable() { Id = 2, Name = "a" });
            list2.Add(new UinitBlukTable() { Id = 3, Name = "a" });
            list2.Add(new UinitBlukTable() { Id = 4, Name = "" });

            db.Insertable(list2.First()).ExecuteCommand();//插入第一条

            var x = Db.Storageable(list2)
                                      .SplitUpdate(it => it.Any())
                                      .SplitInsert(it => true)
                                      .ToStorage();

            //var x2 = Db.Storageable(list2)
            //                     .Saveable()
            //                    .ToStorage();
            Console.WriteLine(" 插入 {0}  更新{1}  错误数据{2} 不计算数据{3}  删除数据{4},总共{5}",
                   x.InsertList.Count,
                   x.UpdateList.Count,
                   x.ErrorList.Count,
                   x.IgnoreList.Count,
                   x.DeleteList.Count,
                   x.TotalList.Count
                );
            foreach (var item in x.ErrorList)
            {
                Console.WriteLine("id等于" + item.Item.Id + " : " + item.StorageMessage);
            }

            int i = x.AsInsertable.ExecuteCommand();
            Console.WriteLine(i + "条成功插入");
            i = x.AsUpdateable.ExecuteCommand();
            Console.WriteLine(i + "条成功更新");

            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }
        private static void IDemo5()
        {
            var db = Db;
            db.CodeFirst.InitTables<UinitBlukTable3>();
            db.DbMaintenance.TruncateTable<UinitBlukTable3>();

            List<UinitBlukTable3> list2 = new List<UinitBlukTable3>();
            list2.Add(new UinitBlukTable3() { Id = 1, Name = "a"  });
            list2.Add(new UinitBlukTable3() { Id = 2, Name = "a" });
            list2.Add(new UinitBlukTable3() { Id = 3, Name = "a" });
            list2.Add(new UinitBlukTable3() { Id = 4, Name = ""  });

            db.Insertable(list2.First()).ExecuteCommand();//插入第一条

            var x = Db.Storageable(list2)
                                      .SplitUpdate(it=>it.Any())
                                      .SplitInsert(it => true)
                                      .ToStorage();

            //var x2 = Db.Storageable(list2)
            //                     .Saveable()
            //                    .ToStorage();
            Console.WriteLine(" 插入 {0}  更新{1}  错误数据{2} 不计算数据{3}  删除数据{4},总共{5}",
                   x.InsertList.Count,
                   x.UpdateList.Count,
                   x.ErrorList.Count,
                   x.IgnoreList.Count,
                   x.DeleteList.Count,
                   x.TotalList.Count
                );
            foreach (var item in x.ErrorList)
            {
                Console.WriteLine("id等于" + item.Item.Id + " : " + item.StorageMessage);
            }

            int i = x.AsInsertable.ExecuteCommand();
            Console.WriteLine(i + "条成功插入");
            i = x.AsUpdateable.ExecuteCommand();
            Console.WriteLine(i + "条成功更新");

            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }

        private static void IDemo4()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
           
            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            list2.Add(new UinitBlukTable() { Id = 1, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 2, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 3, Name = "a", Create = DateTime.Now.AddYears(-2) });
            list2.Add(new UinitBlukTable() { Id = 4, Name ="", Create = DateTime.Now.AddYears(-2) });

            db.Insertable(list2.First()).ExecuteCommand();//插入第一条

            var x = Db.Storageable(list2)
                                      .SplitError(it => string.IsNullOrEmpty(it.Item.Name), "名称不能为空")
                                      .SplitError(it => it.Item.Create<DateTime.Now.AddYears(-1),"不是今年的数据")
                                      .SplitUpdate(it => it.Any(y=>y.Id==it.Item.Id))//存在更新
                                      .SplitInsert(it => true)//剩余的插入
                                      .ToStorage();
            Console.WriteLine(" 插入 {0}  更新{1}  错误数据{2} 不计算数据{3}  删除数据{4},总共{5}" ,
                   x.InsertList.Count,
                   x.UpdateList.Count,
                   x.ErrorList.Count,
                   x.IgnoreList.Count,
                   x.DeleteList.Count,
                   x.TotalList.Count
                );
            foreach (var item in x.ErrorList)
            {
                Console.WriteLine("id等于"+item.Item.Id+" : "+item.StorageMessage);
            }

            int i=x.AsInsertable.ExecuteCommand();
            Console.WriteLine(1 + "条成功插入");
            i=x.AsUpdateable.ExecuteCommand();
            Console.WriteLine(1 + "条成功更新");

            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }
        private static void IDemo3()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            list2.Add(new UinitBlukTable() { Id = 1, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 2, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 3, Name = "a", Create = DateTime.Now });

            var x = Db.Storageable(list2)
                                      .SplitUpdate(it => it.Item.Id == 1)
                                      .SplitInsert(it => it.Item.Id == 2)
                                      .SplitDelete(it => it.Item.Id == 3).ToStorage();
                                
            x.AsInsertable.ExecuteCommand();
            x.AsUpdateable.ExecuteCommand();
            x.AsDeleteable.ExecuteCommand();

            if (x.InsertList.Count != 1)
            {
                throw new Exception("Unit Insert");
            }
            if (x.UpdateList.Count != 1)
            {
                throw new Exception("Unit Insert");
            }
            if (x.DeleteList.Count != 1)
            {
                throw new Exception("Unit Insert");
            }
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }

        private static void IDemo1()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            list2.Add(new UinitBlukTable() { Id = 1, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 2, Name = "a", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 0, Name = "a", Create = DateTime.Now });

            var x = Db.Storageable(list2)
                                      .SplitUpdate(it => it.Item.Id > 0)
                                      .SplitInsert(it => it.Item.Id == 0).ToStorage();
            x.AsInsertable.ExecuteCommand();
            x.AsUpdateable.ExecuteCommand();

            if (x.InsertList.Count > 1)
            {
                throw new Exception("Unit Insert");
            }
            if (x.UpdateList.Count !=2)
            {
                throw new Exception("Unit Insert");
            }
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }
        private static void IDemo2()
        {
            var db = Db;
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
            List<UinitBlukTable> list2 = new List<UinitBlukTable>();
            list2.Add(new UinitBlukTable() { Id = 1, Name = "a1", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 2, Name = "a2", Create = DateTime.Now });
            list2.Add(new UinitBlukTable() { Id = 3, Name = "a3", Create = DateTime.Now });
            db.Insertable(list2[1]).ExecuteCommand();

            var x = Db.Storageable(list2)
                                      .SplitUpdate(it => it.Any(y=>y.Id==it.Item.Id))
                                      .SplitInsert(it => it.NotAny(y => y.Id == it.Item.Id)).ToStorage();
            x.AsInsertable.ExecuteCommand();
            x.AsUpdateable.ExecuteCommand();


            if (x.InsertList.Count!=2)
            {
                throw new Exception("Unit Insert");
            }
            if (x.UpdateList.Count != 1)
            {
                throw new Exception("Unit Insert");
            }
            db.DbMaintenance.TruncateTable<UinitBlukTable>();
        }

        public class UinitBlukTable
        {
            [SqlSugar.SugarColumn(IsPrimaryKey =true)]
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
            [SqlSugar.SugarColumn(IsNullable = true)]
            public DateTime? Create { get; set; }
        }

        [SqlSugar.SugarTable("UinitBlukTable4")]
        public class UinitBlukTable3
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public string Name { get; set; }
            [SqlSugar.SugarColumn(IsPrimaryKey = true)]
            public int Id { get; set; }
            [SqlSugar.SugarColumn(IsNullable = true)]
            public DateTime? Create { get; set; }
        }

    }
}
