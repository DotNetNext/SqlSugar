using ICrewApi.Entity;
using ICrewApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrmTest 
{
    public class UCustom023
    {
        public static List<DSPersonScheduleModel> Init()
        {
            PersonScheduleParam personScheduleParam = new PersonScheduleParam() 
            {
            ENDDATE=DateTime.Now,
            STARTDATE=DateTime.Now, 
            } ;
            List<DSPersonScheduleModel> result = new List<DSPersonScheduleModel>();
            try
            {
                var result1 =NewUnitTest.Db.Queryable<V_PERSON_SCHEDULE_A>()
                     .LeftJoin<T_BAS_AIRPORT>((ps, sa) => ps.DEPARTURE_AIRPORT == sa.AIRPORT_3CODE)
                     .LeftJoin<T_BAS_AIRPORT>((ps, sa, sb) => ps.ARRIVAL_AIRPORT == sb.AIRPORT_3CODE)
                     .Where(ps => personScheduleParam.P_CODE.Contains(ps.P_CODE) && ps.FROM_DATE <= personScheduleParam.ENDDATE && ps.TO_DATE >= personScheduleParam.STARTDATE)
                     .ToSql();
                if (result1.Value.First().DbType != System.Data.DbType.Date) 
                {
                    throw new Exception("unit error");
                }
                if (result1.Value.First().Size != 10)
                {
                    throw new Exception("unit error");
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
