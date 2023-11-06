using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WWB.Park.Entity;
using WWB.Park.Entity.System;
using WWB.Park.Entity.Tenant;

namespace OrmTest
{
    internal class UFilter2
    {
        public static void Init() 
        {
            var db = NewUnitTest.Db;
            int total = 0;
            db.QueryFilter.AddTableFilter<IAgentFilter>(it => it.AgentId == 0);
            db.QueryFilter.AddTableFilter<IDeletedFilter>(it => it.IsDelete == true);

            var pageList = db.Queryable<ParkPartner>()
                .InnerJoin<Park>((a, b) => true)
                .InnerJoin<AgentUser>((a, b, c) => true)
                //  .InnerJoin<SysUser>((a, b, c, d) => c.UserId == d.Id)


                .OrderBy((a) => a.Id)
                .ToSqlString();

            if (pageList != @"SELECT `a`.`park_id`,`a`.`agent_user_id`,`a`.`type`,`a`.`month_parking_rate`,`a`.`temp_parking_rate`,`a`.`month_charge_rate`,`a`.`temp_charge_rate`,`a`.`is_admin`,`a`.`is_delete`,`a`.`add_time`,`a`.`update_time`,`a`.`id` FROM `pk_park_partner` `a` Inner JOIN `pk_park` `b` ON ( 1 = 1 )  AND ( `b`.`agent_id` = 0 )  Inner JOIN `pk_agent_user` `c` ON ( 1 = 1 )  AND ( `c`.`agent_id` = 0 )   WHERE ( `a`.`is_delete` = 1 )ORDER BY `a`.`id` ASC ") 
            {
                throw new Exception("unit error");
            }
        }
    }
}
