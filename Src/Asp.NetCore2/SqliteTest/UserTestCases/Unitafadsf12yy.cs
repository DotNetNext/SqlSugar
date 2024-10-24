using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OrmTest
{
    internal class Unitafadsf12yy
    {

        public static void Init() 
        {
            //创建库1
            var db1 = NewUnitTest.Db;
            db1.DbMaintenance.CreateDatabase();
            db1.CodeFirst.InitTables<Db1Abpusers>();//给测试库1添加表



            //测试代码
            var us = new UserService();
           us.Add(new Db1Abpusers() { Id = 101, Name = "jack" }).GetAwaiter().GetResult();//插入

            var db = NewUnitTest.Db;
            db.DbMaintenance.CreateDatabase();
            db.CodeFirst.InitTables<Order>(); 
            db.DbMaintenance.TruncateTable<Order>();
            var updateObjs = new List<Order> {
                 new Order() { Id = 11, Name = "order11", Price=0 },
                 new Order() { Id = 12, Name = "order12" , Price=0}
            };
            var dt = db.Utilities.ListToDataTable(updateObjs);
            db.CurrentConnectionConfig.MoreSettings=new ConnMoreSettings() { 
              IsCorrectErrorSqlParameterName = true };
            var x = db.Fastest<DataTable>().AS("order").BulkCopy(dt);
            if (x != 2) 
            {
                throw new Exception("unit error");
            }
            var x2 = db.Fastest<DataTable>().AS("order").BulkUpdate(db.Queryable<Order>().Take(2).ToDataTable(),new string[] { "Id"});
            if (x2 != 2)
            {
                throw new Exception("unit error");
            }
        }


        //该用例建议升级5.1.3.35以上版本
        public class UserService
        {
            //也可以用Ioc注入，不注入更方便些（ ”=>" 表示用的时候才去new，不会在实例化类时候去new 
            Repository<Db1Abpusers> _userRepository1 => new Repository<Db1Abpusers>();
            ITenant _db => NewUnitTest.Db;//处理事务


            /// <summary>
            /// 获取用户列表
            /// </summary>
            /// <returns></returns>
            public async Task<bool> Add(Db1Abpusers data1)
            {
                try
                {
                     
                    var isAdmin = true;
                    await _userRepository1.InsertAsync(data1);

                    await _userRepository1.Context.Updateable<Db1Abpusers>().SetColumns(x => new Db1Abpusers
                    {
                        UserType = isAdmin ? EUserType.Admin : EUserType.SuperAdmin
                    }).Where(x => x.Id == data1.Id).ExecuteCommandAsync();
                    await _userRepository1.Context.Queryable<Db1Abpusers>().Select(x => new Db1Abpusers
                    {
                        UserType = isAdmin ? EUserType.Admin : EUserType.SuperAdmin
                    }).Where(x => x.Id == data1.Id).ToListAsync();
                 
                }
                catch (Exception)
                {
                   
                    throw;
                }
                return true;
            }
        }

        /// <summary>
        /// 创建仓储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class Repository<T> : SimpleClient<T> where T : class, new()
        {
            public Repository()
            {
                //固定数据库用法
                base.Context = NewUnitTest.Db;

                //动态库用法一般用于维护数据库连接字符串根据用法
                //if (!SqlSugarHelper.Db.IsAnyConnection("用户读出来的数据库ConfigId")) 
                //{
                //    SqlSugarHelper.Db.AddConnection(new ConnectionConfig() { 数据库读出来信息 });
                //}
                //base.Context = SqlSugarHelper.Db.GetConnectionScope("用户读出来的数据库ConfigId");
            }
        } 
        public enum EUserType
        {
            Admin = 1,
            SuperAdmin = 3
        }

        [Tenant("1")]
        public class Db1Abpusers
        {
            [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
            public int Id { get; set; }
            public string Name { get; set; }
            public EUserType UserType { get; set; }
        }
    }
}
