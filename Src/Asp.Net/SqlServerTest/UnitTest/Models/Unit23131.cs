using Demo;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class Unit23131
    {
        public static void Init() 
        {
           
                //var connStr = "server=192.168.88.190;database=SDWHH2206;uid=whh;pwd=Zowie2023";
                var connStr1 = "Server=192.168.88.180;Port=54321;UID=whh;PWD=whh@123;database=zhsy_whh;searchpath=dbo;";
                //SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = connStr, DbType = DbType.SqlServer });
                SqlSugarClient db = new SqlSugarClient(new ConnectionConfig() { ConnectionString = connStr1, DbType = DbType.Kdbndp });
       
                var list =   db.Queryable<ShouJi>().Select(s => new ShoujiViewDto
                {
                    shouji_guid = s.shouji_guid.SelectAll()
                }).MergeTable().Where(w => w.xuqin_name != null)
                .Where(w => w.shouji_guid == SqlFunc.Subqueryable<ShoujiSubnum>().Where(ss => ss.amount > 0
                                && ss.insurance_section_id ==
                                    SqlFunc.Subqueryable<InsuranceSectionConfig>()
                                    .GroupBy(i => i.insurance_section_id)
                                    .Where(i => i.category_code == "123" && i.insurance_section_id == ss.insurance_section_id)
                                    .Select(i => i.insurance_section_id)
                               ).GroupBy(ss => ss.shouji_guid)
                               .Select(ss => ss.shouji_guid)).ToSql();

            if (!list.Key.Contains("\"INSURANCE_SECTION_CONFIG\" \"I\"")) 
            {
                throw new Exception("unit error");
            }
             
        }
    }
}
