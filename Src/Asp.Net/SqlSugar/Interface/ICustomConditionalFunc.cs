using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugar 
{
    public interface ICustomConditionalFunc
    {
        KeyValuePair<string, SugarParameter[]> GetConditionalSql(ConditionalModel  json,int index);
    }
}
