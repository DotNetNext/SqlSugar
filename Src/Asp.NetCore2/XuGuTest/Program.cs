using Data.Model;
using Data_logic;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
using SqlSugar.DbConvert;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using XuguClient;

namespace XuguTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            Task.Run(async () =>
            {
                var o = new UserService();
                //o.CreateModel(@"F:\!GZH\Models\南海气象数据支撑系统", allTable: true);
                var task = o.GetList();
                var data = task.First();
                data.ID = task.Last().ID + 1;
                //data.C_BINARY = File.ReadAllBytes(@"C:\Users\28679\Desktop\项目网址(新).docx");
                //data.C_DATE = DateTime.Now.AddDays(-10);
                var r = await o.Add(data);
                ////////////////////////var d1 = await o.Delete(o => o.C_BIGINT >=154 && o.C_BIGINT <=166);
                ////////////////////////var d = await o.Delete(o=>o.ID>1);

                var u = await o.Single<T_USER,int>(t=>t.C_DATETIME > DateTime.Today);
                u.C_DATETIME = DateTime.Now.AddDays(2);
                var b = await o.Update(u);
                Console.WriteLine("Hello World!");
            }).Wait();
        }
    }

    public class UserService : BaseDataLogic<T_USER>
    {
        public UserService() {

            //db.CodeFirst.InitTables(typeof(T_USER));
        }
        public List<T_USER> GetList()
        {
            return db.Queryable<T_USER>().Where(d=>d.C_DATETIME < DateTime.Now).ToPageList(1,1);
        }
    }

}
