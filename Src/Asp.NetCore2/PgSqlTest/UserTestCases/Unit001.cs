
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

using SqlSugar;
namespace OrmTest {
    public static partial class ObjectExtension
    {
        /// <summary>
        /// 排除SqlSugar忽略的列
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool IsIgnoreColumn(PropertyInfo pi)
        {
            var sc = pi.GetCustomAttributes<SugarColumn>(false).FirstOrDefault(u => u.IsIgnore == true);
            return sc != null;
        }
        /// <summary>
        /// List转DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this List<T> list)
        {
            DataTable result = new DataTable();
            if (list.Count > 0)
            {
                // result.TableName = list[0].GetType().Name; // 表名赋值
                PropertyInfo[] propertys = list[0].GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    Type colType = pi.PropertyType;
                    if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        colType = colType.GetGenericArguments()[0];
                    }
                    if (IsIgnoreColumn(pi))
                        continue;
                    result.Columns.Add(pi.Name, colType);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    ArrayList tempList = new ArrayList();
                    foreach (PropertyInfo pi in propertys)
                    {
                        if (IsIgnoreColumn(pi))
                            continue;
                        object obj = pi.GetValue(list[i], null);
                        tempList.Add(obj);
                    }
                    object[] array = tempList.ToArray();
                    result.LoadDataRow(array, true);
                }
            }
            return result;
        }
    }

    public class Unit001
    {

       public  static void Init( )
        {
            var db = new SqlSugarScope(new SqlSugar.ConnectionConfig()
            {
                ConnectionString =Config.ConnectionString2,
                DbType = SqlSugar.DbType.PostgreSQL,
                IsAutoCloseConnection = true
            });
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
            db.Aop.OnLogExecuting = (sql,p) =>
            {
                
                Console.WriteLine(sql);

            };

            //建表 
            if (!db.DbMaintenance.IsAnyTable("Test0011", false))
            {
                db.CodeFirst.InitTables<Test0011>();
            }
            List<Test0011> list = new List<Test0011>();
            list.Add(new Test0011() { Id = 1 });
            System.Type entityType = typeof(Test0011);
            var seedDataTable = db.Utilities.ListToDataTable(list);
            seedDataTable.TableName = db.EntityMaintenance.GetEntityInfo(entityType).DbTableName;
            var storage = db.Storageable(seedDataTable).WhereColumns("id").ToStorage();
            var result = storage.AsInsertable.ExecuteCommand();



            Console.WriteLine(result);
            Console.WriteLine("用例跑完");
           
        }
        //建类
        public class Test0011
        {
            [SugarColumn(ColumnDescription = "Id", IsPrimaryKey = true, IsIdentity = false)]
            public int Id { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            [SugarColumn(ColumnDescription = "更新时间", IsOnlyIgnoreInsert = true,IsNullable =true)]
            public DateTime? UpdateTime { get; set; }
        }



    }
}